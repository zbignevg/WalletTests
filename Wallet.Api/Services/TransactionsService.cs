using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Wallet.Api.Repositories;
using Wallet.Api.Services;
using Wallet.Models;

namespace Wallet.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepository _transactionsRepository;
        public TransactionsService(ITransactionsRepository transactionsRepository)
        {
            _transactionsRepository = transactionsRepository;
        }

        public async Task<Transaction?> Get(string id) => 
            await _transactionsRepository.GetAsync(id);

        public async Task CreateAsync(Transaction transaction) => 
            await _transactionsRepository.CreateAsync(transaction);

        public async Task UpdateAsync(string id, Transaction transaction) => 
            await _transactionsRepository.UpdateAsync(id,transaction);

        public async Task RemoveAsync(string id) => 
            await _transactionsRepository.RemoveAsync(id);
    }
}
