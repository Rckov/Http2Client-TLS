using System.Text.Json.Serialization;

namespace Http2Client.Core.Models;

/// <summary>
/// HTTP/2 priority frame for advanced TLS fingerprinting.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>PriorityFrames</c> struct:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L144">types.go#L144</a>
/// </remarks>
public class PriorityFrame
{
    /// <summary>
    /// Priority parameters.
    /// </summary>
    [JsonPropertyName("priorityParam")]
    public PriorityParam PriorityParam { get; set; } = new();

    /// <summary>
    /// Stream ID for this priority.
    /// </summary>
    [JsonPropertyName("streamID")]
    public uint StreamId { get; set; }
}

/// <summary>
/// HTTP/2 priority parameters for stream dependencies.
/// </summary>
public class PriorityParam
{
    /// <summary>
    /// Stream dependency ID.
    /// </summary>
    [JsonPropertyName("streamDep")]
    public uint StreamDep { get; set; }

    /// <summary>
    /// Exclusive dependency flag.
    /// </summary>
    [JsonPropertyName("exclusive")]
    public bool Exclusive { get; set; }

    /// <summary>
    /// Priority weight (1-256). Higher is more important.
    /// </summary>
    [JsonPropertyName("weight")]
    public byte Weight { get; set; }
}