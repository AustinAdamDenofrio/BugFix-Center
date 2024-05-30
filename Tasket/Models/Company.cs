using Tasket.Helper;
using Tasket.Data;
using System.ComponentModel.DataAnnotations;
using Tasket.Client.Models;

namespace Tasket.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }


        public Guid? ImageId {  get; set; }
        public virtual FileUpload? Image { get; set; }
        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public ICollection<ApplicationUser> Members { get; set; } = new HashSet<ApplicationUser>();
        public ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
    }

    public static class CompanyExtensions
    {

        public static CompanyDTO ToDTO(this Company company)
        {
            CompanyDTO dto = new ()
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                ImageUrl = company?.ImageId == null ? $"/api/uploads/{company?.ImageId}" : UploadHelper.DefaultCompanyPicture,
            };

            if (company!.Projects is not null)
            {
                foreach (Project project in company.Projects)
                {
                    dto.Projects.Add(project.ToDTO());
                }
            }

            if (company.Members is not null)
            {
                foreach (ApplicationUser member in company.Members)
                {
                    dto.Members.Add(member.ToDTO());
                }
            }

            if (company.Invites is not null)
            {
                foreach (Invite invite in company.Invites)
                {
                    dto.Invites.Add(invite.ToDTO());
                }
            }

            return dto;
        }
    }
}
