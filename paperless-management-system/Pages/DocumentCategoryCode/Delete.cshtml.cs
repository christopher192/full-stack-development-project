﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Pages.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.Pages.DocumentCategoryCode
{
    [Authorize(Roles = "System Admin")]
    public class DeleteModel : DocumentCategoryCodePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;

        public DeleteModel(ApplicationDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("DataProtection");
        }

        [BindProperty]
        public DocumentCategoryList DocumentCategoryList { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DocumentCategoryList = await _context.DocumentCategoryLists.FirstOrDefaultAsync(m => m.Id == int.Parse(_protector.Unprotect(id)));

            if (DocumentCategoryList == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (DocumentCategoryList.Id == null)
            {
                return NotFound();
            }

            DocumentCategoryList = await _context.DocumentCategoryLists.FindAsync(DocumentCategoryList.Id);

            if (DocumentCategoryList != null)
            {
                _context.DocumentCategoryLists.Remove(DocumentCategoryList);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}