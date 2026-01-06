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
    public class SUserService : IUserService
    {
        private readonly MongoUserRepository _users;
        private readonly IUserContext _userContext;

        public SUserService(MongoUserRepository users, IUserContext userContext)
        {
            _users = users;
            _userContext = userContext;
        }

        public async Task<OperationResult> RegisterAsync(string username, string email, string fullName, string password)
        {
            username = (username ?? string.Empty).Trim();
            email = (email ?? string.Empty).Trim().ToLowerInvariant();
            fullName = (fullName ?? string.Empty).Trim();
            password = password ?? string.Empty;

            if (username.Length < 3)
                return OperationResult.Fail("Kullanıcı adı en az 3 karakter olmalı.");
            if (string.IsNullOrWhiteSpace(email) || !LooksLikeEmail(email))
                return OperationResult.Fail("Geçerli bir e‑posta girin.");
            if (fullName.Length < 3)
                return OperationResult.Fail("Ad Soyad en az 3 karakter olmalı.");
            if (password.Length < 6)
                return OperationResult.Fail("Şifre en az 6 karakter olmalı.");

            var existingUser = await _users.FindByUsernameAsync(username);
            if (existingUser != null)
                return OperationResult.Fail("Bu kullanıcı adı zaten kayıtlı.");

            var existingEmail = await _users.FindByEmailAsync(email);
            if (existingEmail != null)
                return OperationResult.Fail("Bu e‑posta zaten kayıtlı.");

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
            return OperationResult.Ok();
        }

        public async Task<OperationResult> LoginAsync(string identifier, string password)
        {
            identifier = (identifier ?? string.Empty).Trim();
            password = password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
                return OperationResult.Fail("E‑posta/kullanıcı adı ve şifre gerekli.");

            var user = await _users.FindByIdentifierAsync(identifier);
            if (user == null)
                return OperationResult.Fail("Kullanıcı bulunamadı (e‑posta/kullanıcı adı).");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return OperationResult.Fail("Şifre hatalı.");

            _userContext.SetUser(user.Id!, user.Username);
            return OperationResult.Ok();
        }

        public Task LogoutAsync()
        {
            _userContext.Clear();
            return Task.CompletedTask;
        }

        public async Task<MongoUser?> GetUserAsync(string userId)
        {
            return await _users.FindByIdAsync(userId);
        }

        public async Task<bool> UpdateUserAsync(MongoUser user)
        {
            // Validate basic fields
            if (string.IsNullOrWhiteSpace(user.FullName) || string.IsNullOrWhiteSpace(user.Email))
                return false;

            return await _users.UpdateAsync(user);
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _users.FindByIdAsync(userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                return false;

            if (newPassword.Length < 6) return false; // Basic validation

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return await _users.UpdateAsync(user);
        }

        private static bool LooksLikeEmail(string email)
        {
            // basit doğrulama
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}


