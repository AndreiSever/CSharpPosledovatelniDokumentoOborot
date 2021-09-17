using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages.Admin.Groups
{
    public class IndexModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;

        public IndexModel(StudentAssignments.Web.Model.ApplicationContext context)
        {
            _context = context;
        }

        public IList<Group> Group { get;set; }

        public async Task OnGetAsync()
        {
            Group = await _context.Groups.OrderByDescending(g => g.Id).ToListAsync();
        }
    }
}
