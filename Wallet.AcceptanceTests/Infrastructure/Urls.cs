namespace Wallet.AcceptanceTests.Infrastructure
{
    public static class Urls
    {
        public static class BankAccounts
        {
            private static string BankAccountsApiBaseAddress = "/api/bankAccount";
            public static string Get(string bankAccountId) => $"{BankAccountsApiBaseAddress}/{bankAccountId}";
            public static string Post => $"{BankAccountsApiBaseAddress}";
        }

        public static class Users
        {
            private static string UsersApiBaseAddress = "/api/user";
            public static string Create => $"{UsersApiBaseAddress}/create";
        }

    }
}
