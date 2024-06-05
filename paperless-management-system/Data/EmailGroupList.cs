using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WD_ERECORD_CORE.Data
{
    public class EmailGroupList
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR2(50)")]
        [Display(Name = "Group Name")]
        [MaxLength(50)]
        public string GroupName { get; set; } = String.Empty;

        public ICollection<EmailGroupUserList>? EmailGroupUserLists { get; set; }

    }
}
