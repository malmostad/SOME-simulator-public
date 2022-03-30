using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SoMeSimulator.Services;

namespace SoMeSimulator.Controllers
{
    [Route("api/[controller]")]
    public class CronController : ControllerBase
    {
        private readonly IRemove _remove;

        public CronController(IRemove remove) => this._remove = remove;

        [Route("Daily")]
        /// <summary>
        /// Runs daily
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DailyAsync()
        {
            try
            {
                Log.Debug($"Running Remove");

                await _remove.RunAsync();

                return Ok("Daily performed.");
            }
            catch (Exception)
            {
                return BadRequest("Daily failed");
            }

        }
    }
}