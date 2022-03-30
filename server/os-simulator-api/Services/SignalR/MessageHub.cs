using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Serilog;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.SessionLogs;
using SoMeSimulator.Helpers;
using SoMeSimulator.Services.MessageManager.Dto;
using SoMeSimulator.Services.SignalR;

namespace SoMeSimulator.Services.SignalRHubs
{
    public class MessageHub : Hub<IMessageHub>
    {
        private readonly SoMeContext _dbContext;
        private readonly ISendMessage _sendMessage;
        private readonly IEntityFactory _factory;

        public MessageHub(SoMeContext dbContext, ISendMessage sendMessage, IEntityFactory factory)
        {
            _dbContext = dbContext;
            _sendMessage = sendMessage;
            _factory = factory;
        }

        /// <summary>
        /// Create a SignalR group for a participant
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async Task JoinSessionGroup(string guid)
        {
            Log.Debug($"Client joined session {guid}");

            var session = _dbContext.Sessions.FindByGuid(Guid.Parse(guid));

            if (session != null)
            {
                await Groups.AddToGroupAsync(this.Context.ConnectionId, guid);
                await _sendMessage.SendParticipantJoinedToFacilitatorAsync(session.SessionGroup);
            }
        }

        /// <summary>
        /// Connect to SignalR in order to facilitate a session.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task FacilitateGroup(string id)
        {
            Log.Debug($"Facilitator connected {id}");

            if (_dbContext.SessionGroups.FindById(int.Parse(id)) != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, id);
            }
        }

        /// <summary>
        /// Root-level messages from participant
        /// </summary>
        /// <param name="incomingComment"></param>
        /// <returns></returns>
        public async Task SendPost(IncomingComment incomingComment, MessageFlow messageFlow)
        {
            var session = _dbContext.Sessions.FindByGuid(incomingComment.SessionGroup);
            
            if (!session.SessionGroup.IsRunning()) return;
            if (incomingComment.SessionLogId != null) return;

            var sessionLog = _factory.SessionLog((IncomingMessage) incomingComment, session, messageFlow);
            
            await _dbContext.SessionLogs.AddAsync(sessionLog);
            await _dbContext.SaveChangesAsync();

            await ReplyToUser(incomingComment.SessionGroup.ToString(), session, sessionLog);
        }

        /// <summary>
        /// Incoming comments from a participant
        /// </summary>
        /// <param name="incomingComment"></param>
        /// <returns></returns>
        public async Task SendComment(IncomingComment incomingComment)
        {
            var session = _dbContext.Sessions.FindByGuid(incomingComment.SessionGroup);
            var parentSessionLog = _dbContext.SessionLogs.FirstOrDefault(s => s.Id == incomingComment.SessionLogId);
            
            if (!session.SessionGroup.IsRunning()) return;
            if (parentSessionLog == null) return;

            parentSessionLog.Children.Add(_factory.SessionLog(incomingComment, session, parentSessionLog));
            await _dbContext.SaveChangesAsync();
            
            if (parentSessionLog.Level == 2)
            {
                var grandParentSessionLog = _dbContext.SessionLogs.FirstOrDefault(s => s.Id == parentSessionLog.ParentSessionLogId);
                parentSessionLog = grandParentSessionLog;
            }

            await ReplyToUser(incomingComment.SessionGroup.ToString(), session, parentSessionLog);
        }
        
        //Sends a reply to the user.
        private async Task ReplyToUser(string sessionGroup, Session session, SessionLog rootSessionLog)
        {
            var message = new Message()
            {
                MessageCount = session.CountSessionLogs(),
                SessionLog = rootSessionLog,
                SessionGroup = sessionGroup
            };

            await _sendMessage.SendCommentToGroupAsync(message);
            await _sendMessage.SendCommentToFacilitatorAsync(session.SessionGroup, message);
        }

        /// <summary>
        /// Displays a message for the session group.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ShowForGroup(int id)
        {
            Log.Debug($"Showing sessionlog for group: {id}");

            var sessionLog = _dbContext.SessionLogs.FindById(id);
            await _sendMessage.ShowForGroupAsync(sessionLog.Root());
        }
    }
}