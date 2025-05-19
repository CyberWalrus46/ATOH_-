using AtonTask.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonTask.Domain.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByLoginAsync(string login);
        Task<IEnumerable<User>> GetAllActiveAsync();
        Task<IEnumerable<User>> GetOlderThanAsync(int age);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task RestoreAsync(User user);
        Task DeleteAsync(User user, bool softDelete, string revokedBy);
        Task<bool> LoginExistsAsync(string login);
    }
}
