using BoxSampleAutoCAD.BoxIntegration.Components;
using BoxSampleAutoCAD.BoxIntegration.DataModels;
using BoxSampleAutoCAD.BoxIntegration.Policies;
using BoxSampleAutoCAD.Components;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;

namespace BoxSampleAutoCAD.BoxIntegration.Services
{
    /// <summary>
    /// Saves token after a successful login.
    /// Load token when initializing.
    /// Delete token when logout or token expired;
    /// </summary>
    public class BoxTokenService
    {
        private static BoxTokenService mInstance;
        public static BoxTokenService Instance
        {
            get
            {
                mInstance ??= new();
                return mInstance;
            }
        }

        private BoxTokenService()
        {
            LoadAuthCredential();
            LoadOrRefreshToken();
            Timer.Elapsed += (o, e) =>
            {
                var authData = BoxModule.AuthData;
                RefreshToken(authData.RefreshToken);
            };
            BoxAuthProvider.Instance.BoxAuthedEvent += (o, e) =>
            {
                WriteTokenToJson(e.AuthData);
            };
            BoxAuthProvider.Instance.BoxTokenExpiredEvent += (o, e) =>
            {
                DeleteAndClearToken();
            };
            BoxAuthProvider.Instance.BoxUnauthEvent += (o, e) =>
            {
                DeleteAndClearToken();
            };
        }

        private void DeleteAndClearToken()
        {
            ClearTokenData();
            DeleteTokenJson();
        }

        private void ClearTokenData()
        {
            var authData = BoxModule.AuthData;
            authData.AccessToken = string.Empty;
            authData.RefreshToken = string.Empty;
            authData.Expires_in = -1;
            authData.CreatedTime = DateTime.MinValue;
        }

        private void DeleteTokenJson()
        {
            var filePath = GetJsonPath();
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    //Do nothing.
                }
            }
        }

        /// <summary>
        /// This method loads box's auth credential from currently running assembly's folder.
        /// </summary>
        private void LoadAuthCredential()
        {
            var folderPath = AssemblyPath.GetAssemblyPath();
            var filePath = Path.Combine(folderPath, "BoxAuthCredential.json");
            if (File.Exists(filePath))
            {
                var jsonStr = File.ReadAllText(filePath);
                var authCredential = JsonConvert.DeserializeObject<BoxAuthCredential>(jsonStr);
                var authCredentialInMemory = BoxModule.BoxAuthCredential;
                authCredentialInMemory.ClientId = authCredential.ClientId;
                authCredentialInMemory.Secret = authCredential.Secret;
            }
        }

        /// <summary>
        /// Load auth data from json file on start up.
        /// </summary>
        private void LoadOrRefreshToken()
        {
            var filePath = GetJsonPath();
            if (File.Exists(filePath))
            {
                try
                {
                    var jsonStr = File.ReadAllText(filePath);
                    var authDataFromJson = JsonConvert.DeserializeObject<AuthData>(jsonStr, new AuthDataJsonConverter(KeyPolicy.BoxAuthDataKey));
                    if (authDataFromJson.IsValid())
                    {
                        var authData = BoxModule.AuthData;
                        authData.CreatedTime = authDataFromJson.CreatedTime;
                        authData.AccessToken = authDataFromJson.AccessToken;
                        authData.Expires_in = authDataFromJson.Expires_in;
                        var elapsedTime = DateTime.UtcNow.Subtract(authData.CreatedTime).TotalSeconds;
                        SetTimer(authData.Expires_in - elapsedTime);
                    }
                    else if (authDataFromJson.IsRefreshValid())
                    {
                        RefreshToken(authDataFromJson.RefreshToken);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// When auth succeeded, retrieve token and save to json file.
        /// </summary>
        private void WriteTokenToJson(IAuthData authData)
        {
            var jsonStr = JsonConvert.SerializeObject(authData, Formatting.Indented, new AuthDataJsonConverter(KeyPolicy.BoxAuthDataKey));
            try
            {
                var filePath = GetJsonPath();
                var folderPath = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(folderPath);
                File.WriteAllText(filePath, jsonStr);
                var elapsedTime = DateTime.UtcNow.Subtract(authData.CreatedTime).TotalSeconds;
                SetTimer(authData.Expires_in - elapsedTime);
            }
            catch { }
        }

        private async void RefreshToken(string refreshToken)
        {
            try
            {
                var boxAuthProvider = BoxAuthProvider.Instance;
                var newAuthData = await boxAuthProvider.RefreshAsync(refreshToken);
                var authDataInMemory = BoxModule.AuthData;
                if (newAuthData != null)
                {
                    authDataInMemory.AccessToken = newAuthData.AccessToken;
                    authDataInMemory.CreatedTime = newAuthData.CreatedTime;
                    authDataInMemory.Expires_in = newAuthData.Expires_in;
                    authDataInMemory.RefreshToken = newAuthData.RefreshToken;
                    var elapsedTime = DateTime.UtcNow.Subtract(authDataInMemory.CreatedTime).TotalSeconds;
                    SetTimer(authDataInMemory.Expires_in - elapsedTime);
                }
            }
            catch
            {
            }
        }

        private string GetJsonPath()
        {
            return Path.Combine(AppDataPath.GetRoamingFolderPath(),
                "Auth",
                BoxDataPathPolicy.RootDataPath,
                BoxDataNamePolicy.AuthDataName);
        }

        private void SetTimer(double seconds)
        {
            Timer.Interval = seconds * 1000;
            Timer.Enabled = true;
            Timer.Start();
        }

        private Timer Timer { get; } = new();
    }
}
