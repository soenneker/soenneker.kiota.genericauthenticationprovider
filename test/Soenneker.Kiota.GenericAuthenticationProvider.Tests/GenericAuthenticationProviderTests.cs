using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Kiota.GenericAuthenticationProvider.Tests;

[Collection("Collection")]
public sealed class GenericAuthenticationProviderTests : FixturedUnitTest
{
    public GenericAuthenticationProviderTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
