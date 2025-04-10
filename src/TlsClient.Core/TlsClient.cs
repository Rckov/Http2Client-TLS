using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TlsClient.Core.Helpers;
using TlsClient.Core.Models;


namespace TlsClient.Core
{
    public class TlsClient : IDisposable
    {
        private IntPtr LoadedLibrary { get; set; } = IntPtr.Zero;
        private TlsClientOptions Options { get; set; }
        public TlsClient(TlsClientOptions options)
        {
            Options = options ?? throw new Exception("Options is null");
            LoadedLibrary = NativeLoader.LoadNativeAssembly();
        }

        public TlsClient() : this(new TlsClientOptions(TlsClientIdentifier.Chrome132, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 OPR/117.0.0.0")) { }
        public async Task<TlsClientResponse> RequestAsync(TlsClientRequest request)
        {
            request.SessionId = request.SessionId ?? Options.SessionID;
            request.TlsClientIdentifier = request.TlsClientIdentifier ?? Options.TlsClientIdentifier;
            request.TimeoutMilliseconds = request.TimeoutMilliseconds ?? (int)Options.Timeout.TotalMilliseconds;
            request.ProxyUrl = request.ProxyUrl ?? Options.ProxyURL;
            request.IsRotatingProxy = request.IsRotatingProxy ?? Options.IsRotatingProxy;
            request.FollowRedirects = request.FollowRedirects ?? Options.FollowRedirects;
            request.InsecureSkipVerify = request.InsecureSkipVerify ?? Options.InsecureSkipVerify;
            request.DisableIPV4 = request.DisableIPV4 ?? Options.DisableIPV4;
            request.DisableIPV6 = request.DisableIPV6 ?? Options.DisableIPV6;
            request.WithDebug = request.WithDebug ?? Options.WithDebug;
            request.WithDefaultCookieJar = request.WithDefaultCookieJar ?? Options.WithDefaultCookieJar;
            request.WithoutCookieJar = request.WithoutCookieJar ?? Options.WithoutCookieJar;
            // Default headers prop is not working
            request.DefaultHeaders = request.DefaultHeaders ?? Options.DefaultHeaders;
            request.RequestUrl = request.RequestUrl ?? throw new ArgumentNullException(nameof(request.RequestUrl));
            request.Headers = request.Headers ?? new Dictionary<string, string>();

            // Default headers prop is not working
            foreach (var header in request.DefaultHeaders)
            {
                if (!request.Headers.ContainsKey(header.Key))
                {
                    request.Headers.Add(header.Key, header.Value[0]);
                }
            }

            var rawResponse= await TlsClientAsyncWrapper.RequestAsync(RequestHelpers.Prepare(request));
            var response = JsonConvert.DeserializeObject<TlsClientResponse>(rawResponse);
            if (response == null)
            {
                throw new Exception("Response is null, can't convert object from json.");
            }
            // Need to free memory, because the native library allocates memory for the response
            await TlsClientAsyncWrapper.FreeMemoryAsync(response.Id);
            return response;
        }

        public void Dispose()
        {
            if(LoadedLibrary == IntPtr.Zero)
            {
                return;
            }

            NativeLoader.FreeNativeAssembly(LoadedLibrary);
            LoadedLibrary = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }
}
