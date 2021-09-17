using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Model
{
    public class Group
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Выдача заданий")]
        public bool AssignmentsAllowed { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
