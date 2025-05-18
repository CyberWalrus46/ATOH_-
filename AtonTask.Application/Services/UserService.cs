using AtonTask.Domain.Abstractions;
using AtonTask.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonTask.Application.Services
{
    public class UserService(IUserRepository userRepository)
    {
        public async Task<User> CreateUserAsync(User user)
        {
            if (await userRepository.LoginExistsAsync(user.Login))
                throw new ArgumentException("Login already exists");

            await userRepository.AddAsync(user);
            return user;
        }

        public async Task<User?> UpdateUserAsync(string login, Action<User> updateAction, string modifiedBy)
        {
            var user = await userRepository.GetByLoginAsync(login);
            if (user == null) return null;

            updateAction(user);
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = modifiedBy;

            await userRepository.UpdateAsync(user);
            return user;
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
            => await userRepository.GetAllActiveAsync();

        public async Task<User?> GetUserByIdAsync(Guid id)
            => await userRepository.GetByIdAsync(id);

    }

}