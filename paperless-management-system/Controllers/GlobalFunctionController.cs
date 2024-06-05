using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;
using WD_ERECORD_CORE.Function;

namespace WD_ERECORD_CORE.Controllers
{
    [AllowAnonymous]
    public class GlobalFunctionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GlobalFunctionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> AutoSaveMasterForm(int? Id, string? FormData)
        {
            if (Id != null && !String.IsNullOrEmpty(FormData))
            {
                var masterFormList = _context.MasterFormLists.Where(x => x.Id == Id).FirstOrDefault();

                if (masterFormList != null) {
                    // compare both form data
                    // save if found difference
                    var previousFormData = masterFormList.MasterFormData;

                    if (!String.IsNullOrEmpty(previousFormData))
                    {
                        if (!previousFormData.Equals(FormData))
                        {
                            masterFormList.MasterFormData = FormData;

                            _context.Attach(masterFormList);
                            _context.Entry(masterFormList).Property(r => r.MasterFormData).IsModified = true;

                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        masterFormList.MasterFormData = FormData;

                        _context.Attach(masterFormList);
                        _context.Entry(masterFormList).Property(r => r.MasterFormData).IsModified = true;

                        await _context.SaveChangesAsync();
                    }
                }
            }

            return Json("Execute...");
        }

        [HttpGet]
        public async Task<ActionResult> Encrypt(string password)
        {
            if (!String.IsNullOrEmpty(password))
            {
                var str = EncryptDecrypt.Encrypt(password);

                return Json(str);
            }

            return Json("No Password is found.");
        }
    }
}
