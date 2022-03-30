using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Microsoft.EntityFrameworkCore.Storage;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Services.MessageManager.Dto;

namespace SoMeSimulator.Services.SignalR
{
    public interface ISendMessage
    {
        /// <summary>
        /// Send a comment to the session group.
        /// </summary>
        /// <param name="parentMessage"></param>
        /// <returns></returns>
        Task SendCommentToGroupAsync(Message parentMessage);

        /// <summary>
        /// Send a message to the session group.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageToGroupAsync(Message message);

        /// <summary>
        /// Send a trigger (pause, stop etc) to participants. 
        /// </summary>
        /// <param name="sessionTrigger"></param>
        /// <param name="sessionGroup"></param>
        /// <returns></returns>
        Task<Task> SendSessionTriggerToAllAsync(SessionTrigger sessionTrigger, SessionGroup sessionGroup);
        
        /// <summary>
        /// Notify clients about the progress.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="sessionGroup"></param>
        /// <returns></returns>
        Task SendCurrentSessionProgressAsync(double progress, SessionGroup sessionGroup);
        
        /// <summary>
        /// Notify the facilitator that a client has joined.
        /// </summary>
        /// <param name="sessionGroup"></param>
        /// <returns></returns>
        Task SendParticipantJoinedToFacilitatorAsync(SessionGroup sessionGroup);
        
        /// <summary>
        /// Send ScenarioEvents to the facilitator.
        /// </summary>
        /// <param name="sessionGroup"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendEventToFacilitatorAsync(SessionGroup sessionGroup, Message message);
        
        /// <summary>
        /// Send new comments to the facilitator.
        /// </summary>
        /// <param name="sessionGroup"></param>
        /// <param name="parentMessage"></param>
        /// <returns></returns>
        Task SendCommentToFacilitatorAsync(SessionGroup sessionGroup, Message parentMessage);

        /// <summary>
        /// Show a message for the SessionGroup
        /// </summary>
        /// <param name="sessionLog"></param>
        /// <returns></returns>
        Task ShowForGroupAsync(SessionLog sessionLog);

        /// <summary>
        /// Notify all facilitators about the new stresslevel.
        /// </summary>
        /// <param name="stressLevel"></param>
        /// <param name="sessionGroup"></param>
        Task SendStressLevelToFacilitatorAsync(uint stressLevel, SessionGroup sessionGroup);
        
    }
}