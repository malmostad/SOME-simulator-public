using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Remotion.Linq.Clauses;
using Serilog;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SomeSimulator.DTOs;
using SomeSimulator.Interfaces;
using SomeSimulator.Services;
using SoMeSimulator.Services.MessageManager;
using SoMeSimulator.Services.SignalR;

namespace SoMeSimulator.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ScenarioController: ControllerBase
    {
        private readonly ICrudInterface<ScenarioDto> _service;
        public ScenarioController(ICrudInterface<ScenarioDto> service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetScenario/{id}")]
        public async Task<IActionResult> GetScenario(int id)
        {
            try
            {
                var scenario = await _service.GetByIdAsync(id);
                return Ok(scenario);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }
    }
}