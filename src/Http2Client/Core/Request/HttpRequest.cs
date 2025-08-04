using Http2Client.Core.Enums;
using Http2Client.Core.Models;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Request;

/// <summary>
/// HTTP request configuration. Usually built with HttpRequestBuilder instead of setting these manually.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>RequestInput</c> struct in the original library:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L51">types.go#L51</a>
/// </remarks>
public class HttpRequest
{
    /// <summary>
    /// Unique request ID. Gets generated automatically.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The URL to request. Required field.
    /// </summary>
    [JsonPropertyName("requestUrl")]
    public string RequestUrl { get; set; } = string.Empty;

    /// <summary>
    /// HTTP method like GET, POST, etc.
    /// </summary>
    [JsonPropertyName("requestMethod")]
    public string RequestMethod { get; set; } = "GET";

    /// <summary>
    /// Request body content. Can be JSON, form data, or whatever.
    /// </summary>
    [JsonPropertyName("requestBody")]
    public string? RequestBody { get; set; }

    /// <summary>
    /// Headers for this specific request.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = [];

    /// <summary>
    /// Default headers that get merged with request headers.
    /// </summary>
    [JsonPropertyName("defaultHeaders")]
    public Dictionary<string, List<string>> DefaultHeaders { get; set; } = [];

    /// <summary>
    /// Headers used for CONNECT requests (proxy tunneling).
    /// </summary>
    [JsonPropertyName("connectHeaders")]
    public Dictionary<string, List<string>> ConnectHeaders { get; set; } = [];

    /// <summary>
    /// Order to send headers in. Some servers are picky about this.
    /// </summary>
    [JsonPropertyName("headerOrder")]
    public List<string> HeaderOrder { get; set; } = [];

    /// <summary>
    /// Which browser fingerprint to use for TLS.
    /// </summary>
    [JsonPropertyName("tlsClientIdentifier")]
    public BrowserType? BrowserType { get; set; }

    /// <summary>
    /// Custom TLS configuration instead of predefined fingerprint.
    /// </summary>
    [JsonPropertyName("customTlsClient")]
    public CustomHttp2Client? CustomHttp2Client { get; set; }

    /// <summary>
    /// Randomize TLS extension order to look more human.
    /// </summary>
    [JsonPropertyName("withRandomTLSExtensionOrder")]
    public bool WithRandomTLSExtensionOrder { get; set; }

    /// <summary>
    /// Session ID for cookie/connection reuse.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public Guid? SessionId { get; set; }

    /// <summary>
    /// Proxy URL if you want to route through a proxy.
    /// </summary>
    [JsonPropertyName("proxyUrl")]
    public string? ProxyUrl { get; set; }

    /// <summary>
    /// True if the proxy rotates IPs automatically.
    /// </summary>
    [JsonPropertyName("isRotatingProxy")]
    public bool? IsRotatingProxy { get; set; }

    /// <summary>
    /// Bind to specific local IP address.
    /// </summary>
    [JsonPropertyName("localAddress")]
    public string? LocalAddress { get; set; }

    /// <summary>
    /// Override SNI hostname for TLS.
    /// </summary>
    [JsonPropertyName("serverNameOverwrite")]
    public string? ServerNameOverwrite { get; set; }

    /// <summary>
    /// Override Host header value.
    /// </summary>
    [JsonPropertyName("requestHostOverride")]
    public string? RequestHostOverride { get; set; }

    /// <summary>
    /// Request timeout in milliseconds.
    /// </summary>
    [JsonPropertyName("timeoutMilliseconds")]
    public int? TimeoutMilliseconds { get; set; }

    /// <summary>
    /// Request timeout in seconds. Use TimeoutMilliseconds instead.
    /// </summary>
    [JsonPropertyName("timeoutSeconds")]
    public int TimeoutSeconds { get; set; }

    /// <summary>
    /// Whether to follow 3xx redirects automatically.
    /// </summary>
    [JsonPropertyName("followRedirects")]
    public bool? FollowRedirects { get; set; }

    /// <summary>
    /// Force HTTP/1.1 instead of HTTP/2.
    /// </summary>
    [JsonPropertyName("forceHttp1")]
    public bool ForceHttp1 { get; set; }

    /// <summary>
    /// Skip SSL certificate verification. Dangerous but sometimes needed.
    /// </summary>
    [JsonPropertyName("insecureSkipVerify")]
    public bool? InsecureSkipVerify { get; set; }

    /// <summary>
    /// Disable IPv4 connections.
    /// </summary>
    [JsonPropertyName("disableIPV4")]
    public bool? DisableIPv4 { get; set; }

    /// <summary>
    /// Disable IPv6 connections.
    /// </summary>
    [JsonPropertyName("disableIPV6")]
    public bool? DisableIPv6 { get; set; }

    /// <summary>
    /// Catch Go panics from native library.
    /// </summary>
    [JsonPropertyName("catchPanics")]
    public bool CatchPanics { get; set; }

    /// <summary>
    /// Enable debug logging.
    /// </summary>
    [JsonPropertyName("withDebug")]
    public bool? WithDebug { get; set; }

    /// <summary>
    /// Cookies to send with this request.
    /// </summary>
    [JsonPropertyName("requestCookies")]
    public List<ClientCookie> RequestCookies { get; set; } = [];

    /// <summary>
    /// Use automatic cookie jar.
    /// </summary>
    [JsonPropertyName("withDefaultCookieJar")]
    public bool? WithDefaultCookieJar { get; set; }

    /// <summary>
    /// Disable cookie handling completely.
    /// </summary>
    [JsonPropertyName("withoutCookieJar")]
    public bool? WithoutCookieJar { get; set; }

    /// <summary>
    /// Request body contains binary data (Base64 encoded).
    /// </summary>
    [JsonPropertyName("isByteRequest")]
    public bool IsByteRequest { get; set; }

    /// <summary>
    /// Expect binary response data.
    /// </summary>
    [JsonPropertyName("isByteResponse")]
    public bool IsByteResponse { get; set; }

    /// <summary>
    /// Stream response in chunks of this size.
    /// </summary>
    [JsonPropertyName("streamOutputBlockSize")]
    public int? StreamOutputBlockSize { get; set; }

    /// <summary>
    /// Symbol that marks end of streamed response.
    /// </summary>
    [JsonPropertyName("streamOutputEOFSymbol")]
    public string? StreamOutputEOFSymbol { get; set; }

    /// <summary>
    /// File path to save streamed response to.
    /// </summary>
    [JsonPropertyName("streamOutputPath")]
    public string? StreamOutputPath { get; set; }

    /// <summary>
    /// Pin specific certificates for certain hosts.
    /// </summary>
    [JsonPropertyName("certificatePinningHosts")]
    public Dictionary<string, List<string>> CertificatePinningHosts { get; set; } = [];

    /// <summary>
    /// Low-level transport options.
    /// </summary>
    [JsonPropertyName("transportOptions")]
    public TransportOptions? TransportOptions { get; set; }
}