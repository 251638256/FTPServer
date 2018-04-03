using MyFTPServer.MyDBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AdvancedFTPServer
{
    public class FTPUser
    {
        internal bool CanDeleteFiles, CanDeleteFolders, CanRenameFiles,
            CanRenameFolders, CanStoreFiles, CanStoreFolder, CanViewHiddenFiles,
            CanViewHiddenFolders, CanCopyFiles;

        //internal string UserName = "WZY";
        //internal string StartUpDirectory = FTPServer.CommonPath;
        //internal string CurrentWorkingDirectory = "/";
        //internal bool IsAuthenticated = false;
        //string Password = "123";

        internal string UserName = "";
        internal string StartUpDirectory = @"";
        internal string CurrentWorkingDirectory = "\\";
        internal bool IsAuthenticated = false;
        string Password = "";

        internal void LoadProfile(string userName)
        {
            //CanDeleteFiles = CanDeleteFolders = CanRenameFiles = CanRenameFolders = CanStoreFiles = CanStoreFolder = CanViewHiddenFiles = CanViewHiddenFolders = CanCopyFiles = true;

            User user = FtpDbContext.Instance.User.FirstOrDefault(c => c.LoginName == userName);
            if (user != null)
            {
                UserName = user.LoginName;
                Password = user.Password;
                StartUpDirectory = user.WorkdDirectory;
                IsAuthenticated = false;
                CanDeleteFiles = user.CanDeleteFiles;
                CanDeleteFolders = user.CanDeleteFolders;
                CanRenameFiles = user.CanRenameFiles;
                CanRenameFolders = user.CanRenameFolders;
                CanStoreFiles = user.CanStoreFiles;
                CanStoreFolder = user.CanStoreFolder;
                CanViewHiddenFiles = user.CanViewHiddenFiles;
                CanViewHiddenFolders = user.CanViewHiddenFolders;
                CanCopyFiles = user.CanCopyFiles;
            }

            //UserName = "wzy";
            //StartUpDirectory = @"C:\Users\work\Desktop\FTP_Source";
            //IsAuthenticated = true;
            //Password = "123";
            //try
            //{
            //    if (UserName == this.UserName) return;
            //    if ((this.UserName = UserName).Length == 0) return;
            //    //< User UserName = "WZY" Password = "123" Root = "c:/" PermissionSet = "111111100" Enabled = "1" />
            //    //XmlNodeList Users = ApplicationSettings.GetUserList();
            //    XmlDocument document = new XmlDocument();
            //    document.CreateNode()
            //    XmlAttribute xx = document.CreateAttribute("User");


            //    IsAuthenticated = false;

            //    foreach (XmlNode User in Users)
            //    {
            //        if (User.Attributes[0].Value != UserName) continue;

            //        Password = User.Attributes[1].Value;
            //        StartUpDirectory = User.Attributes[2].Value;

            //        char[] Permissions = User.Attributes[3].Value.ToCharArray();

            //        CanStoreFiles = Permissions[0] == '1';
            //        CanStoreFolder = Permissions[1] == '1';
            //        CanRenameFiles = Permissions[2] == '1';
            //        CanRenameFolders = Permissions[3] == '1';
            //        CanDeleteFiles = Permissions[4] == '1';
            //        CanDeleteFolders = Permissions[5] == '1';
            //        CanCopyFiles = Permissions[6] == '1';                    
            //        CanViewHiddenFiles = Permissions[7] == '1';
            //        CanViewHiddenFolders = Permissions[8] == '1';

            //        break;
            //    }
            //}
            //catch (Exception Ex)
            //{
            //    //ApplicationLog.Write(LogSource.FTP, Ex);
            //}
        }

        internal bool Authenticate(string Password)
        {
            if (Password == this.Password) IsAuthenticated = true;
            else IsAuthenticated = false;
            return IsAuthenticated;
        }

        internal bool ChangeDirectory(string Dir)
        {
            CurrentWorkingDirectory = Dir;
            return true;
        }
    }
}
