using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Threading;
using MyFTPServer.Classes;

namespace AdvancedFTPServer
{
    class FTPClient
    {
        #region Construction
        internal DateTime ConnectedTime;
        internal DateTime LastInteraction;

        // Used inside PORT method
        IPEndPoint ClientEndPoint = null;
        internal string SessionID {
            get {
                return ConnectedTime.Ticks.ToString();
            }
        }
        Socket ClientSocket;
        internal FTPUser ConnectedUser;

        internal string EndPoint {
            get {
                return ClientSocket.RemoteEndPoint.ToString();
            }
        }

        internal bool IsConnected {
            get {
                if (ClientSocket == null || !ClientSocket.Connected || ConnectedTime.ToString("HH:mm:ss") == LastInteraction.ToString("HH:mm:ss")) { Disconnect(); return false; }
                return true;
            }
        }

        bool DataTransferEnabled = false;
        TcpListener DataListener = null;

        string Rename_FilePath;

        byte[] BufferData = new byte[1024];

        internal FTPClient(Socket ClientSocket)
        {
            this.ClientSocket = ClientSocket;
            //SubItems[1].Text = ClientSocket.RemoteEndPoint.ToString();
            ClientSocket.NoDelay = false;
            ConnectedTime = DateTime.Now;

            ConnectedUser = new FTPUser();
            //SubItems[3].Text = SubItems[4].Text = (CurrentUser.ConnectedTime = DateTime.Now).ToString("MMM dd, yyyy hh:mm:ss");

            // Report the client that the server is ready to serve.
            SendMessage("220 FTP Ready\r\n");

            // Wait for the command to be sent by the client
            ClientSocket.BeginReceive(BufferData, 0, BufferData.Length, SocketFlags.None, new AsyncCallback(CommandReceived), null);
            //ClientStream.BeginRead(BufferData, 0, BufferData.Length, new AsyncCallback(CommandReceived), null);
        }

        void CommandReceived(IAsyncResult arg)
        {
            #region Read and Parse commands
            int CommandSize = 0;
            try
            {
                CommandSize = ClientSocket.EndReceive(arg);
            }
            catch { }
            if (CommandSize == 0)
            {
                Disconnect(); return;
            }


            // Wait for the next command to be sent by the client
            try
            {
                ClientSocket.BeginReceive(BufferData, 0, BufferData.Length, SocketFlags.None, new AsyncCallback(CommandReceived), null);
            }
            catch { Disconnect(); }

            LastInteraction = DateTime.Now;
            string CommandText = Constant.DefaultEncoding.GetString(BufferData, 0, CommandSize).TrimStart(' ');
            string CmdArguments = null, Command = null;
            int End = 0;
            if ((End = CommandText.IndexOf(' ')) == -1) End = (CommandText = CommandText.Trim()).Length;
            else CmdArguments = CommandText.Substring(End).TrimStart(' ');
            Command = CommandText.Substring(0, End).ToUpper();

            #endregion

            #region Execute Commands
            if (CmdArguments != null && CmdArguments.EndsWith("\r\n")) CmdArguments = CmdArguments.Substring(0, CmdArguments.Length - 2);
            bool CommandExecued = false;
            switch (Command)
            {
                case "USER":
                    if (CmdArguments != null && CmdArguments.Length > 0)
                    {
                        // WHY ??? 为什么在这里发送
                        string userName = CmdArguments;
                        ConnectedUser.LoadProfile(userName);
                        // 向客户端发送这句命令后客户端会马上发送密码信息过来
                        SendMessage("331 Password required!\r\n");
                    }
                    CommandExecued = true;
                    break;
                case "PASS":
                    if (ConnectedUser.UserName == "")
                    {
                        SendMessage("503 Invalid User Name\r\n");
                        return;
                    }

                    if (ConnectedUser.Authenticate(CmdArguments))
                        SendMessage("230 Authentication Successful\r\n");
                    else
                        SendMessage("530 Authentication Failed!\r\n");
                    CommandExecued = true;
                    break;
            }
            if (!CommandExecued)
            {
                // ConnectedUser.IsAuthenticated
                if (ConnectedUser.IsAuthenticated)
                {
                    Console.WriteLine("命令内容 " + Command + "  参数内容 " + CmdArguments);
                    switch (Command)
                    {
                        case "CWD":
                            // 确定目录是否存在
                            string dir = DirectoryHelper.GetExactPath(CmdArguments, ConnectedUser);
                            //string dir = CmdArguments;

                            if (ConnectedUser.ChangeDirectory(dir))
                                SendMessage("250 CWD command successful.\r\n");
                            else SendMessage("550 System can't find directory '" + dir + "'.\r\n");
                            break;
                        case "PWD":
                            // 确定成功后返回当前工作路径
                            string msg = "257 \"" + ConnectedUser.CurrentWorkingDirectory.Replace('\\', '/') + "\"\r\n";
                            SendMessage(msg);
                            break;
                        case "LIST":
                            // 返回当前工作路径中所有的内容
                            LIST(CmdArguments);
                            break;

                        case "CDUP":
                            // 获取上级目录地址设置到currentWorkdDirectory
                            string curpath = DirectoryHelper.CDUP(ConnectedUser.CurrentWorkingDirectory);
                            ConnectedUser.CurrentWorkingDirectory = curpath;
                            SendMessage("250 CDUP command successful.\r\n");
                            break;
                        case "QUIT":
                            SendMessage("221 FTP server signing off\r\n");
                            Disconnect();
                            break;
                        case "PORT":
                            PORT(CmdArguments);
                            break;
                        case "PASV":
                            PASV(CmdArguments);
                            break;
                        case "TYPE":
                            TYPE(CmdArguments);
                            break;
                        case "RETR":
                            // 下载文件
                            RETR(CmdArguments);
                            break;
                        case "STOR":
                            // 上传文件
                            STOR(CmdArguments);
                            break;
                        case "APPE": APPE(CmdArguments);
                            break;
                        case "RNFR":
                            // 重命名
                            RNFR(CmdArguments);
                            break;
                        case "RNTO": RNTO(CmdArguments);
                            break;
                        case "DELE":
                            // 删除文件
                            DELE(CmdArguments);
                            break;
                        case "RMD":
                            // 重命名文件夹
                            RMD(CmdArguments);
                            break;
                        case "MKD":
                            // 创建文件夹
                            MKD(CmdArguments);
                            break;


                        case "NLST": NLST(CmdArguments);
                            break;
                        case "SYST": SendMessage("215 Windows_NT\r\n");
                            break;
                        case "NOOP": SendMessage("200 OK\r\n");
                            break;
                        default: SendMessage($"500 Unknown Command.\r\n");
                            break;

                            //	case "STAT":
                            //		break;

                            //	case "HELP":
                            //		break;

                            //	case "REIN":
                            //		break;

                            //	case "STOU":
                            //		break;

                            //	case "REST":
                            //		break;

                            //	case "ABOR":
                            //		break;
                    }
                }
                else SendMessage("530 Access Denied! Authenticate first\r\n");
            }
            #endregion
        }

        #endregion

        #region Command Methods



        void RETR(string CmdArguments)
        {
            if (!ConnectedUser.CanCopyFiles)
            {
                SendMessage("426 Access Denied.\r\n");
                return;
            }

            string ReturnMessage = string.Empty;

            FileStream FS = null;
            Socket DataSocket = null;
            try
            {
                string Path = ConnectedUser.StartUpDirectory + DirectoryHelper.GetExactPath(CmdArguments,ConnectedUser);
                Path = Path.Substring(0, Path.Length - 1);

                if (!ConnectedUser.CanViewHiddenFiles && (File.GetAttributes(Path) & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    SendMessage("550 Access Denied or invalid path.\r\n");
                    return;
                }

                FS = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch
            {
                ReturnMessage = "550 Access denied or invalid path!\r\n";
                goto FinaliseAll;
            }


            DataSocket = GetDataSocket();
            if (DataSocket == null)
                goto FinaliseAll;

            try
            {
                byte[] data = new byte[(FS.Length > 100000) ? 100000 : (int)FS.Length];
                while (DataSocket.Send(data, 0, FS.Read(data, 0, data.Length), SocketFlags.None) != 0) ;
                ReturnMessage = "226 Transfer Complete.\r\n";
            }
            catch
            {
                ReturnMessage = "426 Transfer aborted.\r\n";
            }

            FinaliseAll:
            if (FS != null) FS.Close(); FS = null;
            if (DataSocket != null && DataSocket.Connected)
            {
                DataSocket.Shutdown(SocketShutdown.Both);
                DataSocket.Close();
            }
            DataSocket = null;
            SendMessage(ReturnMessage);
        }

        void STOR(string CmdArguments)
        {
            if (!ConnectedUser.CanStoreFiles)
            {
                SendMessage("426 Access Denied.\r\n");
                return;
            }
            Stream FS = null;

            string Path = ConnectedUser.StartUpDirectory + DirectoryHelper.GetExactPath(CmdArguments,ConnectedUser);

            if (Path.EndsWith("/") || Path.EndsWith(@"\"))
                Path = Path.Substring(0, Path.Length - 1);

            try
            {
                FS = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch (Exception Ex)
            {
                SendMessage("550 " + Ex.Message + "\r\n");
                return;
            }

            Socket DataSocket = GetDataSocket();
            if (DataSocket == null)
            {
                return;
            }
            try
            {
                int ReadBytes = 1;
                byte[] tmpBuffer = new byte[10000];

                do
                {
                    ReadBytes = DataSocket.Receive(tmpBuffer);
                    FS.Write(tmpBuffer, 0, ReadBytes);
                } while (ReadBytes > 0);

                tmpBuffer = null;

                SendMessage("226 Transfer Complete.\r\n");
                
                // TODO 
                // Parse this File And Save to Datebase

            }
            catch
            {
                SendMessage("426 Connection closed unexpectedly.\r\n");
            }
            finally
            {
                if (DataSocket != null)
                {
                    DataSocket.Shutdown(SocketShutdown.Both);
                    DataSocket.Close();
                    DataSocket = null;
                }
                FS.Close(); FS = null;
            }
        }

        void DELE(string CmdArguments)
        {
            string Path = DirectoryHelper.GetExactPath(CmdArguments,ConnectedUser);

            if (Path.EndsWith("/") || Path.EndsWith(@"\"))
                Path = ConnectedUser.StartUpDirectory + Path.Substring(0, Path.Length - 1);
            else
                Path = ConnectedUser.StartUpDirectory + Path;

            try
            {
                if (File.Exists(Path))
                {
                    if (ConnectedUser.CanDeleteFiles)
                    {
                        FileInfo FI = new FileInfo(Path);
                        FI.Attributes = FileAttributes.Normal; // This is required to delete a readonly file
                        File.Delete(Path);
                        SendMessage("250 File deleted.\r\n");
                    }
                    else SendMessage("550 Access Denied.\r\n");
                }
                else SendMessage("550 File dose not exist.\r\n");
            }
            catch (Exception Ex) { SendMessage("550 " + Ex.Message + ".\r\n"); }
        }

        void APPE(string CmdArguments)
        {
            // Append the file if exists or create a new file.
            SendMessage("500 This functionality is currently Unavailable\r\n");
        }

        void RNFR(string CmdArguments)
        {
            if (!ConnectedUser.CanRenameFiles) { SendMessage("550 Access Denied.\r\n"); return; }

            string Path = ConnectedUser.StartUpDirectory + DirectoryHelper.GetExactPath(CmdArguments,ConnectedUser);
            if (Path.EndsWith("/") || Path.EndsWith(@"\"))
                Path = Path.Substring(0, Path.Length - 1);

            if (Directory.Exists(Path) || File.Exists(Path))
            {
                Rename_FilePath = Path;
                SendMessage("350 Please specify destination name.\r\n");
            }
            else SendMessage("550 File or directory doesn't exist.\r\n");
        }

        void RNTO(string CmdArguments)
        {
            if (Rename_FilePath.Length == 0)
            {
                SendMessage("503 Bad sequence of commands.\r\n");
                return;
            }

            string Path = ConnectedUser.StartUpDirectory + DirectoryHelper.GetExactPath(CmdArguments, ConnectedUser);

            if (Directory.Exists(Path) || File.Exists(Path))
                SendMessage("550 File or folder with the same name already exists.\r\n");
            else
            {
                try
                {
                    if (Directory.Exists(Rename_FilePath))
                    {
                        if (ConnectedUser.CanRenameFolders) { Directory.Move(Rename_FilePath, Path); SendMessage("250 Folder renamed successfully.\r\n"); }
                        else SendMessage("550 Access Denied.\r\n");
                    }
                    else if (File.Exists(Rename_FilePath))
                    {
                        if (ConnectedUser.CanRenameFiles) { File.Move(Rename_FilePath, Path); SendMessage("250 File renamed successfully.\r\n"); }
                        else SendMessage("550 Access Denied.\r\n");
                    }
                    else SendMessage("550 Source file dose not exists.\r\n");
                }
                catch (Exception Ex) { SendMessage("550 " + Ex.Message + ".\r\n"); }
            }
            Rename_FilePath = "";
        }

        void RMD(string CmdArguments)
        {
            if (!ConnectedUser.CanDeleteFolders)
            {
                SendMessage("550 Access Denied.\r\n");
                return;
            }

            string Path = ConnectedUser.StartUpDirectory + DirectoryHelper.GetExactPath(CmdArguments, ConnectedUser);

            if (Directory.Exists(Path))
            {
                try
                {
                    Directory.Delete(Path, true);
                    SendMessage("250 \"" + Path + "\" deleted.\r\n");
                }
                catch (Exception Ex) { SendMessage("550 " + Ex.Message + ".\r\n"); }
            }
            else SendMessage("550 Folder dose not exist.\r\n");
        }

        void MKD(string CmdArguments)
        {
            if (!ConnectedUser.CanStoreFolder)
            {
                SendMessage("550 Access Denied.\r\n");
                return;
            }
            
            
            string Path = ConnectedUser.StartUpDirectory + DirectoryHelper.GetExactPath(CmdArguments, ConnectedUser);

            if (Directory.Exists(Path) || File.Exists(Path))
                SendMessage("550 A file or folder with the same name already exists.\r\n");
            else
            {
                try
                {
                    Directory.CreateDirectory(Path);
                    SendMessage("257 \"" + Path + "\" directory created.\r\n");
                }
                catch (Exception Ex) { SendMessage("550 " + Ex.Message + ".\r\n"); }
            }
        }

        void LIST(string CmdArguments)
        {

            string Path = ConnectedUser.StartUpDirectory + DirectoryHelper.GetExactPath(CmdArguments, ConnectedUser);
            if (!ConnectedUser.CanViewHiddenFolders && (new DirectoryInfo(Path).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                SendMessage("550 Invalid path specified.\r\n");
                return;
            }

            Socket DataSocket = GetDataSocket();
            if (DataSocket == null)
            {
                return;
            }

            try
            {
                string[] FilesList = Directory.GetFiles(Path, "*.*", SearchOption.TopDirectoryOnly);
                string[] FoldersList = Directory.GetDirectories(Path, "*.*", SearchOption.TopDirectoryOnly);
                string strFilesList = "";

                if (ConnectedUser.CanViewHiddenFolders)
                {
                    foreach (string Folder in FoldersList)
                    {
                        string date = Directory.GetCreationTime(Folder).ToString("MM-dd-yy hh:mmtt");
                        strFilesList += date + " <DIR> " + Folder.Substring(Folder.Replace('\\', '/').LastIndexOf('/') + 1) + "\r\n";
                    }
                }
                else
                {
                    foreach (string Folder in FoldersList)
                    {
                        if ((new DirectoryInfo(Folder).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;

                        string date = Directory.GetCreationTime(Folder).ToString("MM-dd-yy hh:mmtt");
                        strFilesList += date + " <DIR> " + Folder.Substring(Folder.Replace('\\', '/').LastIndexOf('/') + 1) + "\r\n";
                    }
                }

                if (ConnectedUser.CanViewHiddenFiles)
                {
                    foreach (string FileName in FilesList)
                    {
                        string date = File.GetCreationTime(FileName).ToString("MM-dd-yy hh:mmtt");
                        strFilesList += date + " " + new FileInfo(FileName).Length.ToString() + " " + FileName.Substring(FileName.Replace('\\', '/').LastIndexOf('/') + 1) + "\r\n";
                    }
                }
                else
                {
                    foreach (string FileName in FilesList)
                    {
                        if ((File.GetAttributes(FileName) & FileAttributes.Hidden) == FileAttributes.Hidden) continue;

                        string date = File.GetCreationTime(FileName).ToString("MM-dd-yy hh:mmtt");
                        strFilesList += date + " " + new FileInfo(FileName).Length.ToString() + " " + FileName.Substring(FileName.Replace('\\', '/').LastIndexOf('/') + 1) + "\r\n";
                    }
                }
                DataSocket.Send(Constant.DefaultEncoding.GetBytes(strFilesList));
                SendMessage("226 Transfer Complete.\r\n");
            }
            catch (DirectoryNotFoundException)
            {
                SendMessage("550 Invalid path specified.\r\n");
            }
            catch (UnauthorizedAccessException ex)
            {
                SendMessage("426 权限不足.\r\n");
            }
            catch (Exception ex)
            {
                SendMessage("426 Connection closed; transfer aborted.\r\n");
            }
            finally
            {
                DataSocket.Shutdown(SocketShutdown.Both);
                DataSocket.Close(); DataSocket = null;
            }
        }

        void NLST(string CmdArguments)
        {
            string Path = ConnectedUser.StartUpDirectory + DirectoryHelper.GetExactPath(CmdArguments, ConnectedUser);
            if (!Directory.Exists(Path))
            {
                SendMessage("550 Invalid Path.\r\n");
                return;
            }

            Socket DataSocket = GetDataSocket();
            if (DataSocket == null)
            {
                return;
            }

            try
            {
                string[] FoldersList = Directory.GetDirectories(Path, "*.*", SearchOption.TopDirectoryOnly);
                string FolderList = "";
                foreach (string Folder in FoldersList)
                {
                    FolderList += Folder.Substring(Folder.Replace('\\', '/').LastIndexOf('/') + 1) + "\r\n";
                }
                DataSocket.Send(Constant.DefaultEncoding.GetBytes(FolderList));
                DataSocket.Shutdown(SocketShutdown.Both);
                DataSocket.Close();

                SendMessage("226 Transfer Complete.\r\n");
            }
            catch
            {
                SendMessage("426 Connection closed; transfer aborted.\r\n");
            }
        }

        void TYPE(string CmdArguments)
        {
            if ((CmdArguments = CmdArguments.Trim().ToUpper()) == "A" || CmdArguments == "I")
                SendMessage("200 Type " + CmdArguments + " Accepted.\r\n");
            else SendMessage("500 Unknown Type.\r\n");
        }

        void PORT(string CmdArguments)
        {
            string[] IP_Parts = CmdArguments.Split(',');
            if (IP_Parts.Length != 6)
            {
                SendMessage("550 Invalid arguments.\r\n");
                return;
            }

            string ClientIP = IP_Parts[0] + "." + IP_Parts[1] + "." + IP_Parts[2] + "." + IP_Parts[3];
            int tmpPort = (Convert.ToInt32(IP_Parts[4]) << 8) | Convert.ToInt32(IP_Parts[5]);

            ClientEndPoint = new IPEndPoint(Dns.GetHostEntry(ClientIP).AddressList[0], tmpPort);

            DataTransferEnabled = false;

            SendMessage("200 Ready to connect to " + ClientIP + "\r\n");
        }

        void PASV(string CmdArguments)
        {
            // Open listener within the specified port range
            int tmpPort = 7000;
            StartListener:
            if (DataListener != null) { DataListener.Stop(); DataListener = null; }
            try
            {
                DataListener = new TcpListener(IPAddress.Any, tmpPort);
                DataListener.Start();
            }
            catch
            {
                if (tmpPort < 8000)
                {
                    tmpPort++;
                    goto StartListener;
                }
                else
                {
                    SendMessage("500 Action Failed Retry\r\n");
                    return;
                }
            }

            //string tmpEndPoint = DataListener.LocalEndpoint.ToString();
            //tmpPort = Convert.ToInt32(tmpEndPoint.Substring(tmpEndPoint.IndexOf(':') + 1));

            string SocketEndPoint = ClientSocket.LocalEndPoint.ToString();
            SocketEndPoint = SocketEndPoint.Substring(0, SocketEndPoint.IndexOf(":")).Replace(".", ",") + "," + (tmpPort >> 8) + "," + (tmpPort & 255);
            DataTransferEnabled = true;

            SendMessage("227 Entering Passive Mode (" + SocketEndPoint + ").\r\n");
        }

        #endregion

        #region General Methods

        internal void Disconnect()
        {
            if (ClientSocket != null && ClientSocket.Connected) ClientSocket.Close(); ClientSocket = null;
            if (DataListener != null) DataListener.Stop(); DataListener = null;
            ClientEndPoint = null;
            ConnectedUser = null;

            BufferData = null;
            Rename_FilePath = null;
            //ApplicationSettings.FtpServer.FTPClients.Remove(this);
            GC.Collect();
        }

        void SendMessage(string Data)
        {
            Console.WriteLine(Data);
            if (Data == null || Data == string.Empty) return;
            try
            {
                ClientSocket.Send(Constant.DefaultEncoding.GetBytes(Data));
            }
            catch { Disconnect(); }
        }


        Socket GetDataSocket()
        {
            Socket DataSocket = null;
            try
            {
                if (DataTransferEnabled)
                {
                    int Count = 0;
                    while (!DataListener.Pending())
                    {
                        Thread.Sleep(1000);
                        Count++;
                        // Time out after 30 seconds
                        if (Count > 29)
                        {
                            SendMessage("425 Data Connection Timed out\r\n");
                            return null;
                        }
                    }

                    DataSocket = DataListener.AcceptSocket();
                    SendMessage("125 Connected, Starting Data Transfer.\r\n");
                }
                else
                {
                    SendMessage("150 Connecting.\r\n");
                    DataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    DataSocket.Connect(ClientEndPoint);
                }
            }
            catch
            {
                SendMessage("425 Can't open data connection.\r\n");
                return null;
            }
            finally
            {
                if (DataListener != null)
                {
                    DataListener.Stop();
                    DataListener = null;
                    GC.Collect();
                }
            }

            DataTransferEnabled = false;

            return DataSocket;
        }

        #endregion
    }
}