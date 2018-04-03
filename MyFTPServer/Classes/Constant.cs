using System;
using System.Collections.Generic;
using System.Text;

namespace MyFTPServer.Classes
{

    public class Constant
    {
        public static string SplitChar
        {
            get
            {
                if (IsUnixOrMacOSX)
                {
                    return "/";
                }
                else
                {
                    return @"\";
                }
            }
        }

        public static bool IsUnixOrMacOSX
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                {
                    return true;
                }
                return false;
            }
        }


    }
}
