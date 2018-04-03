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

            DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder();
            dbContextOptionsBuilder.UseSqlServer(@"Data Source=192.168.0.60\SQLEXPRESS;Initial Catalog=FTPDatabase;User ID=admin;password=123;Integrated Security=false", c => {
                c.UseRowNumberForPaging(false);
            });
            FTPDBContext fTPDBContext = new FTPDBContext(dbContextOptionsBuilder.Options);
            fTPDBContext.Database.EnsureCreated();
            fTPDBContext.Seed();


            string curpath = AppDomain.CurrentDomain.BaseDirectory;
            string configFilePath = Path.Combine(curpath, "mysetting.config");
            if (File.Exists(configFilePath))
            {
                string[] config = File.ReadAllLines(configFilePath);
                foreach (var item in config)
                {
                    string key = item.Split(' ')[0];
                    string value = item.Split(' ')[1];
                    if (key == "workDirectory" && !string.IsNullOrEmpty(value))
                    {
                        FTPServer.CommonPath = value;
                    }
                }
            }
            else
            {
                Console.WriteLine("Input File Path");
                string path = Console.ReadLine();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                FTPServer.CommonPath = path;
                File.WriteAllText(configFilePath, "workDirectory " + path);
            }


            Console.WriteLine("successed");
            Console.WriteLine("server start , listing port : 21");

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
                        // TODO : 增加用户功能
                        break;
                    default:
                        Console.WriteLine("Unkwon common");
                    break;
                }
            }

            label: 
            GC.Collect();
        }
    }
}
