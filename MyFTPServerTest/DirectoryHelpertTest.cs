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
            //string path = DirectoryHelper.GetExactPath(@"/root/myftppath");
            //path.ShouldBe(@"/root/myftppath");

            //string path2 = DirectoryHelper.GetExactPath(@"root/myftppath");
            //path2.ShouldBe(@"/root/myftppath/");


#endif

            Console.WriteLine();
        }

        [Fact]
        public void IsUnixOrMacOsTest()
        {
            // Ã»·¨²â
        }

        [Fact]
        public void CDUPTest()
        {
            string a = DirectoryHelper.CDUP(@"/aa/bb/cc/dd");
            string b = DirectoryHelper.CDUP(@"\aa\bb\cc\dd");
            string c = DirectoryHelper.CDUP(@"/aa/bb/cc/");
            string d = DirectoryHelper.CDUP(@"\aa\bb\cc\dd\");
            string e = DirectoryHelper.CDUP(@"\aa/bb/cc\dd\");

            a.ShouldBe(@"/aa/bb/cc/");
            b.ShouldBe(@"\aa\bb\cc\");
            c.ShouldBe(@"/aa/bb/");
            d.ShouldBe(@"\aa\bb\cc\");
            e.ShouldBe(@"\aa\bb\cc\");

        }
    }
}
