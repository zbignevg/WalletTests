using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Wallet.Models;

namespace Wallet.Services
{
    public class KafkaSendFundsService
    {
        private ILogger<KafkaSendFundsService> _log;
        private CancellationToken _cancellationToken;
        private ProducerConfig _config;

        public KafkaSendFundsService(ILogger<KafkaSendFundsService> log)
        {
            _config = new ProducerConfig()
            {
                BootstrapServers = "localhost:29092"
            };
            _log = log;
        }

        public async Task sendTransaction(Transaction transaction)
        {
            using (var producer = new ProducerBuilder<string?, string>(_config).Build())
            {
                _log.LogInformation(JsonSerializer.Serialize(transaction));
                await producer.ProduceAsync("transactions", new Message<string?, string>()
                {
                    Value = transaction.Id
                }, _cancellationToken);
            }

            await Task.CompletedTask;
        }
    }
}

