using System.Threading.Tasks;
using MongoDB.Driver;
using StudyPlanner.Models;
using StudyPlanner.Services;

namespace StudyPlanner.Repositories
{
    /// <summary>
    /// MongoDB kullanıcı verisi erişimi (users koleksiyonu).
    /// </summary>
    public class MongoUserRepository
    {
        private readonly MongoDbContext _db;

        public MongoUserRepository(MongoDbContext db)
        {
            _db = db;
        }

        public async Task<MongoUser?> FindByUsernameAsync(string username)
        {
            return await _db.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<MongoUser?> FindByEmailAsync(string email)
        {
            return await _db.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<MongoUser?> FindByIdentifierAsync(string identifier)
        {
            // identifier: username or email
            return await _db.Users
                .Find(u => u.Username == identifier || u.Email == identifier)
                .FirstOrDefaultAsync();
        }

        public async Task<MongoUser?> FindByIdAsync(string id)
        {
            return await _db.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<MongoUser> InsertAsync(MongoUser user)
        {
            await _db.Users.InsertOneAsync(user);
            return user;
        }

        public async Task<bool> UpdateAsync(MongoUser user)
        {
            var result = await _db.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return result.ModifiedCount > 0;
        }
    }
}


