using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;

namespace StudyPlanner.Repositories
{
    /// <summary>
    /// StudyPlanItem için Repository implementasyonu
    /// SOLID: Single Responsibility - Sadece veri erişim sorumluluğu
    /// SOLID: Dependency Inversion - IRepository interface'ini implement eder
    /// </summary>
    public class StudyPlanRepository : IRepository<StudyPlanItem>
    {
        private readonly List<StudyPlanItem> _items;

        /// <summary>
        /// Constructor
        /// </summary>
        public StudyPlanRepository()
        {
            _items = new List<StudyPlanItem>();
        }

        /// <summary>
        /// Tüm çalışma planlarını getirir
        /// </summary>
        public async Task<IEnumerable<StudyPlanItem>> GetAllAsync()
        {
            return await Task.FromResult(_items.AsEnumerable());
        }

        /// <summary>
        /// ID'ye göre çalışma planı getirir
        /// </summary>
        public async Task<StudyPlanItem> GetByIdAsync(Guid id)
        {
            return await Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
        }

        /// <summary>
        /// Yeni çalışma planı ekler
        /// </summary>
        public async Task<StudyPlanItem> AddAsync(StudyPlanItem entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            _items.Add(entity);

            return await Task.FromResult(entity);
        }

        /// <summary>
        /// Çalışma planını günceller
        /// </summary>
        public async Task<StudyPlanItem> UpdateAsync(StudyPlanItem entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existing = _items.FirstOrDefault(x => x.Id == entity.Id);
            if (existing != null)
            {
                var index = _items.IndexOf(existing);
                entity.ModifiedDate = DateTime.Now;
                _items[index] = entity;
            }

            return await Task.FromResult(entity);
        }

        /// <summary>
        /// Çalışma planını siler
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                _items.Remove(item);
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        /// <summary>
        /// Kritere göre çalışma planları arar
        /// </summary>
        public async Task<IEnumerable<StudyPlanItem>> FindAsync(Func<StudyPlanItem, bool> predicate)
        {
            return await Task.FromResult(_items.Where(predicate));
        }

        /// <summary>
        /// Toplam kayıt sayısını döner
        /// </summary>
        public async Task<int> CountAsync()
        {
            return await Task.FromResult(_items.Count);
        }

        /// <summary>
        /// Tüm listeyi temizler
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Toplu ekleme yapar
        /// </summary>
        public async Task AddRangeAsync(IEnumerable<StudyPlanItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                await AddAsync(item);
            }
        }
    }
}



