using BoxSampleAutoCAD.Components;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace BoxSampleAutoCAD.BoxIntegration.Views
{
    /// <summary>
    /// Interaction logic for BoxAuthForm.xaml
    /// </summary>
    public partial class BoxAuthForm
    {
        public BoxAuthForm()
        {
            InitializeComponent();
            this.Loaded += AuthForm_Loaded;
        }

        private async Task InitWebView2()
        {
            //Change WebView2's user data folder to a custom location,
            //if the user doesn't have admin privilege, the standard location will cause
            //webview2 fail to initialize.
            var env = await CoreWebView2Environment.CreateAsync(null, Path.Combine(AppDataPath.GetAppDataPath(), "WebView2"));
            await webView2.EnsureCoreWebView2Async(env);
        }

        private async void AuthForm_Loaded(object sender, RoutedEventArgs e)
        {
            await InitWebView2();
            var boxAuthCredential = BoxModule.BoxAuthCredential;
            var boxId = boxAuthCredential.ClientId;
            //Check if auth credential is set.
            if (string.IsNullOrEmpty(boxId))
            {
                //This should not happen
                MessageBox.Show(@"box's auth client id is not set correctly. 
                    Locate to this plugin's folder and make sure BoxAuthCredential.json 
                    exists and its content is set correctly.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var url = $"https://account.box.com/api/oauth2/authorize?client_id={boxId}&response_type=code";
            webView2.Source = new Uri(url);
        }

        public void Navigate(string url)
        {
            webView2.Source = new Uri(url);
        }
    }
}
