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
    public Guid SessionId => _options.SessionID;

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
        _options.Validate(); // Make sure settings make sense
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
    public HttpResponse Send(HttpRequest request)
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
    public CookiesResponse GetCookies(string url)
    {
        ThrowException.NullOrEmpty(url, nameof(url));

        var payload = new GetCookiesRequest()
        {
            Url = url,
            SessionId = _options.SessionID,
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
    public CookiesResponse AddCookies(string url, IEnumerable<ClientCookie> cookies)
    {
        ThrowException.NullOrEmpty(url, nameof(url));
        ThrowException.Null(cookies, nameof(cookies));

        var payload = new AddCookiesRequest()
        {
            Url = url,
            SessionId = _options.SessionID,
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
            var payload = new { sessionId = _options.SessionID };
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
    /// Merge client options with request settings. Request wins if both set.
    /// </summary>
    /// <param name="request">Original request</param>
    /// <returns>Request with merged settings</returns>
    private HttpRequest PrepareRequest(HttpRequest request)
    {
        // Start with a copy of the request
        var req = new HttpRequest
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
            StreamOutputEOFSymbol = request.StreamOutputEOFSymbol,
            StreamOutputPath = request.StreamOutputPath,
            RequestCookies = request.RequestCookies,
            TransportOptions = request.TransportOptions
        };

        // Fill in missing values from client defaults (null coalescing is your friend)
        req.SessionId ??= _options.SessionID;
        req.BrowserType ??= _options.BrowserType;
        req.CustomHttp2Client ??= _options.CustomHttp2Client;
        req.ProxyUrl ??= _options.ProxyUrl;
        req.IsRotatingProxy ??= _options.IsRotatingProxy;
        req.InsecureSkipVerify ??= _options.InsecureSkipVerify;
        req.DisableIPv4 ??= _options.DisableIPv4;
        req.DisableIPv6 ??= _options.DisableIPv6;
        req.WithDebug ??= _options.WithDebug;
        req.WithDefaultCookieJar ??= _options.WithDefaultCookieJar;
        req.WithoutCookieJar ??= _options.WithoutCookieJar;
        req.FollowRedirects ??= _options.FollowRedirects;

        // OR logic - either can force HTTP/1
        req.ForceHttp1 = request.ForceHttp1 || _options.ForceHttp1;
        req.CatchPanics = _options.CatchPanics;
        req.WithRandomTLSExtensionOrder = request.WithRandomTLSExtensionOrder || _options.WithRandomTlsExtensionOrder;

        // Use client timeout if request doesn't specify one
        if (!req.TimeoutMilliseconds.HasValue)
        {
            req.TimeoutMilliseconds = (int)_options.Timeout.TotalMilliseconds;
        }

        // Header merging is tricky - we want defaults but request headers should override
        foreach (KeyValuePair<string, List<string>> pair in _options.DefaultHeaders)
        {
            if (!req.DefaultHeaders.ContainsKey(pair.Key))
            {
                req.DefaultHeaders[pair.Key] = [.. pair.Value];
            }

            if (!req.Headers.ContainsKey(pair.Key) && pair.Value.Count > 0)
            {
                req.Headers[pair.Key] = pair.Value[0];
            }
        }

        // Custom TLS client and browser type are mutually exclusive
        if (req.CustomHttp2Client != null)
        {
            req.BrowserType = null;
        }

        return req;
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