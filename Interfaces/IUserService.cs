using System.Threading.Tasks;

namespace StudyPlanner.Interfaces
{
    public interface IUserService
    {
        Task<(bool Success, string ErrorMessage)> RegisterAsync(string username, string email, string fullName, string password);
        Task<(bool Success, string ErrorMessage)> LoginAsync(string identifier, string password); // username or email
        Task LogoutAsync();
    }
}


