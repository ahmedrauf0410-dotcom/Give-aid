using System.ComponentModel.DataAnnotations;
using GiveAID.Models;

namespace GiveAID.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Please provide your name.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Your Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please provide your email address.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Your Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number (Optional)")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please provide a subject.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Subject must be between 3 and 200 characters.")]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your message.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters.")]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        public SiteSetting? SiteSetting { get; set; }
    }

    public class InviteViewModel
    {
        [Required(ErrorMessage = "Please provide your name.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Your Name")]
        public string SenderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please provide your friend's name.")]
        [StringLength(100, ErrorMessage = "Friend's name cannot exceed 100 characters.")]
        [Display(Name = "Friend's Name")]
        public string FriendName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please provide your friend's email address.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Display(Name = "Friend's Email Address")]
        public string FriendEmail { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Personal message cannot exceed 500 characters.")]
        [Display(Name = "Personal Message (Optional)")]
        public string? PersonalMessage { get; set; }
    }

    public class ProgrammeInterestViewModel
    {
        [Required]
        public int ProgrammeId { get; set; }

        public string ProgrammeTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number (Optional)")]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        [Display(Name = "Why would you like to participate? (Optional)")]
        public string? Message { get; set; }
    }

    public class UserQueryViewModel
    {
        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Subject must be between 3 and 200 characters.")]
        [Display(Name = "Query Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your question or inquiry.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters.")]
        [Display(Name = "Detailed Question / Message")]
        public string Message { get; set; } = string.Empty;

        public IEnumerable<UserQuery> PastQueries { get; set; } = new List<UserQuery>();
    }

    public class HelpCentreViewModel
    {
        public IEnumerable<HelpItem> FAQs { get; set; } = new List<HelpItem>();
        public IEnumerable<string> Categories { get; set; } = new List<string>();
        public string? SearchTerm { get; set; }
        public string? SelectedCategory { get; set; }
        public SiteSetting? SiteSetting { get; set; }
    }
}
