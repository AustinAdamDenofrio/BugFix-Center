using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace Tasket.Client.Models
{
    public class TicketCommentDTO
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


        public int TicketId { get; set; }
        public string? UserId { get; set; }
        public virtual UserDTO? User { get; set; }
    }
}
