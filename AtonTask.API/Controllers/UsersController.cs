using AtonTask.API.DTOs;
using AtonTask.Application.Services;
using AtonTask.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace AtonTask.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(UserService userService) : ControllerBase
    {
        [HttpPost("create")]
        [Authorize(Policy = "ActiveUser")]
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
                    Admin = false,
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

        [HttpPost("create")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateDto dto)
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
                    Admin = dto.Admin,
                    CreatedBy = User.Identity?.Name ?? "system"
                };

                var createdUser = await userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUser), new { login = createdUser.Login }, createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPut("update-1/{login}")]
        //[Authorize(Policy = "ActiveUser")]
        //[Authorize(Policy = "AdminOnly")]
        //public async Task<IActionResult> UpdateUser(string login, [FromBody] UserUpdateDto dto)
        //{
        //    var currentUser = User.Identity?.Name;
        //    if (currentUser != login && !User.IsInRole("Admin"))
        //        return Forbid();

        //    var user = await userService.UpdateUserAsync(login, user =>
        //    {
        //        if (dto.Name != null) user.Name = dto.Name;
        //        if (dto.Gender.HasValue) user.Gender = dto.Gender.Value;
        //        if (dto.Birthday.HasValue) user.Birthday = dto.Birthday;
        //    }, currentUser!);

        //    return user != null ? Ok(user) : NotFound();
        //}


        [HttpPut("update-1/{login}")]
        [Authorize(Policy = "ActiveUser")]
        public async Task<IActionResult> UpdateUser(string login, [FromBody] ChangeLoginDto dto)
        {
            var currentUser = User.Identity?.Name;
            if (currentUser != login && !User.IsInRole("Admin"))
                return Forbid();

            var user = await userService.UpdateUserAsync(login, user =>
            {
                if (dto.NewLogin != null) user.Login = dto.NewLogin;
            }, currentUser!);

            return user != null ? Ok(user) : NotFound();
        }

        [HttpGet("{login}")]
        public async Task<IActionResult> GetUser(string login)
        {
            var currentUserLogin = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserLogin != login)
                return Forbid();

            var user = userService.GetActiveUsersAsync();
            return user != null ? Ok(User) : NotFound();
        }

    }
}

//    [HttpGet]
//    [Authorize(Policy = "AdminOnly")]
//    public async Task<IActionResult> GetActiveUsers()
//    {
//        var users = await userService.GetActiveUsersAsync();
//        return Ok(users);
//    }


//[HttpGet("{login}")]
//public async Task<IActionResult> GetUser(string login)
//{
//    var currentUserLogin = User.Identity?.Name;
//    var isAdmin = User.IsInRole("Admin");

//    if (!isAdmin && currentUserLogin != login)
//        return Forbid();

//    return User != null ? Ok(User) : NotFound();
//}

//    [HttpGet("older-than/{age}")]
//    [Authorize(Policy = "AdminOnly")]
//    public async Task<IActionResult> GetUsersOlderThan(int age)
//    {
//        var users = await mediator.Send(new GetUsersOlderThanQuery(age));
//        return Ok(users);
//    }

//    [HttpDelete("{login}")]
//    [Authorize(Policy = "AdminOnly")]
//    public async Task<IActionResult> DeleteUser(string login, [FromQuery] bool softDelete = true)
//    {
//        await mediator.Send(new DeleteUserCommand(login, softDelete, User.Identity?.Name!));
//        return NoContent();
//    }

//    [HttpPatch("{login}/restore")]
//    [Authorize(Policy = "AdminOnly")]
//    public async Task<IActionResult> RestoreUser(string login)
//    {
//        await mediator.Send(new RestoreUserCommand(login, User.Identity?.Name!));
//        return NoContent();
//    }
//}
//}