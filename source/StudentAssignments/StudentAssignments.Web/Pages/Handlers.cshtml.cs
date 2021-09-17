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
    [Authorize(Roles = "User, Admin")]
    public class HandlersModel : PageModel
    {
        private readonly ApplicationContext _context;

        public HandlersModel(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public class Select2Result
        {
            public List<Select2Item> Results { get; set; }
        }

        public class Select2Item
        {
            public int Id { get; set; }
            public string Text { get; set; }
        }

        public async Task<IActionResult> OnGetGetUsersAsync(int groupId)
        {
            var usersQuery = _context.Users.Where(m => m.GroupId == groupId);
            var users = await usersQuery.Select(u => new Select2Item { Id = u.Id, Text = u.FullName }).ToListAsync();
            var result = new Select2Result { Results = users };

            return new JsonResult(result);
        }
    }
}
