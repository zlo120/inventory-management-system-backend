using Core.DataValidators.Status;
using Core.Models;

namespace Core.Interfaces
{
    public interface IStatusService
    {
        Task<bool> Create(Status status);
        Task<ICollection<Status>> GetAllStatus();
        Task<Status> GetStatusByID(int id);
        Task<bool> Update(UpdateStatusValidator updatedInfo);
        Task<bool> Delete(int id);
    }
}
