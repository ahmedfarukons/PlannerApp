using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;
using StudyPlanner.Services;

namespace StudyPlanner.Repositories
{
    /// <summary>
    /// MongoDB PDF doküman kayıtları erişimi (pdf_documents koleksiyonu).
    /// </summary>
    public class MongoPdfDocumentRepository : IPdfDocumentRepository
    {
        private readonly MongoDbContext _db;

        public MongoPdfDocumentRepository(MongoDbContext db)
        {
            _db = db;
        }

        public async Task<string> UpsertFromSummaryAsync(string userId, string localFilePath, DocumentSummary summary)
        {
            var fileName = summary.FileName;
            if (string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(localFilePath))
                fileName = Path.GetFileName(localFilePath);

            var doc = new MongoPdfDocument
            {
                UserId = userId,
                FileName = fileName ?? string.Empty,
                LocalFilePath = localFilePath ?? string.Empty,
                Summary = summary.Summary ?? string.Empty,
                ModelsUsed = summary.ModelsUsed ?? string.Empty,
                UploadDateUtc = summary.UploadDate.ToUniversalTime(),
                FileSize = summary.FileSize,
                PageCount = summary.PageCount
            };

            // Kullanıcı + dosya yolu unique kabul edilerek upsert
            var filter = Builders<MongoPdfDocument>.Filter.Eq(x => x.UserId, userId) &
                         Builders<MongoPdfDocument>.Filter.Eq(x => x.LocalFilePath, doc.LocalFilePath);

            var options = new FindOneAndReplaceOptions<MongoPdfDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var saved = await _db.PdfDocuments.FindOneAndReplaceAsync(filter, doc, options);
            return saved.Id!;
        }

        public async Task<List<MongoPdfDocument>> GetByUserAsync(string userId, int limit = 200)
        {
            if (limit <= 0) limit = 200;
            return await _db.PdfDocuments
                .Find(x => x.UserId == userId)
                .SortByDescending(x => x.UploadDateUtc)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<MongoPdfDocument?> GetByIdAsync(string userId, string documentId)
        {
            return await _db.PdfDocuments
                .Find(x => x.UserId == userId && x.Id == documentId)
                .FirstOrDefaultAsync();
        }
    }
}


