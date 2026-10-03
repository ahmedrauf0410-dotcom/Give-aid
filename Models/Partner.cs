using System.ComponentModel.DataAnnotations;

namespace GiveAID.Models
{
    public class Partner
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty; // Corporate, NGO, Government, Academic

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? WebsiteUrl { get; set; }

        [MaxLength(300)]
        public string? LogoUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
