using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages.Assignment
{
    public class DetailsModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;

        public DetailsModel(StudentAssignments.Web.Model.ApplicationContext context)
        {
            _context = context;
        }

        public WorkflowModel Workflow { get; set; }

        public class WorkflowModel
        {
            public int Id { get; set; }
            [Display(Name = "Название")]
            public string Title { get; set; }
            [Display(Name = "Описание")]
            public string Description { get; set; }
            [Display(Name = "Дата создания")]
            public DateTime CreatedDate { get; set; }
            [Display(Name = "Завершено")]
            public bool IsCompleted { get; set; }
            [Display(Name = "Текущий этап")]
            public string CurrentStage { get; set; }
            [Display(Name = "Оценка")]
            public int? Grade { get; set; }
            [Display(Name = "Шаблон")]
            public string TemplateName { get; set; }
            [Display(Name = "Документы")]
            public List<DocumentModel> Documents { get; set; }
        }

        public class DocumentModel
        {
            public string Title { get; set; }
            public string Link { get; set; }
            public string Filename { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workflow = await _context.Workflows
                .Include(w => w.CreatedBy)
                .Include(w => w.CurrentStage)
                .Include(w => w.Group)
                .Include(w => w.Template).FirstOrDefaultAsync(m => m.Id == id);

            if (workflow == null)
            {
                return NotFound();
            }

            Workflow = new WorkflowModel
            {
                Id = workflow.Id,
                Title = workflow.Title,
                Description = workflow.Description,
                IsCompleted = workflow.IsCompleted,
                CurrentStage = workflow.CurrentStage.Name,
                CreatedDate = workflow.CreatedDate,
                Grade = workflow.Grade,
                TemplateName = workflow.Template.Name,
                Documents = new List<DocumentModel>()
            };
            if (workflow.IsCompleted)
            {
                var docs = workflow.Documents.Where(d => d.BinaryContent != null);
                foreach (var doc in docs)
                {
                    string fileLink = Url.Page("Details", "GetContent", new { id = workflow.Id, documentId = doc.Id });
                    var documentModel = new DocumentModel { Title = doc.Title, Filename = doc.BinaryContentFilename, Link = fileLink };
                    Workflow.Documents.Add(documentModel);
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnGetGetContentAsync(int? id, int? documentId)
        {
            if (id == null || documentId == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.Where(d => d.WorkflowId == id && d.Id == documentId).FirstOrDefaultAsync();

            if (document == null || document.BinaryContent == null)
            {
                return NotFound();
            }

            byte[] fileBytes = document.BinaryContent;
            string filename = document.BinaryContentFilename;
            //return File(contentBytes, "text/plain", $"template_{template.Id}.xml");
            return File(fileBytes, "application/octet-stream", filename);
        }
    }
}
