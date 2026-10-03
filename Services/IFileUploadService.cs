using Microsoft.AspNetCore.Http;

namespace GiveAID.Services
{
    public interface IFileUploadService
    {
        Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadFileAsync(IFormFile file, string subFolder);
        void DeleteFile(string? relativeFilePath);
    }
}
