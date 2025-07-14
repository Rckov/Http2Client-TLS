using FluentAssertions;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using TlsClient.Core.Helpers.Builders;
using TlsClient.Core.Models.Entities;
using TlsClient.RestSharp.Helpers.Builders;

namespace TlsClient.RestSharp.Tests
{
    public class PostResponse
    {
        [JsonProperty("args")]
        public IDictionary<string, string> Args { get; set; }

        [JsonProperty("form")]
        public IDictionary<string, string> Form { get; set; }

        [JsonProperty("files")]
        public IDictionary<string, string> Files { get; set; }

        [JsonProperty("json")]
        public IDictionary<string, string> Json { get; set; }

        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; }
    }
    public class BodyTests
    {
        [Fact]
        public async void ShouldBeInQuery()
        {
            var tlsClient= new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome132)
                .WithUserAgent("TestClient 1.0")
                .WithFollowRedirects(true)
                .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                .Build();

            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin.org")
                .WithTlsClient(tlsClient)
                .Build();


            var restReq= new RestRequest("/post", Method.Post);
            restReq.AddQueryParameter("credit", "ep.eren");
            restReq.AddQueryParameter("love", "you");
            var restResponse = await restClient.ExecuteAsync<PostResponse>(restReq);
            restResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            restResponse.Data.Should().NotBeNull();
            restResponse.Data.Args.Should().Contain("credit", "ep.eren").And.Contain("love", "you");
        }

        [Fact]
        public async void ShouldBeInForm()
        {
            var tlsClient = new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome132)
                .WithUserAgent("TestClient 1.0")
                .WithFollowRedirects(true)
                .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                .Build();
            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin.org")
                .WithTlsClient(tlsClient)
                .Build();

            var restReq = new RestRequest("/post", Method.Post);
            restReq.AddParameter("credit", "ep.eren");
            restReq.AddParameter("love", "you");
            var restResponse = await restClient.ExecuteAsync<PostResponse>(restReq);
            restResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            restResponse.Data.Should().NotBeNull();
            restResponse.Data.Form.Should().Contain("credit", "ep.eren").And.Contain("love", "you");
        }

        [Fact]
        public async void ShouldBeInFile()
        {
            var txtContent= "Hi from TlsClient!";
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, txtContent);

            var tlsClient = new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome132)
                .WithUserAgent("TestClient 1.0")
                .WithFollowRedirects(true)
                .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                .Build();
            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin.org")
                .WithTlsClient(tlsClient)
                .Build();
            var restReq = new RestRequest("/post", Method.Post);
            restReq.AddFile("textFile", tempFilePath);
            var restResponse = await restClient.ExecuteAsync<PostResponse>(restReq);

            File.Delete(tempFilePath);

            restResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            restResponse.Data.Should().NotBeNull();
            restResponse.Data.Files.Should().ContainKey("textFile");
        }

        [Fact]
        public async void ShouldBeInJson()
        {
            var tlsClient = new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome132)
                .WithUserAgent("TestClient 1.0")
                .WithFollowRedirects(true)
                .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                .Build();
            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin.org")
                .WithTlsClient(tlsClient)
                .Build();
            var restReq = new RestRequest("/post", Method.Post);
            restReq.AddJsonBody(new { credit = "ep.eren", love = "you" });
            var restResponse = await restClient.ExecuteAsync<PostResponse>(restReq);
            restResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            restResponse.Data.Should().NotBeNull();
            restResponse.Data.Json.Should().Contain("credit", "ep.eren").And.Contain("love", "you");
        }

        [Fact]
        public async void ShouldBeMultipartForm()
        {
            var txtContent = "Hi from TlsClient!";
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, txtContent);

            var tlsClient = new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome132)
                .WithUserAgent("TestClient 1.0")
                .WithFollowRedirects(true)
                .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                .Build();
            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin.org")
                .WithTlsClient(tlsClient)
                .Build();
            var restReq = new RestRequest("/post", Method.Post);
            restReq.AlwaysMultipartFormData = true;
            restReq.AddParameter("credit", "ep.eren");
            restReq.AddParameter("love", "you");
            restReq.AddFile("textFile", tempFilePath);
            var restResponse = await restClient.ExecuteAsync<PostResponse>(restReq);
            File.Delete(tempFilePath);
            restResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            restResponse.Data.Should().NotBeNull();
            restResponse.Data.Form.Should().Contain("credit", "ep.eren").And.Contain("love", "you");
            restResponse.Data.Files.Should().ContainKey("textFile");
            restResponse.Data.Headers.Should().ContainKey("Content-Type").WhoseValue.Should().Match(value => value.Contains("multipart/form-data"));
        }

        [Fact]
        public async void ShouldBeUrlEncodedForm()
        {
            var tlsClient = new TlsClientBuilder()
                .WithIdentifier(TlsClientIdentifier.Chrome132)
                .WithUserAgent("TestClient 1.0")
                .WithFollowRedirects(true)
                .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                .Build();
            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin.org")
                .WithTlsClient(tlsClient)
                .Build();
            var restReq = new RestRequest("/post", Method.Post);
            restReq.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            restReq.AddParameter("credit", "ep.eren");
            restReq.AddParameter("love", "you");

            var restResponse = await restClient.ExecuteAsync<PostResponse>(restReq);
            restResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            restResponse.Data.Should().NotBeNull();
            restResponse.Data.Form.Should().Contain("credit", "ep.eren").And.Contain("love", "you");
            restResponse.Data.Headers.Should().ContainKey("Content-Type").WhoseValue.Should().Match(value => value.Contains("x-www-form-urlencoded"));

        }
    }
}