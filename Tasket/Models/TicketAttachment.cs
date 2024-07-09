using System.ComponentModel.DataAnnotations;
using Tasket.Client.Models;
using Tasket.Data;
using Tasket.Helper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tasket.Models
{
    public class TicketAttachment
    {
        #region Private Variables
        private DateTimeOffset _created;
        #endregion

        public int Id { get; set; }

        [Required]
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset Created
        {
            get => _created.ToLocalTime();
            set => _created = value.ToUniversalTime();
        }

        [Required]
        public Guid? UploadId { get; set; }
        public virtual FileUpload? Upload { get; set; }

        [Required]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int TicketId { get; set; }
        public virtual Ticket? Ticket { get; set; }
    }

    public static class TicketAttachmentExtensions
    {

        public static TicketAttachmentDTO ToDTO(this TicketAttachment ticketAttachment)
        {
            TicketAttachmentDTO dto = new TicketAttachmentDTO()
            {
                Id = ticketAttachment.Id,
                FileName = ticketAttachment.FileName,
                Description = ticketAttachment.Description,
                Created = ticketAttachment.Created,
                AttachmentUrl =  $"/api/uploads/{ticketAttachment?.UploadId}",
                User = ticketAttachment!.User?.ToDTO(),
                UserId = ticketAttachment!.User?.Id,
                TicketId = ticketAttachment.Id
            };

            if (ticketAttachment.User is not null)
            {
                UserDTO userDTO = ticketAttachment.User.ToDTO();
                dto.User = userDTO;
            }

            return dto;
        }

    }
}
