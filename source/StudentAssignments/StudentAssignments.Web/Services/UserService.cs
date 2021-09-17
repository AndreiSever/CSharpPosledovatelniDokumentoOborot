using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Services
{
    public class UserService
    {
        private readonly ApplicationContext _context;

        public UserService(ApplicationContext context)
        {
            _context = context;
        }

        public List<User> ListUsers()
        {
            return _context.Users.ToList();
        }
    }
}
