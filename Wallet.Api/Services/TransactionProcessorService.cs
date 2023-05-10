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
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TransactionProcessorService> _logger;
        private readonly ConsumerConfig _consumerConfig;

        public TransactionProcessorService(
            IServiceProvider serviceProvider,
            ILogger<TransactionProcessorService> logger
        ) {
            _serviceProvider = serviceProvider;
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

                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    {
                        TransactionsService transactionService = scope.ServiceProvider.GetRequiredService<TransactionsService>();
                        BankAccountService bankAccountService = scope.ServiceProvider.GetRequiredService<BankAccountService>();

                        var transaction = await transactionService.Get(transactionId);
                        var fromAccount = await bankAccountService.GetByAccNumber(transaction.From);
                        var toAccount = await bankAccountService.GetByAccNumber(transaction.To);

                        if (toAccount is not null)
                        {
                            toAccount.Balance += transaction.Amount;

                            await bankAccountService.Update(toAccount.Id, toAccount);

                            _logger.LogInformation($"New amount {toAccount.Balance}");
                        }
                        else
                        {
                            _logger.LogInformation($"Something went wrong");
                        }
                    }   
                }
            }
        }
    }
}
