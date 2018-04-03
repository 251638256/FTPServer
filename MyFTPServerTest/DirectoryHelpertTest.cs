using System;
using Xunit;
using MyFTPServer.Classes;
using Shouldly;


namespace MyFTPServerTest
{
    public class DirectoryHelpertTest
    {
        [Fact]
        public void GetExactPathTest()
        {
            // Windows : C:\Users\work\Source\Repos\MyFTPServer\MyFTPServer\Classes
            // Linux : /root/myftppath

#if Linux
            DirectoryHelper.GetExactPath(@"\a\b\..\a\..").ShouldBe(@"\a\b\..\a\..\");
#else
            string path = DirectoryHelper.GetExactPath(@"/root/myftppath");
            path.ShouldBe(@"/root/myftppath/");

            string path2 = DirectoryHelper.GetExactPath(@"root/myftppath");
            path2.ShouldBe(@"/root/myftppath/");
#endif

            Console.WriteLine();
        }
    }
}
