using System.ComponentModel.DataAnnotations;

namespace GiveAID.Models
{
    public class ProgrammeInterest
    {
        public int Id { get; set; }

        [Required]
        public int ProgrammeId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Programme Programme { get; set; } = null!;
        public virtual User? User { get; set; }
    }
}
