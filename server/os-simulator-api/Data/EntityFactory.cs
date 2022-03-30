using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Options;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.Comments;
using SomeSimulator.Data.Models.Configurations;
using SoMeSimulator.Data.Models.SessionGroups.Status;
using SoMeSimulator.Data.Models.SessionLogs;
using SoMeSimulator.Data.Models.Types;
using SoMeSimulator.Helpers;
using SoMeSimulator.Services.MessageManager.Dto;

namespace SoMeSimulator.Data
{
    public class EntityFactory : IEntityFactory
    {
        /// <summary>
        /// Creates a new SessionGroup
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="groupName"></param>
        /// <param name="duration"></param>
        /// <param name="facilitator"></param>
        /// <returns></returns>
        public SessionGroup SessionGroup(Scenario scenario, string groupName, TimeSpan duration, Usr facilitator)
        {
            if(facilitator == null)
            {
                throw new Exception("Cant create a SessionGroup without a facilitator");
            }

            if (facilitator.ActiveSessionGroup != null 
                && !facilitator.ActiveSessionGroup.StateMachine.IsInState(SessionStatus.Finished) 
                && !facilitator.ActiveSessionGroup.StateMachine.IsInState(SessionStatus.Cancelled) 
            )
            {
                throw new Exception(
                    "Cant create a SessionGroup for a usr that already have an active SessionGroup.");
            }

            var sessionGroup = new SessionGroup()
            {
                Scenario = scenario,
                Duration = duration,
                GroupName = groupName,
                TypeableCode = AlphaNumericCode.Generate(),
                StressLevel = 50
            };
            
            facilitator.ActiveSessionGroup = sessionGroup;
            
            return sessionGroup;
        }

        /// <summary>
        /// Creates a new Comment entity
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="text"></param>
        /// <param name="commentProp"></param>
        /// <param name="messageFlow"></param>
        /// <param name="phases"></param>
        /// <param name="personDto"></param>
        /// <param name="userName"></param>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public Comment Comment(Scenario scenario, string text, CommentProperties commentProp, MessageFlow messageFlow, IEnumerable<Phase> phases, PersonDto personDto)
        {
            var comment = new Comment
            {
                Scenario = scenario,
                Text = text,
                Props = commentProp,
                Sender = personDto.UserName,
                Avatar = personDto.Avatar,
                MessageFlow = messageFlow
            };

            phases.Select(p => new PhaseComment(comment, p)).ToList().ForEach(pc => comment.PhaseLink.Add(pc));
            
            return comment;
        }

        /// <summary>
        /// Creates a new Session entity
        /// </summary>
        /// <param name="sessionGroup"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        public Session Session(SessionGroup sessionGroup, string participant)
        {
            return new Session()
            {
                SessionGroup = sessionGroup,
                SessionGuid = System.Guid.NewGuid(),
                Participant = participant,
            };
        }


        /// <summary>
        /// Creates a new Phase entity
        /// </summary>
        /// <param name="description"></param>
        /// <param name="startPercent"></param>
        /// <returns></returns>
        public Phase Phase(string description, double startPercent)
        {
            return new Phase()
            {
                
                Description = description,
                StartPercent = startPercent,
            };
        }

        /// <summary>
        /// Creates a new ScenarioEvent
        /// </summary>
        /// <param name="timePercent"></param>
        /// <param name="sender"></param>
        /// <param name="heading"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ScenarioEvent ScenarioEvent(double timePercent, string sender, string heading, string text)
        {
            return new ScenarioEvent()
            {
                Sender = sender,
                TimePercent = timePercent,
                Heading = heading,
                Text = text
            };
        }

        /// <summary>
        /// Creates a new post Entity
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sender"></param>
        /// <param name="avatarUrl"></param>
        /// <param name="messageFlow"></param>
        /// <param name="phases"></param>
        /// <returns></returns>
        public Post Post(string text, string sender, string avatarUrl,
            MessageFlow messageFlow, IEnumerable<Phase> phases)
        {
            var post = new Post()
            {
                Text = text,
                Sender = sender,
                Avatar = avatarUrl,
                MessageFlow = messageFlow
            };

            foreach (var phase in phases)
            {
                PhasePost pp = new PhasePost(post, phase);

                post.PhaseLink.Add(pp);
            }

            return post;
        }
        
        /// <summary>
        /// Creates a new SessionLog from a comment.
        /// </summary>
        /// <param name="sessionLogToComment"></param>
        /// <param name="comment"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public SessionLog SessionLog(SessionLog sessionLogToComment, Comment comment, Session session)
        {
            var sessionLogLevel = sessionLogToComment.Level + 1;

            var sessionLog = SessionLog(comment.Text, comment.Sender, sessionLogLevel,
                MessageType.Comment);

            sessionLog.Session = session;
            sessionLog.Avatar = comment.Avatar;
            sessionLog.CommentId = comment.Id;
            sessionLog.MessageFlow = sessionLogToComment.MessageFlow;
            sessionLogToComment.Children.Add(sessionLog);

            return sessionLog;
        }

        /// <summary>
        /// Creates a SessionLog for an ScenarioEvent
        /// </summary>
        /// <param name="scenarioEvent"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public SessionLog SessionLog(ScenarioEvent scenarioEvent, Session session)
        {
            var sessionLog = SessionLog(scenarioEvent.Text, scenarioEvent.Sender, 1,
                MessageType.ScenarioEvent);

            sessionLog.Heading = scenarioEvent.Heading;
            sessionLog.Session = session;
            sessionLog.ScenarioEventId = scenarioEvent.Id;
            return sessionLog;
        }

        /// <summary>
        /// Creates a SessionLog for an incoming message.
        /// </summary>
        /// <param name="incomingMessage"></param>
        /// <param name="session"></param>
        /// <param name="messageFlow"></param>
        /// <returns></returns>
        public SessionLog SessionLog(IncomingMessage incomingMessage, Session session, MessageFlow messageFlow)
        {
            return new SessionLog()
            {
                Level = 1,
                Text = incomingMessage.Text,
                Heading = incomingMessage.Heading,
                SendDateTime = DateTime.Now,
                Sender = incomingMessage.Sender,
                Type = MessageType.Participant,
                Session = session,
                MessageFlow = messageFlow
            };
        }

        /// <summary>
        /// Creates an SessionLog from a post.
        /// </summary>
        /// <param name="post"></param>
        /// <param name="session"></param>
        /// <param name="messageFlow"></param>
        /// <returns></returns>
        public SessionLog SessionLog(Post post, Session session, MessageFlow messageFlow)
        {
            var sessionLog = SessionLog(post.Text, post.Sender, 1,
                MessageType.Message);

            sessionLog.Session = session;
            sessionLog.PostId = post.Id;
            sessionLog.Avatar = post.Avatar;
            sessionLog.MessageFlow = messageFlow;
            return sessionLog;
        }

        /// <summary>
        /// Creates a SessionLog for an incoming comment.
        /// </summary>
        /// <param name="incomingComment"></param>
        /// <param name="session"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public SessionLog SessionLog(IncomingComment incomingComment, Session session, SessionLog parent)
        {
            var parentLevel = parent != null ? parent.Level + 1 : 1;

            var sessionLog = SessionLog(incomingComment.Text, incomingComment.Sender, parentLevel,
                incomingComment.MessageType, incomingComment.Avatar);

            sessionLog.Session = session;
            sessionLog.MessageFlow = parent.MessageFlow;

            return sessionLog;
        }
        
        /// <summary>
        /// Creates a sessionLog at root-level
        /// </summary>
        /// <param name="incomingComment"></param>
        /// <returns></returns>
        public SessionLog SessionLog(IncomingComment incomingComment, Session session)
        {
            return this.SessionLog(incomingComment, session, null);
        }

        private SessionLog SessionLog(string text, string sender, int level, MessageType messageType)
        {
            return this.SessionLog(text, sender, level, messageType, null);
        }

        private SessionLog SessionLog(string text, string sender, int level, MessageType messageType, string avatar)
        {
            return new SessionLog()
            {
                Text = text,
                Sender = sender,
                Type = messageType,
                Level = level,
                SendDateTime = DateTime.Now,
                Avatar = avatar,
            };
        }
        /// <summary>
        /// Create comment
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Comment Comment(string text)
        {
            return new Comment()
            {
                Text = text
            };
        }

        /// <summary>
        /// Creates a user with a hashed password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="userService"></param>
        /// <returns></returns>
        public Usr User(string email, string password, IUserService userService)
        {

            var usr = new Usr() { Username = email, Password = userService.NewHash(password) };
            return usr;
        }
    }
}
