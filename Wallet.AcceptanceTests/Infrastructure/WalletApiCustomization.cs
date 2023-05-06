using System.Net.Mail;
using AutoFixture;
using MongoDB.Bson;
using Wallet.AcceptanceTests.Models;

namespace Wallet.AcceptanceTests.Infrastructure
{
    public class WalletApiCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var seedFixture = new Fixture();

            fixture.Register(() => seedFixture
                .Build<User>()
                .With(x => x.Id, ObjectId.GenerateNewId().ToString())
                .With(x => x.Email, fixture.Create<MailAddress>().Address)
                .Create());

            fixture.Register(() => seedFixture
                .Build<BankAccount>()
                .With(x => x.Id, ObjectId.GenerateNewId().ToString())
                .Create());
        }
    }
}
