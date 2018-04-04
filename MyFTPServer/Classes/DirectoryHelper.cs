using AdvancedFTPServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyFTPServer.Classes
{
    public static class DirectoryHelper
    {

        public static string GetExactPath(string Path, FTPUser ConnectedUser = null)
        {
            // Windows : C:\Users\work\Source\Repos\MyFTPServer\MyFTPServer\Classes
            // Linux : /root/myftppath

            if (Path == null) Path = "";

            string dir = Path;

            string CurrentWorkingDirectory = ConnectedUser?.CurrentWorkingDirectory ?? "/";
            if ((dir.StartsWith("/") == false && dir.StartsWith(@"\") == false))
                dir = CurrentWorkingDirectory + "/" + dir;
            else
                dir = Path;

            //if (!dir.EndsWith(Constant.SplitChar)) dir += Constant.SplitChar;

            //if (!Path.StartsWith(Constant.SplitChar)) dir = Constant.SplitChar + dir;

            dir = dir.Replace(@"\\", @"\");
            dir = dir.Replace(@"//", @"/");

            return dir;
        }

        public static string CDUP(string workingPath)
        {
            if (workingPath.Contains(@"\") && workingPath.Contains(@"/"))
            {
                workingPath = workingPath.Replace(@"/", @"\");
            }

            string[] pathParts = workingPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length > 1)
            {
                string path = "/";
                for (int i = 0; i < pathParts.Length - 1; i++)
                {
                    path += pathParts[i] + "/";
                }
                return path;
            }

            string[] pathParts2 = workingPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathParts2.Length > 1)
            {
                string path = "\\";
                for (int i = 0; i < pathParts2.Length - 1; i++)
                {
                    path += pathParts2[i] + "\\";
                }
                return path;
            }

            return workingPath;
        }
    }
}
