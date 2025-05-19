using AtonTask.Domain.Abstractions;
using AtonTask.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AtonTask.API
{
    public class JwtService(
    IConfiguration configuration,
    IUserRepository userRepository) : IAuthService
    {
        public Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim("IsAdmin", user.Admin.ToString()),
                new Claim("IsActive", user.IsActive.ToString()),
                new Claim(ClaimTypes.Role, user.Admin ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(
                double.Parse(configuration["Jwt:ExpireMinutes"]!));

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<User?> ValidateCredentialsAsync(string login, string password)
        {
            var user = await userRepository.GetByLoginAsync(login);
            return user != null && user.Password == password && user.IsActive ? user : null;
        }

        public async Task EnsureAdminUserExistsAsync()
        {
            if (!await userRepository.LoginExistsAsync("admin"))
            {
                var adminUser = new User
                {
                    Login = "admin",
                    Password = "admin123",
                    Name = "Admin",
                    Gender = 2,
                    Admin = true,
                    CreatedBy = "system"
                };
                await userRepository.AddAsync(adminUser);
            }
        }
    }
}
