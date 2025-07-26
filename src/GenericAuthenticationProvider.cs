using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Soenneker.Kiota.GenericAuthenticationProvider;

/// <inheritdoc cref="IAuthenticationProvider"/>
public sealed class GenericAuthenticationProvider : IAuthenticationProvider
{
    private readonly string _apiKey;
    private const string _defaultHeaderName = "Authorization";
    private const string _defaultHeaderFormat = "Bearer {0}";

    public GenericAuthenticationProvider(string apiKey)
    {
        _apiKey = apiKey;
    }

    public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        string headerName = _defaultHeaderName;
        string headerFormat = _defaultHeaderFormat;

        if (additionalAuthenticationContext is {Count: > 0})
        {
            if (additionalAuthenticationContext.TryGetValue("headerName", out object? nameObj) && nameObj is string nameStr)
                headerName = nameStr;

            if (additionalAuthenticationContext.TryGetValue("headerFormat", out object? formatObj) && formatObj is string formatStr)
                headerFormat = formatStr;
        }

        request.Headers[headerName] = [string.Format(headerFormat, _apiKey)];

        return Task.CompletedTask;
    }
}