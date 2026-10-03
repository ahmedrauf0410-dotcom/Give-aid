using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GiveAID.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
            {
                return (false, null, "No file was selected for upload.");
            }

            if (file.Length > MaxFileSize)
            {
                return (false, null, $"File size exceeds maximum allowed limit of {MaxFileSize / (1024 * 1024)} MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return (false, null, $"Unsupported file extension '{extension}'. Only JPG, JPEG, PNG, and WEBP images are permitted.");
            }

            if (!_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return (false, null, $"Invalid image content type '{file.ContentType}'.");
            }

            try
            {
                // Ensure target folder exists under wwwroot/uploads/{subFolder}
                var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                var uploadsFolder = Path.Combine(webRoot, "uploads", subFolder);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename using GUID
                var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
                var destinationPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var relativePath = $"/uploads/{subFolder}/{uniqueFileName}";
                return (true, relativePath, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Error saving uploaded file: {ex.Message}");
            }
        }

        public void DeleteFile(string? relativeFilePath)
        {
            if (string.IsNullOrWhiteSpace(relativeFilePath))
                return;

            try
            {
                // Only delete files inside /uploads/ to prevent arbitrary file deletion
                if (!relativeFilePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                    return;

                var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                var fullPath = Path.Combine(webRoot, relativeFilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
                // Suppress deletion errors so primary operations succeed
            }
        }
    }
}
