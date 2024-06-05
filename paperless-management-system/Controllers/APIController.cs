#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Controllers
{
    [Authorize]
    [Route("api/[action]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public APIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public JsonResult GetCostCenter(string CostCenterName)
        {
            var selectedData = _context.CostCenterLists.Where(x => x.Name != "All Department").Select(x => x.Name).Distinct().Select(x => new {
                id = x,
                value = x,
                text = x
            });

            if (!(string.IsNullOrEmpty(CostCenterName) || string.IsNullOrWhiteSpace(CostCenterName)))
            {
                selectedData = selectedData.Where(x => x.value.ToLower().Contains(CostCenterName.ToLower()));
            }

            return new JsonResult(selectedData);
        }

        [HttpGet]
        public JsonResult GetApproverEmail(string CostCenterName, string Approver, string ApproverRole)
        {
            if (!(String.IsNullOrEmpty(CostCenterName) || String.IsNullOrWhiteSpace(CostCenterName)))
            {
                var selectedData = _context.Users.Include(x => x.SystemRoles).Where(x => x.UserName != "admin" && x.SystemRoles.Select(x => x.Name).ToArray().Contains(ApproverRole)).Where(x => x.CostCenterName == CostCenterName).Select(x => new
                {
                    id = x.Email,
                    value = x.Email,
                    text = x.Email + " - " + x.DisplayName + " - " + x.UserName
                });

                if (!(string.IsNullOrEmpty(Approver) || string.IsNullOrWhiteSpace(Approver)))
                {
                    selectedData = selectedData.Where(x => x.text.ToLower().Contains(Approver.ToLower()));
                }

                return new JsonResult(selectedData);
            }
            else
            {
                var selectedData = _context.Users.Where(x => x.UserName != "admin" && x.SystemRoles.Select(x => x.Name).ToArray().Contains(ApproverRole)).Select(x => new {
                    id = x.Email,
                    value = x.Email,
                    text = x.Email + " - " + x.DisplayName + " - " + x.UserName
                });

                if (!(string.IsNullOrEmpty(Approver) || string.IsNullOrWhiteSpace(Approver)))
                {
                    selectedData = selectedData.Where(x => x.text.ToLower().Contains(Approver.ToLower()));
                }

                return new JsonResult(selectedData);
            }
        }

        [HttpGet]
        public JsonResult GetDepartmentLists(string departmentName)
        {
            var selectedData = _context.DepartmentLists.Select(x => new
            {
                id = x.DepartmentCode,
                value = x.DepartmentCode,
                text = x.DepartmentCode + " - " + x.DepartmentDescription
            });

            //if you need to make search item work,add the following code
            if (!(string.IsNullOrEmpty(departmentName) || string.IsNullOrWhiteSpace(departmentName)))
            {
                selectedData = selectedData.Where(x => x.text.ToLower().Contains(departmentName.ToLower()));
            }

            return new JsonResult(selectedData);
        }

        [HttpGet]
        public JsonResult GetSubDepartmentLists(string departmentCode, string subDepartmentName)
        {
            var selectedData = _context.SubDepartmentLists.Include(x => x.DepartmentList).Select(x => new
            {
                id = x.DepartmentList.DepartmentCode + "-" + x.SubDepartmentCode,
                value = x.DepartmentList.DepartmentCode + "-" + x.SubDepartmentCode,
                text = x.DepartmentList.DepartmentCode + "-" + x.SubDepartmentCode + " - " + x.SubDepartmentDescription
            });

            if (!(string.IsNullOrEmpty(departmentCode) || string.IsNullOrWhiteSpace(departmentCode)))
            {
                selectedData = selectedData.Where(x => x.text.StartsWith(departmentCode.ToLower()));
            }

            if (!(string.IsNullOrEmpty(subDepartmentName) || string.IsNullOrWhiteSpace(subDepartmentName)))
            {
                selectedData = selectedData.Where(x => x.text.ToLower().Contains(subDepartmentName.ToLower()));
            }

            return new JsonResult(selectedData);
        }

        [HttpGet]
        public JsonResult GetFactoryLists(string factoryName)
        {
            var selectedData = _context.FactoryLists.Select(x => new
            {
                id = x.LocationCode,
                value = x.LocationCode,
                text = x.LocationCode + " - " + x.LocationDescription
            });

            //if you need to make search item work,add the following code
            if (!(string.IsNullOrEmpty(factoryName) || string.IsNullOrWhiteSpace(factoryName)))
            {
                selectedData = selectedData.Where(x => x.text.ToLower().Contains(factoryName.ToLower()));
            }

            return new JsonResult(selectedData);
        }

        [HttpGet]
        public JsonResult GetDocumentCategoryLists(string documentCategoryName)
        {
            var selectedData = _context.DocumentCategoryLists.Select(x => new
            {
                id = x.CategoryCode,
                value = x.CategoryCode,
                text = x.CategoryCode + " - " + x.CategoryDescription
            });

            //if you need to make search item work,add the following code
            if (!(string.IsNullOrEmpty(documentCategoryName) || string.IsNullOrWhiteSpace(documentCategoryName)))
            {
                selectedData = selectedData.Where(x => x.text.ToLower().Contains(documentCategoryName.ToLower()));
            }

            return new JsonResult(selectedData);
        }

        [HttpGet]
        public JsonResult GetFormApproverLists(string documentCategoryName)
        {
            var selectedData = _context.DocumentCategoryLists.Select(x => new
            {
                id = x.CategoryCode,
                value = x.CategoryCode,
                text = x.CategoryCode + " - " + x.CategoryDescription
            });

            //if you need to make search item work,add the following code
            if (!(string.IsNullOrEmpty(documentCategoryName) || string.IsNullOrWhiteSpace(documentCategoryName)))
            {
                selectedData = selectedData.Where(x => x.text.ToLower().Contains(documentCategoryName.ToLower()));
            }

            return new JsonResult(selectedData);
        }

        public class CostCenterListsJSON
        {
            public string id { get; set; }
            public string value { get; set; }
            public string text { get; set; }
        }

        [HttpGet]
        public IActionResult GetCostCenterLists(string costCenterName)
        {
            var selectedData = _context.CostCenterLists.Select(x => new CostCenterListsJSON
            {
                id = x.Name,
                value = x.Name,
                text = x.Name == "All Department" ? x.Name : x.Name + " - " + x.Code
            }).Distinct();

            //if you need to make search item work,add the following code
            if (!(string.IsNullOrEmpty(costCenterName) || string.IsNullOrWhiteSpace(costCenterName)))
            {
                selectedData = selectedData.Where(x => x.text.ToLower().Contains(costCenterName.ToLower()));
            }

            return new JsonResult(selectedData);
        }


        [HttpGet]
        public IActionResult GetMasterFormOwner(string FindOwner)
        {
            var Owner = _context.MasterFormLists.Select(x => new { Owner = x.Owner, Name = x.CreatedBy }).Distinct().Select(x => new {
                id = x.Owner,
                value = x.Owner,
                text = x.Name + " - " + x.Owner,
                name = x.Name
            });

            if (!(String.IsNullOrEmpty(FindOwner) || String.IsNullOrWhiteSpace(FindOwner)))
            {
                Owner = Owner.Where(x => x.value.ToLower().Contains(FindOwner.ToLower()) || x.name.ToLower().Contains(FindOwner.ToLower()));
            }

            return new JsonResult(Owner);
        }


        [HttpGet]
        public IActionResult AllOwnerList([FromQuery] List<string> myparam, string FindOwner)
        {
            var Owner = _context.ApplicationUsers.Where(x => !myparam.Contains(x.DisplayName)).Select(x => new { x.DisplayName, x.UserName }).Select(x => new {
                id = x.UserName,
                value = x.UserName,
                text = x.DisplayName + " - " + x.UserName,
                name = x.DisplayName
            });

            if (!(String.IsNullOrEmpty(FindOwner) || String.IsNullOrWhiteSpace(FindOwner)))
            {
                Owner = Owner.Where(x => x.value.ToLower().Contains(FindOwner.ToLower()) || x.name.ToLower().Contains(FindOwner.ToLower()));
            }

            return new JsonResult(Owner);
        }
    }
}
