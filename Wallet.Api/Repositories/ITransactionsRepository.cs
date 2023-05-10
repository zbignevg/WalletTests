using Wallet.Models;

namespace Wallet.Api.Repositories
{
    public interface ITransactionsRepository
    {
        public Task<Transaction?> GetAsync(string id);
        public Task CreateAsync(Transaction transaction);
        public Task UpdateAsync(string id, Transaction transaction);
        public Task RemoveAsync(string id);
    }
}
