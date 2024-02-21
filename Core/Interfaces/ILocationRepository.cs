using Core.Models;

namespace Core.Interfaces
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAllLocations();
        Task<Location> GetLocationByID(int id);
    }
}
