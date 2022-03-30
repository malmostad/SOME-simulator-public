using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SomeSimulator.DTOs;
using SomeSimulator.Interfaces;

namespace SoMeSimulator.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CommentController: ControllerBase
    {
        private readonly ICrudInterface<CommentDto> _service;

        public CommentController(ICrudInterface<CommentDto> service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetComment/{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            try
            {
                var comment = await _service.GetByIdAsync(id);
                return Ok(comment);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CreateComment")] 
        public async Task<IActionResult> CreateComment([FromBody] CommentDto commentDto)
        {
            try
            {
                var comment = await _service.CreateAsync(commentDto);
                return Ok(comment);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdateComment")]
        public async Task<IActionResult> UpdateComment([FromBody] CommentDto commentDto)
        {
            try
            {
                var comment = await _service.UpdateAsync(commentDto);
                return Ok(comment);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
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