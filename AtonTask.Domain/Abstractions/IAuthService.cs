using AtonTask.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonTask.Domain.Abstractions
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(User user);
        Task<User?> ValidateCredentialsAsync(string login, string password);
        Task EnsureAdminUserExistsAsync();
    }
}
