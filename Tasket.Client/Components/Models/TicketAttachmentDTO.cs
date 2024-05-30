namespace Tasket.Client.Components.Models
{
    public class TicketAttachmentDTO
    {
        #region Private Variables
        private DateTimeOffset _created;
        #endregion

        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset Created
        {
            get => _created.ToLocalTime();
            set => _created = value.ToUniversalTime();
        }
        public string? AttachmentUrl { get; set; }
        public string? UserId { get; set; }
        public UserDTO? User {  get; set; }
        public int TicketId { get; set; }
    }
}
