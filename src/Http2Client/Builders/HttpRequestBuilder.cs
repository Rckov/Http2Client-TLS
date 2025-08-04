using Http2Client.Core.Enums;
using Http2Client.Core.Models;
using Http2Client.Core.Request;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace Http2Client.Builders;

/// <summary>
/// Builder for <see cref="HttpRequest"/>. Simplifies configuration and building.
/// </summary>
public class HttpRequestBuilder
{
    private readonly HttpRequest _request = new();

    /// <summary>
    /// Sets the target URL. This is the only required field.
    /// </summary>
    public HttpRequestBuilder WithUrl(string url)
    {
        _request.RequestUrl = url ?? throw new ArgumentNullException(nameof(url));
        return this;
    }

    /// <summary>
    /// Sets the HTTP method like GET, POST, PUT, etc. Gets uppercased automatically.
    /// </summary>
    public HttpRequestBuilder WithMethod(string method)
    {
        _request.RequestMethod = method?.ToUpperInvariant() ?? "GET";
        return this;
    }

    /// <summary>
    /// Sets the HTTP method using the built-in <see cref="HttpMethod"/> class.
    /// </summary>
    public HttpRequestBuilder WithMethod(HttpMethod method)
    {
        return WithMethod(method.Method);
    }

    /// <summary>
    /// Sends a raw string as the request body.
    /// You’ll need to set Content-Type yourself if required.
    /// </summary>
    public HttpRequestBuilder WithBody(string body)
    {
        _request.RequestBody = body;
        return this;
    }

    /// <summary>
    /// Serializes the provided object as JSON and sets it as the body.
    /// Automatically sets <c>Content-Type: application/json</c>.
    /// </summary>
    public HttpRequestBuilder WithJsonBody<T>(T data)
    {
        _request.RequestBody = JsonSerializer.Serialize(data);
        return WithHeader("Content-Type", "application/json");
    }

    /// <summary>
    /// Sends binary data as Base64-encoded body. Useful for uploads and binary protocols.
    /// </summary>
    public HttpRequestBuilder WithBinaryBody(byte[] data)
    {
        _request.RequestBody = Convert.ToBase64String(data);
        _request.IsByteRequest = true;
        return this;
    }

    /// <summary>
    /// Indicates that the response is expected to be binary (not text).
    /// </summary>
    public HttpRequestBuilder WithByteResponse(bool enabled = true)
    {
        _request.IsByteResponse = enabled;
        return this;
    }

    /// <summary>
    /// Adds a single custom header to the request. Replaces any existing value with the same name.
    /// </summary>
    public HttpRequestBuilder WithHeader(string key, string value)
    {
        _request.Headers[key] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple headers at once. Useful when copying from another request or config.
    /// </summary>
    public HttpRequestBuilder WithHeaders(Dictionary<string, string> headers)
    {
        foreach (var kvp in headers)
        {
            _request.Headers[kvp.Key] = kvp.Value;
        }

        return this;
    }

    /// <summary>
    /// Adds a list of cookies to the request.
    /// </summary>
    public HttpRequestBuilder WithCookies(List<ClientCookie> cookies)
    {
        _request.RequestCookies.AddRange(cookies);
        return this;
    }

    /// <summary>
    /// Adds a single cookie to the request.
    /// </summary>
    public HttpRequestBuilder AddCookie(ClientCookie cookie)
    {
        _request.RequestCookies.Add(cookie);
        return this;
    }

    /// <summary>
    /// Selects which browser fingerprint to use for TLS.
    /// Useful when dealing with sites that do TLS fingerprinting.
    /// </summary>
    public HttpRequestBuilder WithBrowserType(BrowserType type)
    {
        _request.BrowserType = type;
        return this;
    }

    /// <summary>
    /// Use a fully custom TLS client config instead of a built-in browser profile.
    /// For advanced use cases like spoofing specific ALPN or JA3 values.
    /// </summary>
    public HttpRequestBuilder WithCustomHttp2Client(CustomHttp2Client tlsClient)
    {
        _request.CustomHttp2Client = tlsClient;
        return this;
    }

    /// <summary>
    /// Randomizes the order of TLS extensions. Makes requests look more human.
    /// </summary>
    public HttpRequestBuilder WithRandomTlsExtensions(bool enable = true)
    {
        _request.WithRandomTLSExtensionOrder = enable;
        return this;
    }

    /// <summary>
    /// Routes the request through a proxy. Optionally mark it as rotating (like BrightData, Oxylabs, etc).
    /// </summary>
    public HttpRequestBuilder WithProxy(string proxyUrl, bool? isRotating = null)
    {
        _request.ProxyUrl = proxyUrl;
        _request.IsRotatingProxy = isRotating;
        return this;
    }

    /// <summary>
    /// Binds the outgoing request to a specific local IP address.
    /// Useful for multi-homed servers or testing.
    /// </summary>
    public HttpRequestBuilder WithLocalAddress(string ip)
    {
        _request.LocalAddress = ip;
        return this;
    }

    /// <summary>
    /// Overrides the Host header for this request. Doesn't affect actual TLS SNI.
    /// </summary>
    public HttpRequestBuilder WithHostOverride(string host)
    {
        _request.RequestHostOverride = host;
        return this;
    }

    /// <summary>
    /// Overrides the TLS SNI hostname. Usually not needed unless you're doing domain fronting.
    /// </summary>
    public HttpRequestBuilder WithServerName(string sni)
    {
        _request.ServerNameOverwrite = sni;
        return this;
    }

    /// <summary>
    /// Sets the request timeout. Throws if timeout is zero or negative.
    /// </summary>
    public HttpRequestBuilder WithTimeout(TimeSpan timeout)
    {
        _request.TimeoutMilliseconds = (int)timeout.TotalMilliseconds;
        return this;
    }

    /// <summary>
    /// Controls whether redirects (3xx) should be followed automatically.
    /// </summary>
    public HttpRequestBuilder WithFollowRedirects(bool follow = true)
    {
        _request.FollowRedirects = follow;
        return this;
    }

    /// <summary>
    /// Forces the request to use HTTP/1.1 even if HTTP/2 is supported.
    /// </summary>
    public HttpRequestBuilder WithForceHttp1(bool force = true)
    {
        _request.ForceHttp1 = force;
        return this;
    }

    /// <summary>
    /// Disables SSL certificate validation. Use only if absolutely necessary.
    /// </summary>
    public HttpRequestBuilder WithInsecureSkipVerify(bool skip = true)
    {
        _request.InsecureSkipVerify = skip;
        return this;
    }

    /// <summary>
    /// Prevents the request from using IPv4 addresses.
    /// </summary>
    public HttpRequestBuilder WithDisableIPv4(bool disable = true)
    {
        _request.DisableIPv4 = disable;
        return this;
    }

    /// <summary>
    /// Prevents the request from using IPv6 addresses.
    /// </summary>
    public HttpRequestBuilder WithDisableIPv6(bool disable = true)
    {
        _request.DisableIPv6 = disable;
        return this;
    }

    /// <summary>
    /// Enables debug logging for the request. Helpful when diagnosing issues.
    /// </summary>
    public HttpRequestBuilder WithDebug(bool enable = true)
    {
        _request.WithDebug = enable;
        return this;
    }

    /// <summary>
    /// Enables automatic cookie jar handling for this request.
    /// Cookies will persist across requests with the same session ID.
    /// </summary>
    public HttpRequestBuilder WithCookieJar(bool useDefault = true)
    {
        _request.WithDefaultCookieJar = useDefault;
        return this;
    }

    /// <summary>
    /// Completely disables cookie handling (both read and write).
    /// </summary>
    public HttpRequestBuilder WithoutCookieJar(bool disable = true)
    {
        _request.WithoutCookieJar = disable;
        return this;
    }

    /// <summary>
    /// Saves streamed response data to a file, chunked by blockSize.
    /// Optional: specify EOF symbol for terminating the stream.
    /// </summary>
    public HttpRequestBuilder WithStreamOutput(string? path, int? blockSize = null, string? eofSymbol = null)
    {
        _request.StreamOutputPath = path;
        _request.StreamOutputBlockSize = blockSize;
        _request.StreamOutputEOFSymbol = eofSymbol;
        return this;
    }

    /// <summary>
    /// Pins expected certificates for a given host. Used to detect MITM attacks.
    /// </summary>
    public HttpRequestBuilder WithCertificatePinning(string host, List<string> pins)
    {
        _request.CertificatePinningHosts[host] = pins;
        return this;
    }

    /// <summary>
    /// Sets low-level transport configuration (timeouts, retries, etc).
    /// </summary>
    public HttpRequestBuilder WithTransportOptions(TransportOptions options)
    {
        _request.TransportOptions = options;
        return this;
    }

    /// <summary>
    /// Sets the exact order headers should be sent in.
    /// Some servers check for this (e.g., anti-bot protections).
    /// </summary>
    public HttpRequestBuilder AddHeaderOrder(params string[] headerNames)
    {
        _request.HeaderOrder.AddRange(headerNames);
        return this;
    }

    /// <summary>
    /// Adds default headers. These will be merged with request-specific headers.
    /// </summary>
    public HttpRequestBuilder AddDefaultHeaders(Dictionary<string, List<string>> headers)
    {
        foreach (var kvp in headers)
        {
            _request.DefaultHeaders[kvp.Key] = kvp.Value;
        }

        return this;
    }

    /// <summary>
    /// Adds headers used specifically for CONNECT proxy requests.
    /// </summary>
    public HttpRequestBuilder AddConnectHeaders(Dictionary<string, List<string>> headers)
    {
        foreach (var kvp in headers)
        {
            _request.ConnectHeaders[kvp.Key] = kvp.Value;
        }

        return this;
    }

    /// <summary>
    /// Sets the session ID for connection/cookie reuse.
    /// Requests with the same session ID can share cookies and connections.
    /// </summary>
    public HttpRequestBuilder WithSessionId(Guid sessionId)
    {
        _request.SessionId = sessionId;
        return this;
    }

    /// <summary>
    /// Allows custom modifications directly to the <see cref="HttpRequest"/> object.
    /// Use sparingly for advanced use cases.
    /// </summary>
    public HttpRequestBuilder With(Action<HttpRequest> config)
    {
        config?.Invoke(_request);
        return this;
    }

    /// <summary>
    /// Builds and returns the final request object.
    /// Throws if required fields like URL are missing.
    /// </summary>
    public HttpRequest Build()
    {
        Validate();
        return _request;
    }

    /// <summary>
    /// Checks the current request for required fields and invalid values.
    /// </summary>
    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(_request.RequestUrl))
        {
            throw new InvalidOperationException("RequestUrl is required. Use WithUrl() to set it.");
        }

        if (_request.TimeoutMilliseconds.HasValue && _request.TimeoutMilliseconds <= 0)
        {
            throw new InvalidOperationException("TimeoutMilliseconds must be positive if specified.");
        }
    }
}