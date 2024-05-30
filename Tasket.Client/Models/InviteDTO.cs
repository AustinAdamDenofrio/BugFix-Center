using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tasket.Client.Models
{
    public class InviteDTO
    {
        #region Private Variables
        private DateTimeOffset _inviteDate;
        private DateTimeOffset? _joinDate;
        #endregion

        #region Computed Properties
        [NotMapped]
        public string? FullName => $"{InviteeFirstName} {InviteeLastName}";
        #endregion 

        public int Id { get; set; }

        [Required]
        public DateTimeOffset InviteDate
        {
            get => _inviteDate.ToLocalTime();
            set => _inviteDate = value.ToUniversalTime();
        }

        public DateTimeOffset? JoinDate
        {
            get => _joinDate?.ToLocalTime();
            set => _joinDate = value?.ToUniversalTime();
        }

        [Required]
        public string? InviteeEmail { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} at most {1} characters long", MinimumLength = 2)]
        [Display(Name = "Invitee First Name")]
        public string? InviteeFirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} at most {1} characters long", MinimumLength = 2)]
        [Display(Name = "Invitee Last Name")]
        public string? InviteeLastName { get; set; }
        public string? Message { get; set; }
        public bool IsValid { get; set; }

        public int ProjectId { get; set; }
        public ProjectDTO? Project { get; set; }

        [Required]
        public string? InvitorId { get; set; }

        public UserDTO? Invitor { get; set; }

        [Required]
        public string? InviteeId { get; set; }
        public UserDTO? Invitee { get; set; }
    }
}
