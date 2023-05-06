using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Wallet.Models;

namespace Wallet.Services
{
    public class BankAccountService
    {
        private readonly IMongoCollection<BankAccount> _bankAccountsCollection;
        private readonly IMongoDatabase _database;

        public BankAccountService(IMongoDatabase mongoDatabase, IOptions<WalletDBSettings> walletDbSettings)
        {
            //var mongoClient = new MongoClient(walletDbSettings.Value.ConnectionString);
            //var mongoDatabase = mongoClient.GetDatabase(walletDbSettings.Value.DatabaseName);

            _database = mongoDatabase;
            _bankAccountsCollection = mongoDatabase.GetCollection<BankAccount>(walletDbSettings.Value.BankAccountsCollectionName);
        }

        public async Task<BankAccount> GetAsync(string id) {
            var ba = _bankAccountsCollection.Find(x => x.Id == id);
            //var res = await ba.FirstOrDefaultAsync();
            var res = await ba.FirstOrDefaultAsync();

            return res;
                }
            //await _bankAccountsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task<BankAccount?> GetByAccNumberAsync(string accNumber) =>
            await _bankAccountsCollection.Find(x => x.AccNumber == accNumber).FirstOrDefaultAsync();

        public async Task CreateAsync(BankAccount bankAccount) =>
            await _bankAccountsCollection.InsertOneAsync(bankAccount);

        public async Task UpdateAsync(string id, BankAccount bankAccount) =>
            await _bankAccountsCollection.ReplaceOneAsync(x => x.Id == id, bankAccount);

        public async Task RemoveAsync(string id) =>
            await _bankAccountsCollection.DeleteOneAsync(x => x.Id == id);
    }
}
