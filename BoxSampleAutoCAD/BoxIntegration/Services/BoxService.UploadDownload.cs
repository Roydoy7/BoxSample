using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BoxSampleAutoCAD.BoxIntegration.Services
{
    public partial class BoxService : IBoxService
    {
        #region Upload
        public async Task UploadFile(string filePath, string boxFolderId, string accessToken)
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
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");

                //Create http request
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, api);
                httpRequest.Content = multiPart;

                //Execute
                var response = await client.SendAsync(httpRequest);
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<BoxClientError>(jsonStr);
                    if (error != null)
                        throw new Exception(error?.Message);
                    else
                        throw new Exception("Unknown error.");
                }
            }
        }

        private async Task<string> CheckFile(string filePath, string boxFolderId, string accessToken)
        {
            var api = "https://api.box.com/2.0/files/content";
            //Create attributes part
            var fileName = Path.GetFileName(filePath);
            var attributes = $"{{\"name\":\"{fileName}\", \"parent\":{{\"id\":\"{boxFolderId}\"}}}}";

            //Create client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Create http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Options, api);
            httpRequest.Content = new StringContent(attributes, Encoding.UTF8, "application/json");

            //Execute
            var response = await client.SendAsync(httpRequest);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Unauthorized.");
            }

            //Get response and return 
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var conflictError = JsonConvert.DeserializeObject<ConflictError>(jsonStr);
                return conflictError.ContextInfo.Conflicts.Id;
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                return string.Empty;
            }
            else
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<BoxClientError>(jsonStr);
                if (error != null)
                    throw new Exception(error?.Message);
                else
                    throw new Exception("Unknown error.");
            }
        }
        #endregion

        #region Download
        public async Task<string> DownloadFile(string folderPath, string boxFileId, string accessToken)
        {
            var fileName = await GetFileName(boxFileId, accessToken);
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("Obtain file information fail");

            var filePath = Path.Combine(folderPath, fileName);

            var api = $"https://api.box.com/2.0/files/{boxFileId}/content";
            //Create client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Create http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, api);

            //Execute
            var response = await client.SendAsync(httpRequest);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                using var file = File.OpenWrite(filePath);
                stream.CopyTo(file);
                return filePath;
            }
            else if (response.StatusCode == HttpStatusCode.Accepted)
            {
                throw new Exception("The required file is not ready for download.");
            }
            else
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<BoxClientError>(jsonStr);
                if (error != null)
                    throw new Exception(error?.Message);
                else
                    throw new Exception("Unknown error.");
            }

        }

        private async Task<string> GetFileName(string boxFileId, string accessToken)
        {
            var api = $"https://api.box.com/2.0/files/{boxFileId}";
            //Create client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Create http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, api);

            //Execute 
            var response = await client.SendAsync(httpRequest);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var fileInfo = JsonConvert.DeserializeObject<BoxFile>(jsonStr);
                return fileInfo.Name;
            }
            else
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<BoxClientError>(jsonStr);
                if (error != null)
                    throw new Exception(error?.Message);
                else
                    throw new Exception("Unknown error.");
            }
        }
        #endregion

        public string GetFolderId(string boxFolderUrl)
        {
            if (!IsValidUrl(boxFolderUrl))
                return string.Empty;
            if (!boxFolderUrl.Contains("box.com"))
                return string.Empty;
            if (!boxFolderUrl.Contains("folder"))
                return string.Empty;

            var split = boxFolderUrl
                .Replace("https://", string.Empty)
                .Replace("http://", string.Empty)
                .Split('/');
            if (split.Length == 3)
                return split[2];
            return string.Empty;
        }

        public string GetFileId(string boxFileUrl)
        {
            if (!IsValidUrl(boxFileUrl))
                return string.Empty;
            if (!boxFileUrl.Contains("box.com"))
                return string.Empty;
            if (!boxFileUrl.Contains("file"))
                return string.Empty;
            var split = boxFileUrl
                .Replace("https://", string.Empty)
                .Replace("http://", string.Empty)
                .Split('/');
            if (split.Length == 3)
            {
                if (split[2].Contains("?s="))
                {
                    var fileId = split[2].Substring(0, split[2].IndexOf("?s="));
                    return fileId;
                }
                else
                {
                    return split[2];
                }
            }

            return string.Empty;
        }

        private bool IsValidUrl(string url)
        {
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(url);
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

    }
}
