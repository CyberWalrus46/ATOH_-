using AtonTask.API.DTOs;
using AtonTask.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtonTask.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await authService.ValidateCredentialsAsync(dto.Login, dto.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = await authService.GenerateJwtTokenAsync(user);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddMinutes(60)
            });

            return Ok(new { token });
        }


        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
