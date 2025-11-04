using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Entities
{
    public class Department
    {
        public long Id { get; set; }

        [Required]
        public String Name { get; set; } = string.Empty;
    }
}
