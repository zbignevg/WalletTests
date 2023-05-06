namespace Wallet.AcceptanceTests.Models
{
    public record BankAccount(string Id, string State, string UserId, string Currency, string AccNumber, decimal Balance);
}
