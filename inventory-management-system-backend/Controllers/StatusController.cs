using Core.DataValidators.Status;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventory_management_system_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;
        private readonly ILogger<StatusController> _logger;

        public StatusController(IStatusService statusService, ILogger<StatusController> logger)
        {
            _statusService = statusService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var allStatus = await _statusService.GetAllStatus();
            return Ok(allStatus);
        }

        [HttpPost]
        public async Task<IActionResult> Post(StatusValidator statusInfo)
        {
            var colourCode = statusInfo.ColourCode ?? "#808080";
            var status = new Status()
            {
               Name = statusInfo.Name,
               ColourCode = colourCode
            };

            if (!await _statusService.Create(status))
            {
                return BadRequest("An error occurred when creating your status");
            }

            return Ok("Status created!");
        }

        [HttpPut]
        public async Task<IActionResult> Put(UpdateStatusValidator statusInfo)
        {
            var status = await _statusService.GetStatusByID(statusInfo.ID);
            if (status is null) return BadRequest("Could not find that status");

            if (!await _statusService.Update(statusInfo)) return BadRequest("An error occurred when trying to update this status");

            return Ok("Updated!");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!await _statusService.Delete(id)) return BadRequest("An error occurred when deleting");
            }
            catch (StatusNotFoundException)
            {
                return BadRequest($"We could not find a status with the ID {id}");
            }

            return Ok("Successfully deleted!");
        }
    }
}