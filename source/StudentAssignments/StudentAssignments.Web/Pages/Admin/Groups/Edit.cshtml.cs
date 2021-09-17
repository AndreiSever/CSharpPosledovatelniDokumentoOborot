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

            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "Выдача заданий")]
            public bool AssignmentsAllowed { get; set; }
            public UserModel[] Users { get; set; }
        }

        public class UserModel
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public bool IsRemoved { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FirstOrDefaultAsync(m => m.Id == id);

            if (group == null)
            {
                return NotFound();
            }
            Input = new InputModel
            {
                Id = group.Id,
                Name = group.Name,
                AssignmentsAllowed = group.AssignmentsAllowed,
                Users = group.Users.Select(u => new UserModel { Id = u.Id, FullName = u.FullName }).ToArray()
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == Input.Id);
            if (group == null)
            {
                return NotFound();
            }

            if (Input.Name != group.Name)
            {
                var existingGroup = await _context.Groups.FirstOrDefaultAsync(g => string.Equals(g.Name, Input.Name, StringComparison.OrdinalIgnoreCase));
                if (existingGroup != null)
                {
                    ModelState.AddModelError(string.Empty, "Группа с таким названием уже существует.");
                    return Page();
                }
            }

            group.Name = Input.Name;
            group.AssignmentsAllowed = Input.AssignmentsAllowed;
            List<int> currentUserIds = group.Users.Select(u => u.Id).ToList();
            List<int> inputUserIds = Input.Users.Where(u => !u.IsRemoved).Select(u => u.Id).Distinct().ToList();
            foreach (int userId in inputUserIds)
            {
                if (currentUserIds.Contains(userId))
                {
                    currentUserIds.Remove(userId);
                    continue;
                }
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                user.GroupId = group.Id;
            }
            foreach (int userId in currentUserIds)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                user.GroupId = null;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(group.Id))
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

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
