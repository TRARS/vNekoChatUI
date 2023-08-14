using System;
using System.Net;

namespace vNekoChatUI.Character.Base
{
    public sealed partial class GenericWebProxy
    {
        private static readonly object objlock = new object();
        private static GenericWebProxy? _instance;
        public static GenericWebProxy Instance
        {
            get
            {
                lock (objlock)
                {
                    if (_instance is null)
                    {
                        _instance = new GenericWebProxy();
                    }
                }
                return _instance;
            }
        }
    }

    public sealed partial class GenericWebProxy
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
