using Wallet.Models;

namespace Wallet.Api.Services
{
    public interface ISendFundsService
    {
        public Task sendTransaction(Transaction transaction);
    }
}
