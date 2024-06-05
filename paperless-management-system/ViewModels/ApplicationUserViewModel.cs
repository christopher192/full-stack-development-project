using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WD_ERECORD_CORE.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Display(Name = "Employee Type")]
        public string? EmployeeType { get; set; }

        [Display(Name = "Department")]
        public string? Department { get; set; }

        [Display(Name = "Cost Center Id")]
        public string? CostCenterID { get; set; }

        [Display(Name = "Cost Center Name")]
        public string? CostCenterName { get; set; }

        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Upload Image")]
        [DataType(DataType.Upload)]
        public IFormFile? UploadImage { get; set; }

    }
}
