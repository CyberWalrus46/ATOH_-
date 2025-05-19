using AtonTask.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using AtonTask.Domain.Abstractions;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace AtonTask.API.DTOs
{

    public class UserCreateDto
    {
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string Login { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string Password { get; set; }

        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$")]
        public required string Name { get; set; }
        public required int Gender { get; set; }
        public DateTime? Birthday { get; set; } = null;
        public required bool Admin { get; set; }
    }

    public class UserResponseDto
    {
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string Login { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string Password { get; set; }

        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$")]
        public required string Name { get; set; }
        public required int Gender { get; set; }
        public required DateTime? Birthday { get; set; }
        public required bool? Admin { get; set; }
    }

    public class ChangePersonalInfoDto
    {
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string Login { get; set; }

        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$")]
        public string? Name { get; set; } = null;

        public int? Gender { get; set; } = null;
        public DateTime? Birthday { get; set; } = null;
    }

    public class UserLoginDto
    {
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string Login { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string Password { get; set; }
    }

    public class ChangePasswordDto
    {
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string Login { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string OldPassword { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string NewPassword { get; set; }
    }

    public class ChangeLoginDto
    {
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string OldLogin { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string NewLogin { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public required string Password { get; set; }
    }
}
