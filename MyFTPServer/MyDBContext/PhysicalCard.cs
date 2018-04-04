using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyFTPServer.MyDBContext
{
    [Serializable]
    public class PhysicalCard
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string Sex { get; set; }

        [Required]
        public string Card { get; set; }

        [Required]
        public string Pic { get; set; }

    }
}
