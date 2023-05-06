using Xunit;

namespace Wallet.AcceptanceTests.Infrastructure
{
    [CollectionDefinition("Wallet API tests")]
    public class WalletApiCollection : ICollectionFixture<WalletApiHost>
    {
    }
}
