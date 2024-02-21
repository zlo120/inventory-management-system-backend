using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {

            _locationRepository = locationRepository;

        }
        public async Task<List<Location>> GetAllLocations()
        {
            return await _locationRepository.GetAllLocations();
        }

        public async Task<Location> GetLocationByID(int id)
        {
            return await _locationRepository.GetLocationByID(id);
        }
    }
}
