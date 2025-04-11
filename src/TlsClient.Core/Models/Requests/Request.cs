using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

using TlsClient.Core.Helpers.Converters;
using TlsClient.Core.Models.Entities;

namespace TlsClient.Core.Models.Requests
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/master/cffi_src/types.go#L51
    public class Request
    {
        public Dictionary<string, List<string>> CertificatePinningHosts { get; set; }
        public CustomTlsClient? CustomTlsClient { get; set; }
        public TransportOptions? TransportOptions { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, List<string>> DefaultHeaders { get; set; }
        public Dictionary<string, List<string>> ConnectHeaders { get; set; }
        public string? LocalAddress { get; set; }
        public string? ServerNameOverwrite { get; set; }
        public string? ProxyUrl { get; set; }
        public string? RequestBody { get; set; }
        public string? RequestHostOverride { get; set; }
        public Guid? SessionId { get; set; }
        public int? StreamOutputBlockSize { get; set; }
        public string? StreamOutputEOFSymbol { get; set; }
        public string? StreamOutputPath { get; set; }
        [JsonConverter(typeof(JsonStringConverter<HttpMethod>))]
        public HttpMethod RequestMethod { get; set; } = HttpMethod.Get;
        public string RequestUrl { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringConverter<TlsClientIdentifier>))]
        public TlsClientIdentifier TlsClientIdentifier { get; set; } = TlsClientIdentifier.Chrome131;
        public List<string> HeaderOrder { get; set; } = new List<string>();
        public List<TlsClientCookie> RequestCookies { get; set; } = new List<TlsClientCookie>();
        public int? TimeoutMilliseconds { get; set; }
        public int TimeoutSeconds { get; set; }
        public bool CatchPanics { get; set; }
        public bool? FollowRedirects { get; set; }
        public bool ForceHttp1 { get; set; }
        public bool? InsecureSkipVerify { get; set; }
        public bool IsByteRequest { get; set; }
        public bool IsByteResponse { get; set; }
        public bool? IsRotatingProxy { get; set; }
        public bool? DisableIPV6 { get; set; }
        public bool? DisableIPV4 { get; set; }
        public bool? WithDebug { get; set; }
        public bool? WithDefaultCookieJar { get; set; }
        public bool? WithoutCookieJar { get; set; }
        public bool WithRandomTLSExtensionOrder { get; set; }
    }
}
