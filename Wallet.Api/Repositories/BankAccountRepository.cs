using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Wallet.Models;

namespace Wallet.Api.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly IMongoCollection<BankAccount> _bankAccountsCollection;

        public BankAccountRepository(IMongoDatabase mongoDatabase, IOptions<WalletDBSettings> walletDbSettings)
        {
            _bankAccountsCollection = mongoDatabase.GetCollection<BankAccount>(walletDbSettings.Value.BankAccountsCollectionName);
        }

        public async Task<BankAccount> GetAsync(string id) =>
            await _bankAccountsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

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
