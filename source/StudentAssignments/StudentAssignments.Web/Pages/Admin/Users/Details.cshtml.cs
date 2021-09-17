﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages.Admin.Users
{
    public class DetailsModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;

        public DetailsModel(StudentAssignments.Web.Model.ApplicationContext context)
        {
            _context = context;
        }

        public User UserEntity { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserEntity = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

            if (UserEntity == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
