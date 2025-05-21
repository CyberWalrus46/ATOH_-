using AtonTask.Domain.Abstractions;
using AtonTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonTask.Infrastucture.Data
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetByIdAsync(Guid id)
            => await context.Users.FindAsync(id);

        public async Task<User?> GetByLoginAsync(string login)
            => await context.Users.FirstOrDefaultAsync(u => u.Login == login);


        public async Task<IEnumerable<User>> GetAllActiveAsync()
            => await context.Users.Where(u => u.RevokedOn == null)
                                 .OrderBy(u => u.CreatedOn)
                                 .ToListAsync();

        public async Task<IEnumerable<User>> GetOlderThanAsync(int age)
        {
            var minBirthday = DateTime.UtcNow.AddYears(-age);
            return await context.Users
                .Where(u => u.Birthday.HasValue && u.Birthday <= minBirthday)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                switch (pgEx.SqlState)
                {
                    case PostgresErrorCodes.UniqueViolation:
                        throw new Exception("Login already exists");
                    case PostgresErrorCodes.NotNullViolation:
                        throw new Exception("Required field is empty");
                    default:
                        throw new Exception($"Database error: {pgEx.Message}");
                }
            }
        }

        public async Task RestoreAsync(User user)
        {
            user.RevokedOn = null;
            user.RevokedBy = null;
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user, bool softDelete, string revokedBy)
        {
            if (softDelete)
            {
                user.RevokedOn = DateTime.UtcNow;
                user.RevokedBy = revokedBy;
                await UpdateAsync(user);
            }
            else
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> LoginExistsAsync(string login)
            => await context.Users.AnyAsync(u => u.Login == login);
    }
}
