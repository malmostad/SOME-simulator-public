using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SoMeSimulator.Data.Models;
using SomeSimulator.DTOs;
using SomeSimulator.Interfaces;
using SomeSimulator.Services;

namespace SoMeSimulator.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ICrudInterface<PostDto> _service;

        public PostController(ICrudInterface<PostDto> service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetPost/{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            try
            {
                var postItem = await _service.GetByIdAsync(id);
                return Ok(postItem);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }
        
        [HttpPost]
        [Route("CreatePost")]
        public async Task<IActionResult> CreatePost([FromBody] PostDto postDto)
        {
            try
            {
                var result = await _service.CreateAsync(postDto);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("UpdatePost")]
        public async Task<IActionResult> UpdatePost([FromBody] PostDto postDto)
        {
            try
            {
                var result = await _service.UpdateAsync(postDto);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("DeletePost/{id}")]
        public async Task<IActionResult> DeletePost(int id)
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