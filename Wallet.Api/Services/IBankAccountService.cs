using Wallet.Models;

namespace Wallet.Api.Services
{
    public interface IBankAccountService
    {
        public Task<BankAccount> Get(string id);
        public Task<BankAccount?> GetByAccNumber(string accNumber);
        public Task Create(BankAccount bankAccount);
        public Task Update(string id, BankAccount bankAccount);
        public Task Remove(string id);
        public bool ValidAndSufficientFunds(string accNumber, decimal amount);
    }
}
