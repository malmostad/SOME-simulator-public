using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Internal;
using Serilog;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.Defaults;
using SoMeSimulator.Services.MessageManager.Dto;
using SoMeSimulator.Services.SignalR;

namespace SoMeSimulator.Services.MessageManager
{
    public class ScenarioEventsManager: IManager
    {
        private readonly ISendMessage _sendMessage;
        private readonly SoMeContext _dbContext;
        private readonly IEntityFactory _factory;
        private static object _lock = new Object();

        public ScenarioEventsManager(ISendMessage sendMessage, SoMeContext dbContext, IEntityFactory factory)
        {
            _sendMessage = sendMessage;
            _dbContext = dbContext;
            _factory = factory;
        }

        /// <summary>
        /// Send ScenarioEvents
        /// </summary>
        /// <param name="sessionGroup"></param>
        public async Task Send(SessionGroup sessionGroup)
        {
            var haveLock = false;

            try
            {
                Monitor.TryEnter(_lock, DefaultValues.ScenarioEventTimeoutLock ,ref haveLock);

                if (!haveLock)
                    throw new Exception("Could not get a lock while sending ScenarioEvents");


                var eventsDto = PickScenarioEvents(sessionGroup);

                if (eventsDto.PickedEvents.Any())
                {
                    await SendScenarioEvents(sessionGroup, eventsDto);
                }
                
            }
            finally
            {
                if(haveLock)
                    Monitor.Exit(_lock);
            }
        }

        private async Task SendScenarioEvents(SessionGroup sessionGroup, ScenarioEventsDto scenarioEventsDto)
        {
            Log.Information($"Sending events to session");

            foreach (var pickedEvent in scenarioEventsDto.PickedEvents)
            {
                foreach (var session in sessionGroup.Sessions)
                {
                    var sessionLog = _factory.SessionLog(pickedEvent, session);

                    session.SessionLogs.Add(sessionLog);

                    _dbContext.SessionLogs.Add(sessionLog);

                    _dbContext.SaveChanges();

                    var message = new Message()
                    {
                        MessageCount = session.CountSessionLogs(),
                        SessionLog = sessionLog,
                        SessionGroup = session.SessionGuid.ToString()
                    };

                    if (sessionGroup.Sessions.IndexOf(session) == 0)
                    {
                        await _sendMessage.SendEventToFacilitatorAsync(sessionGroup, message);
                    }

                    await _sendMessage.SendMessageToGroupAsync(message);
                }
            }
        }

        private ScenarioEventsDto PickScenarioEvents(SessionGroup sessionGroup)
        {
            var timeCalc = new TimeCalc.SessionGroupTimeCalc(sessionGroup);

            var currentPhaseTimeCalc = timeCalc.CurrentPhaseTimeCalc();

            Log.Debug($"Percent of session:  {timeCalc.CurrentSessionPercent()}. Percent of Phase {timeCalc.CurrentPhaseTimeCalc().Phase.Description}: {currentPhaseTimeCalc.CurrentPercentOfPhase()}");

            var eventIds = sessionGroup.Sessions.SelectMany(s => s.SessionLogs).Distinct().Select(e => e.ScenarioEventId);

            List<ScenarioEvent> scenarioEvents = new List<ScenarioEvent>();

            if (currentPhaseTimeCalc.Phase != sessionGroup.CurrentPhase) //Shifting phase
            {
                scenarioEvents.AddRange(sessionGroup.CurrentPhase.ScenarioEvents
                    .Where(e => !eventIds.Contains(e.Id))
                    .ToList()
                );
                sessionGroup.CurrentPhase = currentPhaseTimeCalc.Phase;
                _dbContext.SaveChanges();
            }

            var currentPhaseEvents = currentPhaseTimeCalc.Phase.ScenarioEvents
                .Where(e => !eventIds.Contains(e.Id) && e.TimePercent <= currentPhaseTimeCalc.CurrentPercentOfPhase())
                .ToList();

            scenarioEvents.AddRange(currentPhaseEvents);

            return new ScenarioEventsDto()
            {
                PickedEvents = scenarioEvents,
                EventsLeft = currentPhaseEvents.Any()
            };
        }

        private class ScenarioEventsDto
        {
            /// <summary>
            /// Events picked
            /// </summary>
            public IEnumerable<ScenarioEvent> PickedEvents { get; set; }

            /// <summary>
            /// Events left to send
            /// </summary>
            public bool EventsLeft { get; set; }
        }
    }
}