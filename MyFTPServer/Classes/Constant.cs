using System;
using System.Collections.Generic;
using System.Text;

namespace MyFTPServer.Classes
{

    public class Constant
    {
        public static string SplitChar {
            get {
#if Linux
                return "/";
#else
                return @"\";
#endif
            }
        }


    }
}
