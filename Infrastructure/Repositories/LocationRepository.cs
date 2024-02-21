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
        public async Task<List<Location>> GetAllLocations()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location> GetLocationByID(int id)
        {
            return await _context.Locations.Where(l => l.Id == id).FirstOrDefaultAsync();
        }
    }
}