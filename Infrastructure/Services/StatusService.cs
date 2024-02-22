using Core.DataValidators.Status;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _statusRepository;
        public StatusService(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }
        public async Task<bool> Create(Status status)
        {
            return await _statusRepository.Create(status);
        }
        public async Task<Status> GetStatusByID(int id)
        {
            return await _statusRepository.GetStatusByID(id);
        }        

        public async Task<bool> Update(UpdateStatusValidator updatedInfo)
        {
            return await _statusRepository.Update(updatedInfo); 
        }
        public async Task<bool> Delete(int id)
        {
            return await _statusRepository.Delete(id);
        }

        public async Task<ICollection<Status>> GetAllStatus()
        {
            return await _statusRepository.GetAllStatus();
        }
    }
}
