using Core.DataValidators;
using Core.Models;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<bool> Create(CreateUserValidator userInfo);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(int id);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
    }
}
