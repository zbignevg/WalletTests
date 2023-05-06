using System.Net.Http.Json;
using System.Net.Mail;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Wallet.AcceptanceTests.Models;

namespace Wallet.AcceptanceTests.Infrastructure
{
    public class WalletApiHost : IDisposable
    {
        private readonly WebApplicationFactory<Program> _applicationFactory;

        private readonly CancellationTokenSource _cancellationTokenSource;
        public readonly CancellationToken CancellationToken;

        private string? _userJwtToken;

        public WalletApiHost()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var environmentName = configuration.GetValue<string>("Environment");
            var hostUrl = configuration.GetValue<string>("WalletApiHostUrl");

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environmentName);

            _applicationFactory = new WebApplicationFactory<Program>();
            _applicationFactory.Server.BaseAddress = new Uri(hostUrl);
            _applicationFactory.ClientOptions.BaseAddress = new Uri(hostUrl);

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;

            CreateUserAsync().Wait();
        }

        private async Task CreateUserAsync()
        {
            var user = new Fixture()
                .Build<User>()
                .With(x => x.Id, ObjectId.GenerateNewId().ToString)
                .With(x => x.Email, new Fixture().Create<MailAddress>().Address)
                .Create();
            
            var httpClient = GetHttpClient();
            var response = await httpClient.PostAsJsonAsync(Urls.Users.Create, user, CancellationToken);
            _userJwtToken = await response.Content.ReadAsStringAsync(CancellationToken);
        }


        public async Task WithHttpClientAsync(Func<HttpClient, Task> func)
        {
            var httpClient = GetHttpClient();
            await func(httpClient);
        }

        public async Task WithAuthorizedHttpClient(Func<HttpClient, Task> func)
        {
            var authorizedHttpClient = GetHttpAuthorizedClient();
            await func(authorizedHttpClient);
        }

        public async Task<TResult> WithAuthorizedHttpClient<TResult>(Func<HttpClient, Task<TResult>> func)
        {
            var authorizedHttpClient = GetHttpAuthorizedClient();
            return await func(authorizedHttpClient);
        }
        
        public HttpClient GetHttpClient()
        {
            return _applicationFactory.CreateClient();
        }
        
        public HttpClient GetHttpAuthorizedClient()
        {
            var httpClient = GetHttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_userJwtToken}");

            return httpClient;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Token.WaitHandle.WaitOne();

            _applicationFactory.Dispose();
        }
    }
}