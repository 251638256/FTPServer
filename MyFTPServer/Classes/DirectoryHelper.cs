using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyFTPServer.Classes
{
    static class DirectoryHelper
    {

        public static string GetExactPath(string Path)
        {
            // Windows : C:\Users\work\Source\Repos\MyFTPServer\MyFTPServer\Classes
            // Linux : /root/myftppath

            if (Path == null) Path = "";

            string dir = Path.Replace("/", "\\");

            if (!dir.EndsWith("\\")) dir += "\\";

            if (!Path.StartsWith("/")) dir = "\\" + dir;

            ArrayList pathParts = new ArrayList();
            dir = dir.Replace("\\\\", "\\");
            string[] p = dir.Split('\\');
            pathParts.AddRange(p);

            for (int i = 0; i < pathParts.Count; i++)
            {
                if (pathParts[i].ToString() == "..")
                {
                    if (i > 0)
                    {
                        pathParts.RemoveAt(i - 1);
                        i--;
                    }

                    pathParts.RemoveAt(i);
                    i--;
                }
            }

            return dir.Replace("\\\\", "\\");
        }
    }
}
