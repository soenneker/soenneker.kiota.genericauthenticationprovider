using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Soenneker.Kiota.GenericAuthenticationProvider;

/// <inheritdoc cref="IAuthenticationProvider"/>
public sealed class GenericAuthenticationProvider : IAuthenticationProvider
{
    private readonly string _headerName;
    private readonly IEnumerable<string> _headerValue;

    public GenericAuthenticationProvider(string headerName = "Authorization", string headerValue = "Bearer ")
    {
        _headerName = headerName;
        _headerValue = [headerValue];
    }

    public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        request.Headers[_headerName] = _headerValue;
        return Task.CompletedTask;
    }
}