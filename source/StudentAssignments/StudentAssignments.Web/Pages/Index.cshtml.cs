using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {

        }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
