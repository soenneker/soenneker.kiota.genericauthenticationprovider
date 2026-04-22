using Soenneker.Tests.HostedUnit;

namespace Soenneker.Kiota.GenericAuthenticationProvider.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class GenericAuthenticationProviderTests : HostedUnitTest
{
    public GenericAuthenticationProviderTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
