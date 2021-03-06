using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.SqlClient;
using MyFTPServer.MyDBContext;
using System.Linq;

namespace AdvancedFTPServer
{
    class FTPServer
    {
        TcpListener FTPListener;

        internal List<FTPClient> FTPClients = new List<FTPClient>();

        internal string Status
        {
            get { if (FTPListener == null) return "value=\"0\""; else return "value=\"1\" checked"; }
        }

        internal bool IsRunning
        {
            get { return FTPListener != null; }
        }

        internal bool Start()
        {
            try
            {
                Stop();

                FTPListener = new TcpListener(IPAddress.Any, 21);
                FTPListener.Start(20);

                // Start accepting the incoming clients.
                FTPListener.BeginAcceptSocket(new AsyncCallback(NewFTPClientArrived), null);
                return true;
            }
            catch (Exception Ex)
            {
                //ApplicationLog.Write(LogSource.FTP, Ex);
                Console.WriteLine("创建Sockert失败" + Ex.GetBaseException()?.Message);
            }

            return false;
        }

        internal void Stop()
        {
            if (FTPListener != null) FTPListener.Stop(); FTPListener = null;
        }

        public static object AsyncObj = new object();

        /// <summary>
        /// 文件上传完毕的回调 处理上传完毕的文件
        /// </summary>
        public void UpLoadFinished(string path)
        {
            lock (AsyncObj)
            {
                using (FileStream loadFile = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    IFormatter serializer = new BinaryFormatter();
                    List<PhysicalCard> tests2 = serializer.Deserialize(loadFile) as List<PhysicalCard>;

                    var dbcontext = FtpDbContext.Instance;
                    if (tests2 != null && tests2.Any())
                    {
                        dbcontext.PhysicalCard.AddRange(tests2);
                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        // TODO : 记录错误日志 
                    }
                }
            }
        }

        public class Test
        {

        }

        void NewFTPClientArrived(IAsyncResult arg)
        {
            try
            {
                FTPClient client = new FTPClient(FTPListener.EndAcceptSocket(arg));
                client.Uploaded += UpLoadFinished;
                FTPClients.Add(client);
            }
            catch (Exception Ex)
            {
                //ApplicationLog.Write(LogSource.FTP, Ex);
                Console.WriteLine("创建接受者失败");
            }

            try
            {
                // Start accepting the incoming clients.
                FTPListener.BeginAcceptSocket(new AsyncCallback(NewFTPClientArrived), null);
            }
            catch (Exception Ex)
            {
                //ApplicationLog.Write(LogSource.FTP, Ex);
                Console.WriteLine("开始接受失败");
            }
        }
    }
}