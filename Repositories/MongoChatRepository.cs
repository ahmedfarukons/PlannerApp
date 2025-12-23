using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;
using StudyPlanner.Services;
using MongoDB.Driver;

namespace StudyPlanner.Repositories
{
    /// <summary>
    /// MongoDB chat mesajları erişimi (chat_messages koleksiyonu).
    /// </summary>
    public class MongoChatRepository : IChatRepository
    {
        private readonly MongoDbContext _db;

        public MongoChatRepository(MongoDbContext db)
        {
            _db = db;
        }

        public async Task AddMessageAsync(string userId, string documentId, bool isUser, string content, DateTime timestampUtc)
        {
            var msg = new MongoChatMessage
            {
                UserId = userId,
                DocumentId = documentId,
                IsUser = isUser,
                Content = content ?? string.Empty,
                TimestampUtc = timestampUtc
            };

            await _db.ChatMessages.InsertOneAsync(msg);
        }

        public async Task<List<MongoChatMessage>> GetMessagesAsync(string userId, string documentId, int limit = 500)
        {
            if (limit <= 0) limit = 500;
            return await _db.ChatMessages
                .Find(x => x.UserId == userId && x.DocumentId == documentId)
                .SortBy(x => x.TimestampUtc)
                .Limit(limit)
                .ToListAsync();
        }
    }
}


