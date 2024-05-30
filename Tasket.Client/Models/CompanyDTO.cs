using System.ComponentModel;

namespace Tasket.Client.Models
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public ICollection<ProjectDTO> Projects { get; set; } = new HashSet<ProjectDTO>();
        public ICollection<UserDTO> Members { get; set; } = new HashSet<UserDTO>();
        public ICollection<InviteDTO> Invites { get; set; } = new HashSet<InviteDTO>();
    }
}
