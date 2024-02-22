using Core.DataValidators.Location;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inventory_management_system_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILocationService locationService, ILogger<LocationController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateLocationValidator locationData)
        {
            var location = new Location()
            {
                Name = locationData.Name,
                Description = locationData.Description,
                Address = locationData.Address,
            };

            if (!await _locationService.CreateLocation(location))
            {
                return BadRequest("A location with that name already exists!");
            }

            return Ok("Location created!");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var locations = await _locationService.GetAllLocations();

            return Ok(locations);
        }

        [HttpGet("GetLocationById")]
        public async Task<IActionResult> GetLocationById([FromQuery] int id)
        {
            var location = await _locationService.GetLocationByID(id);

            return Ok(location);
        }

        [HttpPut]
        public async Task<IActionResult> Put(UpdateLocationValidator locationData)
        {
            if (! await _locationService.UpdateLocation(locationData)) 
            { 
                return BadRequest("An error has occurred while updating this location"); 
            }

            return Ok("Updated!");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (! await _locationService.DeleteLocation(id))
            {
                return BadRequest("An error has occurred while deleting this location");
            }

            return Ok("Deleted!");
        }
    }
}