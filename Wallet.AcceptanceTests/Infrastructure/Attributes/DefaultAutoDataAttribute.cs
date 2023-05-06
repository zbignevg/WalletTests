using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Wallet.AcceptanceTests.Infrastructure.Attributes
{
    public class DefaultCustomization : CompositeCustomization
    {
        public DefaultCustomization() : base(
            new AutoMoqCustomization(),
            new WalletApiCustomization())
        {
        }
    }

    public class DefaultCustomizationsAttribute : AutoDataAttribute
    {
        public DefaultCustomizationsAttribute()
            : base(() => new Fixture()
                .Customize(new DefaultCustomization()))
        {
        }

    }

    public class DefaultAutoDataAttribute : InlineAutoDataAttribute
    {
        public DefaultAutoDataAttribute()
            : this(new object[0])
        {

        }

        public DefaultAutoDataAttribute(params object[] values) : base(new DefaultCustomizationsAttribute(), values)
        {

        }
    }
}
