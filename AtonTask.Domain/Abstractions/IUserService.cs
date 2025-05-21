using AtonTask.Domain.Entities;

namespace AtonTask.Application.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByLoginAndPasswordAsync(string login, string password);
        Task<User?> GetUserByLoginAsync(string login);
        Task<IEnumerable<User>> GetUsersOlderThan(int age);
        Task RestoreUser(string login);
        Task DeleteUser(string login, bool softDelete, string revokedBy);
        Task<User?> UpdateUserAsync(string login, Action<User> updateAction, string modifiedBy);
    }
}