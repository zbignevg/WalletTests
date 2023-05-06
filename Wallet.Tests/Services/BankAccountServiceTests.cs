using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Models;
using Wallet.Services;
using Xunit;

namespace Wallet.Tests.Services
{
    public class BankAccountServiceTests
    {
        [Fact]
        public async void BankAccountService_ShouldReturnCollectionOfBankAccounts()
        {
            // Arrange
            //var collectionMock = new Mock<IMongoCollection<BankAccount>>();
            //IMongoCollectionExtensions.Fir
            var collectionMock = Mock.Of<IMongoCollection<BankAccount>>();
            //collectionMock.InsertOne(new BankAccount()
            //{
            //    Id = "644008659e40371bf3268ff3"
            //});
            var dbMock = new Mock<IMongoDatabase>();
            dbMock.Setup(_ => _.GetCollection<BankAccount>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(collectionMock);

            var WalletDbSettings = new WalletDBSettings()
            {
                BankAccountsCollectionName = "BankAccounts"
            };
            var mock = new Mock<IOptions<WalletDBSettings>>();
            // We need to set the Value of IOptions to be the WalletDbSettings Class
            mock.Setup(ap => ap.Value).Returns(WalletDbSettings);

            var result = new BankAccountService(dbMock.Object, mock.Object).GetAsync("644008659e40371bf3268ff3");


            //result.Should().NotBeNull()
            //.And.BeAssignableTo<IProjectsContext>();
            //Write a test to assert the ProjectCollection
            result.Should().BeOfType<Task<BankAccount>>();
            result.Result.Should().NotBeNull();
        }
    }
}
