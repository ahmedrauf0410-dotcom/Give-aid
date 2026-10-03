using Microsoft.EntityFrameworkCore;
using GiveAID.Data;
using GiveAID.Models;

namespace GiveAID.Services
{
    public class SiteSettingService : ISiteSettingService
    {
        private readonly ApplicationDbContext _context;

        public SiteSettingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SiteSetting> GetSettingsAsync()
        {
            var setting = await _context.SiteSettings.AsNoTracking().FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new SiteSetting
                {
                    NgoName = "Give-AID",
                    Tagline = "Empowering Communities, Transforming Lives",
                    Email = "info@giveaid.org",
                    Phone = "+1 (555) 019-2834",
                    Address = "742 Evergreen Terrace, Suite 100",
                    CityStateZip = "Springfield, IL 62704",
                    FacebookUrl = "https://facebook.com",
                    TwitterUrl = "https://twitter.com",
                    InstagramUrl = "https://instagram.com",
                    YouTubeUrl = "https://youtube.com",
                    FooterText = "Give-AID is a registered non-profit organization dedicated to fostering sustainable welfare, healthcare, education, and social empowerment across vulnerable populations.",
                    UpdatedAt = DateTime.UtcNow
                };

                _context.SiteSettings.Add(setting);
                await _context.SaveChangesAsync();
            }

            return setting;
        }

        public async Task UpdateSettingsAsync(SiteSetting updated)
        {
            var existing = await _context.SiteSettings.FirstOrDefaultAsync();
            if (existing == null)
            {
                _context.SiteSettings.Add(updated);
            }
            else
            {
                existing.NgoName = updated.NgoName;
                existing.Tagline = updated.Tagline;
                existing.Email = updated.Email;
                existing.Phone = updated.Phone;
                existing.Address = updated.Address;
                existing.CityStateZip = updated.CityStateZip;
                existing.FacebookUrl = updated.FacebookUrl;
                existing.TwitterUrl = updated.TwitterUrl;
                existing.InstagramUrl = updated.InstagramUrl;
                existing.YouTubeUrl = updated.YouTubeUrl;
                existing.FooterText = updated.FooterText;
                if (!string.IsNullOrEmpty(updated.LogoUrl))
                {
                    existing.LogoUrl = updated.LogoUrl;
                }
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
