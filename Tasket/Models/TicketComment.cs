using System.ComponentModel.DataAnnotations;
using Tasket.Client;
using Tasket.Client.Components.Models;
using Tasket.Data;
using Tasket.Helper;

namespace Tasket.Models
{
    public class TicketComment
    {
        #region Private Variables
        private DateTimeOffset _created;
        #endregion

        public int Id { get; set; }

        [Required]
        public string? Content { get; set; }

        public DateTimeOffset Created
        {
            get => _created.ToLocalTime();
            set => _created = value.ToUniversalTime();
        }
        public virtual Ticket? Ticket { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }

    public static class TicketCommentExtensions
    {

        public static TicketCommentDTO ToDTO(this TicketComment ticketComment)
        {
            TicketCommentDTO dto = new TicketCommentDTO()
            {
                Id = ticketComment.Id,
                Content = ticketComment.Content,
                Created = ticketComment.Created,
                TicketId = ticketComment.Id,
                User = ticketComment.User?.ToDTO(),
                UserId = ticketComment.User?.Id,
            };

            return dto;
        }

    }
}
