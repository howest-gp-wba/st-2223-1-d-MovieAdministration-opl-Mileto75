namespace Wba.Oefening.RateAMovie.Web.Services
{
    public interface IFileService
    {
        Task<string> AddOrUpdateFile(IFormFile file, string subPath, IWebHostEnvironment webHostEnvironment, string fileName = "");
    }
}