using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Xml.Linq;

namespace Tasket.Client.Models
{
    public class TicketDTO
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
        public ProjectDTO? Project { get; set; }

        [Required]
        public string? SubmitterUserId { get; set; }
        public UserDTO? SubmitterUser { get; set; }

        public string? DeveloperUserId { get; set; }
        public UserDTO? DeveloperUser { get; set; }

        public ICollection<TicketCommentDTO> Comments { get; set; } = new HashSet<TicketCommentDTO>();
        public ICollection<TicketAttachmentDTO> Attachments { get; set; } = new HashSet<TicketAttachmentDTO>();
    }
}
