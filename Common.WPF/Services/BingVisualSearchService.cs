using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.WPF.Services
{
    public interface IBingVisualSearchService
    {
        public string? ImageUrl { get; }
        Task<Tuple<bool, string?>> UploadImageAsync(string filename);
    }

    //拉一下服务
    public partial class BingVisualSearchService
    {
        IDefWebService _defWebService = ServiceHost.Instance.GetService<IDefWebService>();
    }

    public partial class BingVisualSearchService : IBingVisualSearchService
    {
        private HttpClient _client;

        public BingVisualSearchService()
        {
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = _defWebService.GetWebProxy()
            };

            _client = new HttpClient(httpClientHandler);
        }

    }

    public partial class BingVisualSearchService
    {
        class ImageUrlResponse
        {
            public string blobId { get; set; }
            public string processedBlobId { get; set; }
        }

        private Dictionary<string, string> ImageUrlList = new();//维护一份内部列表，省得每次都重新上传

        private string? image_url = null;
        public string? ImageUrl => image_url;

        public async Task<Tuple<bool, string?>> UploadImageAsync(string filename)
        {
            bool result_flag = false;
            string? result_text = null;

            try
            {
                var url = "https://www.bing.com/images/kblob";

                var payload = new
                {
                    imageInfo = new { },
                    knowledgeRequest = new
                    {
                        invokedSkills = new[] { "ImageById" },
                        subscriptionId = "Bing.Chat.Multimodal",
                        invokedSkillsRequestData = new { enableFaceBlur = false },
                        convoData = new
                        {
                            convoid = "",
                            convotone = "Creative"
                        }
                    }
                };

                var content = new MultipartFormDataContent();
                var jsonString = JsonSerializer.Serialize(payload);
                content.Add(new StringContent(jsonString, Encoding.UTF8, "application/json"));

                byte[] fileData = File.ReadAllBytes(filename);
                var imageBase64 = Convert.ToBase64String(fileData);

                //↓Base64字符串太长不适合做字典key，压一下
                byte[] hashBytes = ComputeSHA256Hash(fileData);
                string hashHexString = ByteArrayToHexString(hashBytes);

                if (ImageUrlList.ContainsKey(hashHexString) is false)
                {
                    ImageUrlList.Add(hashHexString, "");
                }
                else
                {
                    return Tuple.Create(true, ImageUrlList[hashHexString]);
                }
                //↑

                content.Add(new StringContent(imageBase64), "imageBase64");

                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Headers.Add("Referer", "https://www.bing.com/search?q=Bing+AI&showconv=1&FORM=hpcodx");
                    request.Content = content;

                    using (var response = await _client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        var responseJson = await response.Content.ReadAsStringAsync();
                        LogProxy.Instance.Print($"responseJson = \n{responseJson}");
                        //return JsonSerializer.Deserialize<dynamic>(responseJson)["blobId"];
                        //{"blobId":"r2wJNqon6uAFnQ","processedBlobId":""}
                        //https://www.bing.com/images/blob?bcid=r2wJNqon6uAFnQ

                        string? blobId = JsonSerializer.Deserialize<ImageUrlResponse>(responseJson)?.blobId;

                        if (string.IsNullOrWhiteSpace(blobId))
                        {
                            this.image_url = null;
                            result_flag = false;
                            result_text = this.image_url;

                            ImageUrlList.Remove(hashHexString);
                            LogProxy.Instance.Print($"blobId为空值");
                        }
                        else
                        {
                            this.image_url = $"https://www.bing.com/images/blob?bcid={blobId}";
                            result_flag = true;
                            result_text = this.image_url;

                            ImageUrlList[hashHexString] = this.image_url;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.image_url = null;
                result_flag = false;
                result_text = this.image_url;

                LogProxy.Instance.Print($"uploadImage error = {ex.Message}");
            }

            return Tuple.Create(result_flag, result_text);
        }

    }

    public partial class BingVisualSearchService
    {
        private byte[] ComputeSHA256Hash(byte[] inputBytes)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(inputBytes);
            }
        }
        private string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder hexBuilder = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hexBuilder.AppendFormat("{0:x2}", b);
            }
            return hexBuilder.ToString();
        }
    }
}
