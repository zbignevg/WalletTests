using Wallet.Models;

namespace Wallet.Api.Services
{
    public interface ITransactionsService
    {
        public Task<Transaction?> Get(string id);
        public Task CreateAsync(Transaction transaction);
        public Task UpdateAsync(string id, Transaction transaction);
        public Task RemoveAsync(string id);
    }
}
