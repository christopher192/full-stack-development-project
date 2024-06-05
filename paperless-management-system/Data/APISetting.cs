using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WD_ERECORD_CORE.Data
{
    public class APISetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(200)")]
        public string Title { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(250)")]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(500)")]
        public string Url { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(500)")]
        public string? Condition { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        public string? Port { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        public string? SID { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        [Display(Name = "Service Name")]
        public string? ServiceName { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(150)")]
        [Display(Name = "API Type")]
        public string APIType { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(250)")]
        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        [Column(TypeName = "VARCHAR2(300)")]
        public string? Password { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        [Display(Name = "Database Name")]
        public string? DatabaseName { get; set; }

        [Column(TypeName = "VARCHAR2(1000)")]
        [Display(Name = "Database Script")]
        public string? DatabaseScript { get; set; }

        [Column(TypeName = "VARCHAR2(500)")]
        public string? Param { get; set; }
        
        [Column(TypeName = "VARCHAR2(150)")]
        public string? KeyName { get; set; }
        
        [Column(TypeName = "VARCHAR2(150)")]
        public string? Format { get; set; }
    }

    public class APITypes
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
