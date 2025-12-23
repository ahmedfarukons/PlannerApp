using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;

namespace StudyPlanner.Services
{
    /// <summary>
    /// XML dosya işlemleri için data service
    /// SOLID: Single Responsibility - Sadece dosya okuma/yazma sorumluluğu
    /// </summary>
    public class SXmlDataService : IDataService<List<StudyPlanItem>>
    {
        private const string DefaultFileName = "studyplans.xml";
        private readonly string _defaultFilePath;

        /// <summary>
        /// Constructor
        /// </summary>
        public SXmlDataService()
        {
            // Uygulama çalıştırma dizininde varsayılan dosya yolu
            _defaultFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "StudyPlanner",
                DefaultFileName
            );

            // Klasör yoksa oluştur
            var directory = Path.GetDirectoryName(_defaultFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Belirtilen dosyadan verileri yükler
        /// </summary>
        public async Task<List<StudyPlanItem>> LoadFromFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("Dosya yolu boş olamaz", nameof(filePath));

                if (!File.Exists(filePath))
                    return new List<StudyPlanItem>();

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var serializer = new XmlSerializer(typeof(List<StudyPlanItem>));
                    var result = await Task.Run(() => serializer.Deserialize(stream) as List<StudyPlanItem>);
                    return result ?? new List<StudyPlanItem>();
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Dosya yüklenirken hata oluştu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Belirtilen dosyaya verileri kaydeder
        /// </summary>
        public async Task<bool> SaveToFileAsync(List<StudyPlanItem> data, string filePath)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("Dosya yolu boş olamaz", nameof(filePath));

                // Klasör yoksa oluştur
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(List<StudyPlanItem>));
                    await Task.Run(() => serializer.Serialize(stream, data));
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new IOException($"Dosya kaydedilirken hata oluştu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Varsayılan dosyadan verileri yükler
        /// </summary>
        public async Task<List<StudyPlanItem>> LoadAsync()
        {
            return await LoadFromFileAsync(_defaultFilePath);
        }

        /// <summary>
        /// Varsayılan dosyaya verileri kaydeder
        /// </summary>
        public async Task<bool> SaveAsync(List<StudyPlanItem> data)
        {
            return await SaveToFileAsync(data, _defaultFilePath);
        }
    }
}



