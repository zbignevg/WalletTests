using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Wallet.Api.Repositories;
using Wallet.Api.Services;
using Wallet.Models;

namespace Wallet.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        public BankAccountService(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<BankAccount> Get(string id) => 
            await _bankAccountRepository.GetAsync(id);

        public async Task<BankAccount?> GetByAccNumber(string accNumber) => 
            await _bankAccountRepository.GetByAccNumberAsync(accNumber);

        public async Task Create(BankAccount bankAccount) => 
            await _bankAccountRepository.CreateAsync(bankAccount);

        public async Task Update(string id, BankAccount bankAccount) => 
            await _bankAccountRepository.UpdateAsync(id, bankAccount);

        public async Task Remove(string id) =>
            await _bankAccountRepository.RemoveAsync(id);

        public bool ValidAndSufficientFunds(string accNumber, decimal amount)
        {
            BankAccount bankAccount = GetByAccNumber(accNumber).Result;
            
            return bankAccount.Balance >= amount;
        }
    }
}
