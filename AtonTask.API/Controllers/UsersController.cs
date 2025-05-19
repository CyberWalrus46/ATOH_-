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
    public class UsersController(IUserService _userService, IAuthService _authService) : ControllerBase
    {
        [HttpPost("create")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            try
            {
                var user = new User
                {
                    Login = dto.Login,
                    Password = dto.Password,
                    Name = dto.Name,
                    Gender = dto.Gender,
                    Birthday = dto.Birthday,
                    Admin = dto.Admin == true,
                    CreatedBy = User.Identity?.Name ?? "system"
                };

                var createdUser = await _userService.CreateUserAsync(user);

                return Ok("User was created");
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
            try
            {
                var currentUser = User.Identity?.Name;
                if (currentUser != dto.Login && !User.IsInRole("Admin"))
                    return Forbid();

                var user = await _userService.UpdateUserAsync(dto.Login, user =>
                {
                    if (dto.Name != null) user.Name = dto.Name;
                    if (dto.Gender != null) user.Gender = dto.Gender.Value;
                    if (dto.Birthday != null) user.Birthday = dto.Birthday;
                }, currentUser!);
                
                return user != null ? Ok(user) : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-1/changePassword")]
        [Authorize(Policy = "ActiveUser")]
        public async Task<IActionResult> UpdateUser([FromBody] ChangePasswordDto dto)
        {
            try
            {
                var currentUser = User.Identity?.Name;
                if (currentUser != dto.Login && !User.IsInRole("Admin"))
                    return Forbid();

                if (!User.IsInRole("Admin"))
                    await _userService.GetUserByLoginAndPasswordAsync(dto.Login, dto.OldPassword);

                var user = await _userService.UpdateUserAsync(dto.Login, user =>
                {
                    if (dto.NewPassword != null) user.Password = dto.NewPassword;
                }, currentUser!);

                return user != null ? Ok(user) : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-1/changeLogin")]
        [Authorize(Policy = "ActiveUser")]
        public async Task<IActionResult> UpdateUser([FromBody] ChangeLoginDto dto)
        {
            try
            {
                var currentUser = User.Identity?.Name;
                if (currentUser != dto.OldLogin && !User.IsInRole("Admin"))
                    return Forbid();

                if (!User.IsInRole("Admin"))
                    await _userService.GetUserByLoginAndPasswordAsync(dto.OldLogin, dto.Password);

                var user = await _userService.UpdateUserAsync(dto.OldLogin, user =>
                {
                    if (dto.NewLogin != null) user.Login = dto.NewLogin;
                }, currentUser!);
                
                if (user == null) return NotFound();

                var newToken = await _authService.GenerateJwtTokenAsync(user);
                Response.Cookies.Delete("jwt");
                Response.Cookies.Append("jwt", newToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddMinutes(60)
                });

                return Ok(new
                {
                    User = user,
                    Token = newToken
                });

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("activeUsers")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var users = await _userService.GetActiveUsersAsync();

            if (users == null) return NotFound();

            return Ok(users);
        }

        [HttpGet("getByLogin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUserByLogin(string login)
        {
            var user = await _userService.GetUserByLoginAsync(login);

            return Ok(user);
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
                var user = await _userService.GetUserByLoginAndPasswordAsync(login, password);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getOlderThan")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUsersOlderThan(int age)
        {
            var users = await _userService.GetUsersOlderThan(age);

            return Ok(users);
        }
    }
}