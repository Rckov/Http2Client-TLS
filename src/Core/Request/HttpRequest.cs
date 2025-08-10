using Http2Client.Builders;
using Http2Client.Core.Enums;
using Http2Client.Core.Models;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Request;

/// <summary>
/// HTTP request configuration. Use <see cref="HttpRequestBuilder" /> for easier setup.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>RequestInput</c> struct in the original library:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L51">types.go#L51</a>
/// </remarks>
public class HttpRequest
{
    /// <summary>
    /// Request ID. Auto-generated.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Certificate pinning by host.
    /// </summary>
    [JsonPropertyName("certificatePinningHosts")]
    public Dictionary<string, List<string>> CertificatePinningHosts { get; set; } = [];

    /// <summary>
    /// Custom TLS config instead of browser preset.
    /// </summary>
    [JsonPropertyName("customTlsClient")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CustomHttp2Client? CustomHttp2Client { get; set; }

    /// <summary>
    /// Transport layer options.
    /// </summary>
    [JsonPropertyName("transportOptions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TransportOptions? TransportOptions { get; set; }

    /// <summary>
    /// Request headers.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = [];

    /// <summary>
    /// Default headers merged with request headers.
    /// </summary>
    [JsonPropertyName("defaultHeaders")]
    public Dictionary<string, List<string>> DefaultHeaders { get; set; } = [];

    /// <summary>
    /// Headers for CONNECT requests (proxy tunneling).
    /// </summary>
    [JsonPropertyName("connectHeaders")]
    public Dictionary<string, List<string>> ConnectHeaders { get; set; } = [];

    /// <summary>
    /// Local IP address to bind to.
    /// </summary>
    [JsonPropertyName("localAddress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LocalAddress { get; set; }

    /// <summary>
    /// SNI hostname override.
    /// </summary>
    [JsonPropertyName("serverNameOverwrite")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ServerNameOverwrite { get; set; }

    /// <summary>
    /// Proxy URL for routing requests.
    /// </summary>
    [JsonPropertyName("proxyUrl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProxyUrl { get; set; }

    /// <summary>
    /// Request body content.
    /// </summary>
    [JsonPropertyName("requestBody")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RequestBody { get; set; }

    /// <summary>
    /// Host header override.
    /// </summary>
    [JsonPropertyName("requestHostOverride")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RequestHostOverride { get; set; }

    /// <summary>
    /// Session ID for connection reuse.
    /// </summary>
    [JsonPropertyName("sessionId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? SessionId { get; set; }

    /// <summary>
    /// Stream response chunk size.
    /// </summary>
    [JsonPropertyName("streamOutputBlockSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StreamOutputBlockSize { get; set; }

    /// <summary>
    /// End-of-stream symbol.
    /// </summary>
    [JsonPropertyName("streamOutputEOFSymbol")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StreamOutputEofSymbol { get; set; }

    /// <summary>
    /// File path for streaming response.
    /// </summary>
    [JsonPropertyName("streamOutputPath")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StreamOutputPath { get; set; }

    /// <summary>
    /// HTTP method (GET, POST, etc).
    /// </summary>
    [JsonPropertyName("requestMethod")]
    public string RequestMethod { get; set; } = "GET";

    /// <summary>
    /// Target URL. Required.
    /// </summary>
    [JsonPropertyName("requestUrl")]
    public string RequestUrl { get; set; } = string.Empty;

    /// <summary>
    /// Browser fingerprint for TLS.
    /// </summary>
    [JsonPropertyName("tlsClientIdentifier")]
    public BrowserType BrowserType { get; set; } = BrowserType.Chrome131;

    /// <summary>
    /// Header order. Some servers check this.
    /// </summary>
    [JsonPropertyName("headerOrder")]
    public List<string> HeaderOrder { get; set; } = [];

    /// <summary>
    /// Cookies for this request.
    /// </summary>
    [JsonPropertyName("requestCookies")]
    public List<ClientCookie> RequestCookies { get; set; } = [];

    /// <summary>
    /// Timeout in milliseconds.
    /// </summary>
    [JsonPropertyName("timeoutMilliseconds")]
    public int TimeoutMilliseconds { get; set; }

    /// <summary>
    /// Timeout in seconds. Prefer TimeoutMilliseconds.
    /// </summary>
    [JsonPropertyName("timeoutSeconds")]
    public int TimeoutSeconds { get; set; }

    /// <summary>
    /// Catch native library panics.
    /// </summary>
    [JsonPropertyName("catchPanics")]
    public bool CatchPanics { get; set; }

    /// <summary>
    /// Follow redirects automatically.
    /// </summary>
    [JsonPropertyName("followRedirects")]
    public bool FollowRedirects { get; set; }

    /// <summary>
    /// Force HTTP/1.1 over HTTP/2.
    /// </summary>
    [JsonPropertyName("forceHttp1")]
    public bool ForceHttp1 { get; set; }

    /// <summary>
    /// Skip SSL certificate verification.
    /// </summary>
    [JsonPropertyName("insecureSkipVerify")]
    public bool InsecureSkipVerify { get; set; }

    /// <summary>
    /// Request body is binary (Base64 encoded).
    /// </summary>
    [JsonPropertyName("isByteRequest")]
    public bool IsByteRequest { get; set; }

    /// <summary>
    /// Expect binary response.
    /// </summary>
    [JsonPropertyName("isByteResponse")]
    public bool IsByteResponse { get; set; }

    /// <summary>
    /// True if proxy rotates IPs.
    /// </summary>
    [JsonPropertyName("isRotatingProxy")]
    public bool IsRotatingProxy { get; set; }

    /// <summary>
    /// Disable IPv6.
    /// </summary>
    [JsonPropertyName("disableIPV6")]
    public bool DisableIPv6 { get; set; }

    /// <summary>
    /// Disable IPv4.
    /// </summary>
    [JsonPropertyName("disableIPV4")]
    public bool DisableIPv4 { get; set; }

    /// <summary>
    /// Enable debug mode.
    /// </summary>
    [JsonPropertyName("withDebug")]
    public bool WithDebug { get; set; }

    /// <summary>
    /// Use automatic cookie handling.
    /// </summary>
    [JsonPropertyName("withDefaultCookieJar")]
    public bool WithDefaultCookieJar { get; set; }

    /// <summary>
    /// Disable all cookie handling.
    /// </summary>
    [JsonPropertyName("withoutCookieJar")]
    public bool WithoutCookieJar { get; set; }

    /// <summary>
    /// Randomize TLS extension order.
    /// </summary>
    [JsonPropertyName("withRandomTLSExtensionOrder")]
    public bool WithRandomTlsExtensionOrder { get; set; }
}