using System.IO;

namespace BoxSampleAutoCAD.Components
{
    public class AppDataPath
    {
        public static string AppFolderName => "BoxSample";
        public static string FolderName => "BoxSampleAutoCAD";

        public static string GetAppDataPath()
            => Path.Combine(
                PathEx.GetAppDataRoamingPath(),
                AppFolderName,
                FolderName);
        public static string GetRoamingFolderPath()
            => Path.Combine(
                PathEx.GetAppDataRoamingPath(),
                AppFolderName);
    }
}
