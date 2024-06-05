#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.Data.Datatable;
using WD_ERECORD_CORE.Extensions;
using WD_ERECORD_CORE.JSONModel;
using static System.Net.Mime.MediaTypeNames;

namespace WD_ERECORD_CORE.Controllers
{
    [Authorize]
    public class DataTableController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;
        private IWebHostEnvironment Environment;
        public UserManager<ApplicationUser> UserManager { get; set; }
        public RoleManager<IdentityRole> RoleManager { get; set; }

        public DataTableController(ApplicationDbContext context, IDataProtectionProvider provider, UserManager<ApplicationUser> mgr, RoleManager<IdentityRole> roleMgr, IWebHostEnvironment _environment)
        {
            _context = context;
            _protector = provider.CreateProtector("DataProtection");
            UserManager = mgr;
            RoleManager = roleMgr;
            Environment = _environment;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await UserManager.GetUserAsync(HttpContext.User);
        }

        // numberic builder
        [HttpPost]
        public async Task<IActionResult> LoadCostCenterTable([FromBody] DtParameters dtParameters)
        {
            var eformlist = _context.CostCenterLists.Where(x => x.Name != "All Department").Select(x => new CostCenterList() {
                EncryptedId = _protector.Protect(x.Id.ToString()),
                Code = x.Code,
                Name = x.Name,
                JobTitle = x.JobTitle,
                JobClassificationTrack = x.JobClassificationTrack
            });

            var searchCode = dtParameters.Columns[0].Search?.Value;
            var searchName = dtParameters.Columns[1].Search?.Value;
            var searchJobTitle = dtParameters.Columns[2].Search?.Value;
            var searchJobClassificationTrack = dtParameters.Columns[3].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = eformlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchCode))
            {
                result = result.Where(r => r.Code != null && r.Code.ToUpper().Contains(searchCode.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                result = result.Where(r => r.Name != null && r.Name.ToUpper().Contains(searchName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchJobTitle))
            {
                result = result.Where(r => r.JobTitle != null && r.JobTitle.ToUpper().Contains(searchJobTitle.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchJobClassificationTrack))
            {
                result = result.Where(r => r.JobClassificationTrack != null && r.JobClassificationTrack.ToUpper().Equals(searchJobClassificationTrack.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await eformlist.CountAsync();

            return Json(new DtResult<CostCenterList>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadDepartmentTable([FromBody] DtParameters dtParameters)
        {
            var eformlist = _context.DepartmentLists.Select(x => new DepartmentList() {
                EncryptedId = _protector.Protect(x.Id.ToString()),
                DepartmentCode = x.DepartmentCode,
                DepartmentDescription = x.DepartmentDescription
            });

            var searchDepartmentCode = dtParameters.Columns[0].Search?.Value;
            var searchDepartmentDescription = dtParameters.Columns[1].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = eformlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchDepartmentCode))
            {
                result = result.Where(r => r.DepartmentCode != null && r.DepartmentCode.ToUpper().Contains(searchDepartmentCode.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchDepartmentDescription))
            {
                result = result.Where(r => r.DepartmentDescription != null && r.DepartmentDescription.ToUpper().Contains(searchDepartmentDescription.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await eformlist.CountAsync();

            return Json(new DtResult<DepartmentList>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadSubDepartmentTable([FromBody] DtParameters dtParameters)
        {
            var eformlist = _context.SubDepartmentLists.Include(x => x.DepartmentList).Select(x => new SubDepartmentList() {
                EncryptedId = _protector.Protect(x.Id.ToString()),
                DepartmentCode = x.DepartmentList.DepartmentCode,
                SubDepartmentCode = x.SubDepartmentCode,
                SubDepartmentDescription = x.SubDepartmentDescription
            });

            var searchDepartmentCode = dtParameters.Columns[0].Search?.Value;
            var searchSubDepartmentCode = dtParameters.Columns[1].Search?.Value;
            var searchSubDepartmentDescription = dtParameters.Columns[2].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = eformlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchDepartmentCode))
            {
                result = result.Where(r => r.DepartmentCode != null && r.DepartmentCode.ToUpper().Contains(searchDepartmentCode.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchSubDepartmentCode))
            {
                result = result.Where(r => r.SubDepartmentCode != null && r.SubDepartmentCode.ToUpper().Contains(searchSubDepartmentCode.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchSubDepartmentDescription))
            {
                result = result.Where(r => r.SubDepartmentDescription != null && r.SubDepartmentDescription.ToUpper().Contains(searchSubDepartmentDescription.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await eformlist.CountAsync();

            return Json(new DtResult<SubDepartmentList>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadFactoryTable([FromBody] DtParameters dtParameters)
        {
            var eformlist = _context.FactoryLists.Select(x => new FactoryList() { 
                EncryptedId = _protector.Protect(x.Id.ToString()),
                LocationCode = x.LocationCode,
                LocationDescription = x.LocationDescription
            });

            var searchFactoryCode = dtParameters.Columns[0].Search?.Value;
            var searchFactoryDescription = dtParameters.Columns[1].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = eformlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchFactoryCode))
            {
                result = result.Where(r => r.LocationCode != null && r.LocationCode.ToUpper().Contains(searchFactoryCode.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFactoryDescription))
            {
                result = result.Where(r => r.LocationDescription != null && r.LocationDescription.ToUpper().Contains(searchFactoryDescription.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await eformlist.CountAsync();

            return Json(new DtResult<FactoryList>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadDocumentCategoryTable([FromBody] DtParameters dtParameters)
        {
            var eformlist = _context.DocumentCategoryLists.Select(x => new DocumentCategoryList() { 
                EncryptedId = _protector.Protect(x.Id.ToString()),
                CategoryCode = x.CategoryCode,
                CategoryDescription = x.CategoryDescription
            });

            var searchCategoryCode = dtParameters.Columns[0].Search?.Value;
            var searchCategoryDescription = dtParameters.Columns[1].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = eformlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchCategoryCode))
            {
                result = result.Where(r => r.CategoryCode != null && r.CategoryCode.ToUpper().Contains(searchCategoryCode.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchCategoryDescription))
            {
                result = result.Where(r => r.CategoryDescription != null && r.CategoryDescription.ToUpper().Contains(searchCategoryDescription.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await eformlist.CountAsync();

            return Json(new DtResult<DocumentCategoryList>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // user
        [HttpPost]
        public async Task<IActionResult> LoadAllUserTable([FromBody] DtParameters dtParameters)
        {
            var accountList = _context.ApplicationUsers.Include(x => x.SystemRoles).Where(x => x.UserName != "admin").Select(x => new UserJSON
            {
                UserId = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                DisplayName = x.DisplayName,
                EmployeeId = x.EmployeeId,
                EmployeeType = x.EmployeeType,
                Title = x.Title,
                Department = x.Department,
                ManagerName = x.ManagerName,
                ManagerId = x.ManagerId,
                CostCenterName = x.CostCenterName,
                CostCenterId = x.CostCenterID,
                RoleName = string.Join(", ", x.SystemRoles.Select(x => x.Name).ToArray()),
                SystemRoles = x.SystemRoles.Select(x => x.Name).ToList()
            });

            var searchUserName = dtParameters.Columns[0].Search?.Value;
            var searchEmailAddress = dtParameters.Columns[1].Search?.Value;
            var searchEmployeeId = dtParameters.Columns[2].Search?.Value;
            var searchCostCenterName = dtParameters.Columns[3].Search?.Value;
            var searchDepartment = dtParameters.Columns[4].Search?.Value;
            var searchSystemRole = dtParameters.Columns[5].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = accountList.AsQueryable();

            if (!string.IsNullOrEmpty(searchUserName))
            {
                result = result.Where(r => r.DisplayName != null && r.DisplayName.ToUpper().Contains(searchUserName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchEmailAddress))
            {
                result = result.Where(r => r.Email != null && r.Email.ToUpper().Contains(searchEmailAddress.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchEmployeeId))
            {
                result = result.Where(r => r.EmployeeId != null && r.EmployeeId.ToUpper().Contains(searchEmployeeId.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchCostCenterName))
            {
                result = result.Where(r => r.CostCenterName != null && r.CostCenterName.ToUpper().Contains(searchCostCenterName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchDepartment))
            {
                result = result.Where(r => r.Department != null && r.Department.ToUpper().Contains(searchDepartment.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchSystemRole))
            {
                result = result.Where(r => r.SystemRoles.Count() != 0 && r.SystemRoles.Contains(searchSystemRole));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await accountList.CountAsync();

            return Json(new DtResult<UserJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadUserTable([FromBody] DtParameters dtParameters)
        {
            var user = await GetCurrentUser();
            
            var accountList = _context.ApplicationUsers.Include(x => x.SystemRoles).Where(x => x.UserName != "admin" && x.CostCenterID == user.CostCenterID).Select(x => new UserJSON
            {
                UserId = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                DisplayName = x.DisplayName,
                EmployeeId = x.EmployeeId,
                EmployeeType = x.EmployeeType,
                Title = x.Title,
                Department = x.Department,
                ManagerName = x.ManagerName,
                ManagerId = x.ManagerId,
                CostCenterName = x.CostCenterName,
                CostCenterId = x.CostCenterID,
                RoleName = string.Join(", ", x.SystemRoles.Select(x => x.Name).ToArray()),
                SystemRoles = x.SystemRoles.Select(x => x.Name).ToList()
            });

            var searchUserName = dtParameters.Columns[0].Search?.Value;
            var searchEmailAddress = dtParameters.Columns[1].Search?.Value;
            var searchEmployeeId = dtParameters.Columns[2].Search?.Value;
            var searchCostCenterName = dtParameters.Columns[3].Search?.Value;
            var searchDepartment = dtParameters.Columns[4].Search?.Value;
            var searchSystemRole = dtParameters.Columns[5].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = accountList.AsQueryable();

            if (!string.IsNullOrEmpty(searchUserName))
            {
                result = result.Where(r => r.DisplayName != null && r.DisplayName.ToUpper().Contains(searchUserName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchEmailAddress))
            {
                result = result.Where(r => r.Email != null && r.Email.ToUpper().Contains(searchEmailAddress.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchEmployeeId))
            {
                result = result.Where(r => r.EmployeeId != null && r.EmployeeId.ToUpper().Contains(searchEmployeeId.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchCostCenterName))
            {
                result = result.Where(r => r.CostCenterName != null && r.CostCenterName.ToUpper().Contains(searchCostCenterName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchDepartment))
            {
                result = result.Where(r => r.Department != null && r.Department.ToUpper().Contains(searchDepartment.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchSystemRole))
            {
                result = result.Where(r => r.SystemRoles.Count() != 0 && r.SystemRoles.Contains(searchSystemRole));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await accountList.CountAsync();

            return Json(new DtResult<UserJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // master form
        [HttpPost]
        public async Task<IActionResult> LoadMasterFormTable([FromBody] DtParameters dtParameters)
        {

            var formlist = _context.MasterFormLists.Include(x => x.MasterFormDepartments).Select(x => new MasterFormJSON
            {
                Id = x.Id,
                MasterFormName = x.MasterFormName,
                MasterFormDescription = x.MasterFormDescription,
                MasterFormRevision = x.MasterFormRevision,
                MasterFormStatus = x.MasterFormStatus,
                Departments = String.Join(", ", x.MasterFormDepartments.Select(x => x.DepartmentName).ToList()),
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate,
                Editor = x.CurrentEditor,
                AllowUpRevision = x.AllowUpRevision,
                TimeLineData = x.JSON,
                Owner = x.Owner,
                ChangeLog = x.ChangeLog
            });

            // var searchBy = dtParameters.Search?.Value;
            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;
            var searchFormStatus = dtParameters.Columns[2].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[3].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[4].Search?.Value;
            var searchOwner = dtParameters.Columns[5].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.MasterFormName != null && r.MasterFormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.MasterFormDescription != null && r.MasterFormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.MasterFormStatus != null && (r.MasterFormStatus.ToUpper() == searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!string.IsNullOrEmpty(searchOwner))
            {
                result = result.Where(r => (r.Owner != null && r.Owner.ToUpper().Equals(searchOwner.ToUpper()) || (r.CreatedBy != null && r.CreatedBy.ToUpper().Contains(searchOwner.ToUpper()))));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formlist.CountAsync();

            return Json(new DtResult<MasterFormJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // ccb master form approval
        [HttpPost]
        [Authorize(Roles = "System Admin")]
        public async Task<IActionResult> LoadAllCCBApprovalTable([FromBody] DtParameters dtParameters)
        {
            // first filtering, find all departments, approval levels and approvers with pending status
            var filter = _context.MasterFormLists.Where(x => x.MasterFormStatus == "pending").Include(x => x.MasterFormDepartments.Where(x => x.Status == "pending")).ThenInclude(x => x.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "pending")).ThenInclude(x => x.MasterFormCCBApprovers.Where(x => x.ApproverStatus == "pending"));
            var formList = filter.SelectMany(x => x.MasterFormDepartments.Where(x => x.Status == "pending").SelectMany(x => x.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "pending").SelectMany(x => x.MasterFormCCBApprovers.Where(x => x.ApproverStatus == "pending"))), 
                    (p, c) => new MasterFormCCBApprovalJSON { 
                        Id = p.Id, 
                        MasterFormName = p.MasterFormName, 
                        MasterFormDescription = p.MasterFormDescription, 
                        MasterFormRevision = p.MasterFormRevision, 
                        MasterFormStatus = p.MasterFormStatus, 
                        CreatedDate = p.CreatedDate, 
                        CreatedBy = p.CreatedBy, 
                        ModifiedDate = p.ModifiedDate, 
                        ModifiedBy = p.ModifiedBy, 
                        SubmittedDate = p.SubmittedDate, 
                        SubmittedBy = p.SubmittedBy, 
                        ApproverName = c.ApproverName, 
                        ApproverStatus = c.ApproverStatus, 
                        ApproverId = c.Id, 
                        DepartmentName = c.MasterFormCCBApprovalLevel.MasterFormDepartment.DepartmentName, 
                        Remark = c.Remark, 
                        ChangeLog = p.ChangeLog, 
                        TimeLineData = p.JSON
                    });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;
            var searchFormStatus = dtParameters.Columns[2].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[3].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[4].Search?.Value;
            var searchMasterFormId = dtParameters.Columns[5].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formList.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.MasterFormName != null && r.MasterFormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.MasterFormDescription != null && r.MasterFormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.MasterFormStatus != null && r.MasterFormStatus.ToUpper().Contains(searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchMasterFormId))
            {
                result = result.Where(r => r.Id == int.Parse(searchMasterFormId));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formList.CountAsync();

            return Json(new DtResult<MasterFormCCBApprovalJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        [Authorize(Roles = "CCB Approver")]
        public async Task<IActionResult> LoadCCBApprovalTable([FromBody] DtParameters dtParameters)
        {
            var User = await GetCurrentUser();

            // first filtering, find all departments, approval levels and approvers with pending status
            var filter = _context.MasterFormLists.Where(x => x.MasterFormStatus == "pending").Include(x => x.MasterFormDepartments.Where(x => x.Status == "pending")).ThenInclude(x => x.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "pending")).ThenInclude(x => x.MasterFormCCBApprovers.Where(x => x.ApproverStatus == "pending" && x.EmployeeId == User.UserName));
            var formList = filter.SelectMany(x => x.MasterFormDepartments.Where(x => x.Status == "pending").SelectMany(x => x.MasterFormCCBApprovalLevels.Where(x => x.ApprovalStatus == "pending").SelectMany(x => x.MasterFormCCBApprovers.Where(x => x.ApproverStatus == "pending" && x.EmployeeId == User.UserName))),
                    (p, c) => new MasterFormCCBApprovalJSON
                    {
                        Id = p.Id,
                        MasterFormName = p.MasterFormName,
                        MasterFormDescription = p.MasterFormDescription,
                        MasterFormRevision = p.MasterFormRevision,
                        MasterFormStatus = p.MasterFormStatus,
                        CreatedDate = p.CreatedDate,
                        CreatedBy = p.CreatedBy,
                        ModifiedDate = p.ModifiedDate,
                        ModifiedBy = p.ModifiedBy,
                        SubmittedDate = p.SubmittedDate,
                        SubmittedBy = p.SubmittedBy,
                        ApproverName = c.ApproverName,
                        ApproverStatus = c.ApproverStatus,
                        ApproverId = c.Id,
                        DepartmentName = c.MasterFormCCBApprovalLevel.MasterFormDepartment.DepartmentName,
                        Remark = c.Remark,
                        ChangeLog = p.ChangeLog,
                        TimeLineData = p.JSON
                    });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;
            var searchFormStatus = dtParameters.Columns[2].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[3].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[4].Search?.Value;
            var searchMasterFormId = dtParameters.Columns[5].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formList.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.MasterFormName != null && r.MasterFormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.MasterFormDescription != null && r.MasterFormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.MasterFormStatus != null && r.MasterFormStatus.ToUpper().Contains(searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchMasterFormId))
            {
                result = result.Where(r => r.Id == int.Parse(searchMasterFormId));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formList.CountAsync();

            return Json(new DtResult<MasterFormCCBApprovalJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // user manual
        [HttpPost]
        public async Task<IActionResult> LoadUserManualTable([FromBody] DtParameters dtParameters)
        {
            var userManualtlist = _context.UserManualLists.Select(x => new UserManualJSON
            {
                Id = x.Id,
                UserManualName = x.UserManualName,
                UserManualDescription = x.UserManualDescription,
                Status = x.Status,
                DateCreated = x.DateCreated,
                CreatedBy = x.CreatedBy,
                LatestUpdatedDate = x.LatestUpdatedDate,
                LatestUpdatedBy = x.LatestUpdatedBy,
                UserManualFilePath = x.UserManualFilePath
            });

            var searchBy = dtParameters.Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = userManualtlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.UserManualName != null && r.UserManualName.ToUpper().Contains(searchBy.ToUpper()) ||
                    r.UserManualDescription != null && r.UserManualDescription.ToUpper().Contains(searchBy.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await userManualtlist.CountAsync();

            return Json(new DtResult<UserManualJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
    }

        // master form selection
        [HttpPost]
        public async Task<IActionResult> LoadAllMasterFormSelectionTable([FromBody] DtParameters dtParameters)
        {
            var masterFormLists = _context.MasterFormLists.Where(x => x.MasterFormStatus == "active").Select(x => new SelectMasterFormListJSON()
                {
                    Id = x.Id,
                    MasterFormName = x.MasterFormName,
                    MasterFormDescription = x.MasterFormDescription,
                    MasterFormRevision = x.MasterFormRevision
                });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = masterFormLists.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.MasterFormName != null && r.MasterFormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.MasterFormDescription != null && r.MasterFormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await masterFormLists.CountAsync();

            return Json(new DtResult<SelectMasterFormListJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadMasterFormSelectionTable([FromBody] DtParameters dtParameters)
        {
            var user = await GetCurrentUser();
            var userCostCenter = "";
            var allowedDepartment = new List<string>() { "All Department" };

            if (user == null)
            {
                return BadRequest();
            }
            else
            {
                userCostCenter = !String.IsNullOrEmpty(user.CostCenterName) ? user.CostCenterName : "";

                if (!String.IsNullOrEmpty(user.CostCenterName))
                {
                    allowedDepartment.Add(userCostCenter);
                }
            }

            /*            var rawSQL = "";

                        if (Environment.IsProduction())
                        {
                            rawSQL = "SELECT * FROM \"ERECORD\".\"MasterFormLists\" WHERE \"MasterFormStatus\" = 'active' AND json_exists(\"PermittedDepartments\", '$[*]?(@ == \"" + userCostCenter + "\" || @ == \"All Department\")')";
                        }
                        else
                        {
                            rawSQL = "SELECT * FROM \"SYSTEM\".\"MasterFormLists\" WHERE \"MasterFormStatus\" = 'active' AND json_exists(\"PermittedDepartments\", '$[*]?(@ == \"" + userCostCenter + "\" || @ == \"All Department\")')";
                        }

                        var masterFormLists = _context.MasterFormLists.FromSqlRaw(rawSQL).Select(x => new SelectMasterFormListJSON()
                        {
                            Id = x.Id,
                            MasterFormName = x.MasterFormName,
                            MasterFormDescription = x.MasterFormDescription,
                            MasterFormRevision = x.MasterFormRevision
                        });*/

            var masterFormLists = _context.MasterFormLists.Include(x => x.MasterFormDepartments).Where(x => x.MasterFormStatus == "active" && x.MasterFormDepartments.Any(x => allowedDepartment.Contains(x.DepartmentName))).Select(x => new SelectMasterFormListJSON()
            {
                Id = x.Id,
                MasterFormName = x.MasterFormName,
                MasterFormDescription = x.MasterFormDescription,
                MasterFormRevision = x.MasterFormRevision
            });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = masterFormLists.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.MasterFormName != null && r.MasterFormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.MasterFormDescription != null && r.MasterFormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await masterFormLists.CountAsync();

            return Json(new DtResult<SelectMasterFormListJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // form
        [HttpPost]
        public async Task<IActionResult> LoadFormTable([FromBody] DtParameters dtParameters)
        {
            var User = await GetCurrentUser();

            var eformlist = _context.FormLists.Where(x => x.Owner == User.UserName).Where(x => x.ExpiredDate >= DateTime.Now || x.ExpiredDate == null).Select(x => new FormJSON()
            {
                Id = x.Id,
                FormName = x.FormName,
                FormDescription = x.FormDescription,
                FormStatus = x.FormStatus,
                FormRevision = x.FormRevision,
                JSON = x.JSON,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate,
                ModifiedBy = x.ModifiedBy,
            });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;
            var searchFormStatus = dtParameters.Columns[2].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[3].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[4].Search?.Value;
            var searchFormId = dtParameters.Columns[5].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = eformlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.FormName != null && r.FormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.FormDescription != null && r.FormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.FormStatus != null && r.FormStatus.ToUpper().Contains(searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormId))
            {
                result = result.Where(r => r.Id == int.Parse(searchFormId));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await eformlist.CountAsync();

            return Json(new DtResult<FormJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // form approval
        [HttpPost]
        [Authorize(Roles = "System Admin")]
        public async Task<IActionResult> LoadAllFormApprovalTable([FromBody] DtParameters dtParameters)
        {
            // first filtering, find all departments, approval levels and approvers with pending status
            var filter = _context.FormLists.Where(x => x.FormStatus == "pending").Include(x => x.FormListApprovalLevels.Where(x => x.ApprovalStatus == "pending")).ThenInclude(x => x.FormListApprovers.Where(x => x.ApproverStatus == "pending"));
            var formList = filter.SelectMany(x => x.FormListApprovalLevels.Where(x => x.ApprovalStatus == "pending").SelectMany(x => x.FormListApprovers.Where(x => x.ApproverStatus == "pending")),
                    (p, c) => new FormApprovalJSON
                    {
                        Id = p.Id,
                        ApproverId = c.Id,
                        ApproverName = c.ApproverName,
                        FormName = p.FormName,
                        FormDescription = p.FormDescription,
                        FormRevision = p.FormRevision,
                        FormStatus = p.FormStatus,
                        ApproverStatus = c.ApproverStatus,
                        CreatedDate = p.CreatedDate,
                        CreatedBy = p.CreatedBy,
                        ModifiedDate = p.ModifiedDate,
                        ModifiedBy = p.ModifiedBy,
                        JSON = p.JSON
                    });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;
            var searchFormStatus = dtParameters.Columns[2].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[3].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[4].Search?.Value;
            var searchFormId = dtParameters.Columns[5].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formList.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.FormName != null && r.FormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.FormDescription != null && r.FormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.FormStatus != null && r.FormStatus.ToUpper().Contains(searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormId))
            {
                result = result.Where(r => r.Id == int.Parse(searchFormId));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formList.CountAsync();

            return Json(new DtResult<FormApprovalJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        [Authorize(Roles = "Normal User")]
        public async Task<IActionResult> LoadFormApprovalTable([FromBody] DtParameters dtParameters)
        {
            var User = await GetCurrentUser();

            // first filtering, find all departments, approval levels and approvers with pending status
            var filter = _context.FormLists.Where(x => x.FormStatus == "pending").Include(x => x.FormListApprovalLevels.Where(x => x.ApprovalStatus == "pending")).ThenInclude(x => x.FormListApprovers.Where(x => x.ApproverStatus == "pending"));
            var formList = filter.SelectMany(x => x.FormListApprovalLevels.Where(x => x.ApprovalStatus == "pending").SelectMany(x => x.FormListApprovers.Where(x => x.ApproverStatus == "pending" && x.EmployeeId == User.UserName)),
                    (p, c) => new FormApprovalJSON
                    {
                        Id = p.Id,
                        ApproverId = c.Id,
                        ApproverName = c.ApproverName,
                        FormName = p.FormName,
                        FormDescription = p.FormDescription,
                        FormRevision = p.FormRevision,
                        FormStatus = p.FormStatus,
                        ApproverStatus = c.ApproverStatus,
                        CreatedDate = p.CreatedDate,
                        CreatedBy = p.CreatedBy,
                        ModifiedDate = p.ModifiedDate,
                        ModifiedBy = p.ModifiedBy,
                        JSON = p.JSON
                    });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;
            var searchFormStatus = dtParameters.Columns[2].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[3].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[4].Search?.Value;
            var searchFormId = dtParameters.Columns[5].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formList.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.FormName != null && r.FormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.FormDescription != null && r.FormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.FormStatus != null && r.FormStatus.ToUpper().Contains(searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormId))
            {
                result = result.Where(r => r.Id == int.Parse(searchFormId));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formList.CountAsync();

            return Json(new DtResult<FormApprovalJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // master form history
        public async Task<IActionResult> LoadAllMasterFormHistoryTable([FromBody] DtParameters dtParameters)
        {

            var formlist = _context.MasterFormListHistories.Select(x => new AllMasterFormHistoryJSON { 
                Id = x.Id,
                MasterFormName = x.MasterFormName,
                MasterFormDescription = x.MasterFormDescription,
                MasterFormRevision = x.MasterFormRevision,
                MasterFormStatus = x.MasterFormStatus,
                JSON = x.JSON,
                ChangeLog = x.ChangeLog,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate,
                ModifiedBy = x.ModifiedBy
            });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;
            var searchFormStatus = dtParameters.Columns[2].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[3].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[4].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.MasterFormName != null && r.MasterFormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.MasterFormDescription != null && r.MasterFormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.MasterFormStatus != null && (r.MasterFormStatus.ToUpper() == searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }


            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formlist.CountAsync();

            return Json(new DtResult<AllMasterFormHistoryJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        public async Task<IActionResult> LoadMasterFormHistoryTable([FromBody] DtParameters dtParameters)
        {
            var user = await GetCurrentUser();

            if (user == null) {
                return NotFound();
            }

            var rawSQL = "";

            if (Environment.IsProduction())
            {
                rawSQL = "SELECT * FROM \"ERECORD\".\"MasterFormListHistories\" WHERE json_exists(\"CCBApprovalJSON\", '$[*]?(@ == \"" + user.UserName.ToString() + "\")')";
            }
            else
            {
                rawSQL = "SELECT * FROM \"SYSTEM\".\"MasterFormListHistories\" WHERE json_exists(\"CCBApprovalJSON\", '$[*]?(@ == \"" + user.UserName.ToString() + "\")')";
            }

            var formlist = _context.MasterFormListHistories.FromSqlRaw(rawSQL).Select(x => new AllMasterFormHistoryJSON
            {
                Id = x.Id,
                MasterFormName = x.MasterFormName,
                MasterFormDescription = x.MasterFormDescription,
                MasterFormRevision = x.MasterFormRevision,
                MasterFormStatus = x.MasterFormStatus,
                JSON = x.JSON,
                ChangeLog = x.ChangeLog,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate,
                ModifiedBy = x.ModifiedBy
            });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormDescription = dtParameters.Columns[1].Search?.Value;
            var searchFormStatus = dtParameters.Columns[2].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[3].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[4].Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.MasterFormName != null && r.MasterFormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormDescription))
            {
                result = result.Where(r => r.MasterFormDescription != null && r.MasterFormDescription.ToUpper().Contains(searchFormDescription.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.MasterFormStatus != null && (r.MasterFormStatus.ToUpper() == searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }


            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formlist.CountAsync();

            return Json(new DtResult<AllMasterFormHistoryJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // form history
        [HttpPost]
        public async Task<IActionResult> LoadAdminFormHistoryTable([FromBody] DtParameters dtParameters)
        {
            var formListHistory = _context.FormListHistories.Select(x => new FormListHistoryJSON() { 
                Id = x.Id,
                FormName = x.FormName,
                FormDescription = x.FormDescription,
                FormRevision = x.FormRevision,
                FormStatus = x.FormStatus,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate,
                ModifiedBy = x.ModifiedBy,
                JSON = x.JSON
            });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormStatus = dtParameters.Columns[1].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[2].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[3].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formListHistory.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.FormName != null && r.FormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.FormStatus != null && r.FormStatus.ToUpper().Contains(searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formListHistory.CountAsync();

            return Json(new DtResult<FormListHistoryJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadL1AdminFormHistoryTable([FromBody] DtParameters dtParameters)
        {
            var user = await GetCurrentUser();

            if (user == null)
            {
                return NotFound();
            }

            var rawSQL = "";

            if (Environment.IsProduction())
            {
                rawSQL = "SELECT * FROM \"ERECORD\".\"FormListHistories\" WHERE \"Owner\" = '" + user.UserName.ToString() + "' OR json_exists(\"MasterFormDetail\", '$?(@.MasterFormOwner == \"" + user.UserName.ToString() + "\")')";
            }
            else
            {
                rawSQL = "SELECT * FROM \"SYSTEM\".\"FormListHistories\" WHERE \"Owner\" = '" + user.UserName.ToString() + "' OR json_exists(\"MasterFormDetail\", '$?(@.MasterFormOwner == \"" + user.UserName.ToString() + "\")')";
            }

            var formListHistory = _context.FormListHistories.FromSqlRaw(rawSQL).Select(x => new FormListHistoryJSON()
            {
                Id = x.Id,
                FormName = x.FormName,
                FormDescription = x.FormDescription,
                FormRevision = x.FormRevision,
                FormStatus = x.FormStatus,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate,
                ModifiedBy = x.ModifiedBy,
                JSON = x.JSON
            });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormStatus = dtParameters.Columns[1].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[2].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[3].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formListHistory.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.FormName != null && r.FormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.FormStatus != null && r.FormStatus.ToUpper().Contains(searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formListHistory.CountAsync();

            return Json(new DtResult<FormListHistoryJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadUserFormHistoryTable([FromBody] DtParameters dtParameters)
        {
            var user = await GetCurrentUser();
            
            if (user == null)
            {
                return NotFound();
            }
            
            var formListHistory = _context.FormListHistories.Where(x => x.Owner == user.UserName).Select(x => new FormListHistoryJSON()
            {
                Id = x.Id,
                FormName = x.FormName,
                FormDescription = x.FormDescription,
                FormRevision = x.FormRevision,
                FormStatus = x.FormStatus,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate,
                ModifiedBy = x.ModifiedBy,
                JSON = x.JSON
            });

            var searchFormName = dtParameters.Columns[0].Search?.Value;
            var searchFormStatus = dtParameters.Columns[1].Search?.Value;
            var searchFormCreatedDate = dtParameters.Columns[2].Search?.Value;
            var searchFormModifiedDate = dtParameters.Columns[3].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = formListHistory.AsQueryable();

            if (!string.IsNullOrEmpty(searchFormName))
            {
                result = result.Where(r => r.FormName != null && r.FormName.ToUpper().Contains(searchFormName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchFormStatus))
            {
                result = result.Where(r => r.FormStatus != null && r.FormStatus.ToUpper().Contains(searchFormStatus.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchFormCreatedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormCreatedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.CreatedDate.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.CreatedDate.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.CreatedDate.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            if (!String.IsNullOrEmpty(searchFormModifiedDate))
            {
                char seperator = '|';
                string[] datelist = searchFormModifiedDate.Split(seperator);

                if (String.IsNullOrEmpty(datelist[1]))
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date == DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null));
                }
                else
                {
                    result = result.Where(r => r.ModifiedDate.Value.Date >= DateTime.ParseExact(datelist[0], "MM/dd/yyyy", null) && r.ModifiedDate.Value.Date <= DateTime.ParseExact(datelist[1], "MM/dd/yyyy", null));
                }
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await formListHistory.CountAsync();

            return Json(new DtResult<FormListHistoryJSON>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // public history
        [HttpPost]
        public async Task<IActionResult> LoadPublicLogSystemTable([FromBody] DtParameters dtParameters)
        {
            var publicLogSystem = _context.PublicLogSystems;
            /*
                        var searchBy = dtParameters.Search?.Value;*/
            var searchApprovalId = dtParameters.Columns[0].Search?.Value;
            var searchStartDate = dtParameters.Columns[1].Search?.Value;
            var searchEndDate = dtParameters.Columns[2].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = publicLogSystem.AsQueryable();

            if (!string.IsNullOrEmpty(searchApprovalId))
            {
                result = result.Where(r => r.UserName != null && r.UserName.ToUpper().Contains(searchApprovalId.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchStartDate) && !String.IsNullOrEmpty(searchEndDate))
            {
                result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null) && r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
            }
            else
            {
                if (!String.IsNullOrEmpty(searchStartDate))
                {
                    result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null));
                }
                else if (!String.IsNullOrEmpty(searchEndDate))
                {
                    result = result.Where(r => r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
                }
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await publicLogSystem.CountAsync();

            return Json(new DtResult<PublicLogSystem>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadPublicLogApprovalTable([FromBody] DtParameters dtParameters)
        {
            var publicLogApproval = _context.PublicFormApprovals;

            /*            var searchBy = dtParameters.Search?.Value;*/
            var searchApprovalId = dtParameters.Columns[0].Search?.Value;
            var searchStartDate = dtParameters.Columns[1].Search?.Value;
            var searchEndDate = dtParameters.Columns[2].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = publicLogApproval.AsQueryable();

            if (!string.IsNullOrEmpty(searchApprovalId))
            {
                result = result.Where(r => r.UserId != null && r.UserId.ToUpper().Contains(searchApprovalId.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchStartDate) && !String.IsNullOrEmpty(searchEndDate))
            {
                result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null) && r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
            }
            else
            {
                if (!String.IsNullOrEmpty(searchStartDate))
                {
                    result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null));
                }
                else if (!String.IsNullOrEmpty(searchEndDate))
                {
                    result = result.Where(r => r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
                }
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await publicLogApproval.CountAsync();

            return Json(new DtResult<PublicLogApproval>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadPublicLogEmailNotificationTable([FromBody] DtParameters dtParameters)
        {
            var publicLogEmailNotification = _context.PublicLogEmailNotifications;

            /*            var searchBy = dtParameters.Search?.Value;*/
            var searchStartDate = dtParameters.Columns[0].Search?.Value;
            var searchEndDate = dtParameters.Columns[1].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = publicLogEmailNotification.AsQueryable();

            if (!String.IsNullOrEmpty(searchStartDate) && !String.IsNullOrEmpty(searchEndDate))
            {
                result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null) && r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
            }
            else
            {
                if (!String.IsNullOrEmpty(searchStartDate))
                {
                    result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null));
                }
                else if (!String.IsNullOrEmpty(searchEndDate))
                {
                    result = result.Where(r => r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
                }
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await publicLogEmailNotification.CountAsync();

            return Json(new DtResult<PublicLogEmailNotification>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadPublicLogActivityTable([FromBody] DtParameters dtParameters)
        {
            var publicLogActivity = _context.PublicLogActivities;

            /*            var searchBy = dtParameters.Search?.Value;*/
            var searchDeviceId = dtParameters.Columns[0].Search?.Value;
            var searchStartDate = dtParameters.Columns[1].Search?.Value;
            var searchEndDate = dtParameters.Columns[2].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = publicLogActivity.AsQueryable();

            if (!string.IsNullOrEmpty(searchDeviceId))
            {
                result = result.Where(r => r.UserId != null && r.UserId.ToUpper().Contains(searchDeviceId.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchStartDate) && !String.IsNullOrEmpty(searchEndDate))
            {
                result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null) && r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
            }
            else
            {
                if (!String.IsNullOrEmpty(searchStartDate))
                {
                    result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null));
                }
                else if (!String.IsNullOrEmpty(searchEndDate))
                {
                    result = result.Where(r => r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
                }
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await publicLogActivity.CountAsync();

            return Json(new DtResult<PublicLogActivity>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoadPublicLogTransferFormTable([FromBody] DtParameters dtParameters)
        {
            var publicLogActivity = _context.PublicLogTransferForms;

            /*            var searchBy = dtParameters.Search?.Value;*/
            var searchDeviceId = dtParameters.Columns[0].Search?.Value;
            var searchStartDate = dtParameters.Columns[1].Search?.Value;
            var searchEndDate = dtParameters.Columns[2].Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = publicLogActivity.AsQueryable();

            if (!string.IsNullOrEmpty(searchDeviceId))
            {
                result = result.Where(r => r.UserId != null && r.UserId.ToUpper().Contains(searchDeviceId.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchStartDate) && !String.IsNullOrEmpty(searchEndDate))
            {
                result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null) && r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
            }
            else
            {
                if (!String.IsNullOrEmpty(searchStartDate))
                {
                    result = result.Where(r => r.DateTime.Date >= DateTime.ParseExact(searchStartDate, "MM/dd/yyyy", null));
                }
                else if (!String.IsNullOrEmpty(searchEndDate))
                {
                    result = result.Where(r => r.DateTime.Date <= DateTime.ParseExact(searchEndDate, "MM/dd/yyyy", null));
                }
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await publicLogActivity.CountAsync();

            return Json(new DtResult<PublicLogTransferForm>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        // transfer ownership
        [HttpPost]
        public async Task<IActionResult> LoadTransferOwnershipTable([FromBody] DtParameters dtParameters, [FromQuery] List<string> myparam)
        {
            if (myparam.Count != 0)
            {
                var formlist = _context.MasterFormLists.Where(x => myparam.Contains(x.Owner)).OrderBy(x => x.Owner).Select(x => new MasterFormTransferJSON() { 
                    Id = x.Id,
                    MasterFormName = x.MasterFormName,
                    MasterFormDescription = x.MasterFormDescription,
                    MasterFormRevision = x.MasterFormRevision,
                    MasterFormStatus = x.MasterFormStatus,
                    Owner = x.Owner,
                    CreatedBy = x.CreatedBy
                });

                var searchBy = dtParameters.Search?.Value;

                var orderCriteria = "Id";
                var orderAscendingDirection = true;

                if (dtParameters.Order != null)
                {
                    // in this example we just default sort on the 1st column
                    orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                    orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
                }

                var result = formlist.AsQueryable();

                result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

                if (!string.IsNullOrEmpty(searchBy))
                {
                    result = result.Where(r => r.MasterFormName != null && r.MasterFormName.ToUpper().Contains(searchBy.ToUpper()) ||
                        r.MasterFormDescription != null && r.MasterFormDescription.ToUpper().Contains(searchBy.ToUpper())
                    );
                }

                // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
                var filteredResultsCount = await result.CountAsync();
                var totalResultsCount = await formlist.CountAsync();

                return Json(new DtResult<MasterFormTransferJSON>
                {
                    Draw = dtParameters.Draw,
                    RecordsTotal = totalResultsCount,
                    RecordsFiltered = filteredResultsCount,
                    Data = await result
                        .Skip(dtParameters.Start)
                        .Take(dtParameters.Length)
                        .ToListAsync()
                });
            }
            else
            {
                var formlist = _context.MasterFormLists.Where(x => x.Owner == "Return Null Only").Select(x => new MasterFormTransferJSON()
                {
                    Id = x.Id,
                    MasterFormName = x.MasterFormName,
                    MasterFormDescription = x.MasterFormDescription,
                    MasterFormRevision = x.MasterFormRevision,
                    MasterFormStatus = x.MasterFormStatus,
                    Owner = x.Owner,
                    CreatedBy = x.CreatedBy
                });

                var searchBy = dtParameters.Search?.Value;

                var orderCriteria = "Id";
                var orderAscendingDirection = true;

                if (dtParameters.Order != null)
                {
                    // in this example we just default sort on the 1st column
                    orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                    orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
                }

                var result = formlist.AsQueryable();

                result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

                // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
                var filteredResultsCount = await result.CountAsync();
                var totalResultsCount = await formlist.CountAsync();

                return Json(new DtResult<MasterFormTransferJSON>
                {
                    Draw = dtParameters.Draw,
                    RecordsTotal = totalResultsCount,
                    RecordsFiltered = filteredResultsCount,
                    Data = await result
                        .Skip(dtParameters.Start)
                        .Take(dtParameters.Length)
                        .ToListAsync()
                });
            }
        }

        // announcement
        [HttpPost]
        public async Task<IActionResult> LoadAnnouncementTable([FromBody] DtParameters dtParameters)
        {
            var announcementlist = _context.AnnouncementLists;
            var searchBy = dtParameters.Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = announcementlist.AsQueryable();

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.Label1 != null && r.Label1.ToUpper().Contains(searchBy.ToUpper()) ||
                    r.Label2 != null && r.Label2.ToUpper().Contains(searchBy.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await announcementlist.CountAsync();

            return Json(new DtResult<AnnouncementList>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }
    }
}
