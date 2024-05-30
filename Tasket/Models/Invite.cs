using Microsoft.Build.Evaluation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tasket.Client.Models;
using Tasket.Data;
using Tasket.Helper;

namespace Tasket.Models
{
    public class Invite
    {
        #region Private Variables
        private DateTimeOffset _inviteDate;
        private DateTimeOffset? _joinDate;
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
        public Guid CompanyToken { get; set; }

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

        [NotMapped]
        public string? FullName => $"{InviteeFirstName} {InviteeLastName}";

        public string? Message { get; set; }

        public bool IsValid { get; set; }


        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        [Required]
        public string? InvitorId { get; set; }
        public virtual ApplicationUser? Invitor { get; set; }

        public string? InviteeId { get; set; }
        public virtual ApplicationUser? Invitee { get; set; }
    }


    public static class InviteExtensions
    {

        public static InviteDTO ToDTO(this Invite invite)
        {
            InviteDTO dto = new InviteDTO()
            {
                Id = invite.Id,
                InviteDate = invite.InviteDate,
                JoinDate = invite.JoinDate,
                InviteeEmail = invite.InviteeEmail,
                InviteeFirstName = invite.InviteeFirstName, 
                InviteeLastName = invite.InviteeLastName,
                Message = invite.Message,
                IsValid = invite.IsValid,
                ProjectId = invite.ProjectId,
                InvitorId = invite.InvitorId,
                InviteeId = invite.InviteeId,
            };

            return dto;
        }

    }
}
