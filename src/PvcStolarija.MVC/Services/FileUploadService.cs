namespace PvcStolarija.MVC.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Nedozvoljen format slike. Dozvoljeni su: JPG, JPEG, PNG, GIF, WEBP");

            if (file.Length > 5 * 1024 * 1024)
                throw new Exception("Slika je prevelika. Maksimalna velicina je 5MB.");

            // WebRootPath moze biti null — fallback
            var webRoot = _environment.WebRootPath
                ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

            var uploadsFolder = Path.Combine(webRoot, "uploads", folder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<bool> DeleteImageAsync(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            try
            {
                var webRoot = _environment.WebRootPath
                    ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

                var fullPath = Path.Combine(webRoot, imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public string GetDefaultImage()
        {
            return "/uploads/default-avatar.png";
        }
    }
}
