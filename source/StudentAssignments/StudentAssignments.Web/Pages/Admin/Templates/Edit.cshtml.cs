using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages.Admin.Templates
{
    public class EditModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;

        public EditModel(StudentAssignments.Web.Model.ApplicationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "Название")]
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var template = await _context.Templates.FirstOrDefaultAsync(m => m.Id == id);

            if (template == null)
            {
                return NotFound();
            }
            Input = new InputModel { Id = template.Id, Name = template.Name };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == Input.Id);
            if (template == null)
            {
                return NotFound();
            }

            if (Input.Name != template.Name)
            {
                var existingTemplate = await _context.Templates.FirstOrDefaultAsync(t => string.Equals(t.Name, Input.Name, StringComparison.OrdinalIgnoreCase));
                if (existingTemplate != null)
                {
                    ModelState.AddModelError(string.Empty, "Шаблон с таким именем уже существует.");
                    return Page();
                }
            }

            template.Name = Input.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemplateExists(template.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TemplateExists(int id)
        {
            return _context.Templates.Any(e => e.Id == id);
        }
    }
}
