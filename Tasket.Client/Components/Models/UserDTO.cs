using System.ComponentModel.DataAnnotations.Schema;

namespace Tasket.Client.Components.Models
{
    public class UserDTO
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [NotMapped] // Computed Property
        public string? FullName => $"{FirstName} {LastName}";

        public string? ImageUrl { get; set; }
        public string? Email { get; set; }
    }
}
