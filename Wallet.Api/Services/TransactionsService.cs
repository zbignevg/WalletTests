using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Wallet.Models;

namespace Wallet.Services
{
    public class TransactionsService
    {
        private readonly IMongoCollection<Transaction> _transactionsCollection;
        public TransactionsService(IOptions<WalletDBSettings> walletDbSettings)
        {
            var mongoClient = new MongoClient(walletDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(walletDbSettings.Value.DatabaseName);

            _transactionsCollection = mongoDatabase.GetCollection<Transaction>(walletDbSettings.Value.TransactionsCollectionName);
        }

        public async Task<Transaction?> GetAsync(string id) =>
            await _transactionsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Transaction transaction) =>
            await _transactionsCollection.InsertOneAsync(transaction);

        public async Task UpdateAsync(string id, Transaction transaction) =>
            await _transactionsCollection.ReplaceOneAsync(x => x.Id == id, transaction);

        public async Task RemoveAsync(string id) =>
            await _transactionsCollection.DeleteOneAsync(x => x.Id == id);
    }
}
