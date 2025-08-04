using Http2Client.Core.Enums;
using Http2Client.Core.Models;
using Http2Client.Utilities;

using System;
using System.Collections.Generic;
using System.IO;

namespace Http2Client;

/// <summary>
/// All the settings for your Http2Client. Usually built with HttpClientBuilder,
/// but you can create this directly if you want full control.
/// </summary>
public class Http2ClientOptions
{
    /// <summary>
    /// Unique ID for this client's session. Gets auto-generated, but you can set your own.
    /// </summary>
    public Guid SessionID { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Where to find the native TLS library. We'll try to auto-detect if you don't set this.
    /// </summary>
    public string LibraryPath { get; set; } = GetDefaultLibraryPath();

    /// <summary>
    /// Which browser to pretend to be. Chrome131 is a good default that works most places.
    /// </summary>
    public BrowserType BrowserType { get; set; } = BrowserType.Chrome131;

    /// <summary>
    /// Roll your own TLS fingerprint instead of using a preset. Advanced users only.
    /// </summary>
    public CustomHttp2Client? CustomHttp2Client { get; set; }

    /// <summary>
    /// Skip SSL certificate checks. Dangerous but sometimes needed for testing or weird corporate setups.
    /// </summary>
    public bool InsecureSkipVerify { get; set; }

    /// <summary>
    /// Proxy server to route requests through. Format: "http://proxy:port" or "socks5://proxy:port"
    /// </summary>
    public string? ProxyUrl { get; set; }

    /// <summary>
    /// Set to true if your proxy changes IP addresses automatically. Helps with connection pooling.
    /// </summary>
    public bool IsRotatingProxy { get; set; }

    /// <summary>
    /// Block IPv4 connections. Only useful if you want IPv6-only for some reason.
    /// </summary>
    public bool DisableIPv4 { get; set; }

    /// <summary>
    /// Block IPv6 connections. Sometimes needed for networks that don't support it properly.
    /// </summary>
    public bool DisableIPv6 { get; set; }

    /// <summary>
    /// How long to wait for requests before giving up. 60 seconds is usually plenty.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Automatically follow 3xx redirects. Disabled by default so you have control.
    /// </summary>
    public bool FollowRedirects { get; set; } = false;

    /// <summary>
    /// Force HTTP/1.1 even if the server supports HTTP/2. Useful for debugging.
    /// </summary>
    public bool ForceHttp1 { get; set; } = false;

    /// <summary>
    /// Let the client handle cookies automatically. They'll be saved and sent back on future requests.
    /// </summary>
    public bool WithDefaultCookieJar { get; set; } = false;

    /// <summary>
    /// Turn off all cookie handling. Cookies won't be saved or sent automatically.
    /// </summary>
    public bool WithoutCookieJar { get; set; } = false;

    /// <summary>
    /// Headers to include with every request. Individual requests can override these.
    /// </summary>
    public Dictionary<string, List<string>> DefaultHeaders { get; set; } = [];

    /// <summary>
    /// Shortcut to set or retrieve the User-Agent header.
    /// </summary>
    public string? UserAgent
    {
        get => DefaultHeaders.TryGetValue("User-Agent", out var values) && values.Count > 0 ? values[0] : null;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                DefaultHeaders.Remove("User-Agent");
            else
                DefaultHeaders["User-Agent"] = [value];
        }
    }

    /// <summary>
    /// Enable verbose debug logging from the native library.
    /// </summary>
    public bool WithDebug { get; set; }

    /// <summary>
    /// Catch panics from native Go code and avoid crashing the app. Enabled by default.
    /// </summary>
    public bool CatchPanics { get; set; } = true;

    /// <summary>
    /// Randomize TLS extension order. Helps mimic real browsers more closely.
    /// </summary>
    public bool WithRandomTlsExtensionOrder { get; set; }

    /// <summary>
    /// Validates current configuration and throws if something is misconfigured.
    /// Called automatically during client creation.
    /// </summary>
    public void Validate()
    {
        if (Timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("Timeout must be greater than zero.", nameof(Timeout));
        }

        if (!File.Exists(LibraryPath))
        {
            throw new FileNotFoundException($"Native library not found at: {LibraryPath}", LibraryPath);
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
    /// Creates a full deep copy of these options. Useful when you want to create multiple clients with similar settings.
    /// </summary>
    public Http2ClientOptions Clone()
    {
        var clone = new Http2ClientOptions
        {
            SessionID = SessionID,
            LibraryPath = LibraryPath,
            BrowserType = BrowserType,
            CustomHttp2Client = CustomHttp2Client,
            InsecureSkipVerify = InsecureSkipVerify,
            ProxyUrl = ProxyUrl,
            IsRotatingProxy = IsRotatingProxy,
            DisableIPv4 = DisableIPv4,
            DisableIPv6 = DisableIPv6,
            Timeout = Timeout,
            FollowRedirects = FollowRedirects,
            ForceHttp1 = ForceHttp1,
            WithDefaultCookieJar = WithDefaultCookieJar,
            WithoutCookieJar = WithoutCookieJar,
            WithDebug = WithDebug,
            CatchPanics = CatchPanics,
            WithRandomTlsExtensionOrder = WithRandomTlsExtensionOrder
        };

        // Deep copy headers so changes to the clone don't mess with the original
        foreach (var header in DefaultHeaders)
        {
            clone.DefaultHeaders[header.Key] = [.. header.Value];
        }

        return clone;
    }

    /// <summary>
    /// Determines the default path to the native TLS library based on the current platform.
    /// </summary>
    private static string GetDefaultLibraryPath()
    {
        // .dll / .so / .dylib
        var extension = PlatformSupport.GetNativeLibraryExtension();
        return PlatformSupport.GetRuntimePath($"tls-client.{extension}");
    }
}