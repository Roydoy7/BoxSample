using BoxSampleAutoCAD.BoxIntegration.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BoxSampleAutoCAD.BoxIntegration.Components
{
    public class BoxAuthProvider : IAuthProvider
    {
        public event EventHandler<BoxAuthEventArgs> BoxAuthedEvent;
        public event EventHandler<BoxAuthEventArgs> BoxTokenExpiredEvent;
        public event EventHandler BoxUnauthEvent;

        private static BoxAuthProvider mInstance;
        public static BoxAuthProvider Instance
        {
            get
            {
                mInstance ??= new();
                return mInstance;
            }
        }

        private BoxAuthProvider()
        {
        }

        public async Task<IAuthData> AuthAsync(string code)
        {
            //This is used to prevent SSL/TLS exception
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => { return true; };

            if (string.IsNullOrEmpty(code))
                throw new Exception("Code is empty.");

            var authenticationUrl = "https://api.box.com/oauth2/token";
            var boxAuthModel = BoxModule.BoxAuthCredential;

            var boxId = boxAuthModel.ClientId;
            var boxSecret = boxAuthModel.Secret;

            var client = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
              new KeyValuePair<string, string>("grant_type", "authorization_code"),
              new KeyValuePair<string, string>("code", code),
              new KeyValuePair<string, string>("client_id", boxId),
              new KeyValuePair<string, string>("client_secret", boxSecret)
            });

            var response = await client.PostAsync(authenticationUrl, content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsStringAsync();
                var boxToken = JsonConvert.DeserializeObject<BoxToken>(data);

                var authData = new AuthData
                {
                    AccessToken = boxToken.access_token,
                    RefreshToken = boxToken.refresh_token,
                    Expires_in = boxToken.Expires_in,
                    CreatedTime = DateTime.UtcNow,
                };

                BoxAuthedEvent?.Invoke(this, new BoxAuthEventArgs { AuthData = authData });

                return authData;
            }

            return null;
        }

        public async Task<IAuthData> RefreshAsync(string refreshToken)
        {
            //This is used to prevent SSL/TLS exception
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => { return true; };

            var api = $"https://api.box.com/oauth2/token";
            //Get app id and secret
            var boxAuthModel = BoxModule.BoxAuthCredential;

            var boxId = boxAuthModel.ClientId;
            var boxSecret = boxAuthModel.Secret;

            //Create http client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            //Create http request
            var request = new HttpRequestMessage(HttpMethod.Post, api);
            request.Content = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string ,string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", boxId),
                    new KeyValuePair<string, string>("client_secret", boxSecret),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                });

            //Execute
            var response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                var boxToken = JsonConvert.DeserializeObject<BoxToken>(jsonStr);
                var authData = new AuthData
                {
                    AccessToken = boxToken.access_token,
                    RefreshToken = boxToken.refresh_token,
                    Expires_in = boxToken.Expires_in,
                    CreatedTime = DateTime.UtcNow,
                };

                BoxAuthedEvent?.Invoke(this, new BoxAuthEventArgs { AuthData = authData });
                return authData;
            }
            else
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
            }

            BoxTokenExpiredEvent?.Invoke(this, new BoxAuthEventArgs());
            return null;
        }

        public void Unauth()
        {
            BoxUnauthEvent?.Invoke(this, new EventArgs());
        }

        class BoxToken
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public int Expires_in { get; set; }
        }

        public class BoxAuthEventArgs : EventArgs
        {
            public IAuthData AuthData { get; set; }
        }
    }
}
