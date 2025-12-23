using System.Threading.Tasks;
using StudyPlanner.Models;

namespace StudyPlanner.Interfaces
{
    public interface IUserService
    {
        Task<OperationResult> RegisterAsync(string username, string email, string fullName, string password);
        Task<OperationResult> LoginAsync(string identifier, string password); // username or email
        Task LogoutAsync();
    }
}


