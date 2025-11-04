using Api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Entities
{
    public class AppUser
    {
        public long Id { get; set; }
        public long? EmployeeId { get; set; }

        public Employee Employee { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.EMPLOYEE;

        public bool IsActive { get; set; } = true;
    }
}
