using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BoxSampleAutoCAD.BoxIntegration.Services
{
    public partial class BoxService
    {
        public async Task<IEnumerable<BoxFile>> ListFolderItems(string folderId, string accessToken)
        {
            var api = $"https://api.box.com/2.0/folders/{folderId}/items";

            //Create client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            //Create http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, api);

            //Execute
            var response = await client.SendAsync(httpRequest);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<BoxCollection<BoxItem>>(jsonStr);

                return items.Entries
                    .Where(x => x.Type == "file")
                    .Select(x => new BoxFile { Id = x.Id, Name = x.Name });
            }
            //Response code 403,405,405,other
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

    }
}
