using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.ComponentModel.DataAnnotations;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.ViewModels
{
    public class FormMigrationViewModel
    {
        [Display(Name = "Host Name")]
        public string HostName { get; set; } = String.Empty;
        public string Port { get; set; } = String.Empty;

        [Display(Name = "Service Name/ SID")]
        public string ServiceNameOrSID { get; set; } = String.Empty;

        [Display(Name = "User Name")]
        public string UserName { get; set; } = String.Empty;

        public string Password { get; set; } = String.Empty;

        [Required(ErrorMessage = "Master form is required.")]
        public string SelectMasterFormId { get; set; } = String.Empty;

        public MasterFormList MasterFormList { get; set; }
    }
}
