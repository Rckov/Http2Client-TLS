using Http2Client.Core.Enums;
using Http2Client.Core.Models;
using Http2Client.Core.Request;
using Http2Client.Utilities;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Http2Client.Builders;

/// <summary>
/// Builder for <see cref="HttpRequest" />. Simplifies request configuration.
/// </summary>
public class HttpRequestBuilder
{
    private readonly HttpRequest _request = new();

    /// <summary>
    /// Pins certificates for host to detect MITM.
    /// </summary>
    public HttpRequestBuilder WithCertificatePinning(string host, List<string> pins)
    {
        ThrowException.NullOrEmpty(host, nameof(host));
        ThrowException.Null(pins, nameof(pins));

        if (pins.Count == 0)
        {
            throw new ArgumentException("Certificate pins list cannot be empty.", nameof(pins));
        }

        _request.CertificatePinningHosts[host] = pins;
        return this;
    }

    /// <summary>
    /// Uses custom TLS config instead of browser preset.
    /// </summary>
    public HttpRequestBuilder WithCustomHttp2Client(CustomHttp2Client tlsClient)
    {
        ThrowException.Null(tlsClient, nameof(tlsClient));

        _request.CustomHttp2Client = tlsClient;
        return this;
    }

    /// <summary>
    /// Sets transport layer options.
    /// </summary>
    public HttpRequestBuilder WithTransportOptions(TransportOptions options)
    {
        ThrowException.Null(options, nameof(options));

        _request.TransportOptions = options;
        return this;
    }

    /// <summary>
    /// Adds header to request. Replaces existing.
    /// </summary>
    public HttpRequestBuilder WithHeader(string key, string value)
    {
        _request.Headers[key] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple headers at once.
    /// </summary>
    public HttpRequestBuilder WithHeaders(Dictionary<string, string> headers)
    {
        ThrowException.Null(headers, nameof(headers));

        foreach (var kvp in headers)
        {
            _request.Headers[kvp.Key] = kvp.Value;
        }

        return this;
    }

    /// <summary>
    /// Adds default headers merged with request headers.
    /// </summary>
    public HttpRequestBuilder AddDefaultHeaders(Dictionary<string, List<string>> headers)
    {
        ThrowException.Null(headers, nameof(headers));

        foreach (var kvp in headers)
        {
            _request.DefaultHeaders[kvp.Key] = kvp.Value;
        }

        return this;
    }

    /// <summary>
    /// Adds headers for CONNECT proxy requests.
    /// </summary>
    public HttpRequestBuilder AddConnectHeaders(Dictionary<string, List<string>> headers)
    {
        ThrowException.Null(headers, nameof(headers));

        foreach (var kvp in headers)
        {
            _request.ConnectHeaders[kvp.Key] = kvp.Value;
        }

        return this;
    }

    /// <summary>
    /// Binds request to specific local IP.
    /// </summary>
    public HttpRequestBuilder WithLocalAddress(string ip)
    {
        ThrowException.NullOrEmpty(ip, nameof(ip));

        if (!IPAddress.TryParse(ip, out _))
        {
            throw new ArgumentException("Local address must be a valid IP address.", nameof(ip));
        }

        _request.LocalAddress = ip;
        return this;
    }

    /// <summary>
    /// Overrides TLS SNI hostname.
    /// </summary>
    public HttpRequestBuilder WithServerName(string sni)
    {
        ThrowException.NullOrEmpty(sni, nameof(sni));

        _request.ServerNameOverwrite = sni;
        return this;
    }

    /// <summary>
    /// Routes request through proxy. Mark as rotating if needed.
    /// </summary>
    public HttpRequestBuilder WithProxy(string? proxyUrl, bool isRotating = false)
    {
        ThrowException.IsUri(proxyUrl, nameof(proxyUrl));

        _request.ProxyUrl = proxyUrl;
        _request.IsRotatingProxy = isRotating;
        return this;
    }

    /// <summary>
    /// Sends a raw string as the request body. You'll need to set Content-Type yourself if required.
    /// </summary>
    public HttpRequestBuilder WithBody(string body)
    {
        _request.RequestBody = body;
        return this;
    }

    /// <summary>
    /// Sets JSON body and Content-Type header.
    /// </summary>
    public HttpRequestBuilder WithJsonBody<T>(T data)
    {
        ThrowException.Null(data, nameof(data));

        try
        {
            _request.RequestBody = JsonSerializer.Serialize(data);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Failed to serialize data to JSON: {ex.Message}", nameof(data), ex);
        }

        return WithHeader("Content-Type", "application/json");
    }

    /// <summary>
    /// Sets binary data as Base64-encoded body.
    /// </summary>
    public HttpRequestBuilder WithBinaryBody(byte[] data)
    {
        ThrowException.Null(data, nameof(data));

        const int maxSize = 50 * 1024 * 1024; // 50MB
        if (data.Length > maxSize)
        {
            throw new ArgumentException($"Binary data too large: {data.Length} bytes. Maximum allowed: {maxSize} bytes.", nameof(data));
        }

        _request.RequestBody = Convert.ToBase64String(data);
        _request.IsByteRequest = true;
        return this;
    }

    /// <summary>
    /// Overrides Host header. Doesn't affect TLS SNI.
    /// </summary>
    public HttpRequestBuilder WithHostOverride(string host)
    {
        ThrowException.NullOrEmpty(host, nameof(host));

        _request.RequestHostOverride = host;
        return this;
    }

    /// <summary>
    /// Sets session ID for connection reuse.
    /// </summary>
    public HttpRequestBuilder WithSessionId(Guid sessionId)
    {
        _request.SessionId = sessionId;
        return this;
    }

    /// <summary>
    /// Saves streamed response to file.
    /// </summary>
    public HttpRequestBuilder WithStreamOutput(string? path, int? blockSize = null, string? eofSymbol = null)
    {
        if (blockSize.HasValue && blockSize <= 0)
        {
            throw new ArgumentException("StreamOutputBlockSize must be positive if specified.", nameof(blockSize));
        }

        _request.StreamOutputPath = path;
        _request.StreamOutputBlockSize = blockSize;
        _request.StreamOutputEofSymbol = eofSymbol;
        return this;
    }

    /// <summary>
    /// Sets HTTP method using <see cref="HttpMethod" /> class.
    /// </summary>
    public HttpRequestBuilder WithMethod(HttpMethod method)
    {
        ThrowException.Null(method, nameof(method));

        _request.RequestMethod = method.Method;
        return this;
    }

    /// <summary>
    /// Sets target URL. Required field.
    /// </summary>
    public HttpRequestBuilder WithUrl(string url)
    {
        ThrowException.IsUri(url, nameof(url));

        _request.RequestUrl = url;
        return this;
    }

    /// <summary>
    /// Sets browser fingerprint for TLS.
    /// </summary>
    public HttpRequestBuilder WithBrowserType(BrowserType type)
    {
        _request.BrowserType = type;
        return this;
    }

    /// <summary>
    /// Sets header order. Some servers check this.
    /// </summary>
    public HttpRequestBuilder AddHeaderOrder(params string[] headerNames)
    {
        ThrowException.Null(headerNames, nameof(headerNames));

        _request.HeaderOrder.AddRange(headerNames);
        return this;
    }

    /// <summary>
    /// Adds cookies to request.
    /// </summary>
    public HttpRequestBuilder WithCookies(List<ClientCookie> cookies)
    {
        ThrowException.Null(cookies, nameof(cookies));

        _request.RequestCookies.AddRange(cookies);
        return this;
    }

    /// <summary>
    /// Adds single cookie to request.
    /// </summary>
    public HttpRequestBuilder AddCookie(ClientCookie cookie)
    {
        ThrowException.Null(cookie, nameof(cookie));

        _request.RequestCookies.Add(cookie);
        return this;
    }

    /// <summary>
    /// Sets request timeout.
    /// </summary>
    public HttpRequestBuilder WithTimeout(TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("Timeout must be greater than zero.", nameof(timeout));
        }

        if (timeout > TimeSpan.FromMinutes(30))
        {
            throw new ArgumentException("Timeout cannot exceed 30 minutes.", nameof(timeout));
        }

        _request.TimeoutMilliseconds = (int)timeout.TotalMilliseconds;
        _request.TimeoutSeconds = (int)timeout.TotalSeconds;
        return this;
    }

    /// <summary>
    /// Enables catching native library panics. Enabled by default for stability.
    /// </summary>
    public HttpRequestBuilder WithCatchPanics(bool enable = true)
    {
        _request.CatchPanics = enable;
        return this;
    }

    /// <summary>
    /// Controls automatic redirect following.
    /// </summary>
    public HttpRequestBuilder WithFollowRedirects(bool follow = true)
    {
        _request.FollowRedirects = follow;
        return this;
    }

    /// <summary>
    /// Forces HTTP/1.1 over HTTP/2.
    /// </summary>
    public HttpRequestBuilder WithForceHttp1(bool force = true)
    {
        _request.ForceHttp1 = force;
        return this;
    }

    /// <summary>
    /// Disables SSL certificate validation.
    /// </summary>
    public HttpRequestBuilder WithInsecureSkipVerify(bool skip = true)
    {
        _request.InsecureSkipVerify = skip;
        return this;
    }

    /// <summary>
    /// Expect binary response instead of text.
    /// </summary>
    public HttpRequestBuilder WithByteResponse(bool enabled = true)
    {
        _request.IsByteResponse = enabled;
        return this;
    }

    /// <summary>
    /// Disables IPv6 for this request.
    /// </summary>
    public HttpRequestBuilder WithDisableIPv6(bool disable = true)
    {
        _request.DisableIPv6 = disable;
        return this;
    }

    /// <summary>
    /// Disables IPv4 for this request.
    /// </summary>
    public HttpRequestBuilder WithDisableIPv4(bool disable = true)
    {
        _request.DisableIPv4 = disable;
        return this;
    }

    /// <summary>
    /// Enables debug logging for request.
    /// </summary>
    public HttpRequestBuilder WithDebug(bool enable = true)
    {
        _request.WithDebug = enable;
        return this;
    }

    /// <summary>
    /// Enables automatic cookie handling.
    /// </summary>
    public HttpRequestBuilder WithCookieJar(bool useDefault = true)
    {
        _request.WithDefaultCookieJar = useDefault;
        return this;
    }

    /// <summary>
    /// Disables all cookie handling.
    /// </summary>
    public HttpRequestBuilder WithoutCookieJar(bool disable = true)
    {
        _request.WithoutCookieJar = disable;
        return this;
    }

    /// <summary>
    /// Randomizes TLS extension order.
    /// </summary>
    public HttpRequestBuilder WithRandomTlsExtensions(bool enable = true)
    {
        _request.WithRandomTlsExtensionOrder = enable;
        return this;
    }

    /// <summary>
    /// Allows direct HttpRequest modifications.
    /// </summary>
    public HttpRequestBuilder With(Action<HttpRequest> config)
    {
        config?.Invoke(_request);
        return this;
    }

    /// <summary>
    /// Builds final request object.
    /// </summary>
    public HttpRequest Build()
    {
        Validate();
        return _request;
    }

    /// <summary>
    /// Validates request for conflicting settings and required fields.
    /// </summary>
    private void Validate()
    {
        // Required fields - only check if not set through builder methods
        if (string.IsNullOrWhiteSpace(_request.RequestUrl))
        {
            throw new InvalidOperationException("RequestUrl is required. Use WithUrl() to set it.");
        }

        // Conflicting cookie settings
        if (_request.WithDefaultCookieJar == true && _request.WithoutCookieJar == true)
        {
            throw new InvalidOperationException("Cannot enable both WithDefaultCookieJar and WithoutCookieJar.");
        }

        // Conflicting IP settings
        if (_request.DisableIPv4 == true && _request.DisableIPv6 == true)
        {
            throw new InvalidOperationException("Cannot disable both IPv4 and IPv6.");
        }
    }
}