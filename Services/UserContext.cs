using StudyPlanner.Interfaces;

namespace StudyPlanner.Services
{
    /// <summary>
    /// Oturumdaki kullanıcıyı (UserId/Username) tutan basit context.
    /// </summary>
    public class UserContext : IUserContext
    {
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId);
        public string? UserId { get; private set; }
        public string? Username { get; private set; }

        public void SetUser(string userId, string username)
        {
            UserId = userId;
            Username = username;
        }

        public void Clear()
        {
            UserId = null;
            Username = null;
        }
    }
}


