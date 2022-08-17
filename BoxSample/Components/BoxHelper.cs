using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BoxSample.Components
{
    public class BoxHelper
    {
        public static async Task UploadFile(string filePath, string boxFolderId, string accessToken)
        {
            var oldFileId = await CheckFile(filePath, boxFolderId, accessToken);
            var api = "https://upload.box.com/api/2.0/files/content";
            if (!string.IsNullOrEmpty(oldFileId))
                api = $"https://upload.box.com/api/2.0/files/{oldFileId}/content";

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                //Create attributes part
                var fileName = Path.GetFileName(filePath);
                var attributes = $"{{\"name\":\"{fileName}\", \"parent\":{{\"id\":\"{boxFolderId}\"}}}}";

                //Create file part
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"file\"",
                    FileName = $"\"{fileName}\"",
                };

                //Add to multi part
                var multiPart = new MultipartFormDataContent();
                multiPart.Add(new StringContent(attributes), "\"attributes\"");
                multiPart.Add(streamContent);

                //Create client
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");

                //Create http request
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, api);
                httpRequest.Content = multiPart;

                //Execute
                var response = await client.SendAsync(httpRequest);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        public static async Task<string> CheckFile(string filePath, string boxFolderId, string accessToken)
        {
            var api = "https://api.box.com/2.0/files/content";
            //Create attributes part
            var fileName = Path.GetFileName(filePath);
            var attributes = $"{{\"name\":\"{fileName}\", \"parent\":{{\"id\":\"{boxFolderId}\"}}}}";

            //Create client
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Create http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Options, api);
            httpRequest.Content = new StringContent(attributes, Encoding.UTF8, "application/json");

            //Execute
            var response = await client.SendAsync(httpRequest);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            //Get response and return 
            var jsonStr = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var conflictError = JsonConvert.DeserializeObject<ConflictError>(jsonStr);
                return conflictError.ContextInfo.Conflicts.Id;
            }
            return string.Empty;
        }

        class ConflictError
        {
            [JsonProperty("context_info")]
            public ContextInfo ContextInfo { get; set; }
        }

        class ContextInfo
        {
            [JsonProperty("conflicts")]
            public BoxFile Conflicts { get; set; }
        }

        class BoxFile
        {
            public string Id { get; set; }
        }
    }
}
