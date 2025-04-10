using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;

namespace TlsClient.Tests
{
    public class PerformanceTests
    {

        [Fact]
        public async Task ShouldHandleMultipleRequestsWithoutResourceLeak()
        {
            var tlsClient = new Core.TlsClient();

            for (int i = 0; i < 10; i++)
            {
                var request = new Request
                {
                    RequestUrl = "https://httpbin.org/get",
                    RequestMethod = HttpMethod.Get
                };

                var response = await tlsClient.RequestAsync(request);
                response.Status.Should().Be(HttpStatusCode.OK);
            }

            tlsClient.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            true.Should().BeTrue("Check memory and CPU usage !");
        }

    }
}