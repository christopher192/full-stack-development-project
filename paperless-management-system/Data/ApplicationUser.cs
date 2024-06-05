using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "User Name")]
        public override string? UserName { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Employee Id")]
        public string? EmployeeId { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Employee Type")]
        public string? EmployeeType { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Department")]
        public string? Department { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Manager Id")]
        public string? ManagerId { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Report Chain Id")]
        public string? ReportChainID { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Cost Center Name")]
        public string? CostCenterName { get; set; }

        [Column(TypeName = "NVARCHAR2(250)")]
        [Display(Name = "Cost Center Id")]
        public string? CostCenterID { get; set; }

        [Column(TypeName = "CLOB")]
        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        public ICollection<SystemRole>? SystemRoles { get; set; }
    }
}
