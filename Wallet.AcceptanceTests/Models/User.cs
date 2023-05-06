namespace Wallet.AcceptanceTests.Models
{
    public record User(string Id, string Role, string Email, string FirstName, string LasName, string Password, string Salt);
}
