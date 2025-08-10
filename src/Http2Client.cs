using Http2Client.Core.Models;
using Http2Client.Core.Request;
using Http2Client.Core.Response;
using Http2Client.Native;
using Http2Client.Utilities;

using System;
using System.Collections.Generic;

namespace Http2Client;

/// <summary>
/// HTTP/2 client with TLS fingerprinting. Pretends to be different browsers.
/// </summary>
public sealed class Http2Client : IDisposable
{
    private readonly Http2ClientOptions _options;
    private readonly NativeWrapper _wrapper;
    private bool _disposed;

    /// <summary>
    /// Client configuration. Read-only after creation.
    /// </summary>
    public Http2ClientOptions Options => _options;

    /// <summary>
    /// Session ID for this client instance.
    /// </summary>
    public Guid SessionId => _options.SessionId;

    /// <summary>
    /// True if this client has been disposed.
    /// </summary>
    public bool IsDisposed => _disposed;

    /// <summary>
    /// Create client with custom options.
    /// </summary>
    /// <param name="options">Client configuration</param>
    public Http2Client(Http2ClientOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _options.Validate();
        _wrapper = NativeWrapper.Load(_options.LibraryPath);
    }

    /// <summary>
    /// Create client with default options.
    /// </summary>
    public Http2Client() : this(new Http2ClientOptions())
    {
    }

    /// <summary>
    /// Send HTTP request. Main method for making requests.
    /// </summary>
    /// <param name="request">Request to send</param>
    /// <returns>HTTP response</returns>
    public HttpResponse? Send(HttpRequest request)
    {
        ThrowException.Null(request, nameof(request));
        ThrowException.NullOrEmpty(request.RequestUrl, nameof(request.RequestUrl));

        HttpResponse? response = null;

        var prepared = PrepareRequest(request);

        try
        {
            var responseJson = _wrapper.Request(Serializer.SerializeToBytes(prepared));
            response = Serializer.Deserialize<HttpResponse>(responseJson);
            return response;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Request failed: {ex.Message}", ex);
        }
        finally
        {
            // Clean up native memory for this response
            if (response != null && !string.IsNullOrEmpty(response.Id))
            {
                _wrapper.FreeMemory(response.Id);
            }
        }
    }

    /// <summary>
    /// Get cookies from session for specific URL.
    /// </summary>
    /// <param name="url">URL to get cookies for</param>
    /// <returns>Response with cookies</returns>
    public CookiesResponse? GetCookies(string url)
    {
        ThrowException.NullOrEmpty(url, nameof(url));

        var payload = new GetCookiesRequest
        {
            Url = url,
            SessionId = _options.SessionId,
        };

        var responseJson = _wrapper.GetCookiesFromSession(Serializer.SerializeToBytes(payload));
        return Serializer.Deserialize<CookiesResponse>(responseJson);
    }

    /// <summary>
    /// Add cookies to session for specific URL.
    /// </summary>
    /// <param name="url">URL to add cookies for</param>
    /// <param name="cookies">Cookies to add</param>
    /// <returns>Response with updated cookies</returns>
    public CookiesResponse? AddCookies(string url, IEnumerable<ClientCookie> cookies)
    {
        ThrowException.NullOrEmpty(url, nameof(url));
        ThrowException.Null(cookies, nameof(cookies));

        var payload = new AddCookiesRequest
        {
            Url = url,
            SessionId = _options.SessionId,
            Cookies = [.. cookies]
        };

        var responseJson = _wrapper.AddCookiesToSession(Serializer.SerializeToBytes(payload));
        return Serializer.Deserialize<CookiesResponse>(responseJson);
    }

    /// <summary>
    /// Destroy session and clean up resources.
    /// </summary>
    /// <returns>True if successful</returns>
    public bool DestroySession()
    {
        try
        {
            var payload = new { sessionId = _options.SessionId };
            var responseJson = _wrapper.DestroySession(Serializer.SerializeToBytes(payload));
            return !string.IsNullOrEmpty(responseJson);
        }
        catch
        {
            // Swallow exceptions - cleanup is best effort
            return false;
        }
    }

    /// <summary>
    /// Destroy ALL sessions. Use with caution - breaks other clients.
    /// </summary>
    /// <returns>True if successful</returns>
    public bool DestroyAllSessions()
    {
        try
        {
            var responseJson = _wrapper.DestroyAllSessions();
            return !string.IsNullOrEmpty(responseJson);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Takes the user's request and mixes it with our client settings.
    /// If both have the same setting, the request wins.
    /// </summary>
    private HttpRequest PrepareRequest(HttpRequest request)
    {
        var preparedRequest = CopyRequest(request);

        SetHeaders(preparedRequest);
        SetDefaults(preparedRequest);

        return preparedRequest;
    }

    /// <summary>
    /// Makes a fresh copy of the request so we don't mess with the original.
    /// </summary>
    private HttpRequest CopyRequest(HttpRequest request)
    {
        return new HttpRequest
        {
            Id = request.Id,
            RequestUrl = request.RequestUrl,
            RequestMethod = request.RequestMethod,
            RequestBody = request.RequestBody,
            IsByteRequest = request.IsByteRequest,
            IsByteResponse = request.IsByteResponse,
            Headers = new Dictionary<string, string>(request.Headers),
            ConnectHeaders = new Dictionary<string, List<string>>(request.ConnectHeaders),
            DefaultHeaders = new Dictionary<string, List<string>>(_options.DefaultHeaders),
            HeaderOrder = [.. request.HeaderOrder],
            CertificatePinningHosts = new Dictionary<string, List<string>>(request.CertificatePinningHosts),
            StreamOutputBlockSize = request.StreamOutputBlockSize,
            StreamOutputEofSymbol = request.StreamOutputEofSymbol,
            StreamOutputPath = request.StreamOutputPath,
            RequestCookies = request.RequestCookies,
            TransportOptions = request.TransportOptions
        };
    }

    /// <summary>
    /// Fills in any missing settings from our client defaults.
    /// Only touches stuff that wasn't already set in the request.
    /// </summary>
    private void SetDefaults(HttpRequest request)
    {
        // Apply nullable defaults using null coalescing
        request.SessionId ??= _options.SessionId;
        request.CustomHttp2Client ??= _options.CustomHttp2Client;
        request.ProxyUrl ??= _options.ProxyUrl;

        // Apply boolean defaults from client options
        request.IsRotatingProxy = _options.IsRotatingProxy;
        request.InsecureSkipVerify = _options.InsecureSkipVerify;
        request.DisableIPv4 = _options.DisableIPv4;
        request.DisableIPv6 = _options.DisableIPv6;
        request.WithDebug = _options.WithDebug;
        request.WithDefaultCookieJar = _options.WithDefaultCookieJar;
        request.WithoutCookieJar = _options.WithoutCookieJar;
        request.FollowRedirects = _options.FollowRedirects;
        request.CatchPanics = _options.CatchPanics;
        request.TimeoutMilliseconds = (int)_options.Timeout.TotalMilliseconds;

        // Apply browser type
        request.BrowserType = _options.BrowserType;

        // Apply OR logic for certain flags - either request or client can enable
        request.ForceHttp1 = request.ForceHttp1 || _options.ForceHttp1;
        request.WithRandomTlsExtensionOrder = request.WithRandomTlsExtensionOrder || _options.WithRandomTlsExtensionOrder;

        // Custom TLS client overrides browser type
        if (request.CustomHttp2Client != null)
        {
            request.BrowserType = default;
        }
    }

    /// <summary>
    /// Mixes our default headers with the request's headers.
    /// Request headers win if there's a conflict.
    /// </summary>
    private void SetHeaders(HttpRequest request)
    {
        foreach (var defaultHeader in _options.DefaultHeaders)
        {
            // Add to DefaultHeaders if not already present
            if (!request.DefaultHeaders.ContainsKey(defaultHeader.Key))
            {
                request.DefaultHeaders[defaultHeader.Key] = [.. defaultHeader.Value];
            }

            // Add to Headers if not already present and has values
            if (!request.Headers.ContainsKey(defaultHeader.Key) && defaultHeader.Value.Count > 0)
            {
                request.Headers[defaultHeader.Key] = defaultHeader.Value[0];
            }
        }
    }

    /// <summary>
    /// Clean up resources. Use 'using' statements or call this manually.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return; // Already cleaned up
        }

        if (disposing)
        {
            try
            {
                // Try to clean up the session nicely
                DestroySession();
            }
            catch
            {
                // If session cleanup fails, we still want to dispose the wrapper
                // Better to leak a session than crash during disposal
            }

            _wrapper?.Dispose();
        }

        _disposed = true;
    }

    ~Http2Client() => Dispose(false);
}