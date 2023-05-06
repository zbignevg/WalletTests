using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using System.Threading;
using Wallet.Controllers;
using Wallet.Models;

namespace Wallet.Services
{
    public class TransactionProcessorService : BackgroundService
    {
        private readonly TransactionsService _transactionService;
        private readonly BankAccountService _bankAccountService;
        private readonly ILogger<TransactionProcessorService> _logger;
        private readonly ConsumerConfig _consumerConfig;

        public TransactionProcessorService(
            TransactionsService transactionsService, 
            BankAccountService bankAccountService, 
            ILogger<TransactionProcessorService> logger
        ) {
            _transactionService = transactionsService;
            _bankAccountService = bankAccountService;
            _logger = logger;

            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:29092",
                GroupId = Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Latest
            };
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => startConsumer(cancellationToken));
        }

        public async void startConsumer(CancellationToken cancellationToken)
        {
            using (var consumer = new ConsumerBuilder<string?, string>(_consumerConfig).Build())
            {
                consumer.Subscribe("transactions");

                while (cancellationToken.IsCancellationRequested != true)
                {
                    var kafkaRecord = consumer.Consume();
                    var transactionId = kafkaRecord.Value;

                    var transaction = await _transactionService.GetAsync(transactionId);
                    var fromAccount = await _bankAccountService.GetByAccNumberAsync(transaction.From);
                    var toAccount = await _bankAccountService.GetByAccNumberAsync(transaction.To);

                    if (toAccount is not null) {
                        toAccount.Balance += transaction.Amount;

                        await _bankAccountService.UpdateAsync(toAccount.Id, toAccount);

                        _logger.LogInformation($"New amount {toAccount.Balance}");
                    } else
                    {
                        _logger.LogInformation($"Something went wrong");
                    }
                }
            }
        }
    }
}
