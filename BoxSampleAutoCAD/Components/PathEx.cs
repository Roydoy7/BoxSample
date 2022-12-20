using System;

namespace BoxSampleAutoCAD.Components
{
    public class PathEx
    {
        /// <summary>
        /// C:\Users\Username\AppData\Roaming
        /// </summary>
        /// <returns></returns>
        public static string GetAppDataRoamingPath()
            => Environment.ExpandEnvironmentVariables("%APPDATA%");

        /// <summary>
        /// C:\ProgramData
        /// </summary>
        /// <returns></returns>
        public static string GetAllUsersProfilePath()
            => Environment.ExpandEnvironmentVariables("%ALLUSERSPROFILE%");

        /// <summary>
        /// C:\Program Files\Common Files
        /// </summary>
        /// <returns></returns>
        public static string GetCommonProgramFilesPath()
            => Environment.ExpandEnvironmentVariables("%COMMONPROGRAMFILES%");

        /// <summary>
        /// C:\Program Files\Common Files(x86)
        /// </summary>
        /// <returns></returns>
        public static string GetCommonProgramFiles86Path()
            => Environment.ExpandEnvironmentVariables("%COMMONPROGRAMFILES(x86)%");

        /// <summary>
        /// C:\Windows\System32\cmd.exe
        /// </summary>
        /// <returns></returns>
        public static string GetCmdPath()
            => Environment.ExpandEnvironmentVariables("%COMSPEC%");

        /// <summary>
        /// C:
        /// </summary>
        /// <returns></returns>
        public static string GetHomeDriverPath()
            => Environment.ExpandEnvironmentVariables("%HOMEDRIVE%");

        /// <summary>
        /// C:\Users\Username
        /// </summary>
        /// <returns></returns>
        public static string GetHomePath()
            => Environment.ExpandEnvironmentVariables("%HOMEPATH%");

        /// <summary>
        /// C:\Users\Username\AppData\Local
        /// </summary>
        /// <returns></returns>
        public static string GetLocalAppDataPath()
            => Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%");

        /// <summary>
        /// C:\ProgramData
        /// </summary>
        /// <returns></returns>
        public static string GetProgramDataPath()
            => Environment.ExpandEnvironmentVariables("%PROGRAMDATA%");

        /// <summary>
        /// C:\Program Files
        /// </summary>
        /// <returns></returns>
        public static string GetProgramFilesPath()
            => Environment.ExpandEnvironmentVariables("%PROGRAMFILES%");

        /// <summary>
        /// C:\Program Files(x86)
        /// </summary>
        /// <returns></returns>
        public static string GetProgramFiles86Path()
            => Environment.ExpandEnvironmentVariables("%PROGRAMFILES(X86)%");

        /// <summary>
        /// C:\Users\Public
        /// </summary>
        /// <returns></returns>
        public static string GetPublicPath()
            => Environment.ExpandEnvironmentVariables("%PUBLIC%");

        /// <summary>
        /// C:
        /// </summary>
        /// <returns></returns>
        public static string GetSystemDrivePath()
            => Environment.ExpandEnvironmentVariables("%SystemDrive%");

        /// <summary>
        /// C:\Windows
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsPath()
            => Environment.ExpandEnvironmentVariables("%SystemRoot%");

        /// <summary>
        /// C:\Users\Username\AppData\Local\Temp
        /// </summary>
        /// <returns></returns>
        public static string GetUserTmpPath()
            => Environment.ExpandEnvironmentVariables("%TEMP%");

        /// <summary>
        /// C:\Users\Username
        /// </summary>
        /// <returns></returns>
        public static string GetUserProfilePath()
            => Environment.ExpandEnvironmentVariables("%USERPROFILE%");

        /// <summary>
        /// C:\Windows
        /// </summary>
        /// <returns></returns>
        public static string GetWinDir()
            => Environment.ExpandEnvironmentVariables("%WINDIR%");

    }
}
