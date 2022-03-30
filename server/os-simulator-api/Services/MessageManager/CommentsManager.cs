using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Remotion.Linq.Clauses;
using Serilog;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.Comments;
using SomeSimulator.Data.Models.Configurations;
using SoMeSimulator.Data.Models.Defaults;
using SoMeSimulator.Data.Models.SessionLogs;
using SoMeSimulator.Data.Models.Types;
using SoMeSimulator.Helpers;
using SoMeSimulator.Services.MessageManager.Dto;
using SoMeSimulator.Services.SignalR;
using SoMeSimulator.Services.TimeCalc;

namespace SoMeSimulator.Services.MessageManager
{
    public class CommentsManager : IManager
    {
        private readonly ISendMessage _sendMessage;
        private readonly SoMeContext _dbContext;
        private readonly IEntityFactory _factory;
        private readonly IStressLevelCalculator _stressLevelCalculator;
        private readonly IOptions<CommentsSettings> _commentsSettings;

        public CommentsManager(ISendMessage sendMessage, SoMeContext dbContext, IEntityFactory factory, 
        IStressLevelCalculator stressLevelCalculator, IOptions<CommentsSettings> commentsSettings)
        {
            _sendMessage = sendMessage;
            _dbContext = dbContext;
            _factory = factory;
            _stressLevelCalculator = stressLevelCalculator;
            _commentsSettings = commentsSettings;
        }

        /// <summary>
        /// Determines if a post should be posted.
        /// </summary>
        /// <param name="sessionGroup"></param>
        public async Task Send(SessionGroup sessionGroup)
        {
            foreach (var session in sessionGroup.Sessions)
            {
                await Handle(session);
            }
        }

        private async Task Handle(Session session)
        {
            var sessionLogs = CommentableSessionLog(session).ToList();

            if (!sessionLogs.Any()) return;

            foreach (var toComment in sessionLogs)
            {
                
                if (_stressLevelCalculator.ShouldComment(session.SessionGroup.StressLevel))
                {
                    Log.Debug("A message is selected to receive a comment.");
                    var comment = NotSentComment(session);
                    if (comment != null)
                    {
                        var commentParent = toComment.Level == 1 ? toComment : toComment.Parent;

                        CreateSessionLog(session, comment, commentParent);

                        if (toComment.Level == 3)
                        {
                            var commentGrandParent = commentParent.Parent;
                            commentParent = commentGrandParent;
                        }

                        await Post(commentParent);
                    }
                }
            }
        }

        private async Task Post(SessionLog sessionLog)
        {
            var message = new Message()
            {
                SessionGroup = sessionLog.Session.SessionGuid.ToString(),
                MessageCount = sessionLog.Session.CountSessionLogs(),
                SessionLog = sessionLog
            };
            
            await _sendMessage.SendCommentToGroupAsync(message);
            await _sendMessage.SendCommentToFacilitatorAsync(sessionLog.Session.SessionGroup, message);
        }

        private IEnumerable<SessionLog> CommentableSessionLog(Session session)
        {
            var sessionLogs = session.SessionLogs
                .Where(sl => sl.Type == MessageType.Participant)
                .Where(sl =>
                    sl.SendDateTime < DateTime.Now.AddSeconds(-_commentsSettings.Value.TimeWindow.Start) && sl
                        .SendDateTime >
                    DateTime.Now.AddSeconds
                        (-_commentsSettings.Value.TimeWindow.Stop));

            var result1 = sessionLogs.Where(sl => sl.Parent != null && sl.Parent.Children.Count(s =>
                                                      s.Type == MessageType.Comment
                                                      && s.CommentId != null
                                                      && s.BotReplyProperties.HasFlag(BotReplyProperties.Neutral)) <
                                                  _commentsSettings.Value.CommentsPerPost);

            var result2 = sessionLogs.Where(sl => sl.Parent == null && sl.Children.Count(s =>
                                                             s.Type == MessageType.Comment
                                                             && s.CommentId != null
                                                             && s.BotReplyProperties.HasFlag(BotReplyProperties.Neutral)) <
                                                         _commentsSettings.Value.CommentsPerPost);
            
            Log.Debug("Commentable parents: " + result2.Count() + " - Commentable children: " + result1.Count() );
            
            return result1.Union(result2).Distinct();
        }

        private SessionLog CreateSessionLog(Session session, Comment comment, SessionLog parent)
        {

            //var session = _dbContext.Sessions.FindByGuid(incomingComment.SessionGroup);
            //var parentSessionLog = _dbContext.SessionLogs.FirstOrDefault(s => s.Id == incomingComment.SessionLogId);

            //if (!session.SessionGroup.IsRunning()) return;
            //if (parentSessionLog == null) return;

            //parentSessionLog.Children.Add(_factory.SessionLog(incomingComment, session, parentSessionLog));
            //await _dbContext.SaveChangesAsync();

            //if (parentSessionLog.Level == 2)
            //{
            //    var grandParentSessionLog = _dbContext.SessionLogs.FirstOrDefault(s => s.Id == parentSessionLog.ParentSessionLogId);
            //    parentSessionLog = grandParentSessionLog;
            //}

            var child = _factory.SessionLog(parent, comment, session);
            
            _dbContext.SessionLogs.Add(child);
            _dbContext.SaveChanges();

            return child;
        }

        private static Comment NotSentComment(Session session)
        { 
            var phase = new SessionGroupTimeCalc(session.SessionGroup).CurrentPhase();
            
            var comment = session.SessionGroup.Scenario.Comments
                .Where(c => c.PhaseLink.Any(pl => pl.Phase == phase) && c.Props.HasFlag(CommentProperties.Neutral))
                .Where(
                    c => !session
                        .SessionLogs
                        .Select(sl => sl.CommentId)
                        .Contains(c.Id)
                )
                .PickRandom();

            return comment;
        }
    }
}