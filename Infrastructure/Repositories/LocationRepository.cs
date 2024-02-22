using Core.DataValidators.Location;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {

        private readonly Context _context;
        private readonly ILogger<LocationRepository> _logger;
        public LocationRepository(Context context, ILogger<LocationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateLocation(Location location)
        {
            _context.Locations.Add(location);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occured when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }

        public async Task<List<Location>> GetAllLocations()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location> GetLocationByID(int id)
        {
            return await _context.Locations.Where(l => l.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateLocation(UpdateLocationValidator updatedInfo)
        {
            var location = await GetLocationByID(updatedInfo.ID);
            if (location is null)
            {
                return false;
            }

            if (updatedInfo.Name is not null) location.Name = updatedInfo.Name;
            if (updatedInfo.Address is not null) location.Address = updatedInfo.Address;
            if (updatedInfo.Description is not null) location.Description = updatedInfo.Description;

            _context.Update(location);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occured when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }

        }
        public async Task<bool> DeleteLocation(int id)
        {
            var location = await GetLocationByID(id);

            if (location is null)
            {
                return false;
            }

            _context.Locations.Remove(location);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occured when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }

        public async Task<ICollection<InventoryItem>> GetAllInventory(int locationId)
        {
            var location = await GetLocationByID(locationId);
            if (location is null) throw new LocationNotFoundException();

            return location.InventoryItems;
        }
    }
}