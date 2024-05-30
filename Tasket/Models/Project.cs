using System.ComponentModel.DataAnnotations;
using Tasket.Client.Components.Models;
using Tasket.Data;
using Tasket.Helper;

namespace Tasket.Models
{
    public class Project
    {
        #region Private Variables
        private DateTimeOffset _created;
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;
        #endregion

        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public DateTimeOffset Created
        {
            get => _created.ToLocalTime();
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset StartDate
        {
            get => _startDate.ToLocalTime();
            set => _startDate = value.ToUniversalTime();
        }

        public DateTimeOffset EndDate
        {
            get => _endDate.ToLocalTime();
            set => _endDate = value.ToUniversalTime();
        }

        public ProjectPriority Priority { get; set; }

        public bool Archived { get; set; }


        //Why are we getting this?
        //public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        public virtual ICollection<ApplicationUser> Members { get; set; } = new HashSet<ApplicationUser>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }

    public static class ProjectExtensions
    {

        public static ProjectDTO ToDTO(this Project project)
        {
            ProjectDTO dto = new ProjectDTO()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Created = project.Created,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Priority = project.Priority,
                Archived = project.Archived,
            };

            //ToDo: Members
            foreach (ApplicationUser member in project.Members)
            {
                dto.Members.Add(member.ToDTO());
            }

            //ToDo: Tickets
            foreach (Ticket ticket in project.Tickets)
            {
                dto.Tickets.Add(ticket.ToDTO());
            }

            return dto;
        }

    }
}
