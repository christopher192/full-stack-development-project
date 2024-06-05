using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class PublicLogActivity
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        public DateTime DateTime { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        public string UserName { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(150)")]
        public string UserId { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(300)")]
        public string LogDetail { get; set; } = String.Empty;
    }
}
