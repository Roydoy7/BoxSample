using BoxSampleAutoCAD.BoxIntegration.DataModels;
using BoxSampleAutoCAD.BoxIntegration.Services;
using BoxSampleAutoCAD.Components;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace BoxSampleAutoCAD.BoxIntegration.ViewModels
{
    public class BoxDownloadFormViewModel : ViewModelBase2
    {
        public bool IsDownloading { get; set; }
        public bool IsShowOpenExtractedFolderButton { get; set; }
        public string FolderPath { get; set; }
        public string ExtractedFolderPath { get; set; }
        public string FileUrl { get; set; }
        public string Message { get; set; }
        public IAuthData AuthData { get; set; }
        public IBoxService BoxService { get; set; }

        public BoxDownloadFormViewModel()
        {
            BoxService = new BoxService();
            InitAuthData();
        }

        private void InitAuthData()
        {
            AuthData = BoxModule.AuthData;
        }

        public ICommand OpenFolderCommand => new RelayCommand(o =>
        {
            FolderPath = FileBrowser.BrowseFolder();
        });

        public ICommand DownloadCommand => new RelayCommand(async o =>
        {
            if (IsDownloading)
                return;

            Message = string.Empty;
            IsShowOpenExtractedFolderButton = false;

            if (!AuthData.IsValid())
            {
                MessageBox.Show("boxのアカウントでログインしてください。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrEmpty(FolderPath))
            {
                MessageBox.Show("保存フォルダを選択してください。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var boxFileId = string.Empty;
            if (string.IsNullOrEmpty(FileUrl))
            {
                MessageBox.Show("boxフォルダのUrlを入力してください。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                boxFileId = BoxService.GetFileId(FileUrl);
                if (string.IsNullOrEmpty(boxFileId))
                {
                    MessageBox.Show("このUrlはboxファイルのUrlではありません。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            try
            {
                IsDownloading = true;
                var tmpFilePath = await BoxService.DownloadFile(FolderPath, boxFileId, AuthData.AccessToken);
                if (File.Exists(tmpFilePath))
                {
                    Message = "完成";
                    IsShowOpenExtractedFolderButton = true;
                }
                IsDownloading = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                IsDownloading = false;
                IsShowOpenExtractedFolderButton = false;
            }
        });

        public ICommand CloseCommand => new RelayCommand(o =>
        {
            if (IsDownloading)
            {
                MessageBox.Show("ダウンロードしています。少々お待ちください。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
                Close();
        });

    }
}
