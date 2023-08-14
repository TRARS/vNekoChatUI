namespace Common.WPF.Services
{
    public interface IFlagService
    {
        public bool[] TryUseBingBypassDetection { get; set; }
        public bool[] TryUseBingRandomCookie { get; set; }
        public bool[] TryUseBingAutoSave { get; set; }
        public bool TryUseBingVisualSearch { get; set; }
    }

    public partial class FlagService : IFlagService
    {
        /// <summary>
        /// 控制过越狱检测开关。
        /// </summary>
        public bool[] TryUseBingBypassDetection { get; set; } = new bool[1] { false };

        /// <summary>
        /// 随机cookie。已被人机验证挡在门外。
        /// 0802 最近几天又没有人机验证又可以了。
        /// 0806 又凉了
        /// </summary>
        public bool[] TryUseBingRandomCookie { get; set; } = new bool[1] { false };

        /// <summary>
        /// 控制bing是否启用autosave参数
        /// </summary>
        public bool[] TryUseBingAutoSave { get; set; } = new bool[1] { false };

        /// <summary>
        /// 控制bing上传图片后拿到的地址是否通过user请求传递
        /// </summary>
        public bool TryUseBingVisualSearch { get; set; } = false;
    }
}
