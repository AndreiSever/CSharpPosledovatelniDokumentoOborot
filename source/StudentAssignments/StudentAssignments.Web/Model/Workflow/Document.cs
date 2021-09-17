using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Model
{
    public class Document
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Идентификатор")]
        public string DocId { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Title { get; set; }
        public string TextContent { get; set; }
        public byte[] BinaryContent { get; set; }
        public string BinaryContentFilename { get; set; }
        public bool IsRequired { get; set; }
        public int DocumentTypeId { get; set; }
        public int WorkflowId { get; set; }

        public virtual DocumentType DocumentType { get; set; }
        public virtual Workflow Workflow { get; set; }
    }
}
