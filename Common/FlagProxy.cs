namespace Common
{
    //单例
    public partial class FlagProxy
    {
        private static readonly object objlock = new object();
        private static FlagProxy? _instance;
        public static FlagProxy Instance
        {
            get
            {
                lock (objlock)
                {
                    if (_instance is null)
                    {
                        _instance = new FlagProxy();
                    }
                }
                return _instance;
            }
        }
    }

    public partial class FlagProxy
    {
        //
        public bool TryBypassDetection { get; set; }

        //
        public bool TryBypassLimit { get; set; } = true;
    }
}
