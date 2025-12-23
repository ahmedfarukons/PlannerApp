using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using StudyPlanner.Models;

namespace StudyPlanner.Services
{
    /// <summary>
    /// PDF kütüphanesi servis
    /// Kategorileri ve PDF'leri yönetir
    /// </summary>
    public class SPdfLibraryService
    {
        private const string DataFileName = "pdf_library.xml";
        private readonly string _dataFilePath;

        public SPdfLibraryService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "StudyPlanner");
            
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            _dataFilePath = Path.Combine(appFolder, DataFileName);
        }

        /// <summary>
        /// Tüm kategorileri yükle
        /// </summary>
        public async Task<List<Category>> LoadCategoriesAsync()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(_dataFilePath))
                    return GetDefaultCategories();

                try
                {
                    var serializer = new XmlSerializer(typeof(PdfLibraryData));
                    using (var reader = new StreamReader(_dataFilePath))
                    {
                        var data = (PdfLibraryData)serializer.Deserialize(reader);
                        return ConvertToCategories(data);
                    }
                }
                catch
                {
                    return GetDefaultCategories();
                }
            });
        }

        /// <summary>
        /// Kategorileri kaydet
        /// </summary>
        public async Task SaveCategoriesAsync(List<Category> categories)
        {
            await Task.Run(() =>
            {
                var data = ConvertToLibraryData(categories);
                var serializer = new XmlSerializer(typeof(PdfLibraryData));
                
                using (var writer = new StreamWriter(_dataFilePath))
                {
                    serializer.Serialize(writer, data);
                }
            });
        }

        /// <summary>
        /// Varsayılan kategoriler
        /// </summary>
        private List<Category> GetDefaultCategories()
        {
            return new List<Category>
            {
                new Category { Id = 1, Name = "Matematik", Icon = "📐", Color = "#2196F3" },
                new Category { Id = 2, Name = "Fizik", Icon = "⚛️", Color = "#FF5722" },
                new Category { Id = 3, Name = "Kimya", Icon = "🧪", Color = "#4CAF50" },
                new Category { Id = 4, Name = "Biyoloji", Icon = "🧬", Color = "#9C27B0" },
                new Category { Id = 5, Name = "Edebiyat", Icon = "📚", Color = "#FF9800" }
            };
        }

        private List<Category> ConvertToCategories(PdfLibraryData data)
        {
            var categories = new List<Category>();
            
            foreach (var catData in data.Categories)
            {
                var category = new Category
                {
                    Id = catData.Id,
                    Name = catData.Name,
                    Icon = catData.Icon,
                    Color = catData.Color
                };

                foreach (var docData in catData.Documents)
                {
                    category.Documents.Add(new PdfDocument
                    {
                        Id = docData.Id,
                        CategoryId = docData.CategoryId,
                        Title = docData.Title,
                        FilePath = docData.FilePath,
                        Summary = docData.Summary,
                        AddedDate = docData.AddedDate,
                        FileSize = docData.FileSize,
                        PageCount = docData.PageCount
                    });
                }

                categories.Add(category);
            }

            return categories;
        }

        private PdfLibraryData ConvertToLibraryData(List<Category> categories)
        {
            var data = new PdfLibraryData();

            foreach (var category in categories)
            {
                var catData = new CategoryData
                {
                    Id = category.Id,
                    Name = category.Name,
                    Icon = category.Icon,
                    Color = category.Color
                };

                foreach (var doc in category.Documents)
                {
                    catData.Documents.Add(new PdfDocumentData
                    {
                        Id = doc.Id,
                        CategoryId = doc.CategoryId,
                        Title = doc.Title,
                        FilePath = doc.FilePath,
                        Summary = doc.Summary,
                        AddedDate = doc.AddedDate,
                        FileSize = doc.FileSize,
                        PageCount = doc.PageCount
                    });
                }

                data.Categories.Add(catData);
            }

            return data;
        }
    }

    #region Data Transfer Objects

    [Serializable]
    public class PdfLibraryData
    {
        public List<CategoryData> Categories { get; set; } = new List<CategoryData>();
    }

    [Serializable]
    public class CategoryData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public List<PdfDocumentData> Documents { get; set; } = new List<PdfDocumentData>();
    }

    [Serializable]
    public class PdfDocumentData
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; }
        public long FileSize { get; set; }
        public int PageCount { get; set; }
    }

    #endregion
}





