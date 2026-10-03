using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiveAID.Models
{
    public class Donation
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required]
        public int DonationCauseId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(100)]
        public string DonorName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string DonorEmail { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? DonorPhone { get; set; }

        [Required]
        [MaxLength(100)]
        public string CardHolderName { get; set; } = string.Empty;

        /// <summary>
        /// Only masked card number stored (e.g. ************4242). Never raw card numbers!
        /// </summary>
        [Required]
        [MaxLength(24)]
        public string MaskedCardNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(2)]
        public string ExpiryMonth { get; set; } = string.Empty;

        [Required]
        [MaxLength(4)]
        public string ExpiryYear { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string TransactionReference { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string PaymentStatus { get; set; } = "Successful"; // Successful, Failed, Refunded

        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual DonationCause DonationCause { get; set; } = null!;
    }
}
