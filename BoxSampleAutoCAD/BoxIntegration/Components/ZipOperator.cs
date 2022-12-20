using System.IO;
using System.IO.Compression;
using BoxSampleAutoCAD.Components;

namespace BoxSampleAutoCAD.BoxIntegration.Components
{
    public static class ZipOperator
    {
        /// <summary>
        /// Compress current dwg's folder files into a zip.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CompressProjectToZip(string fileName, string folderPath)
        {
            var zipFilePath = Path.Combine(PathEx.GetUserTmpPath(), fileName + ".zip");
            SafelyCreateZipFromDirectory(folderPath, zipFilePath);

            return zipFilePath;
        }

        //Borrowed this from https://stackoverflow.com/questions/19395128/c-sharp-zipfile-createfromdirectory-the-process-cannot-access-the-file-path-t
        private static void SafelyCreateZipFromDirectory(string sourceDirectoryName, string zipFilePath)
        {
            using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Create))
            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                foreach (var file in Directory.GetFiles(sourceDirectoryName, "*.*", SearchOption.AllDirectories))
                {
                    //Ignore tmp files
                    if (file.EndsWith(".dwl") || file.EndsWith(".dwl2") || file.EndsWith(".bak"))
                        continue;
                    var relativePath = file.Replace(sourceDirectoryName, string.Empty).TrimStart('\\');
                    var entry = archive.CreateEntry(relativePath);

                    entry.LastWriteTime = File.GetLastWriteTime(file);
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var stream = entry.Open())
                    {
                        fs.CopyTo(stream, 81920);
                    }
                }
            }
        }

        //Extract a zip file into a folder and return folder path.
        public static string ExtractToFolder(string filePath, string folderPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var newFolderPath = Path.Combine(folderPath, fileName);
            Directory.CreateDirectory(newFolderPath);
            ZipFile.ExtractToDirectory(filePath, newFolderPath);
            return newFolderPath;
        }
    }
}
