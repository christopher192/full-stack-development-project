using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WD_ERECORD_CORE.Data
{
    public class EmailGroupUserList
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR2(150)")]
        [MaxLength(150)]
        public string Username { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR2(50)")]
        [MaxLength(50)]
        public string Email { get; set; } = String.Empty;

        public int? EmailGroupListId { get; set; }

        public virtual EmailGroupList? EmailGroupList { get; set; }

    }
}
