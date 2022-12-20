using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxSampleAutoCAD.BoxIntegration.Services
{
    public interface IBoxService
    {
        Task<string> DownloadFile(string folderPath, string boxFileId, string accessToken);
        string GetFileId(string boxFileUrl);
        string GetFolderId(string boxFolderUrl);
        Task UploadChunk(string filePath, string boxFolderId, string accessToken, Action<string> updateStatus = null);
        Task UploadFile(string filePath, string boxFolderId, string accessToken);
        Task<IEnumerable<BoxFile>> ListFolderItems(string folderId, string accessToken);
    }
}