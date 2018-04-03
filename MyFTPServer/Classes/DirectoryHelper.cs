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

            //string dir = Path.Replace("/", "\\");
            string dir = Path;


            if (!Path.StartsWith(Constant.SplitChar) && ConnectedUser?.CurrentWorkingDirectory != null)
                dir = ConnectedUser.CurrentWorkingDirectory + dir;


            if (!dir.EndsWith(Constant.SplitChar)) dir += Constant.SplitChar;

            if (!Path.StartsWith(Constant.SplitChar)) dir = Constant.SplitChar + dir;

            dir = dir.Replace(@"\\", @"\");
            dir = dir.Replace(@"//", @"/");


            return dir;
        }
    }
}
