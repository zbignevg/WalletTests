using Wallet.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Wallet.Api.Repositories;

namespace Wallet.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepository(IMongoDatabase mongoDatabase, IOptions<WalletDBSettings> walletDbSettings)
        {
            _usersCollection = mongoDatabase.GetCollection<User>(walletDbSettings.Value.UsersCollectionName);
        }

        public async Task<User?> GetAsync(string id) =>
            await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<User?> GetByEmailAsync(string email) =>
            await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task CreateAsync(User user) =>
            await _usersCollection.InsertOneAsync(user);

        public async Task UpdateAsync(string id, User user) =>
            await _usersCollection.ReplaceOneAsync(x => x.Id == id, user);

        public async Task RemoveAsync(string id) =>
            await _usersCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<User?> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            return await _usersCollection.Find(u => u.Email == email && u.Password == password).FirstOrDefaultAsync();
        }
    }
}
