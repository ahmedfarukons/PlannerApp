using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudyPlanner.Models
{
    public class MongoChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("documentId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DocumentId { get; set; } = string.Empty;

        [BsonElement("isUser")]
        public bool IsUser { get; set; }

        [BsonElement("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("timestampUtc")]
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    }
}


