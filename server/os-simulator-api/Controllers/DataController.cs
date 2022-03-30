using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Antiforgery.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.Comments;
using SoMeSimulator.Data.Models.Defaults;
using SoMeSimulator.Data.Models.SessionLogs;

namespace SomeSimulator.Controllers
{
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly SoMeContext _dbContext;
        private readonly IEntityFactory _factory;
        private readonly IUserService _userService;

        public DataController(SoMeContext dbContext, IEntityFactory entityFactory, IUserService userService)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == EnvironmentName.Development || environment == EnvironmentName.Staging)
            {
                _dbContext = dbContext;
                _factory = entityFactory;
                _userService = userService;
            }
            else
                throw new NotSupportedException();
        }

        [HttpGet]
        [Route("CreateUsers")]
        public async Task<IActionResult> CreateUsers()
        {
            if (_dbContext.Users.Any())
                return Ok("Aborted. Users already created.");

            var facilitatorRole = new Role { Name = "Facilitator" };
            _dbContext.Roles.Add(facilitatorRole);

            var contentRole = new Role { Name = "Content" };
            _dbContext.Roles.Add(contentRole);

            var adminRole = new Role { Name = "Admin" };
            _dbContext.Roles.Add(adminRole);

            await _dbContext.SaveChangesAsync();


            var usr1 = _factory.User("admin", "admin1234", _userService);
            _dbContext.Users.Add(usr1);
            var usr2 = _factory.User("facilitator", "fac1234", _userService);
            _dbContext.Users.Add(usr2);
            var usr3 = _factory.User("content", "content1234", _userService);
            _dbContext.Users.Add(usr3);

            await _dbContext.SaveChangesAsync();

            usr1.UserRoles.Add(new UserRole { Role = adminRole });
            usr1.UserRoles.Add(new UserRole { Role = facilitatorRole });

            usr2.UserRoles.Add(new UserRole { Role = facilitatorRole });
            usr3.UserRoles.Add(new UserRole { Role = contentRole });

            await _dbContext.SaveChangesAsync();

            return Ok(
                $"Created users and roles {string.Join(", ", _dbContext.Users.Select(u => u.Username).ToArray())}");
        }

        #region Scenario1
        
        [HttpGet]
        [Route("create")]
        public ActionResult<string> Create()
        {
            var message = "";

            var scenario = new Scenario
            {
                Name = "Name of scenario",
                Description = "Description of scenario"
            };

            _dbContext.Scenarios.Add(scenario);


            CreateScenarioEvents(scenario);
            CreatePosts(scenario);
            CreateComments(scenario);

            message += $" - Created scenario with {scenario.Phases.Count()} phases.";

            _dbContext.Scenarios.Add(scenario);

            _dbContext.SaveChanges();

            return message;
        }

        private void CreateComments(Scenario scenario)
        {
            
            var ps = scenario.Phases.OrderBy(s => s.StartPercent).ToList();

            var p1 = ps.ElementAt(0);
            var p2 = ps.ElementAt(1);
            var p3 = ps.ElementAt(2);
            var p4 = ps.ElementAt(3);
            
            
            var commentDtos = new List<CommentDto>
            {
                new CommentDto("Comment1", CommentProperties.Negative, MessageFlow.Short|MessageFlow.Long, p1),
                new CommentDto("Comment2", CommentProperties.Negative, MessageFlow.Short|MessageFlow.Long, p1,p2),
                new CommentDto("Comment4", CommentProperties.Negative, MessageFlow.Short|MessageFlow.Long, p1,p2,p4),
                new CommentDto("Comment4", CommentProperties.Negative, MessageFlow.Short|MessageFlow.Long, p1,p2,p3,p4)
            };
                

            foreach (var commentDto in commentDtos)
            {
                var fakePerson = new Person("sv");

                var personDto = new PersonDto(fakePerson.UserName, "/circle.svg");

                var comment = _factory.Comment( scenario, commentDto.Text, commentDto.Properties, commentDto.MessageFlow, commentDto.Phases, personDto);

                scenario.Comments.Add(comment);

                _dbContext.Comments.Add(comment);
            }
        }

        private void CreatePosts(Scenario scenario)
        {
            var ps = scenario.Phases.OrderBy(s => s.StartPercent).ToList();

            var p1 = ps.ElementAt(0);
            var p2 = ps.ElementAt(1);
            var p3 = ps.ElementAt(2);
            var p4 = ps.ElementAt(3);

            var createPostDtos = new List<CreatePostDto>()
            {
                new CreatePostDto("Post1", MessageFlow.Short|MessageFlow.Long, p1),
                new CreatePostDto("Post2", MessageFlow.Short|MessageFlow.Long, p1,p2),
                new CreatePostDto("Post3", MessageFlow.Short|MessageFlow.Long, p1,p2,p3),
                new CreatePostDto("Post4", MessageFlow.Short|MessageFlow.Long, p1,p2,p3,p4)
            };

            foreach (var createPostDto in createPostDtos)
            {
                var fakePerson = new Person(DefaultValues.FakeLocale);
                var post = _factory.Post(createPostDto.Text, fakePerson.UserName, "/circle.svg",
                    createPostDto.MessageFlow, createPostDto.Phases);
                _dbContext.Posts.Add(post);
            }
        }

        private void CreateScenarioEvents(Scenario scenario)
        {
            #region p1
            var phase1 = _factory.Phase("Fas 1", 0);
            phase1.ScenarioEvents.Add(_factory.ScenarioEvent(0, "Sender",
                "Heading ScenarioEvent1 Phase1",
                "Content ScenarioEvent1 Phase1"));

            phase1.ScenarioEvents.Add(_factory.ScenarioEvent(0.5, "Sender",
                "Heading ScenarioEvent2 Phase1",
                "Content ScenarioEvent2 Phase1"));
            #endregion

            #region p2

            var phase2 = _factory.Phase("Fas 2", 0.25);
            phase2.ScenarioEvents.Add(_factory.ScenarioEvent(0, "Sender",
                "Heading ScenarioEvent1 Phase2",
                "Content ScenarioEvent1 Phase2"));

            phase2.ScenarioEvents.Add(_factory.ScenarioEvent(0.5, "Sender",
                 "Heading ScenarioEvent2 Phase2",
                 "Content ScenarioEvent2 Phase2"));
            #endregion

            #region p3

            var phase3 = _factory.Phase("Fas 3", 0.5);
            phase3.ScenarioEvents.Add(_factory.ScenarioEvent(0, "Sender",
                "Heading ScenarioEvent1 Phase3",
                "Content ScenarioEvent1 Phase3"));

            phase3.ScenarioEvents.Add(_factory.ScenarioEvent(0.5, "Sender",
                "Heading ScenarioEvent2 Phase3",
                "Content ScenarioEvent2 Phase3"));
            #endregion

            #region p4

            var phase4 = _factory.Phase("Fas 4", 0.75);
            phase4.ScenarioEvents.Add(_factory.ScenarioEvent(0, "Sender",
                "Heading ScenarioEvent1 Phase4",
                "Content ScenarioEvent1 Phase4"));

            phase4.ScenarioEvents.Add(_factory.ScenarioEvent(0.5, "Sender",
                "Heading ScenarioEvent2 Phase4",
                "Content ScenarioEvent2 Phase4"));

            #endregion

            scenario.Phases.Add(phase1);
            scenario.Phases.Add(phase2);
            scenario.Phases.Add(phase3);
            scenario.Phases.Add(phase4);
        }

        #endregion


        [HttpGet]
        [Route("truncate/")]
        public ActionResult<string> Truncate()
        {
            _dbContext.Users.RemoveRange(_dbContext.Users.ToList());
            _dbContext.UserRoles.RemoveRange(_dbContext.UserRoles.ToList());
            _dbContext.Roles.RemoveRange(_dbContext.Roles.ToList());
            
            
            _dbContext.SessionGroups.RemoveRange(_dbContext.SessionGroups.ToList());
            _dbContext.Sessions.RemoveRange(_dbContext.Sessions.ToList());
            _dbContext.SessionLogs.RemoveRange(_dbContext.SessionLogs.ToList());

            _dbContext.PhasePosts.RemoveRange(_dbContext.PhasePosts.ToList());
            _dbContext.PhaseComments.RemoveRange(_dbContext.PhaseComments.ToList());
            
            _dbContext.Scenarios.RemoveRange(_dbContext.Scenarios.ToList());
            _dbContext.Phases.RemoveRange(_dbContext.Phases.ToList());
            _dbContext.ScenarioEvents.RemoveRange(_dbContext.ScenarioEvents.ToList());
            _dbContext.Posts.RemoveRange(_dbContext.Posts.ToList());

            _dbContext.Comments.RemoveRange(_dbContext.Comments.ToList());


            _dbContext.SaveChanges();

            return "Removed all rows.";
        }

        private class CommentDto
        {
            public CommentDto(string text, CommentProperties properties, MessageFlow messageFlow, params Phase[] phases)
            {
                Properties = properties;
                Text = text;
                MessageFlow = messageFlow;
                Phases = phases;
            }

            public Phase[] Phases { get; set; }
            public string Text { get; }
            public string Sender { get; set; }
            public CommentProperties Properties { get; }
            public MessageFlow MessageFlow { get; }
        }

        private class CreatePostDto
        {
            public CreatePostDto(string text, MessageFlow messageFlow, params Phase[] phases)
            {
                Phases = phases.ToList();
                Text = text;
                MessageFlow = messageFlow;
            }

            public string Text { get; }
            public MessageFlow MessageFlow { get; }
            public IEnumerable<Phase> Phases { get; }
        }
    }
}