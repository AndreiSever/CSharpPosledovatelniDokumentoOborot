using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;
using StudentAssignments.Web.Services;

namespace StudentAssignments.Web.Pages.Assignment
{
    public class CreateModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;
        private readonly WorkflowService _workflowService;
        private readonly UserManager<User> _userManager;

        public CreateModel(
            StudentAssignments.Web.Model.ApplicationContext context, 
            WorkflowService workflowService, 
            UserManager<User> userManager)
        {
            _context = context;
            _workflowService = workflowService;
            _userManager = userManager;
        }

        public Template[] Templates { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int TemplateId { get; set; }
            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "Название")]
            public string Title { get; set; }
            [Display(Name = "Описание")]
            public string Desctiption { get; set; }
            [Required]
            [Display(Name = "Исполнители этапов")]
            public StageModel[] Stages { get; set; }
        }

        public class StageModel
        {
            [Required]
            public string StageName { get; set; }
            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            public int? UserId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? templateId)
        {
            if (templateId == null)
            {
                Templates = await _context.Templates.ToArrayAsync();
                return Page();
            }
            else
            {
                var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == templateId);
                if (template == null)
                    return NotFound();
                Xml.WorkflowTemplate tpl = _workflowService.DeserializeXmlTemplate(template.Content);
                Input = new InputModel();
                Input.TemplateId = template.Id;
                Input.Stages = tpl.Stages.Select(s => new StageModel
                    { StageName = s.Title }).ToArray();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == Input.TemplateId);
            if (template == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user.Group == null || !user.Group.AssignmentsAllowed)
                return RedirectToPage("./Index");

            Xml.WorkflowTemplate xmlTemplate = _workflowService.DeserializeXmlTemplate(template.Content);
            var now = DateTime.Now;
            var workflow = new Workflow { CreatedById = user.Id, CreatedDate = now, Group = user.Group, Template = template, Title = Input.Title, Description = Input.Desctiption };
            using (var tran = _context.Database.BeginTransaction())
            {
                _context.Workflows.Add(workflow);
                await _context.SaveChangesAsync();
                var docTypes = await _context.DocumentTypes.ToArrayAsync();
                var docs = new List<Document>();
                foreach (var doc in xmlTemplate.Documents)
                {
                    var docType = docTypes.First(t => t.Name == doc.Type);
                    var document = new Document { Workflow = workflow, DocId = doc.Id, Title = doc.Title, DocumentType = docType, IsRequired = doc.IsRequired };
                    docs.Add(document);
                }
                _context.Documents.AddRange(docs);
                Stage firstStage = null;
                for (int i = 0; i < xmlTemplate.Stages.Length; i++)
                {
                    var st = xmlTemplate.Stages[i];
                    var assignedStage = Input.Stages.First(s => s.StageName == st.Title);
                    var assignee = await _context.Users.FirstAsync(u => u.Id == assignedStage.UserId);
                    var stage = new Stage { Workflow = workflow, Name = st.Title, DurationInDays = st.Duration, CanReject = st.CanReject, Assignee = assignee };
                    _context.Stages.Add(stage);
                    if (i == 0)
                    {
                        stage.StartedDate = now;
                        firstStage = stage;
                    }
                    if (st.InputDocuments != null)
                    {
                        foreach (var inDoc in st.InputDocuments)
                        {
                            var refDoc = docs.First(d => d.DocId == inDoc.Id);
                            var doc = new InputDocument { Document = refDoc, Stage = stage };
                            _context.InputDocuments.Add(doc);
                        }
                    }
                    if (st.OutputDocuments != null)
                    {
                        foreach (var outDoc in st.OutputDocuments)
                        {
                            var refDoc = docs.First(d => d.DocId == outDoc.Id);
                            var doc = new OutputDocument { Document = refDoc, Stage = stage };
                            _context.OutputDocuments.Add(doc);
                        }
                    }
                }
                try
                {
                    await _context.SaveChangesAsync();
                    workflow.CurrentStage = firstStage;
                    await _context.SaveChangesAsync();
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
