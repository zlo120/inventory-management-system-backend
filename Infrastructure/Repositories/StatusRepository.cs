using Core.DataValidators.Status;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class StatusRepository : IStatusRepository
    {

        private readonly Context _context;
        private readonly ILogger<StatusRepository> _logger;
        public StatusRepository(Context context, ILogger<StatusRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Create(Status status)
        {
             _context.Status.Add(status);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occurred when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }

        public async Task<Status> GetStatusByID(int id)
        {
            return await _context.Status.Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> Update(UpdateStatusValidator updatedInfo)
        {
            var status = await GetStatusByID(updatedInfo.ID) ?? throw new StatusNotFoundException();
            if (updatedInfo.Name is not null) status.Name = updatedInfo.Name;
            if (updatedInfo.ColourCode is not null) status.ColourCode = updatedInfo.ColourCode;

            _context.Update(status);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occurred when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var status = await GetStatusByID(id) ?? throw new StatusNotFoundException();

            _context.Status.Remove(status);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occurred when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }

        public async Task<ICollection<Status>> GetAllStatus()
        {
            return await _context.Status.ToListAsync();
        }
    }
}