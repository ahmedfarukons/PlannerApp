using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyPlanner.Interfaces
{
    /// <summary>
    /// Generic Repository Pattern Interface
    /// SOLID: Interface Segregation Principle
    /// SOLID: Dependency Inversion Principle - Üst seviye modüller bu interface'e bağımlı
    /// </summary>
    /// <typeparam name="T">Entity tipi</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Tüm kayıtları getirir
        /// </summary>
        /// <returns>Entity listesi</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// ID'ye göre kayıt getirir
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>Bulunan entity veya null</returns>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Yeni kayıt ekler
        /// </summary>
        /// <param name="entity">Eklenecek entity</param>
        /// <returns>Eklenen entity</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Kayıt günceller
        /// </summary>
        /// <param name="entity">Güncellenecek entity</param>
        /// <returns>Güncellenen entity</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Kayıt siler
        /// </summary>
        /// <param name="id">Silinecek entity'nin ID'si</param>
        /// <returns>Silme işlemi başarılı mı</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Kritere göre kayıt arar
        /// </summary>
        /// <param name="predicate">Arama kriteri</param>
        /// <returns>Bulunan entityler</returns>
        Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);

        /// <summary>
        /// Toplam kayıt sayısını döner
        /// </summary>
        /// <returns>Kayıt sayısı</returns>
        Task<int> CountAsync();
    }
}



