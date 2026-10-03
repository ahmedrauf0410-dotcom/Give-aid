using System.ComponentModel.DataAnnotations;

namespace GiveAID.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Profession { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public virtual ICollection<UserQuery> Queries { get; set; } = new List<UserQuery>();
        public virtual ICollection<ProgrammeInterest> ProgrammeInterests { get; set; } = new List<ProgrammeInterest>();
        public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    }
}
