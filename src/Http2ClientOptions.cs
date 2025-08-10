using Http2Client.Builders;
using Http2Client.Core.Enums;
using Http2Client.Core.Models;
using Http2Client.Utilities;

using System;
using System.Collections.Generic;

namespace Http2Client;

/// <summary>
/// Configuration for <see cref="Http2Client" />. Use <see cref="HttpClientBuilder" /> for easy setup.
/// </summary>
public class Http2ClientOptions
{
    /// <summary>
    /// Custom TLS fingerprint instead of browser preset. For advanced use.
    /// </summary>
    public CustomHttp2Client? CustomHttp2Client { get; set; }

    /// <summary>
    /// Default headers sent with every request.
    /// </summary>
    public Dictionary<string, List<string>> DefaultHeaders { get; set; } = [];

    /// <summary>
    /// Proxy URL. Supports HTTP, HTTPS, and SOCKS5.
    /// </summary>
    public string? ProxyUrl { get; set; }

    /// <summary>
    /// Session ID for this client. Auto-generated if not set.
    /// </summary>
    public Guid SessionId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Browser fingerprint to mimic. Chrome131 works well for most sites.
    /// </summary>
    public BrowserType BrowserType { get; set; } = BrowserType.Chrome133;

    /// <summary>
    /// Header order. Some servers check this.
    /// </summary>
    public List<string> HeaderOrder { get; set; } = [];

    /// <summary>
    /// Request timeout. Default is 60 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Catch native library panics. Enabled by default.
    /// </summary>
    public bool CatchPanics { get; set; } = true;

    /// <summary>
    /// Follow redirects automatically. Disabled by default.
    /// </summary>
    public bool FollowRedirects { get; set; }

    /// <summary>
    /// Force HTTP/1.1 instead of HTTP/2.
    /// </summary>
    public bool ForceHttp1 { get; set; }

    /// <summary>
    /// Skip SSL certificate validation. Use with caution.
    /// </summary>
    public bool InsecureSkipVerify { get; set; }

    /// <summary>
    /// True if proxy rotates IPs automatically.
    /// </summary>
    public bool IsRotatingProxy { get; set; }

    /// <summary>
    /// Disable IPv6 connections.
    /// </summary>
    public bool DisableIPv6 { get; set; }

    /// <summary>
    /// Disable IPv4 connections.
    /// </summary>
    public bool DisableIPv4 { get; set; }

    /// <summary>
    /// Enable debug logging.
    /// </summary>
    public bool WithDebug { get; set; }

    /// <summary>
    /// Enable automatic cookie handling.
    /// </summary>
    public bool WithDefaultCookieJar { get; set; }

    /// <summary>
    /// Disable all cookie handling.
    /// </summary>
    public bool WithoutCookieJar { get; set; }

    /// <summary>
    /// Randomize TLS extension order for better browser mimicking.
    /// </summary>
    public bool WithRandomTlsExtensionOrder { get; set; }

    /// <summary>
    /// Path to native TLS library. Auto-detected if not specified.
    /// </summary>
    public string? LibraryPath { get; set; } = GetDefaultLibraryPath();

    /// <summary>
    /// User-Agent header value.
    /// </summary>
    public string? UserAgent
    {
        get => DefaultHeaders.TryGetValue("User-Agent", out var values) && values.Count > 0 ? values[0] : null;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                DefaultHeaders.Remove("User-Agent");
            }
            else
            {
                DefaultHeaders["User-Agent"] = [value];
            }
        }
    }

    /// <summary>
    /// Validates configuration. Called automatically when creating client.
    /// </summary>
    public void Validate()
    {
        ThrowException.FileNotExists(LibraryPath, nameof(LibraryPath));

        if (!string.IsNullOrEmpty(ProxyUrl))
        {
            ThrowException.IsUri(ProxyUrl, nameof(ProxyUrl));
        }

        if (Timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("Timeout must be positive", nameof(Timeout));
        }

        if (WithDefaultCookieJar && WithoutCookieJar)
        {
            throw new InvalidOperationException("Cannot enable both WithDefaultCookieJar and WithoutCookieJar.");
        }

        if (DisableIPv4 && DisableIPv6)
        {
            throw new InvalidOperationException("Cannot disable both IPv4 and IPv6.");
        }
    }

    /// <summary>
    /// Creates a deep copy of these options.
    /// </summary>
    public Http2ClientOptions Clone()
    {
        var clone = new Http2ClientOptions
        {
            CustomHttp2Client = CustomHttp2Client,
            ProxyUrl = ProxyUrl,
            SessionId = SessionId,
            BrowserType = BrowserType,
            Timeout = Timeout,
            CatchPanics = CatchPanics,
            FollowRedirects = FollowRedirects,
            ForceHttp1 = ForceHttp1,
            InsecureSkipVerify = InsecureSkipVerify,
            IsRotatingProxy = IsRotatingProxy,
            DisableIPv6 = DisableIPv6,
            DisableIPv4 = DisableIPv4,
            WithDebug = WithDebug,
            WithDefaultCookieJar = WithDefaultCookieJar,
            WithoutCookieJar = WithoutCookieJar,
            WithRandomTlsExtensionOrder = WithRandomTlsExtensionOrder,
            LibraryPath = LibraryPath
        };

        // Deep copy headers so changes to the clone don't mess with the original
        foreach (var header in DefaultHeaders)
        {
            clone.DefaultHeaders[header.Key] = [.. header.Value];
        }

        return clone;
    }

    /// <summary>
    /// Gets default native library path for current platform.
    /// </summary>
    private static string GetDefaultLibraryPath()
    {
        // .dll / .so / .dylib
        var extension = PlatformSupport.GetNativeLibraryExtension();
        return PlatformSupport.GetRuntimePath($"tls-client.{extension}");
    }
}