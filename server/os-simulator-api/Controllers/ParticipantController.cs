using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using SomeSimulator.Controllers;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models.SessionGroups.Status;
using SoMeSimulator.Services.SignalR;

namespace SoMeSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly SoMeContext _dbContext;
        private readonly IEntityFactory _factory;
        private readonly ISendMessage _sendMessage;

        public ParticipantController(SoMeContext dbContext, IEntityFactory factory, ISendMessage sendMessage)
        {
            _dbContext = dbContext;
            _factory = factory;
            _sendMessage = sendMessage;
        }

        [HttpPost]
        [Route("join")]
        public async Task<IActionResult> Join(FacilitatorController.JoinSessionPost participation)
        {
            try
            {
                if (participation.Participant.IsNullOrEmpty()) throw new Exception("No participant.");
                if (participation.TypeableCode == null) throw new Exception("No such SessionGroup.");


                var sessionGroup = _dbContext.SessionGroups.SingleOrDefault(sg =>
                    sg.TypeableCode == participation.TypeableCode.ToUpper() && sg.Status == SessionStatus.New);

                if (sessionGroup == null)
                    return StatusCode(400);

                var session = _factory.Session(sessionGroup, participation.Participant);

                _dbContext.Add(session);
                _dbContext.SaveChanges();

                Log.Debug($"Creating session: {session.SessionGuid}");

                await _sendMessage.SendParticipantJoinedToFacilitatorAsync(sessionGroup);

                return Ok(new JoinDto
                {
                    SessionGuid = session.SessionGuid.ToString(),
                    Duration = session.SessionGroup.Duration,
                });
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public IActionResult Get(Guid guid)
        {
            try
            {
                Log.Debug($"Get All for : {guid}");
                var session = _dbContext.Sessions.FindByGuid(guid);

                if (session == null) return new NotFoundResult();

                var sessionLogs = session.SessionLogs.Where(s => s.Level == 1);
                return new JsonResult(sessionLogs, new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetNewSessionGroups")]
        public IActionResult GetNewSessionGroups()
        {
            try
            {
                var sessionGroups = _dbContext.SessionGroups.Where(s =>
                    s.Status == SessionStatus.New);

                return Ok(sessionGroups);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetStatus")]
        public IActionResult GetStatus(Guid guid)
        {
            try
            {
                Log.Debug($"Get All for : {guid}");
                var session = _dbContext.Sessions.FindByGuid(guid);

                if (session == null) return new NotFoundResult();

                return Ok(new {status = Enum.GetName(typeof(SessionStatus), session.SessionGroup.Status)});
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }
    }
}