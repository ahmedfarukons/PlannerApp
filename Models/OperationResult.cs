using System;

namespace StudyPlanner.Models
{
    /// <summary>
    /// Servis çağrılarının standart dönüş tipidir.
    /// Başarılı/başarısız durumu ve hata mesajını tek tipte taşır.
    /// </summary>
    public sealed class OperationResult
    {
        /// <summary>
        /// İşlemin başarılı olup olmadığını belirtir.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Başarısızlık durumunda kullanıcıya gösterilebilecek hata mesajı.
        /// </summary>
        public string ErrorMessage { get; }

        private OperationResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        /// <summary>
        /// Başarılı sonuç üretir.
        /// </summary>
        public static OperationResult Ok()
        {
            return new OperationResult(true, string.Empty);
        }

        /// <summary>
        /// Hatalı sonuç üretir.
        /// </summary>
        public static OperationResult Fail(string errorMessage)
        {
            return new OperationResult(false, errorMessage);
        }
    }

    /// <summary>
    /// Veri döndüren servis çağrılarının standart dönüş tipidir.
    /// </summary>
    /// <typeparam name="T">Dönen veri tipi</typeparam>
    public sealed class OperationResult<T>
    {
        /// <summary>
        /// İşlemin başarılı olup olmadığını belirtir.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Başarısızlık durumunda kullanıcıya gösterilebilecek hata mesajı.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Başarılı durumda dönen veri.
        /// </summary>
        public T? Data { get; }

        private OperationResult(bool success, T? data, string errorMessage)
        {
            Success = success;
            Data = data;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        /// <summary>
        /// Başarılı sonuç üretir.
        /// </summary>
        public static OperationResult<T> Ok(T data)
        {
            return new OperationResult<T>(true, data, string.Empty);
        }

        /// <summary>
        /// Hatalı sonuç üretir.
        /// </summary>
        public static OperationResult<T> Fail(string errorMessage)
        {
            return new OperationResult<T>(false, default, errorMessage);
        }
    }
}



