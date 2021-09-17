using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Model
{
    public class OutputDocument
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int StageId { get; set; }

        public virtual Document Document { get; set; }
        public virtual Stage Stage { get; set; }
    }
}
