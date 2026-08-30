using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
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

    [Test]
    public async Task Explicit_allowlist_prevents_header_leaks()
    {
        var provider = new GenericAuthenticationProvider("X-Api-Key", "secret",
            new Dictionary<string, string> { ["Api-Version"] = "1" },
            new[] { "api.example.com" });
        var request = new RequestInformation { URI = new System.Uri("https://api.example.com/v1") };

        await provider.AuthenticateRequestAsync(request);
        await Assert.That(request.Headers.ContainsKey("X-Api-Key")).IsTrue();
        await Assert.That(request.Headers.ContainsKey("Api-Version")).IsTrue();

        request.URI = new System.Uri("https://attacker.example/v1");
        await provider.AuthenticateRequestAsync(request);
        await Assert.That(request.Headers.ContainsKey("X-Api-Key")).IsFalse();
        await Assert.That(request.Headers.ContainsKey("Api-Version")).IsFalse();
    }

    [Test]
    public async Task Compatibility_mode_pins_the_first_https_host()
    {
        var provider = new GenericAuthenticationProvider(headerValue: "secret");
        var request = new RequestInformation { URI = new System.Uri("https://first.example/v1") };

        await provider.AuthenticateRequestAsync(request);
        request.URI = new System.Uri("https://second.example/v1");
        await provider.AuthenticateRequestAsync(request);

        await Assert.That(request.Headers.ContainsKey("Authorization")).IsFalse();
    }
}
