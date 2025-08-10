using Http2Client.Core.Enums;
using Http2Client.Core.Models;
using Http2Client.Utilities;

using System;
using System.Collections.Generic;

namespace Http2Client.Builders;

/// <summary>
/// Builder for <see cref="Http2Client" /> instances. Chain methods and call Build().
/// </summary>
public class HttpClientBuilder
{
    private readonly Http2ClientOptions _options = new();

    /// <summary>
    /// Uses custom TLS fingerprint instead of browser preset.
    /// </summary>
    public HttpClientBuilder WithCustomHttp2Client(CustomHttp2Client customHttp2Client)
    {
        ThrowException.Null(customHttp2Client, nameof(customHttp2Client));

        _options.CustomHttp2Client = customHttp2Client;
        return this;
    }

    /// <summary>
    /// Sets default header sent with every request.
    /// </summary>
    public HttpClientBuilder WithHeader(string name, string value)
    {
        ThrowException.NullOrEmpty(name, nameof(name));
        ThrowException.Null(value);

        _options.DefaultHeaders[name] = [value];
        return this;
    }

    /// <summary>
    /// Sets multiple default headers at once.
    /// </summary>
    public HttpClientBuilder WithHeaders(Dictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            WithHeader(header.Key, header.Value);
        }

        return this;
    }

    /// <summary>
    /// Uses proxy. Supports HTTP, HTTPS, SOCKS5.
    /// </summary>
    public HttpClientBuilder WithProxy(string? proxyUrl, bool isRotating = false)
    {
        _options.ProxyUrl = proxyUrl;
        _options.IsRotatingProxy = isRotating;
        return this;
    }

    /// <summary>
    /// Sets the Session ID for this client. Auto-generated if not set.
    /// </summary>
    public HttpClientBuilder WithSessionId(Guid sessionId)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("SessionId must be non-empty.", nameof(sessionId));
        }

        _options.SessionId = sessionId;
        return this;
    }

    /// <summary>
    /// Sets browser fingerprint to mimic.
    /// </summary>
    public HttpClientBuilder WithBrowserType(BrowserType browserType)
    {
        _options.BrowserType = browserType;
        return this;
    }

    /// <summary>
    /// Sets the order of HTTP headers. Some servers validate header order.
    /// </summary>
    public HttpClientBuilder WithHeaderOrder(params string[] headerOrder)
    {
        ThrowException.Null(headerOrder, nameof(headerOrder));

        _options.HeaderOrder = [.. headerOrder];
        return this;
    }

    /// <summary>
    /// Request timeout. Default is 60 seconds.
    /// </summary>
    public HttpClientBuilder WithTimeout(TimeSpan timeout)
    {
        _options.Timeout = timeout;
        return this;
    }

    /// <summary>
    /// Catches native library panics.
    /// </summary>
    public HttpClientBuilder WithCatchPanics(bool enable = true)
    {
        _options.CatchPanics = enable;
        return this;
    }

    /// <summary>
    /// Enables automatic redirect following.
    /// </summary>
    public HttpClientBuilder WithFollowRedirects(bool follow = true)
    {
        _options.FollowRedirects = follow;
        return this;
    }

    /// <summary>
    /// Forces HTTP/1.1 over HTTP/2.
    /// </summary>
    public HttpClientBuilder WithForceHttp1(bool force = true)
    {
        _options.ForceHttp1 = force;
        return this;
    }

    /// <summary>
    /// Skips SSL certificate validation.
    /// </summary>
    public HttpClientBuilder WithInsecureSkipVerify(bool skip = true)
    {
        _options.InsecureSkipVerify = skip;
        return this;
    }

    /// <summary>
    /// Disables IPv6 connections.
    /// </summary>
    public HttpClientBuilder WithDisableIPv6(bool disable = true)
    {
        _options.DisableIPv6 = disable;
        return this;
    }

    /// <summary>
    /// Disables IPv4 connections.
    /// </summary>
    public HttpClientBuilder WithDisableIPv4(bool disable = true)
    {
        _options.DisableIPv4 = disable;
        return this;
    }

    /// <summary>
    /// Enables debug logging.
    /// </summary>
    public HttpClientBuilder WithDebug(bool enable = true)
    {
        _options.WithDebug = enable;
        return this;
    }

    /// <summary>
    /// Enables automatic cookie handling.
    /// </summary>
    public HttpClientBuilder WithCookies(bool enabled = true)
    {
        _options.WithDefaultCookieJar = enabled;
        _options.WithoutCookieJar = !enabled;
        return this;
    }

    /// <summary>
    /// Disable all cookie handling.
    /// </summary>
    public HttpClientBuilder WithoutCookieJar(bool enabled = true)
    {
        _options.WithDefaultCookieJar = !enabled;
        _options.WithoutCookieJar = enabled;
        return this;
    }

    /// <summary>
    /// Randomizes TLS extension order.
    /// </summary>
    public HttpClientBuilder WithRandomTlsExtensions(bool enable = true)
    {
        _options.WithRandomTlsExtensionOrder = enable;
        return this;
    }

    /// <summary>
    /// Sets native library path. Auto-detected if not set.
    /// </summary>
    public HttpClientBuilder WithLibraryPath(string libraryPath)
    {
        ThrowException.FileNotExists(libraryPath, nameof(libraryPath));

        _options.LibraryPath = libraryPath;
        return this;
    }

    /// <summary>
    /// Sets User-Agent header.
    /// </summary>
    public HttpClientBuilder WithUserAgent(string userAgent)
    {
        ThrowException.NullOrEmpty(userAgent, nameof(userAgent));

        _options.UserAgent = userAgent;
        return this;
    }

    /// <summary>
    /// Builds final Http2Client instance.
    /// </summary>
    public Http2Client Build()
    {
        return new Http2Client(BuildOptions());
    }

    /// <summary>
    /// Builds options without creating client.
    /// </summary>
    public Http2ClientOptions BuildOptions()
    {
        _options.Validate();

        // Clone to prevent modifications
        return _options.Clone();
    }
}