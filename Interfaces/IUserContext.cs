namespace StudyPlanner.Interfaces
{
    public interface IUserContext
    {
        bool IsAuthenticated { get; }
        string? UserId { get; }
        string? Username { get; }
        void SetUser(string userId, string username);
        void Clear();
    }
}



