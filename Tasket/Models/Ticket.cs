using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Tasket.Client.Models;
using Tasket.Data;
using Tasket.Helper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tasket.Models
{
    public class Ticket
    {
        #region Private Variables
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;
        #endregion


        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }

        public DateTimeOffset Created
        {
            get => _created.ToLocalTime();
            set => _created = value.ToUniversalTime();
        }
        public DateTimeOffset? Updated
        {
            get => _updated?.ToLocalTime(); 
            set => _updated = value?.ToUniversalTime(); 
        }
        public bool Archived { get; set; }
        public bool ArchivedByProject { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        [Required]
        public string? SubmitterUserId { get; set; }
        public virtual ApplicationUser? SubmitterUser { get; set; }

        public string? DeveloperUserId { get; set; }
        public virtual ApplicationUser? DeveloperUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
    }

    public static class TicketExtensions
    {

        public static TicketDTO ToDTO(this Ticket ticket)
        {
            TicketDTO dto = new TicketDTO()
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Created = ticket.Created,
                Updated = ticket.Updated,
                Archived = ticket.Archived,
                ArchivedByProject = ticket.ArchivedByProject,
                Priority = ticket.Priority,
                Type = ticket.Type,
                Status = ticket.Status,
                SubmitterUserId = ticket.SubmitterUserId,
                DeveloperUserId = ticket.DeveloperUserId,
                ProjectId = ticket.ProjectId,
            };

            if(ticket.Project is not null)
            {
                ProjectDTO projectDTO = ticket.Project.ToDTO();
                dto.Project = projectDTO;
            }

            if (ticket.SubmitterUser is not null)
            {
                UserDTO userDTO = ticket.SubmitterUser.ToDTO();
                dto.SubmitterUser = userDTO;
            }

            if (ticket.DeveloperUser is not null)
            {
                UserDTO userDTO = ticket.DeveloperUser.ToDTO();
                dto.SubmitterUser = userDTO;
            }

            foreach (TicketComment comment in ticket.Comments)
            {
                TicketCommentDTO ticketCommentDTO = comment.ToDTO();
                dto.Comments.Add(ticketCommentDTO);
            }

            foreach (TicketAttachment attachment in ticket.Attachments)
            {
                TicketAttachmentDTO ticketAttachmentDTO = attachment.ToDTO();
                dto.Attachments.Add(ticketAttachmentDTO);
            }

            return dto;
        }

    }
}
