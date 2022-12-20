using BoxSampleAutoCAD.BoxIntegration.Components;
using BoxSampleAutoCAD.Components;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace BoxSampleAutoCAD.BoxIntegration.ViewModels
{
    public class BoxAuthFormViewModel : ViewModelBase2
    {
        public BoxAuthFormViewModel()
        {
            CodeHash = new();
        }

        public string AuthorizationUrl { get; private set; }
        //Used to prevent auth repeatedly
        private HashSet<string> CodeHash { get; set; }
        public bool DialogResult { get; private set; }
        public bool IsShowWebView2 { get; set; } = true;
        public bool IsShowSuccessView { get; set; }
        public bool IsShowLoginingView { get; set; }

        public ICommand OnNavigatedCommand => new RelayCommand(async o =>
        {
            //For new WebView2
            if (o is Uri arg)
            {
                var url = arg.ToString();
                if (url.Contains("?code="))
                {
                    IsShowWebView2 = false;
                    IsShowLoginingView = true;
                    IsShowSuccessView = false;

                    //Get the access token.
                    var code = url.Substring(url.IndexOf("?code=") + 6);
                    if (!string.IsNullOrEmpty(code))
                    {
                        //If this code is already used
                        if (!CodeHash.Add(code))
                            return;
                        try
                        {
                            var authProvider = BoxAuthProvider.Instance;
                            var token = await authProvider.AuthAsync(code);
                            if (token != null && !string.IsNullOrEmpty(token?.AccessToken))
                            {
                                var authData = BoxModule.AuthData;
                                authData.AccessToken = token.AccessToken;
                                authData.RefreshToken = token.RefreshToken;
                                authData.Expires_in = token.Expires_in;
                                authData.CreatedTime = DateTime.UtcNow;
                                DialogResult = true;
                                IsShowLoginingView = false;
                                IsShowSuccessView = true;
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
        });

        public ICommand OkCommand => new RelayCommand(o =>
        {
            Close();
        });

    }
}
