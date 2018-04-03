using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyFTPServer.MyDBContext
{
    [Table("Dictionary")]
    public class Dictionary
    {
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        [DefaultValue(0)]
        public int ParentId { get; set; }

        [DefaultValue(0)]
        public bool IsDeleted { get; set; }
    }
}
