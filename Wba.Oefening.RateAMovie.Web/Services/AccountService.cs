using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Wba.Oefening.RateAMovie.Core.Entities;
using Wba.Oefening.RateAMovie.Web.Data;
using Wba.Oefening.RateAMovie.Web.ViewModels;

namespace Wba.Oefening.RateAMovie.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly MovieContext _movieContext;
        private readonly ILogger<AccountService> _logger;

        public AccountService(MovieContext movieContext, ILogger<AccountService> logger)
        {
            _movieContext = movieContext;
            _logger = logger;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            //check user  and
            //check credentials
            var user = await _movieContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(username));

            if (user == null || !Argon2.Verify(user?.Password, password))
            {
                return false;
            }
            return true;
        }

        public async Task<bool> RegisterAsync(string username, string password, string firstname, string lastname)
        {
            User user = new User();
            user.Username = username;
            user.FirstName = firstname;
            user.LastName = lastname;
            user.Password = Argon2.Hash(password);
            //add to the context
            _movieContext.Users.Add(user);
            try
            {
                await _movieContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException e)
            {
                //perform logging here in production environment
                _logger.LogError(e.Message);
                return false;
            }
        }
    }
}
