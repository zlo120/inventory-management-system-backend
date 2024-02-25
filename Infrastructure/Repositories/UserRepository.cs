using Core.DataValidators.User;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;
        private readonly ISecurityService _securityService;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(Context context, ILogger<UserRepository> logger, ISecurityService securityService)
        {
            _context = context;
            _logger = logger;
            _securityService = securityService;
        }

        public async Task<bool> Create(User user)
        {
            _context.Users.Add(user);
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

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateUser(User user)
        {
            var originalUser = await _context.Users.Where(u => u.Id == user.Id).FirstOrDefaultAsync();
            if (originalUser is null)
            {
                return false;
            }

            originalUser.Email = user.Email;

            _context.Update(originalUser);

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

        public async Task<bool> DeleteUser(int id)
        {
            var originalUser = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            if (originalUser is null)
            {
                return false;
            }

            _context.Users.Remove(originalUser);

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

        public async Task<bool> ChangeUserGroup(int userId, UserGroups group)
        {
            var user = await GetUserById(userId) ?? throw new ArgumentNullException(nameof(userId));

            user.GroupId = (int)group;

            _context.Update(user);

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

        public async Task<bool> UserHasCreatedPassword(string email, string password)
        {
            var user = await GetUserByEmail(email);

            await _securityService.Update(new UpdateUserValidator
            {
                Email = email,
                Password = password,
            });

            user.UserCreatedPassword = true;

            _context.Update(user);

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

        public async Task<ICollection<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
    }
}