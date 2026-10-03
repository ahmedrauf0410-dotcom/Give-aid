using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using GiveAID.Models;

namespace GiveAID.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalDonations { get; set; }
        public decimal TotalDonationAmount { get; set; }
        public int TotalCauses { get; set; }
        public int TotalProgrammes { get; set; }
        public int TotalPartners { get; set; }
        public int TotalGalleryImages { get; set; }
        public int TotalQueries { get; set; }
        public int TotalPendingQueries { get; set; }
        public int TotalProgrammeInterests { get; set; }
        public int TotalContactMessages { get; set; }
        public int TotalInvitations { get; set; }

        public IEnumerable<Donation> RecentDonations { get; set; } = new List<Donation>();
        public IEnumerable<User> RecentUsers { get; set; } = new List<User>();
        public IEnumerable<UserQuery> RecentQueries { get; set; } = new List<UserQuery>();
        public IEnumerable<DonationCause> TopCauses { get; set; } = new List<DonationCause>();
    }

    public class CauseEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cause Name is required.")]
        [StringLength(150, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Short summary is required.")]
        [StringLength(300)]
        public string ShortDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full description is required.")]
        public string FullDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target fundraising amount is required.")]
        [Range(10.0, 10000000.0, ErrorMessage = "Target amount must be at least $10.00.")]
        public decimal TargetAmount { get; set; }

        public decimal RaisedAmount { get; set; }

        public string? ExistingImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ProgrammeEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Programme title is required.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Short description is required.")]
        [StringLength(300)]
        public string ShortDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full description is required.")]
        public string FullDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required.")]
        public DateTime EventDate { get; set; } = DateTime.UtcNow.AddDays(14);

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Upcoming"; // Upcoming, Ongoing, Completed

        public string? ExistingImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class PartnerEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Partner name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100)]
        public string Category { get; set; } = "Corporate"; // Corporate, Foundation, NGO, Government

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string? WebsiteUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public string? ExistingLogoUrl { get; set; }

        public IFormFile? LogoFile { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class AboutSectionEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string SectionKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Section title is required.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Subtitle { get; set; }

        [Required(ErrorMessage = "Content text is required.")]
        public string Content { get; set; } = string.Empty;

        public string? ExistingImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class GalleryItemEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100)]
        public string Category { get; set; } = "General";

        [StringLength(500)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public string? ExistingImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class HelpItemEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Question is required.")]
        [StringLength(250)]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Answer is required.")]
        public string Answer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100)]
        public string Category { get; set; } = "General";

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class QueryReplyViewModel
    {
        public int Id { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "In Progress"; // Pending, In Progress, Resolved

        [Required(ErrorMessage = "Admin response is required.")]
        [StringLength(2000, MinimumLength = 5)]
        public string AdminResponse { get; set; } = string.Empty;
    }

    public class SiteSettingEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "NGO Name is required.")]
        [StringLength(100)]
        public string NgoName { get; set; } = "Give-AID";

        [StringLength(200)]
        public string? Tagline { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City, State, Zip is required.")]
        public string CityStateZip { get; set; } = string.Empty;

        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? YouTubeUrl { get; set; }

        [Required(ErrorMessage = "Footer text is required.")]
        [StringLength(500)]
        public string FooterText { get; set; } = string.Empty;

        public string? ExistingLogoUrl { get; set; }
        public IFormFile? LogoFile { get; set; }
    }
}
