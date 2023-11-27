namespace Wba.Oefening.RateAMovie.Web.Services
{
    public interface IAccountService
    {
        Task<bool> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string password, string firstname,
            string lastname);
    }
}
