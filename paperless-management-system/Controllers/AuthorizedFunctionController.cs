using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using WD_ERECORD_CORE.Data;
using ClosedXML.Excel;

namespace WD_ERECORD_CORE.Controllers
{
    [Authorize]
    public class AuthorizedFunctionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> SignInManager;
        public UserManager<ApplicationUser> UserManager { get; set; }

        public AuthorizedFunctionController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> mgr)
        {
            _context = context;
            SignInManager = signInManager;
            UserManager = mgr;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await UserManager.GetUserAsync(HttpContext.User);
        }

        [HttpPost]
        [SupportedOSPlatform("windows")]
        public async Task<JsonResult> ValidatePassword(string password)
        {
            var Result = false;

            var LocalUser = new List<string> { "admin", "approver-qa", "ccbapprover-lv1", "ccbapprover-lv2", "ccbapprover-lv3" };

            if (string.IsNullOrEmpty(password))
            {
                return new JsonResult(new { Result });
            }

            var CurrentUser = await GetCurrentUser();

            if (LocalUser.Contains(CurrentUser.UserName))
            {
                Result = SignInManager.UserManager.CheckPasswordAsync(CurrentUser, password).Result;

                return new JsonResult(new { Result });
            }
            else
            {
                try
                {
                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "ad.shared", "OU=MPP,OU=Malaysia,OU=StdUsers,OU=UsersAndGroups,OU=Accounts,DC=ad,DC=shared"))
                    {
                        Result = pc.ValidateCredentials(CurrentUser.UserName, password);

                        return new JsonResult(new { Result });
                    }
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { ErrorMessage = "Something wrong during connection to LDAP" });
                }

            }

            return new JsonResult(new { Result });
        }

        [HttpGet]
        public IActionResult ExportExcel(int Id)
        {
            var formdata = _context.FormListHistories.Where(x => x.Id == Id).FirstOrDefault();
            var formName = formdata.FormName;
            var JSON = formdata.FormSubmittedData;
            var JObject = JsonConvert.DeserializeObject<dynamic>(formdata.FormSubmittedData);

            DataTable getData()
            {
                //Creating DataTable  
                DataTable dt = new DataTable();
                //Setiing Table Name  
                dt.TableName = formName;
                //Add Columns  
                /*dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("City", typeof(string));*/

                dt.Columns.Add("Form Property", typeof(string));
                dt.Columns.Add("Form Value", typeof(string));

                //Add Rows in DataTable  
                /*                dt.Rows.Add(1, "Anoop Kumar Sharma", "Delhi");
                                dt.Rows.Add(2, "Andrew", "U.P.");*/

                dt.Rows.Add("Form Name", formdata.FormName);
                dt.Rows.Add("Revision", formdata.FormRevision);
                dt.Rows.Add("Status", formdata.FormStatus);
                dt.Rows.Add("Created Date", formdata.CreatedDate);
                dt.Rows.Add("Created By", formdata.CreatedBy);
                dt.Rows.Add("Modified Date", formdata.ModifiedDate);
                dt.Rows.Add("Modifed By", formdata.ModifiedBy);

                dt.AcceptChanges();
                return dt;

            }

            DataTable getData2()
            {
                //Creating DataTable  
                DataTable dt = new DataTable();
                //Setiing Table Name  
                dt.TableName = formName;
                //Add Columns  
                /*dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("City", typeof(string));*/

                dt.Columns.Add("Component Name", typeof(string));
                dt.Columns.Add("Value", typeof(string));

                //Add Rows in DataTable  
                /*                dt.Rows.Add(1, "Anoop Kumar Sharma", "Delhi");
                                dt.Rows.Add(2, "Andrew", "U.P.");*/

                foreach (JProperty property in JObject.Properties())
                {
                    /*                    if (property.Name != "employeeId" && property.Name != "employeeId1" && property.Name != "department" && property.Name != "dateTime")
                                        {
                                            dt.Rows.Add(property.Name, property.Value);
                                        }*/
                    if (property.Value.Type == JTokenType.Array)
                    {
                        /*                        var jsonArray = (JArray)property.Value;
                                                var stringCombine = "";

                                                for (int i = 0; i < jsonArray.Count; i++)
                                                {
                                                    var jsonArrayValue = (JObject)jsonArray[i];

                                                    foreach (JProperty value in jsonArrayValue.Properties())
                                                    {
                                                        stringCombine += String.Format("{0} - {1} \n", value.Name, value.Value);
                                                    }
                                                }

                                                dt.Rows.Add(property.Name, stringCombine);*/

                        var JChildObject = JsonConvert.DeserializeObject<JArray>(property.Value.ToString()).ToObject<List<JObject>>();

                        var iterativeNumber = 1;

                        foreach (JProperty childProperty in JChildObject.Properties())
                        {
                            var childPropertyName = property.Name + "(" + iterativeNumber + ")" + " - " + childProperty.Name;
                            dt.Rows.Add(childPropertyName, childProperty.Value);

                            iterativeNumber++;
                        }
                    }
                    else
                    {
                        dt.Rows.Add(property.Name, property.Value);
                    }
                }

                dt.AcceptChanges();
                return dt;

            }

            DataTable dt = getData();
            DataTable dt2 = getData2();

            string fileName = formName + ".xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Worksheet(1).Cell(1, 3).InsertTable(dt2);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}
