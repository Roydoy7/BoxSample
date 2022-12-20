using BoxSampleAutoCAD.BoxIntegration.DataModels;
using BoxSampleAutoCAD.BoxIntegration.Services;
using BoxSampleAutoCAD.Components;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace BoxSampleAutoCAD.BoxIntegration.ViewModels
{
    public class BoxUploadFormViewModel : ViewModelBase2
    {
        public bool IsUploading { get; set; }
        public string FolderUrl { get; set; }
        public string FilePath { get; set; }
        public string Message { get; set; }
        public IAuthData AuthData { get; set; }
        public IBoxService BoxService { get; set; }
        public BoxUploadFormViewModel()
        {
            BoxService = new BoxService();
            AuthData = BoxModule.AuthData;
        }

        public ICommand OpenFileCommand => new RelayCommand(o =>
        {
            foreach (var filePath in FileBrowser.BrowseFile("Any Files", new string[] { "*" }, false))
            {
                FilePath = filePath;
                break;
            }
        });

        public ICommand UploadCommand => new RelayCommand(async o =>
        {
            if (IsUploading)
                return;
            Message = string.Empty;

            if (!AuthData.IsValid())
            {
                MessageBox.Show("boxのアカウントでログインしてください。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var boxFolderId = string.Empty;
            if (string.IsNullOrEmpty(FolderUrl))
            {
                MessageBox.Show("boxフォルダのUrlを入力してください。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                boxFolderId = BoxService.GetFolderId(FolderUrl);
                if (string.IsNullOrEmpty(boxFolderId))
                {
                    MessageBox.Show("このUrlはboxのUrlではありません。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("ファイル名前を選択してください。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var filePath = FilePath;
            if (File.Exists(filePath))
            {
                IsUploading = true;
                try
                {
                    var fi = new FileInfo(filePath);
                    //If file size is larger than 50MB, use a session based upload
                    if (fi.Length > 50 * 1024 * 1024)
                        await BoxService.UploadChunk(filePath, boxFolderId, AuthData.AccessToken, msg => Message = msg);
                    //Small file size
                    else
                        await BoxService.UploadFile(filePath, boxFolderId, AuthData.AccessToken);
                    IsUploading = false;
                    Message = "完成";
                }
                catch (Exception e)
                {
                    IsUploading = false;
                    Message = string.Empty;
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        });

        public ICommand CloseCommand => new RelayCommand(o =>
        {
            if (IsUploading)
            {
                MessageBox.Show("アップロードしています。少々お待ちください。", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
                Close();
        });
    }
}
