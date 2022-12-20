using BoxSampleAutoCAD.BoxIntegration.DataModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

namespace BoxSampleAutoCAD.BoxIntegration.Components
{
    public class BoxUserProvider : IUserProvider
    {
        private static BoxUserProvider mInstance;
        public static BoxUserProvider Instance
        {
            get
            {
                mInstance ??= new();
                return mInstance;
            }
        }

        private BoxUserProvider()
        {
        }

        public async Task<IUserModel> GetCurrentUser()
        {
            try
            {
                //This is used to prevent SSL/TLS exception
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, cert, chain, sslPolicyErrors) => { return true; };

                var authData = BoxModule.AuthData;
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.box.com/2.0/users/me");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authData.AccessToken);
                var httpClient = new HttpClient();
                //Get user
                var response = await httpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var boxUser = JsonConvert.DeserializeObject<BoxUser>(await response.Content.ReadAsStringAsync());
                    var userModel = new UserModel
                    {
                        Id = boxUser.Id,
                        Email = boxUser.Login,
                        Name = boxUser.Name,
                        Avatar_Url = boxUser.Avatar_Url,
                    };
                    return userModel;
                }
                return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
        }

        class BoxUser
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Login { get; set; }
            public string Avatar_Url { get; set; }
        }
    }
}
