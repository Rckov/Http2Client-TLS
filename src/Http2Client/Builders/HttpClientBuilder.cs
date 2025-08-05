using Http2Client.Core.Enums;
using Http2Client.Core.Models;

using System;
using System.Collections.Generic;

namespace Http2Client.Builders;

/// <summary>
/// Builder for Http2Client instances. Chain methods and call Build().
/// </summary>
public class HttpClientBuilder
{
    private readonly Http2ClientOptions _options = new();

    /// <summary>
    /// Sets native library path. Auto-detected if not set.
    /// </summary>
    public HttpClientBuilder WithLibraryPath(string libraryPath)
    {
        _options.LibraryPath = libraryPath;
        return this;
    }

    /// <summary>
    /// Request timeout. Default is 60 seconds.
    /// </summary>
    public HttpClientBuilder WithTimeout(TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("Timeout must be positive", nameof(timeout));
        }

        _options.Timeout = timeout;
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
    /// Sets browser fingerprint to mimic.
    /// </summary>
    public HttpClientBuilder WithBrowserType(BrowserType browserType)
    {
        _options.BrowserType = browserType;
        return this;
    }

    /// <summary>
    /// Uses custom TLS fingerprint instead of browser preset.
    /// </summary>
    public HttpClientBuilder WithCustomHttp2Client(CustomHttp2Client customHttp2Client)
    {
        _options.CustomHttp2Client = customHttp2Client;
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
    /// Randomizes TLS extension order.
    /// </summary>
    public HttpClientBuilder WithRandomTlsExtensions(bool enable = true)
    {
        _options.WithRandomTlsExtensionOrder = enable;
        return this;
    }

    /// <summary>
    /// Uses proxy. Supports HTTP, HTTPS, SOCKS5.
    /// </summary>
    public HttpClientBuilder WithProxy(string proxyUrl, bool isRotating = false)
    {
        _options.ProxyUrl = proxyUrl;

        // Helps with connection pooling
        _options.IsRotatingProxy = isRotating;
        return this;
    }

    /// <summary>
    /// Disables IPv4 connections.
    /// </summary>
    public HttpClientBuilder DisableIPv4(bool disable = true)
    {
        _options.DisableIPv4 = disable;
        return this;
    }

    /// <summary>
    /// Disables IPv6 connections.
    /// </summary>
    public HttpClientBuilder DisableIPv6(bool disable = true)
    {
        _options.DisableIPv6 = disable;
        return this;
    }

    /// <summary>
    /// Enables automatic redirect following.
    /// </summary>
    public HttpClientBuilder FollowRedirects(bool follow = true)
    {
        _options.FollowRedirects = follow;
        return this;
    }

    /// <summary>
    /// Forces HTTP/1.1 over HTTP/2.
    /// </summary>
    public HttpClientBuilder ForceHttp1(bool force = true)
    {
        _options.ForceHttp1 = force;
        return this;
    }

    /// <summary>
    /// Enables automatic cookie handling.
    /// </summary>
    public HttpClientBuilder WithCookies(bool enabled = true)
    {
        _options.WithDefaultCookieJar = enabled;

        // These are mutually exclusive
        _options.WithoutCookieJar = !enabled;
        return this;
    }

    /// <summary>
    /// Catches native library panics.
    /// </summary>
    public HttpClientBuilder CatchPanics(bool enable = true)
    {
        _options.CatchPanics = enable;
        return this;
    }

    /// <summary>
    /// Sets User-Agent header.
    /// </summary>
    public HttpClientBuilder WithUserAgent(string userAgent)
    {
        _options.UserAgent = userAgent;
        return this;
    }

    /// <summary>
    /// Sets default header sent with every request.
    /// </summary>
    public HttpClientBuilder SetHeader(string name, string value)
    {
        // Replaces existing value
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
            SetHeader(header.Key, header.Value);
        }

        return this;
    }

    /// <summary>
    /// Builds options without creating client.
    /// </summary>
    public Http2ClientOptions BuildOptions()
    {
        ValidateBuilder();
        _options.Validate();

        // Clone to prevent modifications
        return _options.Clone();
    }

    /// <summary>
    /// Validates builder for conflicting settings.
    /// </summary>
    private void ValidateBuilder()
    {
        if (_options.WithDefaultCookieJar && _options.WithoutCookieJar)
        {
            throw new InvalidOperationException("Cannot enable both WithDefaultCookieJar and WithoutCookieJar");
        }

        if (_options.DisableIPv4 && _options.DisableIPv6)
        {
            throw new InvalidOperationException("Cannot disable both IPv4 and IPv6");
        }
    }

    /// <summary>
    /// Builds final Http2Client instance.
    /// </summary>
    public Http2Client Build()
    {
        return new Http2Client(BuildOptions());
    }
}