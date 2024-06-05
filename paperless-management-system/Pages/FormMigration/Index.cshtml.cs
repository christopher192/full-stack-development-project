using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;

namespace WD_ERECORD_CORE.Pages.FormMigration
{
    public class IndexModel : FormMigrationPageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FormMigrationViewModel FormMigrationViewModel { get; set; }

        public IActionResult OnGet()
        {
            this.FormMigrationViewModel = new FormMigrationViewModel();

            return Page();
        }

        public JsonResult OnGetMasterFormList(string searchBy)
        {
            var selectedData = _context.MasterFormLists.Select(x => new
            {
                id = x.Id,
                value = x.MasterFormName + " - " + x.MasterFormDescription,
                text = x.MasterFormName + " - " + x.MasterFormDescription,
                name = x.MasterFormName,
                description = x.MasterFormDescription
            });

            if (!String.IsNullOrEmpty(searchBy))
            {
                selectedData = selectedData.Where(x => x.name.ToLower().Contains(searchBy.ToLower()) || (x.description != null && x.description.ToLower().Contains(searchBy.ToLower())));
            }

            return new JsonResult(selectedData);
        }

        public JsonResult OnPostMasterForm([FromBody] int request)
        {
            if (request == null)
            {
                return new JsonResult(null);
            }

            var data = _context.MasterFormLists.Where(x => x.Id == request).Include(x => x.MasterFormDepartments).ThenInclude(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).FirstOrDefault();

            if (data == null)
            {
                return new JsonResult(null);
            }

            return new JsonResult(data);
        }

        public IActionResult OnPostVerifyConnection()
        {
            ModelState["FormMigrationViewModel.SelectMasterFormId"].Errors.Clear();

            string connectionStr = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)(HOST = " + this.FormMigrationViewModel.HostName + ")(PORT = " + this.FormMigrationViewModel.Port + "))" + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + this.FormMigrationViewModel.ServiceNameOrSID + ")));" + "User Id= " + this.FormMigrationViewModel.UserName + ";Password=" + this.FormMigrationViewModel.Password + ";";
            
            try
            {
                OracleConnection dbConnection = new OracleConnection(connectionStr);

                dbConnection.Open();

                dbConnection.Close();
                dbConnection.Dispose();

                ViewData["Database Connection Success"] = "Database Connection Success!";
            }
            catch (Exception ex)
            {
                ViewData["Database Connection Warning"] = ex.Message;
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
