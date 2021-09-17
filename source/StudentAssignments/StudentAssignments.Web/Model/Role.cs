using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Model
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
