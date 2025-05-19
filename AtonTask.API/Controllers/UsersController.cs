using AtonTask.API.DTOs;
using AtonTask.Application.Services;
using AtonTask.Domain.Abstractions;
using AtonTask.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace AtonTask.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(UserService userService) : ControllerBase
    {
        [HttpPost("create")]
        [Authorize(Policy = "ActiveUser")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateDto dto)
        {
            try
            {
                var isAdmin = User.IsInRole("Admin") || User.HasClaim("IsAdmin", "True");

                if (!isAdmin && dto.Admin == true)
                    return Forbid();

                var user = new User
                {
                    Login = dto.Login,
                    Password = dto.Password,
                    Name = dto.Name,
                    Gender = dto.Gender,
                    Birthday = dto.Birthday,
                    Admin = dto.Admin == true,
                    CreatedBy = User.Identity?.Name ?? "user"
                };

                var createdUser = await userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUser), new { login = createdUser.Login }, createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-1/changePersonalInfo")]
        [Authorize(Policy = "ActiveUser")]
        public async Task<IActionResult> UpdateUser([FromBody] ChangePersonalInfoDto dto)
        {
            var currentUser = User.Identity?.Name;
            if (currentUser != dto.Login && !User.IsInRole("Admin"))
                return Forbid();

            var user = await userService.UpdateUserAsync(dto.Login, user =>
            {
                if (dto.Name != null) user.Name = dto.Name;
                if (dto.Gender.HasValue) user.Gender = dto.Gender.Value;
                if (dto.Birthday.HasValue) user.Birthday = dto.Birthday;
            }, currentUser!);

            return user != null ? Ok(user) : NotFound();
        }

        [HttpPut("update-1/changePassword")]
        [Authorize(Policy = "ActiveUser")]
        public async Task<IActionResult> UpdateUser([FromBody] ChangePasswordDto dto)
        {
            var currentUser = User.Identity?.Name;
            if (currentUser != dto.Login && !User.IsInRole("Admin"))
                return Forbid();

            try
            {
                if (!User.IsInRole("Admin"))
                    await userService.GetUserByLoginAndPasswordAsync(dto.Login, dto.OldPassword);
            }
            catch (Exception ex)
            {
                BadRequest(ex);
            }

            var user = await userService.UpdateUserAsync(dto.Login, user =>
                {
                    if (dto.NewPassword != null) user.Password = dto.NewPassword;
                }, currentUser!);

            return user != null ? Ok(user) : NotFound(); 
        }

        [HttpPut("update-1/changeLogin")]
        [Authorize(Policy = "ActiveUser")]
        public async Task<IActionResult> UpdateUser([FromBody] ChangeLoginDto dto)
        {
            var currentUser = User.Identity?.Name;
            if (currentUser != dto.OldLogin && !User.IsInRole("Admin"))
                return Forbid();

            try
            {
                if (!User.IsInRole("Admin"))
                    await userService.GetUserByLoginAndPasswordAsync(dto.OldLogin, dto.Password);
            }
            catch (Exception ex)
            {
                BadRequest(ex);
            }

            var user = await userService.UpdateUserAsync(dto.OldLogin, user =>
            {
                if (dto.NewLogin != null) user.Login = dto.NewLogin;
            }, currentUser!);

            return user != null ? Ok(user) : NotFound();
        }

        [HttpGet("activeUsers")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var users = await userService.GetActiveUsersAsync();
            return Ok(users);
        }

        [HttpGet("getByLogin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUserByLogin(string login)
        {
            var users = await userService.GetUserByLoginAsync(login);

            return Ok(users);
        }

        [HttpGet("getByLoginAndPassword")]
        [Authorize(Policy = "ActiveUser")]
        public async Task<IActionResult> GetUser(string login, string password)
        {
            var currentUser = User.Identity?.Name;
            if (currentUser != login || User.IsInRole("Admin"))
                return Forbid();

            try
            {
                var user = await userService.GetUserByLoginAndPasswordAsync(login, password);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getOlderThan")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUsersOlderThan(int age)
        {
            
            var users = await userService.GetUsersOlderThan(age);

            return Ok(users);
        }
    }
}