using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TlsClient.Core.Helpers.Builders;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Core.Tests
{
    public class CustomClientTests
    {
        [Fact]
        public async Task ShouldBeOk()
        {
            // Reference: https://bogdanfinn.gitbook.io/open-source-oasis/tls-client/custom-client-profile#shared-library-and-standalone-api
            var tlsClient = new TlsClientBuilder()
                .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                .WithCustomTlsClient(new CustomTlsClient()
                {
                    Ja3String = "771,2570-4865-4866-4867-49195-49199-49196-49200-52393-52392-49171-49172-156-157-47-53,2570-18-5-27-11-0-10-35-16-65037-51-13-23-43-17513-65281-45-2570,2570-25497-29-23-24,0",
                    H2Settings = new Dictionary<string, uint>()
                    {
                        { "HEADER_TABLE_SIZE", 65536 },
                        { "INITIAL_WINDOW_SIZE", 6291456 },
                        { "MAX_CONCURRENT_STREAMS", 0 },
                        { "MAX_HEADER_LIST_SIZE", 262144 },
                    },
                    H2SettingsOrder = new List<string>()
                    {
                        "HEADER_TABLE_SIZE",
                        "MAX_CONCURRENT_STREAMS",
                        "INITIAL_WINDOW_SIZE",
                        "MAX_HEADER_LIST_SIZE"
                    },
                    SupportedSignatureAlgorithms = new List<string>()
                    {
                        "ECDSAWithP256AndSHA256",
                        "PSSWithSHA256",
                        "PKCS1WithSHA256",
                        "ECDSAWithP384AndSHA384",
                        "PSSWithSHA384",
                        "PKCS1WithSHA384",
                        "PSSWithSHA512",
                        "PKCS1WithSHA512",
                    },
                    SupportedVersions = new List<string>()
                    {
                        "GREASE",
                        "1.3",
                        "1.2"
                    },
                    
                    KeyShareCurves = new List<string>()
                    {
                        "GREASE",
                        "X25519Kyber768",
                        "X25519",
                    },
                    ALPNProtocols= new List<string>()
                    {
                        "h2",
                        "http/1.1",
                    },
                    ALPSProtocols = new List<string>()
                    {
                        "h2",
                    },
                    ECHCandidatePayloads = new List<ushort>()
                    {
                        128,
                        160,
                        192,
                        224,
                    },
                    CertCompressionAlgos = new List<string>()
                    {
                        "brotli",
                    },
                    PseudoHeaderOrder = new List<string>()
                    {
                        ":method",
                        ":authority",
                        ":scheme",
                        ":path",
                    },
                    ECHCandidateCipherSuites= new List<ECHCandidateCipherSuite>()
                    {
                        new ECHCandidateCipherSuite()
                        {
                            AeadId= "AEAD_AES_128_GCM",
                            KdfID= "HKDF_SHA256"
                        },
                        new ECHCandidateCipherSuite() { 
                            KdfID= "HKDF_SHA256",
                            AeadId= "AEAD_CHACHA20_POLY1305"
                        }
                    },
                    SupportedDelegatedCredentialsAlgorithms= new List<string>(),
                    ConnectionFlow = 15663105,
                    //RecordSizeLimit = 0,
                    //PriorityFrames = null
                    //HeaderPriority = null

                })
                .WithUserAgent("TestClient 1.0")
                .Build();

            var response = await tlsClient.RequestAsync(new Request()
            {
                RequestUrl = "https://tls.peet.ws/api/all",
                RequestMethod = HttpMethod.Get,
            });

            response.Status.Should().Be(HttpStatusCode.OK);
            response.Body.Should().Contain("TestClient 1.0");
        }
    }
}
