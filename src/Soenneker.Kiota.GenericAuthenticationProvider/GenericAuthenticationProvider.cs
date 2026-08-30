using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Soenneker.Kiota.GenericAuthenticationProvider;

public sealed class GenericAuthenticationProvider : IAuthenticationProvider
{
    private readonly string _headerName;
    private readonly IReadOnlyDictionary<string, string> _additionalHeaders;
    private readonly IEnumerable<string> _headerValue;
    private readonly AllowedHostsValidator? _allowedHostsValidator;
    private readonly bool _allowInsecureHttp;
    private readonly object _hostLock = new();
    private string? _pinnedHost;

    public GenericAuthenticationProvider(string headerName = "Authorization", string headerValue = "Bearer ",
        Dictionary<string, string>? additionalHeaders = null, IEnumerable<string>? allowedHosts = null, bool allowInsecureHttp = false)
    {
        _headerName = headerName;
        _additionalHeaders = additionalHeaders is null ? new Dictionary<string, string>() : new Dictionary<string, string>(additionalHeaders);
        _headerValue = [headerValue];
        string[] hosts = allowedHosts?.Where(static host => !string.IsNullOrWhiteSpace(host)).ToArray() ?? [];
        _allowedHostsValidator = hosts.Length == 0 ? null : new AllowedHostsValidator(hosts);
        _allowInsecureHttp = allowInsecureHttp;
    }

    /// <summary>
    /// Authenticates request Async for the generic authentication provider.
    /// </summary>
    /// <param name="request">request that defines the request to send.</param>
    /// <param name="additionalAuthenticationContext">additional Authentication Context to process.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the authenticate request async operation is complete.</returns>
    public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        request.Headers.Remove(_headerName);

        foreach (string header in _additionalHeaders.Keys)
            request.Headers.Remove(header);

        Uri uri = request.URI;
        bool transportAllowed = uri.Scheme == Uri.UriSchemeHttps || _allowInsecureHttp && uri.Scheme == Uri.UriSchemeHttp;
        if (!transportAllowed || !IsHostAllowed(uri))
            return Task.CompletedTask;

        request.Headers[_headerName] = _headerValue;

        if (_additionalHeaders.Count > 0)
        {
            foreach (KeyValuePair<string, string> kvp in _additionalHeaders)
            {
                request.Headers[kvp.Key] = [kvp.Value];
            }
        }

        return Task.CompletedTask;
    }

    private bool IsHostAllowed(Uri uri)
    {
        if (_allowedHostsValidator is not null)
            return _allowedHostsValidator.IsUrlHostValid(uri);

        lock (_hostLock)
        {
            _pinnedHost ??= uri.Host;
            return string.Equals(_pinnedHost, uri.Host, StringComparison.OrdinalIgnoreCase);
        }
    }
}
