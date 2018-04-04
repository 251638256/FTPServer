using AdvancedFTPServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyFTPServer.Classes
{
    public static class DirectoryHelper
    {

        public static string GetExactPath(string Path, FTPUser ConnectedUser)
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

            dir = dir.Replace(@"\\", @"\");
            dir = dir.Replace(@"//", @"/");

            if (dir.Contains("/") && !dir.EndsWith("/"))
            {
                dir += "/";
            }
            else if (dir.Contains(@"\") && !dir.EndsWith(@"\"))
            {
                dir += @"\";
            }

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
            else if (pathParts.Length == 1)
            {
                return "/";
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
            else if (pathParts2.Length == 1)
            {
                return "\\";
            }

            return workingPath;
        }
    }
}
