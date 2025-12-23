using System;
using MongoDB.Driver;
using StudyPlanner.Models;

namespace StudyPlanner.Services
{
    /// <summary>
    /// MongoDB erişimi için context sınıfı (koleksiyonları tek noktadan sağlar).
    /// </summary>
    public class MongoDbContext
    {
        public IMongoDatabase Database { get; }

        public IMongoCollection<MongoUser> Users => Database.GetCollection<MongoUser>("users");
        public IMongoCollection<MongoPdfDocument> PdfDocuments => Database.GetCollection<MongoPdfDocument>("pdf_documents");
        public IMongoCollection<MongoChatMessage> ChatMessages => Database.GetCollection<MongoChatMessage>("chat_messages");

        public MongoDbContext(string connectionString, string databaseName)
        {
            // Fast-fail timeouts so UI doesn't look "stuck" when Mongo isn't running.
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            settings.ConnectTimeout = TimeSpan.FromSeconds(5);
            settings.SocketTimeout = TimeSpan.FromSeconds(10);

            var client = new MongoClient(settings);
            Database = client.GetDatabase(databaseName);
        }
    }
}


