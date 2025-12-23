using System.Threading.Tasks;
using System.Collections.Generic;
using StudyPlanner.Models;

namespace StudyPlanner.Interfaces
{
    public interface IPdfDocumentRepository
    {
        Task<string> UpsertFromSummaryAsync(string userId, string localFilePath, DocumentSummary summary);
        Task<List<MongoPdfDocument>> GetByUserAsync(string userId, int limit = 200);
        Task<MongoPdfDocument?> GetByIdAsync(string userId, string documentId);
    }
}


