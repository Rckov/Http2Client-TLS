[![Target Frameworks](https://img.shields.io/badge/Target%20Frameworks-netstandard2.0%20%7C%20net5.0%20%7C%20net6.0%20%7C%20net8.0%20%7C%20net9.0-512BD4)]()
[![Build and Release](https://github.com/Rckov/Http2Client-TLS/actions/workflows/release.yml/badge.svg)](https://github.com/Rckov/Http2Client-TLS/actions/workflows/release.yml)
[![GitHub Release](https://img.shields.io/github/v/release/Rckov/Http2Client-TLS)](https://github.com/Rckov/Http2Client-TLS/releases/latest)

# Http2Client

Http2Client is a fork of [TlsClient.NET](https://github.com/ErenKrt/TlsClient.NET), providing customizable HTTP/2 clients with TLS fingerprinting capabilities. Based on [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client/), it allows you to mimic specific browser fingerprints and control detailed aspects of TLS behavior in your .NET applications.

>**Note: The licensing status of the original TlsClient.NET repository is currently unclear. This fork does not claim ownership of the original code. Use it responsibly, and refer to the original projects for context and licensing information.**

## Installation

```bash
dotnet add package Http2Client
```

### Native Library Dependencies

Http2Client requires the native TLS library from the original [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client) repository. Download the appropriate library for your platform from the [latest release](https://github.com/bogdanfinn/tls-client/releases/latest):

| Operating System | Architecture | Library File | Download Link |
|------------------|--------------|--------------|---------------|
| Windows | x64 (64-bit) | `tls-client-windows-64-1.11.0.dll` | [Download](https://github.com/bogdanfinn/tls-client/releases/download/v1.11.0/tls-client-windows-64-1.11.0.dll) |
| Windows | x86 (32-bit) | `tls-client-windows-32-1.11.0.dll` | [Download](https://github.com/bogdanfinn/tls-client/releases/download/v1.11.0/tls-client-windows-32-1.11.0.dll) |
| Linux | AMD64 (64-bit) | `tls-client-linux-ubuntu-amd64-1.11.0.so` | [Download](https://github.com/bogdanfinn/tls-client/releases/download/v1.11.0/tls-client-linux-ubuntu-amd64-1.11.0.so) |
| Linux | ARM64 | `tls-client-linux-arm64-1.11.0.so` | [Download](https://github.com/bogdanfinn/tls-client/releases/download/v1.11.0/tls-client-linux-arm64-1.11.0.so) |
| Linux | ARMv7 | `tls-client-linux-armv7-1.11.0.so` | [Download](https://github.com/bogdanfinn/tls-client/releases/download/v1.11.0/tls-client-linux-armv7-1.11.0.so) |
| macOS | AMD64 (Intel) | `tls-client-darwin-amd64-1.11.0.dylib` | [Download](https://github.com/bogdanfinn/tls-client/releases/download/v1.11.0/tls-client-darwin-amd64-1.11.0.dylib) |
| macOS | ARM64 (Apple Silicon) | `tls-client-darwin-arm64-1.11.0.dylib` | [Download](https://github.com/bogdanfinn/tls-client/releases/download/v1.11.0/tls-client-darwin-arm64-1.11.0.dylib) |

> **📋 Full Library List**: For additional architectures and XGO builds (including Linux 386, ARM variants, PowerPC, RISC-V, s390x), see the complete list at [v1.11.0 release page](https://github.com/bogdanfinn/tls-client/releases/tag/v1.11.0).

**Installation Instructions:**

1. Download the appropriate library file for your platform from the table above
2. Place the native library in your application's output directory, or
3. Specify the custom path using the `WithLibraryPath()` method in your code

**Example for Windows:**
```csharp
using var client = new HttpClientBuilder()
    .WithLibraryPath("tls-client-windows-64-1.11.0.dll")
    .Build();
```

> **Important**: The native library is required for Http2Client to function properly. Always use the latest version from the [original repository releases](https://github.com/bogdanfinn/tls-client/releases).

> **Note**: When building your application, ensure the correct native library is included in your published application. For cross-platform deployment, you may need to include multiple libraries and select the appropriate one at runtime based on the target platform.

## Basic Usage

Below is a full example that shows how to configure an `Http2Client`, build a request, and send it.

```csharp
using Http2Client;
using Http2Client.Builders;
using Http2Client.Core.Enums;
using Http2Client.Core.Request;

// Create an Http2Client instance using the builder
using var client = new HttpClientBuilder()
    .WithLibraryPath("tls-client-windows-64-1.11.0.dll")
    .WithBrowserType(BrowserType.Chrome133)
    .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithCookies()
    .Build();

// Create a request
var request = new HttpRequest
{
    RequestUrl = "https://httpbin.org/post",
    RequestMethod = "POST",
    RequestBody = "{\"message\": \"Hello from Http2Client\"}",
    Headers = { ["Content-Type"] = "application/json" }
};

// Send the request
var response = client.Send(request);

Console.WriteLine($"Status: {response.Status}");
Console.WriteLine($"Body: {response.Body}");
```

### Session Management

Http2Client provides methods for managing cookies and sessions:

```csharp
// Get cookies for a specific URL
var cookies = client.GetCookies("https://example.com");

// Add cookies to session
var newCookies = new List<ClientCookie> 
{
    new ClientCookie { Name = "session", Value = "abc123" }
};
client.AddCookies("https://example.com", newCookies);

// Clean up session when done
client.DestroySession();
```

## HttpClientBuilder

The `HttpClientBuilder` class provides a fluent interface to create and configure an `Http2Client` instance with custom options such as browser fingerprints, headers, proxy settings, timeouts, and other TLS behaviors.

```csharp
using var client = new HttpClientBuilder()
    .WithBrowserType(BrowserType.Chrome133)
    .WithUserAgent("Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithCookies()
    .Build();
```


### Builder Methods

| Method | Description |
|--------|-------------|
| `WithLibraryPath(string)` | Sets the path to the native TLS library. |
| `WithBrowserType(BrowserType)` | Sets the browser fingerprint to mimic. |
| `WithUserAgent(string)` | Sets the User-Agent header. |
| `WithTimeout(TimeSpan)` | Sets the request timeout. |
| `WithProxy(string, bool)` | Configures proxy URL and rotation setting. |
| `WithInsecureSkipVerify(bool)` | Skips SSL certificate verification. |
| `WithRandomTlsExtensions(bool)` | Randomizes TLS extension order. |
| `WithCookies(bool)` | Enables or disables automatic cookie handling. |
| `WithDebug(bool)` | Enables debug logging. |
| `DisableIPv4(bool)` | Disables IPv4 connections. |
| `DisableIPv6(bool)` | Disables IPv6 connections. |
| `FollowRedirects(bool)` | Enables automatic redirect following. |
| `ForceHttp1(bool)` | Forces HTTP/1.1 instead of HTTP/2. |
| `SetHeader(string, string)` | Sets a default header. |
| `WithHeaders(Dictionary<string, string>)` | Sets multiple default headers. |

### Advanced Example

```csharp
using var client = new HttpClientBuilder()
    .WithLibraryPath("tls-client-windows-64-1.11.0.dll")
    .WithBrowserType(BrowserType.Firefox133)
    .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64)")
    .WithProxy("http://127.0.0.1:8888", isRotating: true)
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithDebug(true)
    .FollowRedirects(true)
    .WithInsecureSkipVerify(false)
    .DisableIPv6(true)
    .SetHeader("X-Custom-Header", "MyValue")
    .WithCookies(true)
    .Build();
```

## HttpRequest

The `HttpRequest` class represents an HTTP request with all necessary configuration. You can create it directly or use the builder pattern.

### HttpRequestBuilder

For more complex request configuration, use the fluent `HttpRequestBuilder`:

```csharp
var request = new HttpRequestBuilder()
    .WithUrl("https://api.example.com/data")
    .WithMethod("POST")
    .WithJsonBody(new { name = "John", age = 30 })
    .WithBrowserType(BrowserType.Chrome133)
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithProxy("http://proxy.example.com:8080")
    .Build();

var response = client.Send(request);
```

### Direct Usage

```csharp
var request = new HttpRequest
{
    RequestUrl = "https://example.com/api/data",
    RequestMethod = "POST",
    RequestBody = "{\"id\": 123, \"name\": \"example\"}",
    Headers = 
    {
        ["Content-Type"] = "application/json",
        ["Authorization"] = "Bearer token"
    },
    BrowserType = BrowserType.Chrome133,
    TimeoutMilliseconds = 30000
};
```

### Key Properties

| Property | Description |
|----------|-------------|
| `RequestUrl` | The target URL (required). |
| `RequestMethod` | HTTP method (GET, POST, etc.). |
| `RequestBody` | Request body content. |
| `Headers` | Dictionary of HTTP headers. |
| `BrowserType` | Browser fingerprint to use. |
| `TimeoutMilliseconds` | Request timeout in milliseconds. |
| `ProxyUrl` | Proxy server URL. |
| `InsecureSkipVerify` | Skip SSL certificate verification. |
| `FollowRedirects` | Follow HTTP redirects automatically. |

## Features

- HTTP/2 and HTTP/1.1 support
- TLS fingerprinting with multiple browser profiles
- Cross-platform (Windows, Linux, macOS)
- Proxy support (HTTP, HTTPS, SOCKS5)
- Cookie management
- Custom headers and SSL configuration

## Examples

### GET Request with Custom Headers

```csharp
using var client = new HttpClientBuilder()
    .WithLibraryPath("tls-client-windows-64-1.11.0.dll")
    .WithBrowserType(BrowserType.Chrome133)
    .Build();

var request = new HttpRequest
{
    RequestUrl = "https://httpbin.org/headers",
    RequestMethod = "GET",
    Headers = 
    {
        ["X-Custom-Header"] = "custom-value",
        ["User-Agent"] = "MyApp/1.0"
    }
};

var response = client.Send(request);
Console.WriteLine(response.Body);
```

### POST Request with JSON Body

```csharp
var request = new HttpRequest
{
    RequestUrl = "https://httpbin.org/post",
    RequestMethod = "POST",
    RequestBody = "{\"name\": \"John\", \"age\": 30}",
    Headers = { ["Content-Type"] = "application/json" }
};

var response = client.Send(request);
```

### Using Proxy

```csharp
using var client = new HttpClientBuilder()
    .WithLibraryPath("tls-client-windows-64-1.11.0.dll")
    .WithBrowserType(BrowserType.Chrome133)
    .WithProxy("http://proxy.example.com:8080")
    .Build();
```

## Target Frameworks

Http2Client supports multiple .NET versions:

- **.NET Standard 2.0** - For maximum compatibility
- **.NET 5.0** - Legacy LTS support
- **.NET 6.0** - LTS support
- **.NET 8.0** - Current LTS
- **.NET 9.0** - Latest version


## License

This project is a fork of [TlsClient.NET](https://github.com/ErenKrt/TlsClient.NET). The original implementation is based on [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client).
