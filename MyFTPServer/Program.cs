using AdvancedFTPServer;
using Microsoft.EntityFrameworkCore;
using MyFTPServer.MyDBContext;
using Microsoft.EntityFrameworkCore.Extensions;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MyFTPServer
{
    class Program
    {
        private static FTPServer fTPServer;

        static void Main(string[] args)
        {
            //Console.WriteLine("Wating...");
            //Console.ReadKey();
            var instance = FtpDbContext.Instance;

            //string curpath = AppDomain.CurrentDomain.BaseDirectory;
            //string configFilePath = Path.Combine(curpath, "mysetting.config");
            //if (File.Exists(configFilePath))
            //{
            //    string[] config = File.ReadAllLines(configFilePath);
            //    foreach (var item in config)
            //    {
            //        string key = item.Split(' ')[0];
            //        string value = item.Split(' ')[1];
            //        if (key == "workDirectory" && !string.IsNullOrEmpty(value))
            //        {
            //            FTPServer.CommonPath = value;
            //        }
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Input File Path");
            //    string path = Console.ReadLine();
            //    if (!Directory.Exists(path))
            //    {
            //        Directory.CreateDirectory(path);
            //    }
            //    FTPServer.CommonPath = path;
            //    File.WriteAllText(configFilePath, "workDirectory " + path);
            //}
            Console.WriteLine("server started , listening port : 21");

            fTPServer = new FTPServer();
            fTPServer.Start();

            Console.WriteLine("Press q or Q to exit this application");
            while (true)
            {
                string con = Console.ReadLine().ToLower();
                switch(con)
                {
                    case "q":
                        fTPServer.Stop();
                        goto label;
                        break;
                    case "adduser":
                        User user = new User();
                        Console.WriteLine("Input LoginName!");
                        user.LoginName = Console.ReadLine();
                        Console.WriteLine("Input Password!");
                        user.Password = Console.ReadLine();
                        Console.WriteLine("Input Work Directory!");
                        user.WorkdDirectory = Console.ReadLine();
                        user.CanCopyFiles = user.CanDeleteFiles = user.CanDeleteFolders = user.CanRenameFiles = user.CanRenameFolders = user.CanStoreFiles = user.CanStoreFolder = user.CanViewHiddenFiles = user.CanViewHiddenFolders = true;
                        FtpDbContext.Instance.Entry<User>(user).State = EntityState.Added;
                        FtpDbContext.Instance.SaveChanges();
                        Console.WriteLine("add successed");
                        break;
                    case "cls":
                        Console.Clear();
                        break;
                    default:
                        //Console.WriteLine("Unkwon common");
                    break;
                }
            }

            label: 
            GC.Collect();
        }
    }
}
