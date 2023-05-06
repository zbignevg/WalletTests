﻿namespace Wallet.Models
{
    public class WalletDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UsersCollectionName { get; set; } = null!;
        public string BankAccountsCollectionName { get; set; } = null!;
        public string TransactionsCollectionName { get; set; } = null!;
    }
}
