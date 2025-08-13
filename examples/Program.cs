using Http2Client.Builders;
using Http2Client.Core.Enums;
using Http2Client.Core.Request;

using System;
using System.Collections.Generic;
using System.Text.Json;

internal class Program
{
    private static void Main()
    {
        Http2Client.Http2Client.Initialize("Native\\tls-client-windows-64-1.11.0.dll");

        try
        {
            BasicGetRequest();
            PostJsonRequest();
            CookieHandling();
            HeadersAndProxy();
            ErrorHandlingAndTimeouts();
        }
        finally
        {
            Http2Client.Http2Client.Cleanup();
        }

        Console.ReadLine();
    }

    private static void BasicGetRequest()
    {
        Console.WriteLine("Basic GET request:");

        using var client = new HttpClientBuilder()
            .WithUserAgent("Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36")
            .WithRandomTlsExtensions()
            .Build();

        var request = new HttpRequest
        {
            RequestUrl = "https://tls.peet.ws/api/all",
            RequestMethod = "GET",
            BrowserType = BrowserType.Chrome133
        };

        var response = client.Send(request);
        Console.WriteLine($"Status: {response?.Status}");
        Console.WriteLine($"Response size: {response?.Body?.Length ?? 0} chars");
        Console.WriteLine();
    }

    private static void PostJsonRequest()
    {
        Console.WriteLine("POST request with JSON:");

        using var client = new HttpClientBuilder()
            .WithUserAgent("Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36")
            .WithHeader("Content-Type", "application/json")
            .WithCookies()
            .Build();

        var jsonData = new { name = "Test User", email = "test@example.com" };
        var jsonString = JsonSerializer.Serialize(jsonData);

        var request = new HttpRequest
        {
            RequestUrl = "https://httpbin.org/post",
            RequestMethod = "POST",
            RequestBody = jsonString,
            BrowserType = BrowserType.Chrome133,
            Headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json"
            }
        };

        var response = client.Send(request);
        Console.WriteLine($"Status: {response?.Status}");
        Console.WriteLine($"Sent data: {jsonString}");
        Console.WriteLine();
    }

    private static void CookieHandling()
    {
        Console.WriteLine("Cookie handling:");

        using var client = new HttpClientBuilder()
            .WithUserAgent("Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36")
            .WithCookies(true)
            .Build();

        var request1 = new HttpRequest
        {
            RequestUrl = "https://httpbin.org/cookies/set/session_id/abc123",
            RequestMethod = "GET",
            BrowserType = BrowserType.Chrome133
        };

        var response1 = client.Send(request1);
        Console.WriteLine($"First request (set cookies): {response1?.Status}");

        var cookies = client.GetCookies("https://httpbin.org");
        Console.WriteLine($"Got cookies: {cookies?.Cookies?.Count ?? 0}");

        var request2 = new HttpRequest
        {
            RequestUrl = "https://httpbin.org/cookies",
            RequestMethod = "GET",
            BrowserType = BrowserType.Chrome133
        };

        var response2 = client.Send(request2);
        Console.WriteLine($"Second request (with cookies): {response2?.Status}");
        Console.WriteLine();
    }

    private static void HeadersAndProxy()
    {
        Console.WriteLine("Custom headers:");

        using var client = new HttpClientBuilder()
            .WithUserAgent("Custom-Agent/1.0")
            .WithHeader("Accept", "application/json")
            .WithHeader("Accept-Language", "en-US,en;q=0.9")
            .WithHeader("Accept-Encoding", "gzip, deflate, br")
            .WithHeaderOrder("User-Agent", "Accept", "Accept-Language", "Accept-Encoding")
            .WithFollowRedirects(true)
            .Build();

        var request = new HttpRequest
        {
            RequestUrl = "https://httpbin.org/headers",
            RequestMethod = "GET",
            BrowserType = BrowserType.Chrome133,
            Headers = new Dictionary<string, string>
            {
                ["X-Custom-Header"] = "CustomValue",
                ["X-Request-ID"] = Guid.NewGuid().ToString()
            }
        };

        var response = client.Send(request);
        Console.WriteLine($"Status: {response?.Status}");
        Console.WriteLine("Custom headers sent");
        Console.WriteLine();
    }

    private static void ErrorHandlingAndTimeouts()
    {
        Console.WriteLine("Error handling and timeouts:");

        using var client = new HttpClientBuilder()
            .WithUserAgent("Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36")
            .WithTimeout(TimeSpan.FromSeconds(10))
            .WithCatchPanics(true)
            .WithInsecureSkipVerify(false)
            .WithDebug(false)
            .Build();

        try
        {
            var request = new HttpRequest
            {
                RequestUrl = "https://httpbin.org/delay/2",
                RequestMethod = "GET",
                BrowserType = BrowserType.Chrome133
            };

            var response = client.Send(request);
            Console.WriteLine($"Delayed request: {response?.Status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }

        try
        {
            var request = new HttpRequest
            {
                RequestUrl = "https://nonexistent-domain-12345.com",
                RequestMethod = "GET",
                BrowserType = BrowserType.Chrome133
            };

            var response = client.Send(request);
            Console.WriteLine($"Nonexistent domain: {response?.Status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expected error: {ex.Message.Split('\n')[0]}");
        }

        Console.WriteLine();
    }
}