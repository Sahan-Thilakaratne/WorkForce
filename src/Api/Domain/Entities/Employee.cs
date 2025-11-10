using Api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Entities
{
    public class Employee
    {
        public long Id { get; set; }

        public string? EmpCode { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; }

        public string? Email { get; set; }

        public long? DepartmentId { get; set; }

        public Department? Department { get; set; }

        public UserRole UserRole { get; set; } = UserRole.EMPLOYEE;

        public DateOnly DateJoined { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
