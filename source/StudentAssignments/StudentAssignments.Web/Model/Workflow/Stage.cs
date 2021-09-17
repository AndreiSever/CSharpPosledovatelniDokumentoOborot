using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Model
{
    public class Stage
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Срок")]
        public int DurationInDays { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int TimeSpentInMinutes { get; set; }
        [Display(Name = "Возврат")]
        public bool CanReject { get; set; }
        public int WorkflowId { get; set; }
        public int AssigneeId { get; set; }

        public virtual Workflow Workflow { get; set; }
        [ForeignKey("AssigneeId")]
        public virtual User Assignee { get; set; }
        public virtual ICollection<InputDocument> InputDocuments { get; set; }
        public virtual ICollection<OutputDocument> OutputDocuments { get; set; }
    }
}
