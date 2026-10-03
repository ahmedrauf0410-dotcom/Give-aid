using System.ComponentModel.DataAnnotations;
using GiveAID.Models;

namespace GiveAID.ViewModels
{
    public class HomeViewModel
    {
        public SiteSetting SiteSetting { get; set; } = new();
        public IEnumerable<DonationCause> FeaturedCauses { get; set; } = new List<DonationCause>();
        public IEnumerable<Programme> UpcomingProgrammes { get; set; } = new List<Programme>();
        public IEnumerable<Partner> Partners { get; set; } = new List<Partner>();
        public IEnumerable<GalleryItem> GalleryPreview { get; set; } = new List<GalleryItem>();
        public IDictionary<string, AboutSection> AboutSections { get; set; } = new Dictionary<string, AboutSection>();

        // Impact Statistics
        public decimal TotalDonationsAmount { get; set; }
        public int TotalDonationsCount { get; set; }
        public int TotalProgrammesCount { get; set; }
        public int TotalVolunteersCount { get; set; }
        public int TotalBeneficiariesCount { get; set; } = 25400; // Realistic NGO baseline
    }

    public class AboutViewModel
    {
        public AboutSection? WhatWeDo { get; set; }
        public AboutSection? OurMission { get; set; }
        public AboutSection? OurTeam { get; set; }
        public AboutSection? CareerWithUs { get; set; }
        public AboutSection? OurAchievements { get; set; }
        public AboutSection? OurSupporters { get; set; }
        public AboutSection? ReadAboutUs { get; set; }
        public IEnumerable<Partner> Partners { get; set; } = new List<Partner>();
    }
}
