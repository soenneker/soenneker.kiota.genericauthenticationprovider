[![](https://img.shields.io/nuget/v/soenneker.kiota.genericauthenticationprovider.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.kiota.genericauthenticationprovider/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.kiota.genericauthenticationprovider/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.kiota.genericauthenticationprovider/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.kiota.genericauthenticationprovider/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.kiota.genericauthenticationprovider/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.kiota.genericauthenticationprovider.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.kiota.genericauthenticationprovider/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.kiota.genericauthenticationprovider/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.kiota.genericauthenticationprovider/actions/workflows/codeql.yml)

# Soenneker.Kiota.GenericAuthenticationProvider

A Kiota authentication provider for custom credential headers, optional companion headers, and host restrictions.

## Install

```bash
dotnet add package Soenneker.Kiota.GenericAuthenticationProvider
```

## Usage

```csharp
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Kiota.GenericAuthenticationProvider;

var authentication = new GenericAuthenticationProvider(
    headerName: "X-Api-Key",
    headerValue: apiKey,
    additionalHeaders: new Dictionary<string, string>
    {
        ["Api-Version"] = "2026-01-01"
    },
    allowedHosts: new[] { "api.example.com" });

var adapter = new HttpClientRequestAdapter(
    authentication,
    httpClient: httpClient);
```

Host names do not include a scheme or path. On an allowed HTTPS request, the provider replaces the configured credential header and companion headers with the configured values. On an off-host request, it removes those headers so a reused `RequestInformation` or Kiota raw-URL call cannot carry credentials elsewhere.

Always supply `allowedHosts` when the API host is known. For compatibility, omitting it pins the provider to the first HTTPS host it authenticates; later hosts receive no credential headers. An explicit allowlist is safer because it also protects the first request.

Plain HTTP is rejected by default. Set `allowInsecureHttp: true` only for a controlled local endpoint. The provider snapshots `additionalHeaders` during construction, so later dictionary changes do not alter live authentication behavior.
