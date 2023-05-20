using System;
using System.Net;

namespace vNekoChatUI.Base.Helper.Generic
{
    public sealed partial class DefWebProxy
    {
        private static readonly object objlock = new object();
        private static DefWebProxy? _instance;
        public static DefWebProxy Instance
        {
            get
            {
                lock (objlock)
                {
                    if (_instance is null)
                    {
                        _instance = new DefWebProxy();
                    }
                }
                return _instance;
            }
        }
    }

    public sealed partial class DefWebProxy
    {
        private UriBuilder proxy_local = new UriBuilder("127.0.0.1") { Port = 7078 };
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
