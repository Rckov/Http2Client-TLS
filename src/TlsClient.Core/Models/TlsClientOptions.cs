using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Models
{
    public class TlsClientOptions
    {
        public Dictionary<string, List<string>> DefaultHeaders { get; set; }
        public Guid SessionID { get; set; }
        public TlsClientIdentifier TlsClientIdentifier { get; set; } = TlsClientIdentifier.Chrome131;
        public string? ProxyURL { get; set; }
        public bool IsRotatingProxy { get; set; } = false;
        public TimeSpan Timeout { get; set; }
        public bool FollowRedirects { get; set; } = false;
        public bool InsecureSkipVerify { get; set; } = false;
        public bool DisableIPV4 { get; set; } = false;
        public bool DisableIPV6 { get; set; } = false;
        public bool WithDebug { get; set; } = false;
        public bool WithDefaultCookieJar { get; set; } = false;
        public bool WithoutCookieJar { get; set; } = false;

        public TlsClientOptions(TlsClientIdentifier clientIdentifier, string userAgent)
        {
            DefaultHeaders = new Dictionary<string, List<string>>()
            {
                { "User-Agent", new List<string>() { userAgent } }
            };

            SessionID = Guid.NewGuid();
            Timeout = TimeSpan.FromSeconds(0);
        }
    }
}
