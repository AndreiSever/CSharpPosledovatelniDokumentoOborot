using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Model
{
    public class Template
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "XML-контент")]
        public string Content { get; set; }
        [Required]
        public string Filename { get; set; }
    }
}
