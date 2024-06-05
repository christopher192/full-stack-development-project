using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class PublicLogEmailNotification
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        public DateTime DateTime { get; set; }

        [Column(TypeName = "VARCHAR2(2000)")]
        public string LogDetail { get; set; } = String.Empty;
    }
}
