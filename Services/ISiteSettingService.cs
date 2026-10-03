using GiveAID.Models;

namespace GiveAID.Services
{
    public interface ISiteSettingService
    {
        Task<SiteSetting> GetSettingsAsync();
        Task UpdateSettingsAsync(SiteSetting updated);
    }
}
