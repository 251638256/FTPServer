using AdvancedFTPServer;
using System;
using System.IO;

namespace MyFTPServer
{
    class Program
    {
        private static FTPServer fTPServer;

        static void Main(string[] args)
        {
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
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                if (consoleKeyInfo.KeyChar == 'q' || consoleKeyInfo.KeyChar == 'Q')
                {
                    fTPServer.Stop();
                    break;
                }
            }

            GC.Collect();
        }
    }
}
