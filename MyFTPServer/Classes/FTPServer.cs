using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;

namespace AdvancedFTPServer
{
    class FTPServer
    {
        TcpListener FTPListener;

        internal ArrayList FTPClients = new ArrayList();

        public static string CommonPath;
        
        internal string Status
        {
            get { if (FTPListener == null)return "value=\"0\""; else return "value=\"1\" checked"; }
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

        void NewFTPClientArrived(IAsyncResult arg)
        {
            try
            {
                FTPClients.Add(new FTPClient(FTPListener.EndAcceptSocket(arg)));
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