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
            get { return _updated; }
            set { _updated = value?.ToUniversalTime(); }
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
                SubmitterUser = ticket.SubmitterUser?.ToDTO(),
                DeveloperUser = ticket.DeveloperUser?.ToDTO(),
            };

            if (ticket.Project != null) dto.Project = ticket.Project.ToDTO();

            foreach (TicketComment comment in ticket.Comments)
            {
                dto.Comments.Add(comment.ToDTO());
            }

            foreach (TicketAttachment ticketAttachment in ticket.Attachments)
            {
                TicketAttachmentDTO attachmentDto = ticketAttachment.ToDTO();
                dto.Attachments.Add(attachmentDto);
            }

            return dto;
        }

    }
}
