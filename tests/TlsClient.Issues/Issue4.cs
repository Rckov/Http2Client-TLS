using FluentAssertions;
using RestSharp;
using RestSharp.Interceptors;
using System.Net;
using TlsClient.Core.Helpers.Builders;
using TlsClient.Core.Models.Entities;
using TlsClient.RestSharp.Helpers.Builders;

namespace TlsClient.Issues
{
    public class Issue4
    {
        [Fact]
        public async Task ShouldErrorWithCatch()
        {
            using var tlsClient = new TlsClientBuilder()
                          .WithIdentifier(TlsClientIdentifier.Chrome132)
                          .WithUserAgent("TestClient 1.0")
                          .WithFollowRedirects(true)
                          .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                          .WithTimeout(TimeSpan.FromSeconds(10))
                          .Build();


            var restClient = new TlsRestClientBuilder()
                .WithBaseUrl("https://httpbin.org")
                .WithTlsClient(tlsClient)
                .Build();

            var restReq = new RestRequest("/status/500", Method.Get);
            var restResponse = await restClient.ExecuteAsync(restReq);

            restResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task ShouldMemoryLeakRestSharp_MultiThreaded()
        {
            int threadCount = 10;
            int requestsPerThread = 500; // You can increase for more stress

            while (true)
            {
                var tasks = new List<Task>();

                for (int i = 0; i < threadCount; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        using var tlsClient = new TlsClientBuilder()
                            .WithIdentifier(TlsClientIdentifier.Chrome132)
                            .WithUserAgent("TestClient 1.0")
                            .WithFollowRedirects(true)
                            .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                            .WithTimeout(TimeSpan.FromSeconds(10))
                            .Build();


                        var restClient = new TlsRestClientBuilder()
                            .WithBaseUrl("https://httpbin.org")
                            .WithTlsClient(tlsClient)
                            .Build();

                        for (int j = 0; j < requestsPerThread; j++)
                        {
                            var restReq = new RestRequest("/status/500", Method.Get);
                            var restResponse = await restClient.ExecuteAsync(restReq);
                            Console.WriteLine(restResponse.Content);
                        }
                    }));
                }

                await Task.WhenAll(tasks);
            }
        }

        [Fact]
        public async Task ShouldMemoryLeak_MultiThreaded()
        {
            int threadCount = 10;
            int requestsPerThread = 500; // You can increase for more stress

            while (true)
            {
                var tasks = new List<Task>();

                for (int i = 0; i < threadCount; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        using var tlsClient = new TlsClientBuilder()
                            .WithIdentifier(TlsClientIdentifier.Chrome132)
                            .WithUserAgent("TestClient 1.0")
                            .WithFollowRedirects(true)
                            .WithLibraryPath("D:\\Tools\\TlsClient\\tls-client-windows-64-1.9.1.dll")
                            .WithTimeout(TimeSpan.FromSeconds(10))
                            .Build();

                        var request = new RequestBuilder()
                        .WithUrl("http://example.com")
                        .WithMethod(HttpMethod.Get)
                        .WithHeader("User-Agent", "TestClient 1.0")
                        .Build();

                        

                        for (int j = 0; j < requestsPerThread; j++)
                        {
                            var restResponse = await tlsClient.RequestAsync(request);
                            Console.WriteLine(restResponse.Body);
                        }
                    }));
                }

                await Task.WhenAll(tasks);
            }
        }


    }
}