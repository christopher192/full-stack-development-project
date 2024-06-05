using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class FactoryList
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Location Code")]
        [Column(TypeName = "VARCHAR2(10)")]
        [Required(ErrorMessage = "Location Code is required")]
        public string LocationCode { get; set; } = String.Empty;

        [Display(Name = "Location Description")]
        [Column(TypeName = "VARCHAR2(200)")]
        public string? LocationDescription { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
