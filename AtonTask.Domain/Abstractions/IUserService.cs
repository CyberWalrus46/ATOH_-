using AtonTask.Domain.Entities;

namespace AtonTask.Application.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task DeleteUser(User user, bool softDelete, string revokedBy);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByLoginAndPasswordAsync(string login, string password);
        Task<User?> GetUserByLoginAsync(string login);
        Task<IEnumerable<User>> GetUsersOlderThan(int age);
        Task RestoreUser(User user);
        Task<User?> UpdateUserAsync(string login, Action<User> updateAction, string modifiedBy);
    }
}