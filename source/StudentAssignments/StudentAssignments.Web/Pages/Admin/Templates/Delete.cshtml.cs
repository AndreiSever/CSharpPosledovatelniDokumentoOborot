using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages.Admin.Templates
{
    public class DeleteModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;

        public DeleteModel(StudentAssignments.Web.Model.ApplicationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Template Template { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Template = await _context.Templates.FirstOrDefaultAsync(m => m.Id == id);

            if (Template == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Template = await _context.Templates.FindAsync(id);

            if (Template != null)
            {
                _context.Templates.Remove(Template);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
