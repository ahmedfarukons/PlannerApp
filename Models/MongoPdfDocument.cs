using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudyPlanner.Models
{
    public class MongoPdfDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("fileName")]
        public string FileName { get; set; } = string.Empty;

        [BsonElement("localFilePath")]
        public string LocalFilePath { get; set; } = string.Empty;

        [BsonElement("summary")]
        public string Summary { get; set; } = string.Empty;

        [BsonElement("modelsUsed")]
        public string ModelsUsed { get; set; } = string.Empty;

        [BsonElement("uploadDateUtc")]
        public DateTime UploadDateUtc { get; set; } = DateTime.UtcNow;

        [BsonElement("fileSize")]
        public long FileSize { get; set; }

        [BsonElement("pageCount")]
        public int PageCount { get; set; }
    }
}


