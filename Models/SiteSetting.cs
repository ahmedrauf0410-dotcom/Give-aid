using System.ComponentModel.DataAnnotations;

namespace GiveAID.Models
{
    public class SiteSetting
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string NgoName { get; set; } = "Give-AID";

        [MaxLength(200)]
        public string? Tagline { get; set; } = "Empowering Communities, Transforming Lives";

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = "info@giveaid.org";

        [Required]
        [MaxLength(50)]
        public string Phone { get; set; } = "+1 (555) 019-2834";

        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = "742 Evergreen Terrace, Suite 100";

        [Required]
        [MaxLength(100)]
        public string CityStateZip { get; set; } = "Springfield, IL 62704";

        [MaxLength(250)]
        public string? FacebookUrl { get; set; } = "https://facebook.com";

        [MaxLength(250)]
        public string? TwitterUrl { get; set; } = "https://twitter.com";

        [MaxLength(250)]
        public string? InstagramUrl { get; set; } = "https://instagram.com";

        [MaxLength(250)]
        public string? YouTubeUrl { get; set; } = "https://youtube.com";

        [Required]
        [MaxLength(500)]
        public string FooterText { get; set; } = "Give-AID is a registered non-profit organization dedicated to fostering sustainable welfare, healthcare, education, and social empowerment.";

        [MaxLength(300)]
        public string? LogoUrl { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
