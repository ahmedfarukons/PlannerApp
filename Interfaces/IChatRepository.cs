using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using StudyPlanner.Models;

namespace StudyPlanner.Interfaces
{
    public interface IChatRepository
    {
        Task AddMessageAsync(string userId, string documentId, bool isUser, string content, DateTime timestampUtc);
        Task<List<MongoChatMessage>> GetMessagesAsync(string userId, string documentId, int limit = 500);
    }
}


