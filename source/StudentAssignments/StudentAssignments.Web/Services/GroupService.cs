using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Services
{
    public class GroupService
    {
        private readonly ApplicationContext _context;

        public GroupService(ApplicationContext context)
        {
            _context = context;
        }

        public List<Group> ListGroups()
        {
            return _context.Groups.ToList();
        }
    }
}
