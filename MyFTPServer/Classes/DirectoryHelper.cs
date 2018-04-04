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
            string[] pathParts = workingPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            string path = "/";
            if (pathParts.Length > 1)
            {
                for (int i = 0; i < pathParts.Length - 1; i++)
                {
                    path += pathParts[i] + "/";
                }
            }

            return path;
        }
    }
}
