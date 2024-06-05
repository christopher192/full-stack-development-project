using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class DocumentCategoryList
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Category Code")]
        [Column(TypeName = "VARCHAR2(10)")]
        [Required(ErrorMessage = "Category Code is required")]
        public string CategoryCode { get; set; } = String.Empty;

        [Display(Name = "Category Description")]
        [Column(TypeName = "VARCHAR2(200)")]
        public string? CategoryDescription { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
