using System.IO.Compression;
using System.Text;

namespace Common.WebWpfCommon
{
    public partial class TextCompressor
    {
        private static readonly Lazy<TextCompressor> lazyObject = new(() => new TextCompressor());
        public static TextCompressor Instance => lazyObject.Value;
    }

    public partial class TextCompressor
    {
        /// <summary>
        /// 压缩
        /// </summary>
        public byte[] CompressText(string text)
        {
            byte[] rawData = Encoding.UTF8.GetBytes(text);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                {
                    deflateStream.Write(rawData, 0, rawData.Length);
                }

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        public string DecompressData(byte[] compressedData)
        {
            using (MemoryStream memoryStream = new MemoryStream(compressedData))
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
                {
                    using (MemoryStream decompressedStream = new MemoryStream())
                    {
                        deflateStream.CopyTo(decompressedStream);
                        byte[] decompressedBytes = decompressedStream.ToArray();
                        return Encoding.UTF8.GetString(decompressedBytes);
                    }
                }
            }
        }
    }
}
