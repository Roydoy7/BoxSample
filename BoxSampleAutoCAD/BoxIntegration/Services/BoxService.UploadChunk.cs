using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BoxSampleAutoCAD.BoxIntegration.Services
{
    public partial class BoxService
    {
        public async Task UploadChunk(string filePath, string boxFolderId, string accessToken, Action<string> updateStatus = null)
        {
            //Check if old file exists
            var fileId = await CheckFile(filePath, boxFolderId, accessToken);

            //Get file information
            var fi = new FileInfo(filePath);
            var fileName = Path.GetFileName(filePath);
            var fileSize = fi.Length;

            //Check file size
            if (fileSize == 0)
                throw new Exception("File size can not be 0.");

            //Create upload session
            BoxUploadSession session = null;
            if (string.IsNullOrEmpty(fileId))
                session = await UploadWithSession(fileName, fileSize, boxFolderId, accessToken);
            else
                session = await UploadWithNewVersionSession(fileName, fileId, fileSize, boxFolderId, accessToken);
            if (session == null)
                throw new Exception("Can not create upload session.");

            var partSize = session.PartSize;
            long.TryParse(partSize, out long partSizeLong);
            var numberOfParts = GetNumberOfParts(fileSize, partSizeLong);

            //Upload parts
            using var fileStream = new FileStream(filePath, FileMode.Open);
            var partInfos = await UploadPartsInSessionAsync(new Uri(session.SessionEndpoints.UploadPart), numberOfParts, partSizeLong, fileStream, fileSize, accessToken, updateStatus);

            var boxSessionParts = new BoxSessionParts
            {
                Parts = partInfos.Select(x => x.Part).ToList(),
            };

            //Commit
            var sha = GetSha1Hash(fileStream);
            await CommitSessionAsync(new Uri(session.SessionEndpoints.Commit), sha, boxSessionParts, accessToken);
        }

        private async Task<BoxUploadSession> UploadWithSession(string fileName, long fileSize, string boxFolderId, string accessToken)
        {
            var api = $"https://upload.box.com/api/2.0/files/upload_sessions";

            var sessionRequest = new BoxFileUploadSessionRequest
            {
                FolderId = boxFolderId,
                FileSize = fileSize,
                FileName = fileName,
            };

            //Create client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            //Create http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, api);
            httpRequest.Content = new StringContent(JsonConvert.SerializeObject(sessionRequest), Encoding.UTF8, "application/json");

            //Execute
            var response = await client.SendAsync(httpRequest);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var uploadSession = JsonConvert.DeserializeObject<BoxUploadSession>(jsonStr);
                return uploadSession;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Unauthorized.");
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("Storage limit is reached.");
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new Exception("File with the same name exists.");
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

        private async Task<BoxUploadSession> UploadWithNewVersionSession(string fileName, string fileId, long fileSize, string boxFolderId, string accessToken)
        {
            var api = $"https://upload.box.com/api/2.0/files/{fileId}/upload_sessions";

            var sessionRequest = new BoxFileUploadSessionRequest
            {
                FileSize = fileSize,
                FileName = fileName,
            };

            //Create client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            //Create http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, api);
            httpRequest.Content = new StringContent(JsonConvert.SerializeObject(sessionRequest), Encoding.UTF8, "application/json");

            //Execute
            var response = await client.SendAsync(httpRequest);

            //OK
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var uploadSession = JsonConvert.DeserializeObject<BoxUploadSession>(jsonStr);
                return uploadSession;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Unauthorized.");
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new Exception("File with the same name exists.");
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

        private async Task<IEnumerable<BoxSessionPart>> UploadPartsInSessionAsync(Uri uploadPartsUri, int numberOfParts, long partSize, Stream stream,
            long fileSize, string accessToken, Action<string> updateStatus = null)
        {
            var list = new List<BoxSessionPart>();

            for (var i = 0; i < numberOfParts; i++)
            {
                // Split file as per part size
                var partOffset = partSize * i;
                var partFileStream = GetFilePart(stream, partSize, partOffset);
                var sha = GetSha1Hash(partFileStream);
                partFileStream.Position = 0;

                //Create client
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                //Create http request
                var httpRequest = new HttpRequestMessage(HttpMethod.Put, uploadPartsUri);
                httpRequest.Headers.TryAddWithoutValidation("Digest", "sha=" + sha);
                httpRequest.Headers.TryAddWithoutValidation("Content-Type", "application/octet-stream");
                httpRequest.Content = new ReusableContent(partFileStream);
                httpRequest.Content.Headers.Add("Content-Range", "bytes " + partOffset + "-" + (partOffset + partFileStream.Length - 1) + "/" + fileSize);

                //Execute
                var response = await client.SendAsync(httpRequest);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    var partInfo = JsonConvert.DeserializeObject<BoxSessionPart>(jsonStr);
                    list.Add(partInfo);
                    updateStatus?.Invoke($"{i + 1}/{numberOfParts}");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Unauthorized.");
                }
                //Response code is 409, 412, 416, default
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

            return list;
        }

        private async Task<bool> CommitSessionAsync(Uri commitSessionUrl, string sha, BoxSessionParts sessionPartsInfo, string accessToken)
        {
            var retry = 5;
            var rand = new Random();
            var content = JsonConvert.SerializeObject(sessionPartsInfo);

            while (retry > 0)
            {
                //Create client
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                //Create http request
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, commitSessionUrl);
                httpRequest.Headers.TryAddWithoutValidation("Digest", "sha=" + sha);
                httpRequest.Content = new StringContent(content, Encoding.UTF8, "application/json");

                //Execute
                var response = await client.SendAsync(httpRequest);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return true;
                }
                //The server is still processing this upload session
                else if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    //Wait for a random time period, and retry
                    await Task.Delay(rand.Next(2000));
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Unauthorized.");
                }
                //Response code 409,412,other
                else
                {
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<BoxClientError>(jsonStr);
                    throw new Exception(error?.Message);
                }
                retry--;
            }

            return false;
        }

        private int GetNumberOfParts(long totalSize, long partSize)
        {
            if (partSize == 0)
            {
                throw new Exception("Part Size cannot be 0");
            }

            var numberOfParts = Convert.ToInt32(totalSize / partSize);
            if (totalSize % partSize != 0)
            {
                numberOfParts++;
            }
            return numberOfParts;
        }

        private Stream GetFilePart(Stream stream, long partSize, long partOffset)
        {
            // Default the buffer size to 4K.
            const int BufferSize = 4096;

            var buffer = new byte[BufferSize];
            stream.Position = partOffset;
            var partStream = new MemoryStream();
            int bytesRead;
            do
            {
                bytesRead = stream.Read(buffer, 0, 4096);
                if (bytesRead > 0)
                {
                    long bytesToWrite = bytesRead;
                    var shouldBreak = false;
                    if (partStream.Length + bytesRead >= partSize)
                    {
                        bytesToWrite = partSize - partStream.Length;
                        shouldBreak = true;
                    }

                    partStream.Write(buffer, 0, Convert.ToInt32(bytesToWrite));

                    if (shouldBreak)
                    {
                        break;
                    }
                }
            } while (bytesRead > 0);

            return partStream;
        }

        private string GetSha1Hash(Stream stream)
        {
            stream.Position = 0;

            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(stream);

                return Convert.ToBase64String(hash);
            }
        }
    }

    #region Chunk Upload

    public class BoxFileUploadSessionRequest
    {
        [JsonProperty(PropertyName = "folder_id")]
        public string FolderId { get; set; }
        [JsonProperty(PropertyName = "file_size")]
        public long FileSize { get; set; }
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }
    }

    public class BoxUploadSession
    {
        public string Id { get; set; }
        public string Type { get; set; }
        [JsonProperty(PropertyName = "session_expires_at")]
        public string SessionExpiresAt { get; set; }
        [JsonProperty(PropertyName = "part_size")]
        public string PartSize { get; set; }
        [JsonProperty(PropertyName = "session_endpoints")]
        public BoxSessionEndpoint SessionEndpoints { get; set; }
        [JsonProperty(PropertyName = "total_parts")]
        public int TotalParts { get; set; }
        [JsonProperty(PropertyName = "num_parts_processed")]
        public int NumPartsProcessed { get; set; }
    }

    public class BoxSessionEndpoint
    {
        [JsonProperty(PropertyName = "list_parts")]
        public string ListParts { get; set; }
        [JsonProperty(PropertyName = "commit")]
        public string Commit { get; set; }
        [JsonProperty(PropertyName = "log_event")]
        public string LogEvent { get; set; }
        [JsonProperty(PropertyName = "upload_part")]
        public string UploadPart { get; set; }
        [JsonProperty(PropertyName = "abort")]
        public string Abort { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

    internal class ReusableContent : HttpContent
    {
        private Stream _innerContent;
        private readonly long _contentLength;

        public ReusableContent(Stream stream)
        {
            _innerContent = stream;
            _contentLength = stream.Length;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            _innerContent.Position = 0;
            await _innerContent.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _contentLength;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            // Don't call dispose on stream content as it will close the base stream.
            _innerContent = null;
            base.Dispose(disposing);
        }
    }
    #endregion

    #region Commit Model
    public class BoxSessionParts
    {
        /// <summary>
        /// List of session parts uploaded.
        /// </summary>
        [JsonProperty(PropertyName = "parts")]
        public virtual IEnumerable<BoxSessionPartInfo> Parts { get; set; }
    }

    public class BoxSessionPart
    {
        [JsonProperty(PropertyName = "part")]
        public virtual BoxSessionPartInfo Part { get; set; }
    }

    /// <summary>
    /// Represents a single part of a session.
    /// </summary>
    public class BoxSessionPartInfo
    {
        /// <summary>
        /// String representing the Unique 8 digit part ID.
        /// </summary>
        [JsonProperty(PropertyName = "part_id")]
        public virtual string PartId { get; set; }

        /// <summary>
        /// Offset in bytes for the file part that was uploaded.
        /// </summary>
        [JsonProperty(PropertyName = "offset")]
        public virtual long Offset { get; set; }

        /// <summary>
        /// Size in bytes for the file part that was uploaded.
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public virtual long Size { get; set; }

        /// <summary>
        /// String with hexadecimal representation of part’s SHA-1.
        /// </summary>
        [JsonProperty(PropertyName = "sha1")]
        public virtual string Sha1 { get; set; }
    }
    #endregion

    #region Error Model
    public class BoxClientError
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
    #endregion
}
