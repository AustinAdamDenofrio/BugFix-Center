using System.ComponentModel.DataAnnotations;

namespace Tasket.Client.Models
{
    public class ProjectDTO
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

        public ICollection<UserDTO> Members { get; set; } = new HashSet<UserDTO>();
        public ICollection<TicketDTO> Tickets { get; set; } = new HashSet<TicketDTO>();
    }
}
