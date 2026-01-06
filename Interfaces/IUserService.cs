using System.Threading.Tasks;
using StudyPlanner.Models;

namespace StudyPlanner.Interfaces
{
    public interface IUserService
    {
        Task<OperationResult> RegisterAsync(string username, string email, string fullName, string password);
        Task<OperationResult> LoginAsync(string identifier, string password); // username or email
        Task LogoutAsync();
        
        Task<MongoUser?> GetUserAsync(string userId);
        Task<bool> UpdateUserAsync(MongoUser user);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}


