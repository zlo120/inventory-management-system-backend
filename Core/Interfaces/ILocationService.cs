using Core.DataValidators.Location;
using Core.Models;

namespace Core.Interfaces
{
    public interface ILocationService
    {
        Task<bool> CreateLocation(Location location);
        Task<List<Location>> GetAllLocations();
        Task<Location> GetLocationByID(int id);
        Task<bool> UpdateLocation(UpdateLocationValidator updatedInfo);
        Task<bool> DeleteLocation(int id);
        Task<ICollection<InventoryItem>> GetAllInventory(int locationId);
    }
}
