namespace PvcStolarija.MVC.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);
        Task<bool> DeleteImageAsync(string filePath);
        string GetDefaultImage();
    }
}
