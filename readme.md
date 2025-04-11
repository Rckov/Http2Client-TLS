# TlsClient.NET

`TlsClient.NET` is a .NET library implementation of [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client/), providing customizable HTTP clients with advanced TLS (Transport Layer Security) configurations. It allows you to mimic specific browser fingerprints and control detailed aspects of TLS behavior in your .NET applications.


## Table of Contents

- [Core Usage](#core-usage)
  - [Installation](#installation)
  - [Basic Usage](#basic-usage)
  - [TlsClientBuilder](#tlsclientbuilder)
  - [RequestBuilder](#requestbuilder)
- [Important Classes](#important-classes)
  - [Request](https://github.com/ErenKrt/TlsClient.NET/blob/master/src/TlsClient.Core/Models/Requests/Request.cs)
  - [Response](https://github.com/ErenKrt/TlsClient.NET/blob/master/src/TlsClient.Core/Models/Responses/Response.cs)
  - [TlsClient](https://github.com/ErenKrt/TlsClient.NET/blob/master/src/TlsClient.Core/TlsClient.cs)
- [Integrations](#integrations)
  - [TlsClient.RestSharp](#tlsclientrestsharp)
  - [TlsClient.HttpClient](#tlsclienthttpclient)
- [Contributing](#contributing)

## Core Usage

### Installation

You can install the package via NuGet:

```bash
dotnet add package TlsClient.Core
```

Or add the project as a direct reference:

```bash
dotnet add reference ../TlsClient.Core/TlsClient.Core.csproj
```

### Basic Usage

Below is a full example that shows how to configure a `TlsClient`, build a request using `RequestBuilder`, and send the request.

```csharp
// Create a TlsClient instance
var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("TestClient 1.0")
    .WithFollowRedirects(true)
    .WithTimeout(TimeSpan.FromSeconds(15))
    .Build();

// Create a Request
var request = new RequestBuilder()
    .WithUrl("https://httpbin.org/post")
    .WithMethod(HttpMethod.Post)
    .WithHeader("Content-Type", "application/json")
    .WithBody(new { message = "Hello from TlsClient" })
    .Build();

// Send the request
var response = await tlsClient.RequestAsync(request);
```

## TlsClientBuilder
The `TlsClientBuilder` class is used to create and configure a `TlsClient` instance with custom options such as headers, proxy settings, timeouts, and other TLS behaviors. It provides a fluent interface to simplify the client setup process.
Alternatively, instead of using `TlsClientBuilder`, you can directly create a [`TlsClient`](https://github.com/ErenKrt/TlsClient.NET/blob/master/src/TlsClient.Core/TlsClient.cs)

```csharp
var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("TestClient 1.0")
    .WithFollowRedirects(false)
    .Build();
```

### Supported Client Identifiers

TlsClient supports multiple browser fingerprints through the `TlsClientIdentifier`.

[List Of Identifiers](https://github.com/ErenKrt/TlsClient.NET/blob/master/src/TlsClient.Core/Models/Entities/TlsClientIdentifier.cs)

### Builder Methods

| Method | Description |
|--------|-------------|
| `WithIdentifier(TlsClientIdentifier)` | Sets the TLS fingerprint (e.g. Chrome132). |
| `WithUserAgent(string)` | Adds a User-Agent header. |
| `WithProxyUrl(string proxyUrl, bool isRotating = false)` | Configures a proxy URL. |
| `WithTimeout(TimeSpan)` | Sets the request timeout. |
| `WithDebug(bool enabled = true)` | Enables or disables debug mode. |
| `WithFollowRedirects(bool enabled = true)` | Enables or disables redirect handling. |
| `WithSkipTlsVerification(bool skip = true)` | Skips TLS certificate verification. |
| `DisableIPV4(bool disabled = true)` | Disables IPv4. |
| `DisableIPV6(bool disabled = true)` | Disables IPv6. |
| `WithDefaultCookieJar(bool enabled = true)` | Enables the default cookie jar. |
| `WithoutCookieJar(bool enabled = true)` | Disables all cookie management. |
| `AddHeader(string key, string value)` | Adds a custom HTTP header. |

### Advanced Example

```csharp
var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64)")
    .WithProxyUrl("http://127.0.0.1:8888", isRotating: true)
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithDebug(true)
    .WithFollowRedirects(true)
    .WithSkipTlsVerification(false)
    .DisableIPV6(true)
    .AddHeader("X-Custom-Header", "MyValue")
    .Build();
```

For more comprehensive examples of TlsClient, please check the [testing directory](https://github.com/ErenKrt/TlsClient.NET/tree/master/tests/TlsClient.Core.Tests) in the GitHub repository.

---

## RequestBuilder

The `RequestBuilder` class is used to create a detailed HTTP request that can be executed by `TlsClient`. Alternatively, instead of using the `RequestBuilder`, you can directly instantiate the [`Request`](https://github.com/ErenKrt/TlsClient.NET/blob/master/src/TlsClient.Core/Models/Requests/Request.cs) class and use it with `RequestAsync`.

### Usage

```csharp
var request = new RequestBuilder()
    .WithUrl("https://example.com/api/data")
    .WithMethod(HttpMethod.Post)
    .WithHeader("Authorization", "Bearer token")
    .WithBody(new { id = 123, name = "example" })
    .WithByteResponse()
    .Build();
```

### Builder Methods

| Method | Description |
|--------|-------------|
| `WithUrl(string url)` | Sets the request URL. |
| `WithMethod(HttpMethod)` | Sets the HTTP method (GET, POST, etc.). |
| `WithHeader(string key, string value)` | Adds a single header. |
| `WithHeaders(Dictionary<string, string>)` | Adds multiple headers. |
| `WithBody(string body)` | Sets the request body as a string. |
| `WithBody(byte[] bytes)` | Sets the request body as raw bytes. |
| `WithBody(object data)` | Serializes and sets the request body from an object. |
| `WithByteRequest()` | Marks the request to be sent as raw byte data. |
| `WithByteResponse()` | Marks the response to be read as bytes. |
| `Build()` | Finalizes and returns the `Request` object. |

## Integrations

### TlsClient.RestSharp

`TlsClient` integrates with RestSharp to provide a familiar API for making HTTP requests while leveraging `TlsClient`'s advanced TLS capabilities. This integration allows you to use RestSharp's convenient request/response model while maintaining control over TLS fingerprinting and other security features.

#### Installation

```bash
dotnet add package TlsClient.RestSharp
```

#### Basic Usage

The `TlsRestClientBuilder` allows you to create a RestClient that uses `TlsClient` under the hood:

```csharp
// Create a TlsClient instance
var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("TestClient 1.0")
    .WithFollowRedirects(true)
    .WithTimeout(TimeSpan.FromSeconds(15))
    .Build();

// Create a RestClient with TlsClient integration
var restClient = new TlsRestClientBuilder()
    .WithBaseUrl("https://httpbin.org")
    .WithTlsClient(tlsClient)
    .Build();

// Use standard RestSharp request objects
var request = new RestRequest("/get", Method.Get);
request.AddParameter("param1", "value1");

// Execute like a normal RestSharp client
var response = await restClient.ExecuteAsync(request);
```

#### Builder Methods

| Method | Description |
|--------|-------------|
| `WithBaseUrl(string url)` | Sets the base URL for all requests. |
| `WithTlsClient(TlsClient)` | Configures the RestClient to use the specified TlsClient instance. |
| `Build()` | Creates and returns the configured RestClient. |

For more comprehensive examples of TlsClient.RestSharp in action, including timeout handling, JSON serialization, and more advanced scenarios, please check the [testing directory](https://github.com/ErenKrt/TlsClient.NET/tree/master/tests/TlsClient.RestSharp.Tests) in the GitHub repository.

### TlsClient.HttpClient

TlsClient can be integrated with the standard .NET HttpClient through a custom HttpClientHandler implementation. This allows you to leverage the advanced TLS capabilities of TlsClient while using the familiar HttpClient API.

#### Installation

```bash
dotnet add package TlsClient.HttpClient
```

#### Basic Usage

The `TlsClientHandler` class acts as a bridge between TlsClient and HttpClient:

```csharp
// Create a TlsClient instance
var tlsClient = new TlsClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("TestClient 1.0")
    .WithFollowRedirects(true)
    .WithTimeout(TimeSpan.FromSeconds(15))
    .Build();

// Create a handler using the TlsClient
var handler = new TlsClientHandler(tlsClient);

// Create standard HttpClient with the TlsClient handler
var httpClient = new HttpClient(handler);

// Use HttpClient as usual
var response = await httpClient.GetAsync("https://httpbin.org/get");
var content = await response.Content.ReadAsStringAsync();
```

## Contributing

Contributions to TlsClient.NET are welcome and appreciated! Since this is a .NET implementation of the [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client/) library:

- **For .NET Specific Contributions**: Submit issues and pull requests to this repository for:
  - .NET implementation improvements
  - C# code quality enhancements
  - .NET-specific feature additions
  - Documentation for .NET users
  - Integration with other .NET libraries/frameworks

- **For Core TLS Functionality Issues**: For contributions related to the underlying TLS fingerprinting functionality, browser profiles, or core protocol handling, please contribute to the [original bogdanfinn/tls-client repository](https://github.com/bogdanfinn/tls-client/).

When submitting a pull request to this repository, please:
1. Fork the repository and create a new branch for your feature
2. Add tests for new functionality
3. Ensure all tests pass
4. Update documentation as needed
5. Follow the existing code style

Feel free to reach out if you have any questions about contributing. Together we can make TlsClient.NET even better!

## Copyright and Contact

© 2025 TlsClient.NET

Maintained by:
- GitHub: [ErenKrt](https://github.com/ErenKrt)
- Instagram: [ep.eren](https://instagram.com/ep.eren)

This project is a .NET implementation of the [bogdanfinn/tls-client](https://github.com/bogdanfinn/tls-client/) library.