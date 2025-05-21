using AtonTask.Domain.Abstractions;
using AtonTask.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AtonTask.Application.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
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

        public async Task<User?> GetUserByLoginAsync(string login)
            => await userRepository.GetByLoginAsync(login);

        public async Task<User?> GetUserByLoginAndPasswordAsync(string login, string password)
        {
            var user = await userRepository.GetByLoginAsync(login);

            if (user == null)
                throw new ArgumentException("Invalid login");

            if (user.Password != password)
                throw new ArgumentException("Invalid login or password");

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersOlderThan(int age)
            => await userRepository.GetOlderThanAsync(age);

        public async Task DeleteUser(string login, bool softDelete, string revokedBy)
        {
            var user = await userRepository.GetByLoginAsync(login);

            if (user == null)
                throw new ArgumentException("User doesn't exist");

            await userRepository.DeleteAsync(user, softDelete, revokedBy);
        }

        public async Task RestoreUser(string login)
        {
            var user = await userRepository.GetByLoginAsync(login);

            if (user == null)
                throw new ArgumentException("User doesn't exist");

            await userRepository.RestoreAsync(user);
        }
    }

}