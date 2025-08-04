using Http2Client.Core.Enums;
using Http2Client.Core.Models;

using System;
using System.Collections.Generic;

namespace Http2Client.Builders;

/// <summary>
/// Easy way to build Http2Client instances. Chain methods together and call Build().
/// </summary>
public class HttpClientBuilder
{
    private readonly Http2ClientOptions _options = new();

    /// <summary>
    /// Set path to the native library. Usually not needed - we auto-detect it.
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
    /// Turn on debug logging. Helpful when stuff breaks.
    /// </summary>
    public HttpClientBuilder WithDebug(bool enable = true)
    {
        _options.WithDebug = enable;
        return this;
    }

    /// <summary>
    /// Which browser to pretend to be. Chrome131 is usually safe.
    /// </summary>
    public HttpClientBuilder WithBrowserType(BrowserType browserType)
    {
        _options.BrowserType = browserType;
        return this;
    }

    /// <summary>
    /// Use custom TLS fingerprint instead of built-in browser profiles.
    /// </summary>
    public HttpClientBuilder WithCustomHttp2Client(CustomHttp2Client customHttp2Client)
    {
        _options.CustomHttp2Client = customHttp2Client;
        return this;
    }

    /// <summary>
    /// Skip SSL checks. Dangerous but sometimes needed for testing.
    /// </summary>
    public HttpClientBuilder WithInsecureSkipVerify(bool skip = true)
    {
        _options.InsecureSkipVerify = skip;
        return this;
    }

    /// <summary>
    /// Randomize TLS extensions to look more human. Usually a good idea.
    /// </summary>
    public HttpClientBuilder WithRandomTlsExtensions(bool enable = true)
    {
        _options.WithRandomTlsExtensionOrder = enable;
        return this;
    }

    /// <summary>
    /// Use a proxy. Supports HTTP, HTTPS, and SOCKS5.
    /// </summary>
    public HttpClientBuilder WithProxy(string proxyUrl, bool isRotating = false)
    {
        _options.ProxyUrl = proxyUrl;

        // Helps with connection pooling
        _options.IsRotatingProxy = isRotating;
        return this;
    }

    /// <summary>
    /// Force IPv6-only connections. For specific network setups.
    /// </summary>
    public HttpClientBuilder DisableIPv4(bool disable = true)
    {
        _options.DisableIPv4 = disable;
        return this;
    }

    /// <summary>
    /// Force IPv4-only connections. Sometimes needed for older networks.
    /// </summary>
    public HttpClientBuilder DisableIPv6(bool disable = true)
    {
        _options.DisableIPv6 = disable;
        return this;
    }

    /// <summary>
    /// Auto-follow redirects. Most browsers do this by default.
    /// </summary>
    public HttpClientBuilder FollowRedirects(bool follow = true)
    {
        _options.FollowRedirects = follow;
        return this;
    }

    /// <summary>
    /// Force HTTP/1.1 instead of HTTP/2. Sometimes needed for compatibility.
    /// </summary>
    public HttpClientBuilder ForceHttp1(bool force = true)
    {
        _options.ForceHttp1 = force;
        return this;
    }

    /// <summary>
    /// Enable automatic cookie handling. Cookies will be stored and sent automatically.
    /// </summary>
    public HttpClientBuilder WithCookies(bool enabled = true)
    {
        _options.WithDefaultCookieJar = enabled;

        // These are mutually exclusive
        _options.WithoutCookieJar = !enabled;
        return this;
    }

    /// <summary>
    /// Catch Go panics from the native library. Usually you want this enabled.
    /// </summary>
    public HttpClientBuilder CatchPanics(bool enable = true)
    {
        _options.CatchPanics = enable;
        return this;
    }

    /// <summary>
    /// Set the User-Agent header. Pick something that matches your TLS fingerprint.
    /// </summary>
    public HttpClientBuilder WithUserAgent(string userAgent)
    {
        _options.UserAgent = userAgent;
        return this;
    }

    /// <summary>
    /// Set a default header that will be sent with every request.
    /// </summary>
    public HttpClientBuilder SetHeader(string name, string value)
    {
        // Replaces existing value
        _options.DefaultHeaders[name] = [value];
        return this;
    }

    /// <summary>
    /// Set multiple default headers at once. Convenient for bulk setup.
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
    /// Build the options object without creating the client yet.
    /// </summary>
    public Http2ClientOptions BuildOptions()
    {
        ValidateBuilder();
        _options.Validate();

        // Clone to prevent modifications
        return _options.Clone();
    }

    /// <summary>
    /// Check for conflicting settings before building.
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
    /// Build the final Http2Client instance.
    /// </summary>
    public Http2Client Build()
    {
        return new Http2Client(BuildOptions());
    }
}