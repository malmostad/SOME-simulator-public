using System;
using System.Collections.Generic;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.Comments;
using SoMeSimulator.Data.Models.SessionLogs;
using SoMeSimulator.Services.MessageManager.Dto;

namespace SoMeSimulator.Data
{
    public class PersonDto
    {
        public PersonDto(string userName, string avatar)
        {
            UserName = userName;
            Avatar = avatar;
        }

        public string UserName { get; private set; }
        public string Avatar { get; private set; }
    }

    public interface IEntityFactory
    {
        /// <summary>
        /// Creates a new SessionGroup
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="groupName"></param>
        /// <param name="duration"></param>
        /// <param name="facilitator"></param>
        /// <returns></returns>
        SessionGroup SessionGroup(Scenario scenario, string groupName, TimeSpan duration, Usr facilitator);

        /// <summary>
        /// Creates a new Session entity
        /// </summary>
        /// <param name="sessionGroup"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        Session Session(SessionGroup sessionGroup, string participant);

        /// <summary>
        /// Creates a new Phase entity
        /// </summary>
        /// <param name="description"></param>
        /// <param name="startPercent"></param>
        /// <returns></returns>
        Phase Phase(string description, double startPercent);

        /// <summary>
        /// Creates a new ScenarioEvent
        /// </summary>
        /// <param name="timePercent"></param>
        /// <param name="sender"></param>
        /// <param name="heading"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        ScenarioEvent ScenarioEvent(double timePercent, string sender, string heading, string text);

        /// <summary>
        /// Creates a new post Entity
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sender"></param>
        /// <param name="avatarUrl"></param>
        /// <param name="messageFlow"></param>
        /// <param name="phases"></param>
        /// <returns></returns>
        Post Post(string text, string sender, string avatarUrl, MessageFlow messageFlow, IEnumerable<Phase> phases);

        /// <summary>
        /// Creates a new SessionLog from a comment.
        /// </summary>
        /// <param name="sessionLogToComment"></param>
        /// <param name="comment"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        SessionLog SessionLog(SessionLog sessionLogToComment, Comment comment, Session session);

        /// <summary>
        /// Creates a SessionLog for an ScenarioEvent
        /// </summary>
        /// <param name="scenarioEvent"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        SessionLog SessionLog(ScenarioEvent scenarioEvent, Session session);

        /// <summary>
        /// Creates a SessionLog for an incoming message.
        /// </summary>
        /// <param name="incomingMessage"></param>
        /// <param name="session"></param>
        /// <param name="messageFlow"></param>
        /// <returns></returns>
        SessionLog SessionLog(IncomingMessage incomingMessage, Session session, MessageFlow messageFlow);

        /// <summary>
        /// Creates an SessionLog from a post.
        /// </summary>
        /// <param name="post"></param>
        /// <param name="session"></param>
        /// <param name="messageFlow"></param>
        /// <returns></returns>
        SessionLog SessionLog(Post post, Session session, MessageFlow messageFlow);

        /// <summary>
        /// Creates a SessionLog for an incoming comment.
        /// </summary>
        /// <param name="incomingComment"></param>
        /// <param name="session"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        SessionLog SessionLog(IncomingComment incomingComment, Session session, SessionLog parent);

        /// <summary>
        /// Creates a comment
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Comment Comment(string text);

        /// <summary>
        /// Creates a user with a hashed password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="userService"></param>
        /// <returns></returns>
        Usr User(string email, string password, IUserService userService);

        /// <summary>
        /// Creates a comment
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="text"></param>
        /// <param name="commentProp"></param>
        /// <param name="messageFlow"></param>
        /// <param name="phases"></param>
        /// <param name="personDto"></param>
        /// <returns></returns>
        Comment Comment(Scenario scenario, string text, CommentProperties commentProp, MessageFlow messageFlow,
            IEnumerable<Phase> phases, PersonDto personDto);

    }
}