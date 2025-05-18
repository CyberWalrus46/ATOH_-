using AtonTask.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using AtonTask.Domain.Abstractions;
using System.Security.Claims;
using System.Text;

namespace AtonTask.API.DTOs
{
    public class UserCreateDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }

    public class AdminCreateDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Admin { get; set; }
    }

    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }

    public class UserLoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangeLoginDto
    {
        public string NewLogin { get; set; }
        public string Password { get; set; }
    }
}
