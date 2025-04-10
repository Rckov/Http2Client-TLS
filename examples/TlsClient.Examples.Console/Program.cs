// See https://aka.ms/new-console-template for more information
using TlsClient.Core;
using TlsClient.Core.Models;

Console.WriteLine("Hello, World!");


var clientOptions = new TlsClientOptions(TlsClientIdentifier.Chrome132, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0")
{
    ProxyURL = "",
    FollowRedirects = true,
};
var client = new TlsClient.Core.TlsClient(clientOptions);
var clientWithoutOptions= new TlsClient.Core.TlsClient();
var cc= await client.RequestAsync(new TlsClientRequest()
{
    RequestUrl= "https://www.guzel.net.tr",
    RequestMethod= TlsClientMethod.GET,
    WithDebug = true,
});
client.Dispose();
var gg = "a";

using(var client2 = new TlsClient.Core.TlsClient())
{
    var cc2 = await client2.RequestAsync(new TlsClientRequest()
    {
        RequestUrl = "https://www.guzel.net.tr",
        RequestMethod = TlsClientMethod.GET,
        WithDebug = true,
    });
    var gg2 = "a";
}