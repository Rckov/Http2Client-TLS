using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Http2Client.Core.Models;

/// <summary>
/// Custom TLS fingerprint configuration. For advanced users who want full control.
/// Most people should just use the predefined TlsClientIdentifier values instead.
/// </summary>
/// <remarks>
/// Corresponds to the Go <c>CustomTlsClient</c> struct:
/// <a href="https://github.com/bogdanfinn/tls-client/blob/974499ad9c57a510919e1493babfe60ec558d12b/cffi_src/types.go#L90">types.go#L90</a>
/// </remarks>
public class CustomHttp2Client
{
    /// <summary>
    /// HTTP/2 settings to advertise (e.g., SETTINGS_MAX_CONCURRENT_STREAMS).
    /// </summary>
    [JsonPropertyName("h2Settings")]
    public Dictionary<string, uint> Http2Settings { get; set; } = [];

    /// <summary>
    /// HTTP/2 header priority configuration.
    /// </summary>
    [JsonPropertyName("headerPriority")]
    public PriorityParam? HeaderPriority { get; set; }

    /// <summary>
    /// Certificate compression algorithms to support (e.g., "brotli").
    /// </summary>
    [JsonPropertyName("certCompressionAlgos")]
    public List<string> CertificateCompressionAlgorithms { get; set; } = [];

    /// <summary>
    /// JA3 fingerprint string. Leave empty to auto-generate.
    /// </summary>
    [JsonPropertyName("ja3String")]
    public string Ja3Fingerprint { get; set; } = string.Empty;

    /// <summary>
    /// Order to send HTTP/2 settings in. Affects fingerprint realism.
    /// </summary>
    [JsonPropertyName("h2SettingsOrder")]
    public List<string> Http2SettingsOrder { get; set; } = [];

    /// <summary>
    /// Elliptic curves to advertise for key exchange (e.g., "X25519", "P-256").
    /// </summary>
    [JsonPropertyName("keyShareCurves")]
    public List<string> KeyShareCurves { get; set; } = [];

    /// <summary>
    /// ALPN protocols to advertise (e.g., "h2", "http/1.1").
    /// </summary>
    [JsonPropertyName("alpnProtocols")]
    public List<string> AlpnProtocols { get; set; } = [];

    /// <summary>
    /// ALPS protocols for application layer settings.
    /// </summary>
    [JsonPropertyName("alpsProtocols")]
    public List<string> AlpsProtocols { get; set; } = [];

    /// <summary>
    /// ECH candidate payloads (as uint16 values).
    /// </summary>
    [JsonPropertyName("ECHCandidatePayloads")]
    public List<ushort> EchCandidatePayloads { get; set; } = [];

    /// <summary>
    /// ECH cipher suites for encrypted client hello.
    /// </summary>
    [JsonPropertyName("ECHCandidateCipherSuites")]
    public List<CandidateCipherSuite> EchCandidateCipherSuites { get; set; } = [];

    /// <summary>
    /// HTTP/2 priority frames to send for stream prioritization.
    /// </summary>
    [JsonPropertyName("priorityFrames")]
    public List<PriorityFrame> PriorityFrames { get; set; } = [];

    /// <summary>
    /// Order to send HTTP/2 pseudo-headers (e.g., ":method", ":path").
    /// </summary>
    [JsonPropertyName("pseudoHeaderOrder")]
    public List<string> PseudoHeaderOrder { get; set; } = [];

    /// <summary>
    /// Delegated credentials algorithms to support.
    /// </summary>
    [JsonPropertyName("supportedDelegatedCredentialsAlgorithms")]
    public List<string> SupportedDelegatedCredentialsAlgorithms { get; set; } = [];

    /// <summary>
    /// Signature algorithms to advertise (e.g., "ECDSA", "RSA").
    /// </summary>
    [JsonPropertyName("supportedSignatureAlgorithms")]
    public List<string> SupportedSignatureAlgorithms { get; set; } = [];

    /// <summary>
    /// TLS versions to support (e.g., "TLS 1.2", "TLS 1.3").
    /// </summary>
    [JsonPropertyName("supportedVersions")]
    public List<string> SupportedVersions { get; set; } = [];

    /// <summary>
    /// HTTP/2 connection-level flow control window size.
    /// </summary>
    [JsonPropertyName("connectionFlow")]
    public uint ConnectionFlow { get; set; }

    /// <summary>
    /// TLS record size limit (in bytes).
    /// </summary>
    [JsonPropertyName("recordSizeLimit")]
    public ushort RecordSizeLimit { get; set; }
}