using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.Types;
using SoMeSimulator.Services.MessageManager;
using SoMeSimulator.Services.MessageManager.Dto;
using SoMeSimulator.Services.SignalRHubs;

namespace SoMeSimulator.Services.SignalR
{
    public class SendMessage : ISendMessage
    {
        private const string MessagesMethodName = "messages";
        private const string SessionTriggersMethodName = "sessionTriggers";
        private const string ProgressMethodName = "progress";
        private const string CommentsMethodName = "comments";
        private const string ParticipantJoinedMethodName = "participantJoined";
        private const string EventsMethodName = "events";
        private const string ActivityLogsMethodName = "activityLog";
        private const string ShowForGroupName = "showForGroup";
        private const string StressLevelMethodName = "changeStressLevel";

        private readonly IHubContext<MessageHub> _hub;

        public SendMessage(IHubContext<MessageHub> hub)
        {
            _hub = hub;
        }

        /// <inheritdoc />
        public Task ShowForGroupAsync(SessionLog sessionLog)
        {
            Log.Debug($"Showing message: {JsonConvert.SerializeObject(sessionLog).ToString()}");


            return _hub.Clients.Groups(SessionGuids(sessionLog.Session.SessionGroup))
                .SendAsync(ShowForGroupName, SerializeObject(sessionLog));
        }

        /// <inheritdoc />
        public Task SendStressLevelToFacilitatorAsync(uint stressLevel, SessionGroup sessionGroup)
        {
            Log.Debug($"Facilitator changed stresslevel.");

            return _hub.Clients.Group(sessionGroup.Id.ToString())
                .SendAsync(StressLevelMethodName, stressLevel);
        }

        /// <inheritdoc />
        public Task SendMessageToGroupAsync(Message message)
        {
            Log.Debug($"Sending message: {JsonConvert.SerializeObject(message).ToString()}");
            return _hub.Clients.Groups(message.SessionGroup).SendAsync(MessagesMethodName, SerializeObject(message));
        }

        /// <inheritdoc />
        public Task SendCommentToGroupAsync(Message message)
        {
            Log.Debug($"Sending comment: {JsonConvert.SerializeObject(message).ToString()}");
            return _hub.Clients.Groups(message.SessionGroup).SendAsync(CommentsMethodName, SerializeObject(message));
        }

        /// <inheritdoc />
        private static string SerializeObject(Message message)
        {
            return JsonConvert.SerializeObject(message);
        }

        /// <inheritdoc />
        private static string SerializeObject(SessionLog sessionLog)
        {
            return JsonConvert.SerializeObject(sessionLog);
        }

        /// <inheritdoc />
        private static string SerializeObject(SessionTrigger sessionTrigger)
        {
            return JsonConvert.SerializeObject(sessionTrigger);
        }


        /// <inheritdoc />
        public async Task<Task> SendSessionTriggerToAllAsync(SessionTrigger sessionTrigger, SessionGroup sessionGroup)
        {
            Log.Debug($"Sending trigger to all connected: {JsonConvert.SerializeObject(sessionTrigger).ToString()}");

            await _hub.Clients.Group(sessionGroup.Id.ToString())
                .SendAsync(SessionTriggersMethodName, SerializeObject(sessionTrigger));

            return _hub.Clients.Groups(SessionGuids(sessionGroup))
                .SendAsync(SessionTriggersMethodName, SerializeObject(sessionTrigger));
        }


        /// <inheritdoc />
        private static ReadOnlyCollection<string> SessionGuids(SessionGroup sessionGroup)
        {
            var sessionGuids = sessionGroup.Sessions.Select(s => s.SessionGuid.ToString()).ToList().AsReadOnly();
            return sessionGuids;
        }


        /// <inheritdoc />
        public Task SendCurrentSessionProgressAsync(double progress, SessionGroup sessionGroup)
        {
            Log.Debug($"Sending progress to all connected: {progress}");
            //return _hub.Clients.Groups(SessionGuids((sessionGroup))).SendAsync(ProgressMethodName, progress);
            return _hub.Clients.All.SendAsync(ProgressMethodName, progress);
        }


        /// <inheritdoc />
        public Task SendParticipantJoinedToFacilitatorAsync(SessionGroup sessionGroup)
        {
            Log.Debug($"Sending participant joined to {sessionGroup.Id}");
            return _hub.Clients.Group(sessionGroup.Id.ToString()).SendAsync(ParticipantJoinedMethodName);
        }


        /// <inheritdoc />
        public Task SendEventToFacilitatorAsync(SessionGroup sessionGroup, Message message)
        {
            Log.Debug($"Session related event or post sent.");
            return _hub.Clients.Group(sessionGroup.Id.ToString()).SendAsync(EventsMethodName, message.OnlySessionLog());
        }


        /// <inheritdoc />
        public Task SendCommentToFacilitatorAsync(SessionGroup sessionGroup, Message message)
        {
            Log.Debug($"Participant related messages.");
            return _hub.Clients.Group(sessionGroup.Id.ToString())
                .SendAsync(ActivityLogsMethodName, message.OnlySessionLog());
        }
    }
}