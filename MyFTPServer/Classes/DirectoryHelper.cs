using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyFTPServer.Classes
{
    public static class DirectoryHelper
    {

        public static string GetExactPath(string Path)
        {
            // Windows : C:\Users\work\Source\Repos\MyFTPServer\MyFTPServer\Classes
            // Linux : /root/myftppath

            if (Path == null) Path = "";

            //string dir = Path.Replace("/", "\\");
            string dir = Path;

            if (!dir.EndsWith(Constant.SplitChar)) dir += Constant.SplitChar;

            if (!Path.StartsWith(Constant.SplitChar)) dir = Constant.SplitChar + dir;

            dir = dir.Replace(@"\\", @"\");

            return dir;
        }
    }
}
