using System.ComponentModel.DataAnnotations;

namespace GiveAID.Models
{
    public class AboutSection
    {
        public int Id { get; set; }

        /// <summary>
        /// Key identifier: WhatWeDo, OurMission, OurTeam, CareerWithUs, OurAchievements, OurSupporters, ReadAboutUs
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string SectionKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Subtitle { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
