using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SomeSimulator.DTOs;
using SomeSimulator.Interfaces;
using SomeSimulator.Services;

namespace SoMeSimulator.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ScenarioEventController : ControllerBase
    {
        private readonly ICrudInterface<ScenarioEventDto> _service;
        
        public ScenarioEventController(ICrudInterface<ScenarioEventDto> service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetScenarioEvent/{id}")]
        public async Task<IActionResult> GetScenarioEvent(int id)
        {
            try
            {
                var scenarioEventItem = await _service.GetByIdAsync(id);
                return Ok(scenarioEventItem);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CreateScenarioEvent")]
        public async Task<IActionResult> CreateScenarioEvent([FromBody] ScenarioEventDto eventItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var scenarioEventDto = await _service.CreateAsync(eventItem);
                return Ok(scenarioEventDto);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("UpdateScenarioEvent")]
        public async Task<IActionResult> UpdateScenarioEvent([FromBody] ScenarioEventDto eventItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var scenarioEventDto = await _service.UpdateAsync(eventItem);
                return Ok(scenarioEventDto);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("DeleteScenarioEvent/{id}")]
        public async Task<IActionResult> DeleteScenarioEvent(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }
    }
}
