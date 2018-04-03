using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyFTPServer.MyDBContext
{

    [Table("User")]
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string LoginName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string WorkdDirectory { get; set; }

        [DefaultValue(0xFFFFFFFF)]
        public int Permission { get; set; }

        [DefaultValue(false)]
        public bool CanDeleteFiles { get; set; }

        [DefaultValue(false)]
        public bool CanDeleteFolders { get; set; }

        [DefaultValue(false)]
        public bool CanRenameFiles { get; set; }

        [DefaultValue(false)]
        public bool CanRenameFolders { get; set; }

        [DefaultValue(false)]
        public bool CanStoreFiles { get; set; }

        [DefaultValue(false)]
        public bool CanStoreFolder { get; set; }

        [DefaultValue(false)]
        public bool CanViewHiddenFiles { get; set; }

        [DefaultValue(false)]
        public bool CanViewHiddenFolders { get; set; }

        [DefaultValue(false)]
        public bool CanCopyFiles { get; set; }
    }
}