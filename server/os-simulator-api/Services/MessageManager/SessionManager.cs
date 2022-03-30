using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Serilog;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SomeSimulator.Data.Models.Configurations;
using SoMeSimulator.Data.Models.SessionGroups.Commands;
using SoMeSimulator.Data.Models.SessionGroups.Status;
using SoMeSimulator.Services.MessageManager.Dto;
using SoMeSimulator.Services.SignalR;

namespace SoMeSimulator.Services.MessageManager
{
    public class SessionManager
    {
        private readonly ISendMessage _sendMessage;
        private readonly SoMeContext _dbContext;
        private readonly IEntityFactory _factory;
        private readonly IEnumerable<IManager> _managers;

        public SessionManager(ISendMessage sendMessage, SoMeContext dbContext, IEntityFactory factory, 
        IStressLevelCalculator stressLevelCalculator, IOptions<CommentsSettings> commentsSettings )
        {
            _sendMessage = sendMessage;
            _dbContext = dbContext;
            _factory = factory;

            _managers = new List<IManager>()
            {
                new ScenarioEventsManager(sendMessage, dbContext, factory),
                new PostsManager(sendMessage, dbContext, factory, stressLevelCalculator),
                new CommentsManager(sendMessage, dbContext, factory, stressLevelCalculator, commentsSettings),
                new ProgressManager(sendMessage)
            };
        }

        /// <summary>
        /// Sends all messages that needs to be sent.
        /// </summary>
        public void SendToRunningSessions()
        {
            var runningSessions = GetActiveSessionGroups();
            if (!runningSessions.Any()) return;
            
            Log.Debug("Session is running");

            runningSessions.ToList().ForEach(async r =>
            {
                await SendToSession(r);
            });
        }

        private async Task SendToSession(SessionGroup sessionGroup)
        {
            _managers.ToList().ForEach(m => m.Send(sessionGroup));

            if (!sessionGroup.TimeLeft())
            {
                await StopSession(sessionGroup);
                _dbContext.SaveChanges();
            }
        }

        private async Task StopSession(SessionGroup sessionGroup)
        {
            new Stop().Execute(sessionGroup);

            await _sendMessage.SendSessionTriggerToAllAsync(
                new SessionTrigger(new Dialog(true, "Sessionen är slut", "Sessionen är slut", "Ok"), sessionGroup.Status), sessionGroup);

            Log.Debug($"Stopped : {sessionGroup.Id} at {sessionGroup.StopDate}");
        }

        private IIncludableQueryable<SessionGroup, ICollection<ScenarioEvent>> GetActiveSessionGroups()
        {
            return _dbContext.SessionGroups
                .Where(s => s.Status == SessionStatus.Running)
                .Include(s => s.Scenario)
                .ThenInclude(s => s.Phases)
                .ThenInclude(p => p.ScenarioEvents);
        }

        /// <summary>
        /// Checks if there is a running or paused session.
        /// </summary>
        /// <returns></returns>
        public bool HasActiveSessionGroups() {
            return _dbContext.SessionGroups.Any(s => s.Status == SessionStatus.Running || s.Status == SessionStatus.Paused);
        }
    }
}