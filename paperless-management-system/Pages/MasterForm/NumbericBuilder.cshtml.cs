using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.ViewModels;
using IdentityApp.Pages.Identity;
using NuGet.Packaging.Rules;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Data.SqlClient.Server;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WD_ERECORD_CORE.Pages.MasterForm
{
    public class NumbericBuilderModel : MasterFormPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        [BindProperty]
        public string FormMode { get; set; }

        [BindProperty]
        public bool AllowEditNumbericScheme { get; set; } = true;

        [BindProperty]
        public string? MasterFormData { get; set; }

        [BindProperty]
        public bool isPrevious { get; set; } = false;

        [BindProperty]
        public NumbericBuilderViewModel numbericBuilderViewModel { get; set; } = new NumbericBuilderViewModel();

        public NumbericBuilderModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        private bool ValidateFile(IFormFile File)
        {
            string fileExtension = Path.GetExtension(File.FileName).ToLower();
            string[] allowedFileTypes = { ".pdf" };

            if (allowedFileTypes.Contains(fileExtension))
            {
                return true;
            }

            return false;
        }

        public async Task<IActionResult> OnGetAsync(string? RequestFormMode, int? Id)
        {
            // check whether the request send from masterform/ index
            if (!String.IsNullOrEmpty(RequestFormMode))
            {
                if (RequestFormMode == "Create")
                {
                    this.FormMode = "Create";
                }
                else if (RequestFormMode == "Edit" && Id != null)
                {
                    this.FormMode = "Edit";

                    var getMasterForm = _context.MasterFormLists.Where(x => x.Id == Id).FirstOrDefault();

                    if (getMasterForm != null)
                    {
                        if (getMasterForm.MasterFormStatus != "rejected" && getMasterForm.MasterFormStatus != "editing") 
                        {
                            return Page();
                        }

                        var User = await GetCurrentUser();

                        if (getMasterForm.CurrentEditor != null && getMasterForm.CurrentEditor != User.UserName)
                        {
                            return NotFound();
                        }
                        else
                        {
                            this.AllowEditNumbericScheme = false;

                            this.numbericBuilderViewModel.MasterFormId = Id;
                            this.numbericBuilderViewModel.MasterFormId = getMasterForm.Id;
                            this.numbericBuilderViewModel.DepartmentCode = getMasterForm.DepartmentCode;
                            this.numbericBuilderViewModel.SubDepartmentCode = getMasterForm.SubDepartmentCode;
                            this.numbericBuilderViewModel.DocumentCategoryCode = getMasterForm.DocumentCategoryCode;
                            this.numbericBuilderViewModel.FactoryCode = getMasterForm.FactoryCode;
                            this.numbericBuilderViewModel.MasterFormDescription = getMasterForm.MasterFormDescription;
                            this.numbericBuilderViewModel.GuidelineFile = getMasterForm.GuidelineFile;
                            this.numbericBuilderViewModel.UniqueGuidelineFile = getMasterForm.UniqueGuidelineFile;
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else if (RequestFormMode == "Previous" && Id != null) 
                {
                    // check whether the request send from masterform/ numbericbuilder
                    if (!String.IsNullOrEmpty(TempData["RequestFormMode"] as string))
                    {
                        var getMasterForm = _context.MasterFormLists.Where(x => x.Id == Id).FirstOrDefault();

                        if (getMasterForm != null)
                        {
                            this.AllowEditNumbericScheme = false;

                            if (TempData["RequestFormMode"] as string == "Create")
                            {
                                this.FormMode = "Create";
                            }
                            else if (TempData["RequestFormMode"] as string == "Edit")
                            {
                                this.FormMode = "Edit";
                            }
                            else if (TempData["RequestFormMode"] as string == "EditOnly")
                            {
                                this.FormMode = "EditOnly";
                            }

                            this.isPrevious = true;

                            this.numbericBuilderViewModel.MasterFormId = getMasterForm.Id;
                            this.numbericBuilderViewModel.DepartmentCode = getMasterForm.DepartmentCode;
                            this.numbericBuilderViewModel.SubDepartmentCode = getMasterForm.SubDepartmentCode;
                            this.numbericBuilderViewModel.DocumentCategoryCode = getMasterForm.DocumentCategoryCode;
                            this.numbericBuilderViewModel.FactoryCode = getMasterForm.FactoryCode;
                            this.numbericBuilderViewModel.MasterFormDescription = getMasterForm.MasterFormDescription;
                            this.numbericBuilderViewModel.GuidelineFile = getMasterForm.GuidelineFile;
                            this.numbericBuilderViewModel.UniqueGuidelineFile = getMasterForm.UniqueGuidelineFile;
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
                else if (RequestFormMode == "DuplicateForm" && Id != null)
                {
                    var getMasterForm = _context.MasterFormLists.Where(x => x.Id == Id).FirstOrDefault();

                    if (getMasterForm != null)
                    {
                        this.FormMode = "Create";
                        this.MasterFormData = getMasterForm.MasterFormData;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else if (RequestFormMode == "UpRevision" && Id != null)
                {
                    var masterFormId = this.UpRevision(Id ?? -1);

                    // if -1 fail to duplicate
                    if (masterFormId == -1) 
                    {
                        return NotFound();
                    }
                    // redirect to .. if success duplicate with new master form id
                    else 
                    {
                        TempData["RequestFormMode"] = "Create";
                        return RedirectToPage("MasterFormBuilder", new { Id = masterFormId });
                    }
                }
                else if (RequestFormMode == "EditOnly" && Id != null)
                {
                    this.FormMode = "EditOnly";

                    var getMasterForm = _context.MasterFormLists.Where(x => x.Id == Id).FirstOrDefault();

                    if (getMasterForm != null)
                    {
                        if (getMasterForm.MasterFormStatus != "active")
                        {
                            return Page();
                        }

                        var User = await GetCurrentUser();

                        if (getMasterForm.CurrentEditor != null && getMasterForm.CurrentEditor != User.UserName)
                        {
                            return NotFound();
                        }
                        else
                        {
                            this.AllowEditNumbericScheme = false;

                            this.numbericBuilderViewModel.MasterFormId = Id;
                            this.numbericBuilderViewModel.MasterFormId = getMasterForm.Id;
                            this.numbericBuilderViewModel.DepartmentCode = getMasterForm.DepartmentCode;
                            this.numbericBuilderViewModel.SubDepartmentCode = getMasterForm.SubDepartmentCode;
                            this.numbericBuilderViewModel.DocumentCategoryCode = getMasterForm.DocumentCategoryCode;
                            this.numbericBuilderViewModel.FactoryCode = getMasterForm.FactoryCode;
                            this.numbericBuilderViewModel.MasterFormDescription = getMasterForm.MasterFormDescription;
                            this.numbericBuilderViewModel.GuidelineFile = getMasterForm.GuidelineFile;
                            this.numbericBuilderViewModel.UniqueGuidelineFile = getMasterForm.UniqueGuidelineFile;
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            else 
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(this.FormMode)) 
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostNextAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var User = await GetCurrentUser();
            string ContentRootPath = _env.ContentRootPath + "/GuidelineFiles/";
            var FileName = "";
            var UniqueFileName = "";

            // create new master form in 'create' mode
            if (this.FormMode == "Create" && this.AllowEditNumbericScheme)
            {
                var SubDepartmentCode = this.numbericBuilderViewModel.SubDepartmentCode.Split("-")[1];

                // upload/ validate files
                if (this.numbericBuilderViewModel.UploadFile != null)
                {
                    FileName = this.numbericBuilderViewModel.UploadFile.FileName;
                    var FileExtension = Path.GetExtension(this.numbericBuilderViewModel.UploadFile.FileName);

                    UniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), FileExtension);
                    var FilePath = Path.Combine(ContentRootPath, UniqueFileName);

                    using (var stream = System.IO.File.Create(FilePath))
                    {
                        await this.numbericBuilderViewModel.UploadFile.CopyToAsync(stream);
                    }
                }

                // find duplication
                var FindDuplicate = (_context.MasterFormLists.Where(x => x.DepartmentCode == this.numbericBuilderViewModel.DepartmentCode && x.SubDepartmentCode == this.numbericBuilderViewModel.SubDepartmentCode && x.FactoryCode == this.numbericBuilderViewModel.FactoryCode 
                    && x.DocumentCategoryCode == this.numbericBuilderViewModel.DocumentCategoryCode).Max(x => (int?)x.AssignedNumberCode)) ?? -1;

                // set assigned number code
                var AssignedNumberCode = FindDuplicate == -1 ? 1 : (FindDuplicate + 1);

                var CreateMasterForm = new MasterFormList
                {
                    // create numberic details
                    MasterFormName = String.Format("e{0}-{1}-{2}-{3}-{4}", this.numbericBuilderViewModel.DepartmentCode, SubDepartmentCode, 
                        this.numbericBuilderViewModel.FactoryCode, this.numbericBuilderViewModel.DocumentCategoryCode, AssignedNumberCode.ToString().PadLeft(5, '0')),
                    MasterFormDescription = this.numbericBuilderViewModel.MasterFormDescription,
                    DepartmentCode = this.numbericBuilderViewModel.DepartmentCode,
                    SubDepartmentCode = this.numbericBuilderViewModel.SubDepartmentCode,
                    DocumentCategoryCode = this.numbericBuilderViewModel.DocumentCategoryCode,
                    FactoryCode = this.numbericBuilderViewModel.FactoryCode,
                    AssignedNumberCode = AssignedNumberCode,

                    // create user details
                    Owner = User.UserName.ToString(),
                    OwnerCostCenter = User.CostCenterName.ToString(),
                    CreatedBy = User.DisplayName.ToString(),
                    OwnerEmailAddress = User.Email.ToString(),
                    CreatedDate = DateTime.Now,

                    // create master form details
                    MasterFormRevision = "1.0.0.0",
                    CurrentEditor = User.UserName,
                    MasterFormStatus = "editing"
                };

                // create file details
                if (this.numbericBuilderViewModel.UploadFile != null)
                {
                    // check if file exisis, otherwise don`t save
                    if (System.IO.File.Exists(Path.GetTempFileName())) {
                        CreateMasterForm.GuidelineFile = FileName;
                        CreateMasterForm.UniqueGuidelineFile = UniqueFileName;
                    }
                }

                // if master form data is not empty and null, then it is duplicate form request
                if (!String.IsNullOrEmpty(this.MasterFormData))
                {
                    CreateMasterForm.MasterFormData = this.MasterFormData;
                }

                _context.MasterFormLists.Add(CreateMasterForm);
                
                PublicLogActivity PA = new PublicLogActivity() { DateTime = DateTime.Now, UserId = User.UserName, UserName = User.DisplayName, LogDetail = String.Format("{0} create master form {1}", User.DisplayName, CreateMasterForm.MasterFormName) };
                _context.PublicLogActivities.Add(PA);

                await _context.SaveChangesAsync();

                TempData["RequestFormMode"] = this.FormMode;
                return RedirectToPage("MasterFormBuilder", new { Id = CreateMasterForm.Id });
            }
            // edit existing master form in 'create' mode, edit master form description and re-upload files only
            else if (this.FormMode == "Create" && !this.AllowEditNumbericScheme)
            {
                var UpdateMasterForm = _context.MasterFormLists.Where(x => x.Id == this.numbericBuilderViewModel.MasterFormId).FirstOrDefault();

                if (UpdateMasterForm != null)
                {
                    // upload/ validate files
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // delete exsiting files
                        // check if file exisis, otherwise do nothing
                        if (!String.IsNullOrEmpty(UpdateMasterForm.UniqueGuidelineFile) && !String.IsNullOrEmpty(UpdateMasterForm.GuidelineFile)) 
                        {
                            // check if file exisis in server, otherwise do nothing
                            var DeleteFilePath = Path.Combine(ContentRootPath, UpdateMasterForm.UniqueGuidelineFile);

                            if (System.IO.File.Exists(DeleteFilePath))
                            {
                                System.IO.File.Delete(DeleteFilePath);
                            }
                        }

                        FileName = this.numbericBuilderViewModel.UploadFile.FileName;
                        var FileExtension = Path.GetExtension(this.numbericBuilderViewModel.UploadFile.FileName);

                        UniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), FileExtension);
                        var FilePath = Path.Combine(ContentRootPath, UniqueFileName);

                        using (var stream = System.IO.File.Create(FilePath))
                        {
                            await this.numbericBuilderViewModel.UploadFile.CopyToAsync(stream);
                        }
                    }

                    // create file details
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // check if file exisis, otherwise don`t save
                        if (System.IO.File.Exists(Path.GetTempFileName()))
                        {
                            UpdateMasterForm.GuidelineFile = FileName;
                            UpdateMasterForm.UniqueGuidelineFile = UniqueFileName;
                        }
                    }

                    UpdateMasterForm.MasterFormDescription = this.numbericBuilderViewModel.MasterFormDescription;

                    _context.MasterFormLists.Update(UpdateMasterForm);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

                TempData["RequestFormMode"] = this.FormMode;
                return RedirectToPage("MasterFormBuilder", new { Id = UpdateMasterForm.Id });
            }
            else if (this.FormMode == "Edit")
            {
                var UpdateMasterForm = _context.MasterFormLists.Where(x => x.Id == this.numbericBuilderViewModel.MasterFormId).FirstOrDefault();

                if (UpdateMasterForm != null)
                {
                    // upload/ validate files
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // delete exsiting files
                        // check if file exisis, otherwise do nothing
                        if (!String.IsNullOrEmpty(UpdateMasterForm.UniqueGuidelineFile) && !String.IsNullOrEmpty(UpdateMasterForm.GuidelineFile))
                        {
                            // check if file exisis in server, otherwise do nothing
                            var DeleteFilePath = Path.Combine(ContentRootPath, UpdateMasterForm.UniqueGuidelineFile);

                            if (System.IO.File.Exists(DeleteFilePath))
                            {
                                System.IO.File.Delete(DeleteFilePath);
                            }
                        }

                        FileName = this.numbericBuilderViewModel.UploadFile.FileName;
                        var FileExtension = Path.GetExtension(this.numbericBuilderViewModel.UploadFile.FileName);

                        UniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), FileExtension);
                        var FilePath = Path.Combine(ContentRootPath, UniqueFileName);

                        using (var stream = System.IO.File.Create(FilePath))
                        {
                            await this.numbericBuilderViewModel.UploadFile.CopyToAsync(stream);
                        }
                    }

                    // create file details
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // check if file exisis, otherwise don`t save
                        if (System.IO.File.Exists(Path.GetTempFileName()))
                        {
                            UpdateMasterForm.GuidelineFile = FileName;
                            this.numbericBuilderViewModel.GuidelineFile = FileName;

                            UpdateMasterForm.UniqueGuidelineFile = UniqueFileName;
                            this.numbericBuilderViewModel.UniqueGuidelineFile = UniqueFileName;
                        }
                    }

                    UpdateMasterForm.MasterFormDescription = this.numbericBuilderViewModel.MasterFormDescription;

                    if (UpdateMasterForm.MasterFormStatus == "rejected")
                    {
                        UpdateMasterForm.MasterFormStatus = "editing";
                    }

                    if (!this.isPrevious)
                    {
                        UpdateMasterForm.ModifiedDate = DateTime.Now;
                        UpdateMasterForm.ModifiedBy = User.DisplayName;

                        PublicLogActivity PA = new PublicLogActivity() { DateTime = DateTime.Now, UserId = User.UserName, UserName = User.DisplayName, LogDetail = String.Format("{0} edit master form {1}", User.DisplayName, UpdateMasterForm.MasterFormName) };
                        _context.PublicLogActivities.Add(PA);
                    }

                    if (UpdateMasterForm.CurrentEditor == null) 
                    {
                        UpdateMasterForm.CurrentEditor = User.UserName;
                    }

                    _context.MasterFormLists.Update(UpdateMasterForm);
                    await _context.SaveChangesAsync();

                    TempData["RequestFormMode"] = this.FormMode;
                    return RedirectToPage("MasterFormBuilder", new { Id = UpdateMasterForm.Id });
                }
                else
                {
                    return NotFound();
                }
            }
            else if (this.FormMode == "EditOnly")
            {
                var UpdateMasterForm = _context.MasterFormLists.Where(x => x.Id == this.numbericBuilderViewModel.MasterFormId).FirstOrDefault();

                if (UpdateMasterForm != null)
                {
                    // upload/ validate files
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // delete exsiting files
                        // check if file exisis, otherwise do nothing
                        if (!String.IsNullOrEmpty(UpdateMasterForm.UniqueGuidelineFile) && !String.IsNullOrEmpty(UpdateMasterForm.GuidelineFile))
                        {
                            // check if file exisis in server, otherwise do nothing
                            var DeleteFilePath = Path.Combine(ContentRootPath, UpdateMasterForm.UniqueGuidelineFile);

                            if (System.IO.File.Exists(DeleteFilePath))
                            {
                                System.IO.File.Delete(DeleteFilePath);
                            }
                        }

                        FileName = this.numbericBuilderViewModel.UploadFile.FileName;
                        var FileExtension = Path.GetExtension(this.numbericBuilderViewModel.UploadFile.FileName);

                        UniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), FileExtension);
                        var FilePath = Path.Combine(ContentRootPath, UniqueFileName);

                        using (var stream = System.IO.File.Create(FilePath))
                        {
                            await this.numbericBuilderViewModel.UploadFile.CopyToAsync(stream);
                        }
                    }

                    // create file details
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // check if file exisis, otherwise don`t save
                        if (System.IO.File.Exists(Path.GetTempFileName()))
                        {
                            UpdateMasterForm.GuidelineFile = FileName;
                            this.numbericBuilderViewModel.GuidelineFile = FileName;

                            UpdateMasterForm.UniqueGuidelineFile = UniqueFileName;
                            this.numbericBuilderViewModel.UniqueGuidelineFile = UniqueFileName;
                        }
                    }

                    UpdateMasterForm.MasterFormDescription = this.numbericBuilderViewModel.MasterFormDescription;

                    _context.MasterFormLists.Update(UpdateMasterForm);
                    await _context.SaveChangesAsync();

                    TempData["RequestFormMode"] = this.FormMode;
                    return RedirectToPage("FormApprovalLevel", new { Id = UpdateMasterForm.Id });
                }
                else
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var User = await GetCurrentUser();
            string ContentRootPath = _env.ContentRootPath + "/GuidelineFiles/";
            var FileName = "";
            var UniqueFileName = "";

            // create new master form in 'create' mode
            if (this.FormMode == "Create" && this.AllowEditNumbericScheme)
            {
                var SubDepartmentCode = this.numbericBuilderViewModel.SubDepartmentCode.Split("-")[1];

                // upload/ validate files
                if (this.numbericBuilderViewModel.UploadFile != null)
                {
                    FileName = this.numbericBuilderViewModel.UploadFile.FileName;
                    var FileExtension = Path.GetExtension(this.numbericBuilderViewModel.UploadFile.FileName);

                    UniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), FileExtension);
                    var FilePath = Path.Combine(ContentRootPath, UniqueFileName);

                    using (var stream = System.IO.File.Create(FilePath))
                    {
                        await this.numbericBuilderViewModel.UploadFile.CopyToAsync(stream);
                    }
                }

                // find duplication
                var FindDuplicate = (_context.MasterFormLists.Where(x => x.DepartmentCode == this.numbericBuilderViewModel.DepartmentCode && x.SubDepartmentCode == this.numbericBuilderViewModel.SubDepartmentCode && x.FactoryCode == this.numbericBuilderViewModel.FactoryCode
                    && x.DocumentCategoryCode == this.numbericBuilderViewModel.DocumentCategoryCode).Max(x => (int?)x.AssignedNumberCode)) ?? -1;

                // set assigned number code
                var AssignedNumberCode = FindDuplicate == -1 ? 1 : (FindDuplicate + 1);

                var CreateMasterForm = new MasterFormList
                {
                    // create numberic details
                    MasterFormName = String.Format("e{0}-{1}-{2}-{3}-{4}", this.numbericBuilderViewModel.DepartmentCode, SubDepartmentCode,
                        this.numbericBuilderViewModel.FactoryCode, this.numbericBuilderViewModel.DocumentCategoryCode, AssignedNumberCode.ToString().PadLeft(5, '0')),
                    MasterFormDescription = this.numbericBuilderViewModel.MasterFormDescription,
                    DepartmentCode = this.numbericBuilderViewModel.DepartmentCode,
                    SubDepartmentCode = this.numbericBuilderViewModel.SubDepartmentCode,
                    DocumentCategoryCode = this.numbericBuilderViewModel.DocumentCategoryCode,
                    FactoryCode = this.numbericBuilderViewModel.FactoryCode,
                    AssignedNumberCode = AssignedNumberCode,

                    // create user details
                    Owner = User.UserName.ToString(),
                    CreatedBy = User.DisplayName.ToString(),
                    OwnerCostCenter = User.CostCenterName.ToString(),
                    OwnerEmailAddress = User.Email.ToString(),
                    CreatedDate = DateTime.Now,

                    // create master form details
                    MasterFormRevision = "1.0.0.0",
                    MasterFormStatus = "editing"
                };

                // create file details
                if (this.numbericBuilderViewModel.UploadFile != null)
                {
                    // check if file exisis, otherwise don`t save
                    if (System.IO.File.Exists(Path.GetTempFileName()))
                    {
                        CreateMasterForm.GuidelineFile = FileName;
                        this.numbericBuilderViewModel.GuidelineFile = FileName;

                        CreateMasterForm.UniqueGuidelineFile = UniqueFileName;
                        this.numbericBuilderViewModel.UniqueGuidelineFile = UniqueFileName;
                    }
                }

                // if master form data is not empty and null, then it is duplicate form request
                if (!String.IsNullOrEmpty(this.MasterFormData))
                {
                    CreateMasterForm.MasterFormData = this.MasterFormData;
                }

                _context.MasterFormLists.Add(CreateMasterForm);

                PublicLogActivity PA = new PublicLogActivity() { DateTime = DateTime.Now, UserId = User.UserName, UserName = User.DisplayName, LogDetail = String.Format("{0} create master form {1}", User.DisplayName, CreateMasterForm.MasterFormName) };
                _context.PublicLogActivities.Add(PA);

                await _context.SaveChangesAsync();

                TempData["Save"] = "Close Window";
                return Page();
            }
            // edit existing master form in 'create' mode, edit master form description and re-upload files only
            else if (this.FormMode == "Create" && !this.AllowEditNumbericScheme)
            {
                var UpdateMasterForm = _context.MasterFormLists.Where(x => x.Id == this.numbericBuilderViewModel.MasterFormId).FirstOrDefault();

                if (UpdateMasterForm != null)
                {
                    // upload/ validate files
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // delete exsiting files
                        // check if file exisis, otherwise do nothing
                        if (!String.IsNullOrEmpty(UpdateMasterForm.UniqueGuidelineFile) && !String.IsNullOrEmpty(UpdateMasterForm.GuidelineFile))
                        {
                            // check if file exisis in server, otherwise do nothing
                            var DeleteFilePath = Path.Combine(ContentRootPath, UpdateMasterForm.UniqueGuidelineFile);

                            if (System.IO.File.Exists(DeleteFilePath))
                            {
                                System.IO.File.Delete(DeleteFilePath);
                            }
                        }

                        FileName = this.numbericBuilderViewModel.UploadFile.FileName;
                        var FileExtension = Path.GetExtension(this.numbericBuilderViewModel.UploadFile.FileName);

                        UniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), FileExtension);
                        var FilePath = Path.Combine(ContentRootPath, UniqueFileName);

                        using (var stream = System.IO.File.Create(FilePath))
                        {
                            await this.numbericBuilderViewModel.UploadFile.CopyToAsync(stream);
                        }
                    }

                    // create file details
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // check if file exisis, otherwise don`t save
                        if (System.IO.File.Exists(Path.GetTempFileName()))
                        {
                            UpdateMasterForm.GuidelineFile = FileName;
                            this.numbericBuilderViewModel.GuidelineFile = FileName;

                            UpdateMasterForm.UniqueGuidelineFile = UniqueFileName;
                            this.numbericBuilderViewModel.UniqueGuidelineFile = UniqueFileName;
                        }
                    }

                    UpdateMasterForm.MasterFormDescription = this.numbericBuilderViewModel.MasterFormDescription;
                    
                    if (UpdateMasterForm.CurrentEditor != null)
                    {
                        UpdateMasterForm.CurrentEditor = null;
                    }

                    _context.MasterFormLists.Update(UpdateMasterForm);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

                TempData["Save"] = "Close Window";
                return Page();
            }
            else if (this.FormMode == "Edit")
            {
                var UpdateMasterForm = _context.MasterFormLists.Where(x => x.Id == this.numbericBuilderViewModel.MasterFormId).FirstOrDefault();

                if (UpdateMasterForm != null)
                {
                    // upload/ validate files
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // delete exsiting files
                        // check if file exisis, otherwise do nothing
                        if (!String.IsNullOrEmpty(UpdateMasterForm.UniqueGuidelineFile) && !String.IsNullOrEmpty(UpdateMasterForm.GuidelineFile))
                        {
                            // check if file exisis in server, otherwise do nothing
                            var DeleteFilePath = Path.Combine(ContentRootPath, UpdateMasterForm.UniqueGuidelineFile);

                            if (System.IO.File.Exists(DeleteFilePath))
                            {
                                System.IO.File.Delete(DeleteFilePath);
                            }
                        }

                        FileName = this.numbericBuilderViewModel.UploadFile.FileName;
                        var FileExtension = Path.GetExtension(this.numbericBuilderViewModel.UploadFile.FileName);

                        UniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), FileExtension);
                        var FilePath = Path.Combine(ContentRootPath, UniqueFileName);

                        using (var stream = System.IO.File.Create(FilePath))
                        {
                            await this.numbericBuilderViewModel.UploadFile.CopyToAsync(stream);
                        }
                    }

                    // create file details
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // check if file exisis, otherwise don`t save
                        if (System.IO.File.Exists(Path.GetTempFileName()))
                        {
                            UpdateMasterForm.GuidelineFile = FileName;
                            this.numbericBuilderViewModel.GuidelineFile = FileName;

                            UpdateMasterForm.UniqueGuidelineFile = UniqueFileName;
                            this.numbericBuilderViewModel.UniqueGuidelineFile = UniqueFileName;
                        }
                    }

                    UpdateMasterForm.MasterFormDescription = this.numbericBuilderViewModel.MasterFormDescription;
                    UpdateMasterForm.ModifiedDate = DateTime.Now;
                    UpdateMasterForm.ModifiedBy = User.DisplayName;

                    if (UpdateMasterForm.MasterFormStatus == "rejected")
                    {
                        UpdateMasterForm.MasterFormStatus = "editing";
                    }

                    if (!this.isPrevious)
                    {
                        UpdateMasterForm.ModifiedDate = DateTime.Now;
                        UpdateMasterForm.ModifiedBy = User.DisplayName;

                        PublicLogActivity PA = new PublicLogActivity() { DateTime = DateTime.Now, UserId = User.UserName, UserName = User.DisplayName, LogDetail = String.Format("{0} edit master form {1}", User.DisplayName, UpdateMasterForm.MasterFormName) };
                        _context.PublicLogActivities.Add(PA);
                    }

                    if (UpdateMasterForm.CurrentEditor != null)
                    {
                        UpdateMasterForm.CurrentEditor = null;
                    }

                    _context.MasterFormLists.Update(UpdateMasterForm);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }
                
                TempData["Save"] = "Close Window";
                return Page();
            }
            else if (this.FormMode == "EditOnly")
            {
                var UpdateMasterForm = _context.MasterFormLists.Where(x => x.Id == this.numbericBuilderViewModel.MasterFormId).FirstOrDefault();

                if (UpdateMasterForm != null)
                {
                    // upload/ validate files
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // delete exsiting files
                        // check if file exisis, otherwise do nothing
                        if (!String.IsNullOrEmpty(UpdateMasterForm.UniqueGuidelineFile) && !String.IsNullOrEmpty(UpdateMasterForm.GuidelineFile))
                        {
                            // check if file exisis in server, otherwise do nothing
                            var DeleteFilePath = Path.Combine(ContentRootPath, UpdateMasterForm.UniqueGuidelineFile);

                            if (System.IO.File.Exists(DeleteFilePath))
                            {
                                System.IO.File.Delete(DeleteFilePath);
                            }
                        }

                        FileName = this.numbericBuilderViewModel.UploadFile.FileName;
                        var FileExtension = Path.GetExtension(this.numbericBuilderViewModel.UploadFile.FileName);

                        UniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), FileExtension);
                        var FilePath = Path.Combine(ContentRootPath, UniqueFileName);

                        using (var stream = System.IO.File.Create(FilePath))
                        {
                            await this.numbericBuilderViewModel.UploadFile.CopyToAsync(stream);
                        }
                    }

                    // create file details
                    if (this.numbericBuilderViewModel.UploadFile != null)
                    {
                        // check if file exisis, otherwise don`t save
                        if (System.IO.File.Exists(Path.GetTempFileName()))
                        {
                            UpdateMasterForm.GuidelineFile = FileName;
                            this.numbericBuilderViewModel.GuidelineFile = FileName;

                            UpdateMasterForm.UniqueGuidelineFile = UniqueFileName;
                            this.numbericBuilderViewModel.UniqueGuidelineFile = UniqueFileName;
                        }
                    }

                    UpdateMasterForm.MasterFormDescription = this.numbericBuilderViewModel.MasterFormDescription;

                    _context.MasterFormLists.Update(UpdateMasterForm);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

                TempData["Save"] = "Close Window";
                return Page();
            }

            return Page();
        }

        private int UpRevision(int MasterFormId)
        {
            if (MasterFormId == -1) 
            {
                return -1;
            }
            
            var user = GetCurrentUser().Result;

            var masterForm = _context.MasterFormLists.Include(x => x.MasterFormDepartments).ThenInclude(x => x.MasterFormCCBApprovalLevels).ThenInclude(x => x.MasterFormCCBApprovers).Where(x => x.Id == MasterFormId).AsNoTracking().FirstOrDefault();

            if (masterForm != null)
            {
                if (masterForm.MasterFormStatus == "active" && masterForm.AllowUpRevision == true)
                {
                    // create file path
                    string contentRootPath = _env.ContentRootPath + "/GuidelineFiles/";

                    var masterFormParentId = masterForm.Id;
                    var currentRevision = masterForm.MasterFormRevision;
                    var removeSpecialSymbol = int.Parse(Regex.Replace(currentRevision, @"[^0-9a-zA-Z:,]+", ""));
                    var increaseRev = (removeSpecialSymbol + 1000).ToString();
                    var finalRev = "";

                    for (int i = 0; i < increaseRev.Length; i++)
                    {
                        finalRev += increaseRev[i];

                        if (i == increaseRev.Length - 1)
                        {
                            break;
                        }

                        finalRev += ".";
                    }

                    var departments = masterForm.MasterFormDepartments.ToList();
                    var approverLevels = departments.SelectMany(x => x.MasterFormCCBApprovalLevels).ToList();
                    var approvers = approverLevels.SelectMany(x => x.MasterFormCCBApprovers).ToList();

                    // reset child entities departments, approval leves, approver
                    departments.ForEach(x => x.Id = 0);
                    departments.ForEach(x => x.Status = "no action");
                    approverLevels.ForEach(x => x.Id = 0);
                    approverLevels.ForEach(x => x.ApprovalStatus = "none");
                    approverLevels.ForEach(x => x.LastSend = null);
                    approvers.ForEach(x => x.Id = 0);
                    approvers.ForEach(x => x.ApproverStatus = "none");
                    approvers.ForEach(x => x.Remark = null);
                    approvers.ForEach(x => x.ApproveDate = null);

                    if (!String.IsNullOrEmpty(masterForm.UniqueGuidelineFile) && !String.IsNullOrEmpty(masterForm.GuidelineFile))
                    {
                        // get file path
                        var getFilePath = contentRootPath;
                        var sourceFile = Path.Combine(contentRootPath, masterForm.UniqueGuidelineFile);

                        // if file exist
                        if (System.IO.File.Exists(sourceFile))
                        {
                            var getSourceFileExtension = Path.GetExtension(sourceFile);
                            var uniqueFileName = String.Format(@"{0}-{1}{2}", Guid.NewGuid(), DateTime.Now.ToString("yyMMddHHmmssff"), getSourceFileExtension);
                            var copyFile = Path.Combine(getFilePath, uniqueFileName);

                            System.IO.File.Copy(sourceFile, copyFile);

                            masterForm.UniqueGuidelineFile = uniqueFileName;
                        }
                    }

                    // reset parent entity
                    masterForm.Id = 0;
                    masterForm.MasterFormRevision = finalRev;
                    masterForm.MasterFormStatus = "editing";
                    masterForm.Owner = user.UserName;
                    masterForm.CreatedDate = DateTime.Now;
                    masterForm.CreatedBy = user.DisplayName;
                    masterForm.OwnerCostCenter = user.CostCenterName;
                    masterForm.OwnerEmailAddress = user.Email;
                    masterForm.ModifiedDate = null;
                    masterForm.ModifiedBy = null;
                    masterForm.SubmittedDate = null;
                    masterForm.SubmittedBy = null;
                    masterForm.CurrentEditor = user.UserName;
                    masterForm.AllowUpRevision = false;
                    masterForm.RunningNumber = 1;
                    masterForm.JSON = null;
                    masterForm.PermittedDepartments = null;
                    masterForm.MasterFormParentId = masterFormParentId;

                    _context.MasterFormLists.Add(masterForm);

                    // disable master form parent up revision
                    var masterFormParent = _context.MasterFormLists.Where(x => x.Id == masterFormParentId).FirstOrDefault();

                    if (masterFormParent != null)
                    {
                        masterFormParent.AllowUpRevision = false;

                        _context.Attach(masterFormParent);
                        _context.Entry(masterFormParent).Property(x => x.AllowUpRevision).IsModified = true;
                    }

                    _context.SaveChanges();

                    return masterForm.Id;
                }
                else 
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
    }
}
