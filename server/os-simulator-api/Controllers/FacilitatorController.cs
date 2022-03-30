using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.SessionGroups.Commands;
using SoMeSimulator.Data.Models.SessionGroups.Status;
using SoMeSimulator.Data.Models.Types;
using SoMeSimulator.Helpers;
using SoMeSimulator.Services.MessageManager.Dto;
using SoMeSimulator.Services.SignalR;
using SoMeSimulator.Data.Models.SessionLogs;
using SoMeSimulator.Data.Models.Comments;
using SomeSimulator.Services.FakeAliasService;
using SoMeSimulator.Services.MessageManager;
using SoMeSimulator.Services.TimeCalc;

namespace SomeSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public partial class FacilitatorController : ControllerBase
    {
        private readonly SoMeContext _dbContext;
        private readonly IEntityFactory _factory;
        private readonly ISendMessage _sendMessage;
        private readonly IUserService _userService;
        private readonly IFakeAlias _fakeAlias;
        private readonly PostsManager _postManager;
        private ScenarioEventsManager _scenarioEcentManager;

        public FacilitatorController(SoMeContext dbContext, IEntityFactory factory, ISendMessage sendMessage, 
        IStressLevelCalculator stressLevelCalculator, IUserService userService, IFakeAlias fakeAlias)
        {
            _dbContext = dbContext;
            _factory = factory;
            _sendMessage = sendMessage;
            _userService = userService;
            _fakeAlias = fakeAlias;
            _postManager = new PostsManager(_sendMessage, _dbContext, _factory, stressLevelCalculator);
            _scenarioEcentManager = new ScenarioEventsManager(_sendMessage, _dbContext, _factory);
        }

        [Authorize(Roles = "Facilitator")]
        [HttpGet]
        [Route("getAlias")]
        public IActionResult GetAlias()
        {
            try
            {
                var alias = _fakeAlias.GenerateAlias();

                return Ok(new {Alias = alias});
            }
            catch (Exception)
            {
                return BadRequest();
            }
            
        }

        [Authorize(Roles = "Facilitator")]
        [HttpPost]
        [Route("leave")]
        public IActionResult Leave()
        {
            var usr = _userService.GetLoggedInUsr();

            if (usr == null || usr.ActiveSessionGroup == null) return NotFound();
            
            new Uncouple().Execute(usr.ActiveSessionGroup);

            _dbContext.SaveChanges();
            
            return Ok();
        } 
        
        [HttpGet]
        [Route("scenarios")]
        public IActionResult Scenarios()
        {
            try
            {
                var scenarios = _dbContext.Scenarios.ToList();
                return Ok(
                    scenarios
                );
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Used be the facilitator to create a SessionGroup.
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
        [Authorize(Roles = "Facilitator")]
        [HttpPost]
        [Route("create")]
        public IActionResult CreateSessionGroup(CreateSessionGroupPost postData)
        {
            try
            {
                var scenario = GetScenario(postData.ScenarioId);
                var sessionGroup = CreateSessionGroup(scenario, postData);
                
                if (sessionGroup == null)
                    return StatusCode(500);    

                _dbContext.SessionGroups.Add(sessionGroup);
                _dbContext.SaveChanges();
                Log.Debug($"Creating session: {sessionGroup.Id}");

                return Ok(sessionGroup);
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                return StatusCode(500);
            }
        }

        private SessionGroup CreateSessionGroup(Scenario scenario, CreateSessionGroupPost postData)
        {
            var usr = _userService.GetLoggedInUsr();

            if (usr == null) return null;
            
                
            var sessionGroup =
                _factory.SessionGroup(scenario, postData.GroupName, TimeSpan.FromMinutes(postData.Minutes), usr);

            while (_dbContext.SessionGroups.Any(s =>
                s.TypeableCode == sessionGroup.TypeableCode && s.Id != sessionGroup.Id))
                sessionGroup.TypeableCode = AlphaNumericCode.Generate();

            return sessionGroup;
        }

        private Scenario GetScenario(int scenarioId)
        {
            return _dbContext.Scenarios.FindById(scenarioId);
        }

        [Authorize(Roles = "Facilitator")]
        [HttpGet]
        [Route("scenario")]
        public IActionResult Scenario()
        {
            try
            {
                var usr = _userService.GetLoggedInUsr();

                if (usr == null)
                    return Unauthorized();
                
                if(usr.ActiveSessionGroup == null) 
                    return NotFound();

                var sessionGroup = usr.ActiveSessionGroup;

                return Ok(
                    new ScenarioDto
                    {
                        Scenario = sessionGroup.Scenario,
                        Phases = GetPhases(sessionGroup.Scenario),
                        Events = GetEvents(sessionGroup).ToList()
                    }
                );
            }
            catch (Exception e)
            { 
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Facilitator")]
        [Route("GetSessionGroup/")]
        public IActionResult GetSessionGroup()
        {
            var usr = _userService.GetLoggedInUsr();

            if (usr == null)
                return Unauthorized();

            if(usr.ActiveSessionGroup == null)
                return NotFound();

            return  new JsonResult(usr.ActiveSessionGroup);

        }


        private IEnumerable<EventTimeDto> GetEvents(SessionGroup sessionGroup)
        {
            var phases = sessionGroup.Scenario.Phases.OrderBy(p => p.StartPercent);
            var timeSum = 0.0;

            for (var i = 0; i < phases.Count(); i++)
            {
                var phase = phases.ElementAt(i);

                var phaseDuration = 0.0;

                if (i < phases.Count() - 1)
                    phaseDuration = phases.ElementAt(i + 1).StartPercent - phase.StartPercent;
                else
                    phaseDuration = 1 - phase.StartPercent;

                foreach (var scenarioEvent in phase.ScenarioEvents)
                    yield return new EventTimeDto
                    {
                        Heading = scenarioEvent.Heading,
                        ProgressPoint = phaseDuration * scenarioEvent.TimePercent + timeSum
                    };

                timeSum += phaseDuration;
            }
        }

        private IEnumerable<PhaseTimeDto> GetPhases(Scenario scenario)
        {
            var phases = scenario
                .Phases
                .OrderBy(p => p.StartPercent).ToList();

            for (var i = 0; i < phases.Count(); i++)
                yield return new PhaseTimeDto
                {
                    Heading = phases[i].Description,
                    Start = phases[i].StartPercent,
                    End = i < phases.Count - 1 ? phases[i + 1].StartPercent : 1
                };
        }
        
        [Authorize(Roles = "Facilitator")]
        [HttpPost]
        [Route("start/{sessionGroupId}")]
        public async Task<ActionResult<string>> Start(int? sessionGroupId)
        {
            try
            {
                var sessionGroup = _dbContext.SessionGroups.Single(s => s.Id == sessionGroupId);

                new Start().Execute(sessionGroup);

                var guid = string.Join(" ", sessionGroup.Sessions.Select(s => s.SessionGuid));

                Log.Debug($"Starting sessions: {guid}");

                _dbContext.SaveChanges();

                await _sendMessage.SendSessionTriggerToAllAsync(
                    new SessionTrigger(new Dialog(true, "Övningen har startat", "", "Ok"), sessionGroup.Status),
                    sessionGroup);

                await _postManager.Send(sessionGroup, true);
                await _scenarioEcentManager.Send(sessionGroup);

                return guid;
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                return StatusCode(500);
            }
        }
        [Authorize(Roles = "Facilitator")]
        [HttpPost]
        [Route("stop/{sessionGroupId}")]
        public async Task<ActionResult<string>> Stop(int? sessionGroupId)
        {
            var sessionGroup = _dbContext.SessionGroups.FirstOrDefault(s =>
                (s.Status == SessionStatus.Running || 
                 s.Status == SessionStatus.Paused || 
                 s.Status == SessionStatus.New) 
                && s.Id == sessionGroupId);

            new Stop().Execute(sessionGroup);

            _dbContext.SaveChanges();

            Log.Debug($"Stopped : {sessionGroup.Id} at {sessionGroup.StopDate}");

            await _sendMessage.SendSessionTriggerToAllAsync(
                new SessionTrigger(new Dialog(true, "Övningen är stoppad", "", "Ok"),
                    sessionGroup.Status), sessionGroup);

            return string.Join(" ", sessionGroup.Sessions.Select(s => s.SessionGuid));
        }
        
        [Authorize(Roles = "Facilitator")]
        [HttpPost]
        [Route("cancel/{sessionGroupId}")]
        public async Task<ActionResult<string>> Cancel(int? sessionGroupId)
        {
            var sessionGroup =
                _dbContext.SessionGroups.FirstOrDefault(s => s.Status == SessionStatus.New && s.Id == sessionGroupId);

            new Cancel().Execute(sessionGroup);

            _dbContext.SaveChanges();

            Log.Debug($"Cancelled : {sessionGroup.Id} at {sessionGroup.StopDate}");

            await _sendMessage.SendSessionTriggerToAllAsync(
                new SessionTrigger(new Dialog(true, "Övningen avbröts", "", "Ok"),
                    sessionGroup.Status), sessionGroup);

            return string.Join(" ", sessionGroup.Sessions.Select(s => s.SessionGuid));
        }
        
        [Authorize(Roles = "Facilitator")]
        [HttpPost]
        [Route("Pause/{sessionGroupId}")]
        public async Task<IActionResult> Pause(int? sessionGroupId, bool notifyParticipants = true)
        {
            try
            {
                var sessionGroup =
                    _dbContext.SessionGroups.First(s => s.Status == SessionStatus.Running && s.Id == sessionGroupId);

                if (sessionGroup == null) return NotFound();

                new Pause().Execute(sessionGroup);
                _dbContext.SaveChanges();

                Log.Debug($"Paused : {sessionGroup.Id} at ${sessionGroup.PauseStart}");

                Dialog dialog = null;
                
                dialog = new Dialog(notifyParticipants == true, "Övningen är pausad", "", "Ok");

                await _sendMessage.SendSessionTriggerToAllAsync(
                    new SessionTrigger(dialog, sessionGroup.Status), sessionGroup);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }
        
        [Authorize(Roles = "Facilitator")]
        [HttpGet]
        [Route("GetActivityLogs/{sessionGroupId}")]
        public IActionResult GetAllFacilitator(int sessionGroupId)
        {
            try
            {
                var sessionGroup = _dbContext.SessionGroups.FindById(sessionGroupId);
                
                if(sessionGroup == null) return NotFound();

                var sessionLogs = ParticipantRelatedSessionLogs(sessionGroup);
                
                var rootSessionLogs = GetRootSessionLogs(sessionLogs).OrderByDescending(s => s.SendDateTime);
                
                return Ok(rootSessionLogs);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }
        
        [Authorize(Roles = "Facilitator")]
        [HttpGet]
        [Route("SetStressLevel/{sessionGroupId}/{stressLevel}")]
        public IActionResult SetStressLevel(int sessionGroupId, uint stressLevel)
        {
            try
            {
                var sessionGroup = _dbContext.SessionGroups.FindById(sessionGroupId);

                sessionGroup.StressLevel = stressLevel;
                
                _dbContext.SaveChanges();

                _sendMessage.SendStressLevelToFacilitatorAsync(stressLevel, sessionGroup);
                
                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [Authorize(Roles = "Facilitator")]
        [HttpGet]
        [Route("GetEvents/{sessionGroupId}")]
        public IActionResult GetEvents(int sessionGroupId)
        {
            try
            {
                var sessionGroup = _dbContext.SessionGroups.FindById(sessionGroupId);
                
                if(sessionGroup == null) {
                    return NotFound();
                }

                if (!sessionGroup.Sessions.Any())
                {
                    return Ok(Enumerable.Empty<SessionLog>());
                }

                return UniqueEvents(sessionGroup);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        private IActionResult UniqueEvents(SessionGroup sessionGroup)
        {
            var session = sessionGroup.Sessions.First();

            var sessionLogs = session.SessionLogs
                .Where(s => s.Type == MessageType.ScenarioEvent || s.Type == MessageType.Message)
                .OrderByDescending(s => s.SendDateTime);

            return Ok(sessionLogs);
        }

        private static IEnumerable<SessionLog> GetRootSessionLogs(IEnumerable<SessionLog> sessionLogs)
        {
            return sessionLogs.Select(s => s.Root()).Distinct();
        }

        private static IEnumerable<SessionLog> ParticipantRelatedSessionLogs(SessionGroup sessionGroup)
        {
            var allSessionLogs =  sessionGroup.Sessions.SelectMany(s => s.SessionLogs).ToList();
            var participantsSessionLogs = allSessionLogs
                .Where(s => s.Type == MessageType.Participant)
                .OrderBy(s => s.SendDateTime);
            return participantsSessionLogs;
        }

        [Authorize(Roles = "Facilitator")]
        [HttpPost]
        [Route("UnPause/{sessionGroupId}")]
        public async Task<IActionResult> UnPause(int? sessionGroupId)
        {
            try
            {
                var sessionGroup =
                    _dbContext.SessionGroups.FirstOrDefault(s =>
                        s.Status == SessionStatus.Paused && s.Id == sessionGroupId);

                if (sessionGroup == null) return NotFound();

                new UnPause().Execute(sessionGroup);
                _dbContext.SaveChanges();

                Log.Debug($"Session running again: {sessionGroup.Id} at ${DateTime.Now}");

                await _sendMessage.SendSessionTriggerToAllAsync(
                    new SessionTrigger(new Dialog(true, "Övningen har återupptagits", "", "Ok"),
                        SessionStatus.Running), sessionGroup);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [Authorize(Roles = "Facilitator")]
        [HttpPost]
        [Route("addSessionLogAction")]
        public async Task<IActionResult> AddSessionLogActionAsync(SessionLogAction sessionLogAction)
        {
            try
            {
                var sessionLog = _dbContext.SessionLogs.FirstOrDefault(s => s.Id == sessionLogAction.SessionLogId);

                if (sessionLog == null) return NotFound();

                var shouldSend = !sessionLogAction.BotReplyProperties.HasFlag(sessionLog.BotReplyProperties);
                sessionLog.BotReplyProperties = sessionLogAction.BotReplyProperties;
                sessionLog.Tag = sessionLogAction.SessionLogTag;
                
                if(shouldSend) {
                    PickComment(sessionLog, sessionLogAction);
                }
                
                await Send(sessionLog, sessionLog.Session);
                

                _dbContext.Update(sessionLog);
                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        private void PickComment(SessionLog parentSessionLog, SessionLogAction sessionLogAction)
        {
            var props = sessionLogAction.BotReplyProperties == BotReplyProperties.Negative? CommentProperties.Negative: CommentProperties.Positive;
            var session = parentSessionLog.Session;
            var phase = new SessionGroupTimeCalc(session.SessionGroup).CurrentPhase();

            
            Comment comment = session.SessionGroup.Scenario.Comments
                .Where(c => c.PhaseLink.Any(pl => pl.Phase == phase))
                .Where(
                    c => c.Props.HasFlag(props) && !session.SessionLogs.Where(sl => sl.Type == MessageType.Comment)
                    .Select(s => s.CommentId).Contains(c.Id)
                ).PickRandom();

            if(comment == null) return;
 
            var commentSessionLog = _factory.SessionLog(parentSessionLog, comment, parentSessionLog.Session);
        }

        private async Task Send(SessionLog parent, Session session) {
            var message = new Message()
                {
                    SessionGroup = parent.Session.SessionGuid.ToString(),
                    MessageCount = parent.Session.CountSessionLogs(),
                    SessionLog = parent,
                };

            var send1 = _sendMessage.SendCommentToGroupAsync(message);
            var send2 =  _sendMessage.SendCommentToFacilitatorAsync(session.SessionGroup, message);
            
            await send1;
            await send2;
        }
    }
}