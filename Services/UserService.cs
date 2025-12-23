using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;
using StudyPlanner.Repositories;

namespace StudyPlanner.Services
{
    /// <summary>
    /// MongoDB üzerinde kullanıcı kayıt/giriş işlemleri (BCrypt ile şifre doğrulama).
    /// </summary>
    public class UserService : IUserService
    {
        private readonly MongoUserRepository _users;
        private readonly IUserContext _userContext;

        public UserService(MongoUserRepository users, IUserContext userContext)
        {
            _users = users;
            _userContext = userContext;
        }

        public async Task<(bool Success, string ErrorMessage)> RegisterAsync(string username, string email, string fullName, string password)
        {
            username = (username ?? string.Empty).Trim();
            email = (email ?? string.Empty).Trim().ToLowerInvariant();
            fullName = (fullName ?? string.Empty).Trim();
            password = password ?? string.Empty;

            if (username.Length < 3)
                return (false, "Kullanıcı adı en az 3 karakter olmalı.");
            if (string.IsNullOrWhiteSpace(email) || !LooksLikeEmail(email))
                return (false, "Geçerli bir e‑posta girin.");
            if (fullName.Length < 3)
                return (false, "Ad Soyad en az 3 karakter olmalı.");
            if (password.Length < 6)
                return (false, "Şifre en az 6 karakter olmalı.");

            var existingUser = await _users.FindByUsernameAsync(username);
            if (existingUser != null)
                return (false, "Bu kullanıcı adı zaten kayıtlı.");

            var existingEmail = await _users.FindByEmailAsync(email);
            if (existingEmail != null)
                return (false, "Bu e‑posta zaten kayıtlı.");

            var user = new MongoUser
            {
                Username = username,
                Email = email,
                FullName = fullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAtUtc = DateTime.UtcNow
            };

            await _users.InsertAsync(user);
            _userContext.SetUser(user.Id!, user.Username);
            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> LoginAsync(string identifier, string password)
        {
            identifier = (identifier ?? string.Empty).Trim();
            password = password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
                return (false, "E‑posta/kullanıcı adı ve şifre gerekli.");

            var user = await _users.FindByIdentifierAsync(identifier);
            if (user == null)
                return (false, "Kullanıcı bulunamadı (e‑posta/kullanıcı adı).");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (false, "Şifre hatalı.");

            _userContext.SetUser(user.Id!, user.Username);
            return (true, string.Empty);
        }

        public Task LogoutAsync()
        {
            _userContext.Clear();
            return Task.CompletedTask;
        }

        private static bool LooksLikeEmail(string email)
        {
            // basit doğrulama
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}


