using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Bson;
using Wallet.AcceptanceTests.Infrastructure;
using Wallet.AcceptanceTests.Infrastructure.Attributes;
using Xunit;
using BankAccount = Wallet.AcceptanceTests.Models.BankAccount;

namespace Wallet.AcceptanceTests
{
    [Collection("Wallet API tests")]
    public class BankAccountsApiTests
    {
        private readonly WalletApiHost _walletApiHost;

        public BankAccountsApiTests(WalletApiHost walletApiHost)
        {
            _walletApiHost = walletApiHost;
        }

        [Fact]
        public async Task GetBankAccount_ShouldReturnUnauthorizedStatusForUnauthorizedClient()
        {
            await _walletApiHost.WithHttpClientAsync(async httpClient =>
            {
                var response = await httpClient.GetAsync(Urls.BankAccounts.Get(ObjectId.GenerateNewId().ToString()), _walletApiHost.CancellationToken);
                response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
            });
        }

        [Fact]
        public async Task GetBankAccount_ShouldNotFoundForNonExistingBankAccountId()
        {
            var bankAccountId = ObjectId.GenerateNewId().ToString();

            await _walletApiHost.WithAuthorizedHttpClient(async httpClient =>
            {
                var response = await httpClient.GetAsync(Urls.BankAccounts.Get(bankAccountId), _walletApiHost.CancellationToken);
                response.Should().HaveStatusCode(HttpStatusCode.NotFound);
            });
        }

        [Theory]
        [DefaultAutoData]
        public async Task GetBankAccount_ShouldReturnBankAccountDetails(BankAccount bankAccount)
        {
            await _walletApiHost.WithAuthorizedHttpClient(async httpClient =>
            {
                var response = await httpClient.PostAsJsonAsync(Urls.BankAccounts.Post, bankAccount, _walletApiHost.CancellationToken);
                response.Should().HaveStatusCode(HttpStatusCode.Created);

                var getBankAccountResponse = await httpClient.GetAsync(Urls.BankAccounts.Get(bankAccount.Id), _walletApiHost.CancellationToken);
                getBankAccountResponse.Should().HaveStatusCode(HttpStatusCode.OK);

                var responseContent = await getBankAccountResponse.Content.ReadFromJsonAsync<BankAccount>(cancellationToken: _walletApiHost.CancellationToken);
                responseContent.Should().BeEquivalentTo(bankAccount);
            });
        }

        [Theory]
        [DefaultAutoData]
        public async Task PostBankAccount_ShouldReturnUnauthorizedStatusForUnauthorizedClient(BankAccount bankAccount)
        {
            await _walletApiHost.WithHttpClientAsync(async httpClient =>
            {
                var response = await httpClient.PostAsJsonAsync(Urls.BankAccounts.Post, bankAccount, _walletApiHost.CancellationToken);
                response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
            });
        }

        [Theory]
        [DefaultAutoData]
        public async Task PostBankAccount_ShouldSaveNewBankAccount(BankAccount bankAccount)
        {
            await _walletApiHost.WithAuthorizedHttpClient(async httpClient =>
            {
                var response = await httpClient.PostAsJsonAsync(Urls.BankAccounts.Post, bankAccount, _walletApiHost.CancellationToken);
                response.Should().HaveStatusCode(HttpStatusCode.Created);

                var responseContent = await response.Content.ReadFromJsonAsync<BankAccount>(cancellationToken: _walletApiHost.CancellationToken);
                responseContent.Should().BeEquivalentTo(bankAccount);
            });
        }
    }
}