using System.ComponentModel.DataAnnotations;

namespace GiveAID.Models
{
    public class HelpItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Question { get; set; } = string.Empty;

        [Required]
        public string Answer { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = "General"; // General, Donations, Volunteering, Programmes, Account

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
