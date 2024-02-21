using Core.Models;

namespace Core.Interfaces
{
    public interface ILocationService
    {
        Task<List<Location>> GetAllLocations();
        Task<Location> GetLocationByID(int id);
    }
}
