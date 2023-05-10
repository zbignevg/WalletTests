using Wallet.Models;

namespace Wallet.Api.Repositories
{
    public interface IBankAccountRepository
    {
        public Task<BankAccount> GetAsync(string id);
        public Task<BankAccount?> GetByAccNumberAsync(string accNumber);
        public Task CreateAsync(BankAccount bankAccount);
        public Task UpdateAsync(string id, BankAccount bankAccount);
        public Task RemoveAsync(string id);
    }
}
