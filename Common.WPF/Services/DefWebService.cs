using System;
using System.Net;

namespace Common.WPF.Services
{
    public interface IDefWebService
    {
        public WebProxy GetWebProxy();
    }

    public sealed partial class DefWebService : IDefWebService
    {
        private UriBuilder proxy_local = new UriBuilder("127.0.0.1") { Port = 7890 };
        private string? username = null;
        private string? password = null;

        public WebProxy GetWebProxy()
        {
            return new WebProxy
            {
                Address = proxy_local.Uri,
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password),
            };
        }
    }
}
