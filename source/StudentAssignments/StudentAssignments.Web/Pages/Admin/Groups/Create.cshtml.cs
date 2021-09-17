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

namespace StudentAssignments.Web.Pages.Admin.Groups
{
    public class CreateModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;

        public CreateModel(StudentAssignments.Web.Model.ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "Название")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "Выдача заданий")]
            public bool AssignmentsAllowed { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingGroup = await _context.Groups.FirstOrDefaultAsync(g => string.Equals(g.Name, Input.Name, StringComparison.OrdinalIgnoreCase));
            if (existingGroup != null)
            {
                ModelState.AddModelError(string.Empty, "Группа с таким названием уже существует.");
                return Page();
            }
            var group = new Group { Name = Input.Name, AssignmentsAllowed = Input.AssignmentsAllowed };
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}