using Http2Client.Builders;
using Http2Client.Core.Enums;
using Http2Client.Core.Request;

using System;

internal class Program
{
    private const string PATH_LIB = "Native\\tls-client-windows-64-1.11.0.dll";

    private static void Main(string[] args)
    {
        using var client = CreateClientBuilder().Build();

        var request = new HttpRequest()
        {
            RequestUrl = "https://tls.peet.ws/api/all",
            RequestMethod = "GET",
            BrowserType = BrowserType.Chrome133,
        };

        var response = client.Send(request);

        Console.WriteLine(response.Status);
        Console.WriteLine(response.Body);
    }

    private static HttpClientBuilder CreateClientBuilder()
    {
        //const string Proxy = "https://localhost:1777";
        const string UserAgent = "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36";

        return new HttpClientBuilder()
            .WithUserAgent(UserAgent)
            .WithCookies()
            .WithLibraryPath(PATH_LIB)
            .WithRandomTlsExtensions();
    }
}