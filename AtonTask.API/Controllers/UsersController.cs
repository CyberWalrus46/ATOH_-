using AtonTask.API.DTOs;
using AtonTask.Application.Services;
using AtonTask.Domain.Abstractions;
using AtonTask.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AtonTask.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(IUserService _userService, IAuthService _authService) : ControllerBase
    {
        [HttpPost]
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

            if (users == null) 
                return NotFound();

            return Ok(users);
        }


        [HttpGet("getByLogin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUserByLogin(UserByLoginDto dto)
        {
            var user = await _userService.GetUserByLoginAsync(dto.Login);

            if (user == null)
                return NotFound();

            var response = new UserResponseDto
            {
                Name = user.Name,
                Gender = user.Gender,
                Birthday = user.Birthday,
                IsActive = user.IsActive
            };

            return Ok(response);
        }


        [HttpGet("getByLoginAndPassword")]
        [Authorize(Policy = "ActiveUser")]
        public async Task<IActionResult> GetByLoginAndPassword(GetUserByLoginAndPasswordDto dto)
        {
            var currentUser = User.Identity?.Name;
            if (currentUser != dto.Login || User.IsInRole("Admin"))
                return Forbid();

            try
            {
                var user = await _userService.GetUserByLoginAndPasswordAsync(dto.Login, dto.Password);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("getOlderThan")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUsersOlderThan(GetUsersOlderThanDto dto)
        {
            var users = await _userService.GetUsersOlderThan(dto.Age);

            if (users == null)
                return NotFound();

            return Ok(users);
        }


        [HttpDelete]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUser(DeleteUserDto dto)
        {
            try
            {
                await _userService.DeleteUser(dto.Login, dto.SoftDelete, User.Identity?.Name ?? "system");

                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("restore")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RestoreUser(UserByLoginDto dto)
        {
            try
            {
                await _userService.RestoreUser(dto.Login);

                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}