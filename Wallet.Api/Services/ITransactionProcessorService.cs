using Wallet.Models;

namespace Wallet.Api.Services
{
    public interface ITransactionProcessorService
    {
        public void startConsumer(CancellationToken cancellationToken);
    }
}
