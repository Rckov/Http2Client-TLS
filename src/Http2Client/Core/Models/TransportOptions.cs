using System.Text.Json.Serialization;

namespace Http2Client.Core.Models;

/// <summary>
/// Low-level HTTP transport options. For fine-tuning connection behavior.
/// Most users won't need to touch these.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>TransportOptions</c> struct:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L129">types.go#L129</a>
/// </remarks>
public class TransportOptions
{
    /// <summary>
    /// How long to keep idle connections open (in milliseconds). Zero means no limit.
    /// </summary>
    [JsonPropertyName("idleConnTimeout")]
    public long? IdleConnTimeout { get; set; }

    /// <summary>
    /// Maximum number of idle connections to keep.
    /// </summary>
    [JsonPropertyName("maxIdleConns")]
    public int MaxIdleConns { get; set; }

    /// <summary>
    /// Maximum idle connections per host.
    /// </summary>
    [JsonPropertyName("maxIdleConnsPerHost")]
    public int MaxIdleConnsPerHost { get; set; }

    /// <summary>
    /// Maximum total connections per host.
    /// </summary>
    [JsonPropertyName("maxConnsPerHost")]
    public int MaxConnsPerHost { get; set; }

    /// <summary>
    /// Maximum bytes to read from response headers.
    /// </summary>
    [JsonPropertyName("maxResponseHeaderBytes")]
    public long MaxResponseHeaderBytes { get; set; }

    /// <summary>
    /// Size of write buffer for connections.
    /// </summary>
    [JsonPropertyName("writeBufferSize")]
    public int WriteBufferSize { get; set; }

    /// <summary>
    /// Size of read buffer for connections.
    /// </summary>
    [JsonPropertyName("readBufferSize")]
    public int ReadBufferSize { get; set; }

    /// <summary>
    /// Disable HTTP keep-alive connections.
    /// </summary>
    [JsonPropertyName("disableKeepAlives")]
    public bool DisableKeepAlives { get; set; }

    /// <summary>
    /// Disable automatic response compression.
    /// </summary>
    [JsonPropertyName("disableCompression")]
    public bool DisableCompression { get; set; }
}