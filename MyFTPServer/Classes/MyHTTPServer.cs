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
using AdvancedFTPServer;

namespace MyFTPServer.Classes
{
    class MyHTTPServer
    {

        TcpListener FTPListener;

        private List<MyHTTPClient> FTPClients = new List<MyHTTPClient>();

        internal string Status {
            get { if (FTPListener == null) return "value=\"0\""; else return "value=\"1\" checked"; }
        }

        internal bool IsRunning {
            get { return FTPListener != null; }
        }

        internal bool Start()
        {
            try
            {
                Stop();

                FTPListener = new TcpListener(IPAddress.Any, 998);
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

        void NewFTPClientArrived(IAsyncResult arg)
        {
            try
            {
                MyHTTPClient client = new MyHTTPClient(FTPListener.EndAcceptSocket(arg));
                //client.Uploaded += UpLoadFinished;
                FTPClients.Add(client);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("创建接受者失败");
            }

            try
            {
                FTPListener.BeginAcceptSocket(new AsyncCallback(NewFTPClientArrived), null);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("开始接受失败");
            }
        }

    }
}
