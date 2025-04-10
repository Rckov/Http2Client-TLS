// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using TlsClient.Core;
using TlsClient.Core.Helpers.Builders;
using TlsClient.Core.Models;
using TlsClient.Core.Models.Entities;
using TlsClient.Core.Models.Requests;


[DllImport("kernel32.dll")]
static extern IntPtr GetModuleHandle(string lpModuleName);

var clientOptions = new TlsClientOptions(TlsClientIdentifier.Chrome132, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0")
{
    ProxyURL = "",
    FollowRedirects = true,
};
var client = new TlsClient.Core.TlsClient(clientOptions);
var clientWithoutOptions= new TlsClient.Core.TlsClient();
var cc= await client.RequestAsync(new Request()
{
    RequestUrl= "https://www.guzel.net.tr",
    RequestMethod= HttpMethod.Get,
});

var client4 = new ClientBuilder()
    .WithIdentifier(TlsClientIdentifier.Chrome132)
    .WithUserAgent("g0")
    .WithFollowRedirects(true)
    .WithSkipTlsVerification(true)
    .AddHeader("a", "b")
    .Build();

var request = new RequestBuilder()
    .WithUrl("https://httpbin.org/user-agent")
    .WithMethod(HttpMethod.Get)
    //.WithByteResponse()
    .Build();

var zz = await client4.RequestAsync(request);

var hh = await client.RequestAsync(request);

 var ll= await client.GetCookiesAsync("https://www.guzel.net.tr");
await client.AddCookiesAsync("https://www.guzel.net.tr", new List<TlsClientCookie>()
{
    new TlsClientCookie("a","b")
});

//await client.DestroyAsync();
//await client.DestroyAllAsync();
client.Dispose();

ll = await client.GetCookiesAsync("https://www.guzel.net.tr");

var gg = "a";

using(var client2 = new TlsClient.Core.TlsClient())
{
    var cc2 = await client2.RequestAsync(new Request()
    {
        RequestUrl = "https://www.guzel.net.tr",
        RequestMethod = HttpMethod.Get,
        WithDebug = true,
    });
    var gg2 = "a";
}