using System.ComponentModel.DataAnnotations;
using GiveAID.Models;

namespace GiveAID.ViewModels
{
    public class DonateViewModel
    {
        [Required(ErrorMessage = "Please select a cause to support.")]
        [Display(Name = "Donation Cause")]
        public int DonationCauseId { get; set; }

        [Required(ErrorMessage = "Please enter or select a donation amount.")]
        [Range(1.00, 100000.00, ErrorMessage = "Donation amount must be between $1.00 and $100,000.00")]
        [Display(Name = "Donation Amount ($)")]
        public decimal Amount { get; set; } = 50.00m;

        [Required(ErrorMessage = "Please provide your full name.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name")]
        public string DonorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please provide a valid email address.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email Address")]
        public string DonorEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number (Optional)")]
        public string? DonorPhone { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        [Display(Name = "Leave a Message or Dedication (Optional)")]
        public string? Notes { get; set; }

        // ================= DUMMY CARD VALIDATION FIELDS =================

        [Required(ErrorMessage = "Please enter the cardholder's name as printed on the card.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Cardholder name must be between 3 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s.'-]+$", ErrorMessage = "Cardholder name can only contain letters, spaces, dots, and hyphens.")]
        [Display(Name = "Cardholder Name")]
        public string CardHolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Card number is required.")]
        [RegularExpression(@"^(\d{4}[\s-]?){3}\d{4}$|^\d{15,16}$", ErrorMessage = "Please enter a valid 15 or 16-digit card number.")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiry month is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Expiry month must be between 01 and 12.")]
        [Display(Name = "MM")]
        public string ExpiryMonth { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiry year is required.")]
        [RegularExpression(@"^(20[2-9][0-9]|\d{2})$", ErrorMessage = "Please provide a valid 2 or 4-digit expiry year.")]
        [Display(Name = "YYYY")]
        public string ExpiryYear { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV security code is required.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
        [Display(Name = "CVV")]
        public string CVV { get; set; } = string.Empty;

        // View support
        public IEnumerable<DonationCause> AvailableCauses { get; set; } = new List<DonationCause>();
        public DonationCause? SelectedCause { get; set; }
    }

    public class DonationReceiptViewModel
    {
        public int DonationId { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CauseName { get; set; } = string.Empty;
        public string DonorName { get; set; } = string.Empty;
        public string DonorEmail { get; set; } = string.Empty;
        public string? DonorPhone { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public string MaskedCardNumber { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Successful";
        public DateTime DonationDate { get; set; }
        public string? Notes { get; set; }
    }
}
