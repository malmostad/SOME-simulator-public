using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Serilog;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.SessionLogs;
using SoMeSimulator.Helpers;
using SoMeSimulator.Services.MessageManager.Dto;
using SoMeSimulator.Services.SignalR;
using SoMeSimulator.Data.Models.Defaults;
using SomeSimulator.Data.Models.Configurations;

namespace SoMeSimulator.Services.MessageManager
{
    public class PostsManager : IManager
    {
        private readonly ISendMessage _sendMessage;
        private readonly SoMeContext _dbContext;
        private readonly IEntityFactory _factory;
        private readonly IStressLevelCalculator _stressLevelCalculator;
         
        public PostsManager(ISendMessage sendMessage, SoMeContext dbContext, IEntityFactory factory, 
        IStressLevelCalculator stressLevelCalculator)
        {
            _sendMessage = sendMessage;
            _dbContext = dbContext;
            _factory = factory;
            _stressLevelCalculator = stressLevelCalculator;
        }

        /// <summary>
        /// Determines of a post should be posted.
        /// </summary>
        /// <param name="sessionGroup"></param>
        /// <returns></returns>
        public async Task Send(SessionGroup sessionGroup) {
            await this.Send(sessionGroup, false);
        }

        /// <summary>
        /// Determines of a post should be posted. With force = true a message will be sent.
        /// </summary>
        /// <param name="sessionGroup"></param>
        /// <param name="force">Force a message to be sent.</param>
        public async Task Send(SessionGroup sessionGroup, bool force)
        {
            if (!ShouldPost(sessionGroup.StressLevel) || force) return;

            var timeCalc = new TimeCalc.SessionGroupTimeCalc(sessionGroup);
            var phase = timeCalc.CurrentPhaseTimeCalc().Phase;

            var messageFlow = SelectMessageFlow();
            var post = PickPost(sessionGroup, phase, messageFlow);
            
            if(post == null) 
            {
                Log.Warning("Out of posts.");
                return;
            }

            foreach (var session in sessionGroup.Sessions)
            {
                var sessionLog = CreateSessionLog(session, post, messageFlow);

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

        private SessionLog CreateSessionLog(Session session, Post post, MessageFlow messageFlow)
        {
            var sessionLog = _factory.SessionLog(post, session, messageFlow);
            session.SessionLogs.Add(sessionLog);
            _dbContext.SessionLogs.Add(sessionLog);
            _dbContext.SaveChanges();

            return sessionLog;
        }

        private static Post PickPost(SessionGroup sessionGroup, Phase phase, MessageFlow messageFlow)
        {

            var post = phase.PostLink.Select(pl => pl.Post)
                .Where(p => 
                    p.MessageFlow.HasFlag(messageFlow) && !sessionGroup.Sessions
                    .SelectMany(s => s.SessionLogs)
                    .Select(sl => sl.PostId)
                    .Contains(p.Id))
                    .PickRandom();

            return post;
        }

        private static MessageFlow SelectMessageFlow()
        {
            
            var rand = new Random().Next(0, 100);

            if (rand < 50)
            {
                return MessageFlow.Short;
            }

            return MessageFlow.Long;
        }
        
        /// <summary>
        /// Current stresslevel
        /// </summary>
        /// <param name="stressLevel"></param>
        /// <returns></returns>
        private bool ShouldPost(uint stressLevel)
        {
            return _stressLevelCalculator.ShouldPost(stressLevel);
        }
    }
}