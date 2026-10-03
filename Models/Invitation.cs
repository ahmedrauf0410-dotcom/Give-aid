using System.ComponentModel.DataAnnotations;

namespace GiveAID.Models
{
    public class Invitation
    {
        public int Id { get; set; }

        public int? SenderUserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SenderName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FriendName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string FriendEmail { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? PersonalMessage { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual User? SenderUser { get; set; }
    }
}
