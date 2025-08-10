using System.Text.Json.Serialization;

namespace Http2Client.Core.Models;

/// <summary>
/// Low-level transport options for connection tuning.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>TransportOptions</c> struct:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L129">types.go#L129</a>
/// </remarks>
public class TransportOptions
{
    /// <summary>
    /// Idle connection timeout in milliseconds.
    /// </summary>
    [JsonPropertyName("idleConnTimeout")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? IdleConnTimeout { get; set; }

    /// <summary>
    /// Max idle connections.
    /// </summary>
    [JsonPropertyName("maxIdleConns")]
    public int MaxIdleConns { get; set; }

    /// <summary>
    /// Max idle connections per host.
    /// </summary>
    [JsonPropertyName("maxIdleConnsPerHost")]
    public int MaxIdleConnsPerHost { get; set; }

    /// <summary>
    /// Max total connections per host.
    /// </summary>
    [JsonPropertyName("maxConnsPerHost")]
    public int MaxConnsPerHost { get; set; }

    /// <summary>
    /// Max response header bytes.
    /// </summary>
    [JsonPropertyName("maxResponseHeaderBytes")]
    public long MaxResponseHeaderBytes { get; set; }

    /// <summary>
    /// Write buffer size.
    /// </summary>
    [JsonPropertyName("writeBufferSize")]
    public int WriteBufferSize { get; set; }

    /// <summary>
    /// Read buffer size.
    /// </summary>
    [JsonPropertyName("readBufferSize")]
    public int ReadBufferSize { get; set; }

    /// <summary>
    /// Disable keep-alive connections.
    /// </summary>
    [JsonPropertyName("disableKeepAlives")]
    public bool DisableKeepAlives { get; set; }

    /// <summary>
    /// Disable response compression.
    /// </summary>
    [JsonPropertyName("disableCompression")]
    public bool DisableCompression { get; set; }
}