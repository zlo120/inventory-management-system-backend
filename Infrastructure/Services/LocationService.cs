using Core.DataValidators.Location;
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

        public async Task<bool> CreateLocation(Location location)
        {
            return await _locationRepository.CreateLocation(location);
        }

        public async Task<List<Location>> GetAllLocations()
        {
            return await _locationRepository.GetAllLocations();
        }

        public async Task<Location> GetLocationByID(int id)
        {
            return await _locationRepository.GetLocationByID(id);
        }

        public async Task<bool> UpdateLocation(UpdateLocationValidator updatedInfo)
        {
            return await _locationRepository.UpdateLocation(updatedInfo);
        }

        public async Task<bool> DeleteLocation(int id)
        {
            return await _locationRepository.DeleteLocation(id);
        }

        public async Task<ICollection<InventoryItem>> GetAllInventory(int locationId)
        {
            return await _locationRepository.GetAllInventory(locationId);
        }
    }
}
