using System.ComponentModel.DataAnnotations;

namespace WD_ERECORD_CORE.ViewModels
{
    public class NumbericBuilderViewModel
    {
        public int? MasterFormId { get; set; }

        [Display(Name = "Master Form Description")]
        public string? MasterFormDescription { get; set; }

        [Required]
        [Display(Name = "Department Code")]
        public string DepartmentCode { get; set; }

        [Required]
        [Display(Name = "Sub Department Code")]
        public string SubDepartmentCode { get; set; }

        [Required]
        [Display(Name = "Factory Code")]
        public string FactoryCode { get; set; }

        [Required]
        [Display(Name = "Document Category Code")]
        public string DocumentCategoryCode { get; set; }

        [Display(Name = "Upload File")]
        public IFormFile? UploadFile { get; set; }

        public string? GuidelineFile { get; set; }

        public string? UniqueGuidelineFile { get; set; }
    }

    public class FormDesignViewModel
    {
        public int? MasterFormId { get; set; }

        public string? MasterFormDesignData { get; set; }
    }

    public class ReviewSubmitViewModel
    {
        public int? MasterFormId { get; set; }

        public string? MasterFormName { get; set; }

        public string? MasterFormDescription { get; set; }

        public string? MasterFormCreatedBy { get; set; }

        public string? Owner { get; set; }

        public string? MasterFormData { get; set; }

        public string? GuidelineFile { get; set; }

        public string? UniqueGuidelineFile { get; set; }

        [Display(Name = "Change Log")]
        public string? ChangeLog { get; set; }
    }
}
