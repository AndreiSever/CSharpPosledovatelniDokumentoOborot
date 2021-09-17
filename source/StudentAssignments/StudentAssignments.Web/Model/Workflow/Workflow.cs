using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Model
{
    public class Workflow
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int GroupId { get; set; }
        public int TemplateId { get; set; }
        public int? CurrentStageId { get; set; }
        public int CreatedById { get; set; }
        public bool IsCompleted { get; set; }
        public int? Grade { get; set; }

        public virtual Group Group { get; set; }
        public virtual Template Template { get; set; }
        [ForeignKey("CurrentStageId")]
        public virtual Stage CurrentStage { get; set; }
        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; }
        [InverseProperty("Workflow")]
        public virtual ICollection<Stage> Stages { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}
