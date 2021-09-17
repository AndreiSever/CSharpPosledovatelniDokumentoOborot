using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages.Assignment
{
    public class SubmitModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public SubmitModel(StudentAssignments.Web.Model.ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public WorkflowModel Workflow { get; set; }
            public StageModel Stage { get; set; }
            public List<InputDocumentModel> InputDocuments { get; set; }
            public List<OutputDocumentModel> OutputDocuments { get; set; }
            [Display(Name = "Оценка"), DataType(DataType.Text)]
            public int? Grade { get; set; }
            public bool? IsRejected { get; set; }
        }

        public class WorkflowModel
        {
            public int Id { get; set; }
            public string TemplateName { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
        }

        public class StageModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public bool CanReject { get; set; }
            public bool IsFinal { get; set; }
        }

        public class InputDocumentModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string TextContent { get; set; }
            public string BinaryContentLink { get; set; }
            public string BinaryContentFilename { get; set; }
        }

        public class OutputDocumentModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
            public string Key { get; set; }
            public string TextContent { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var workflow = await _context.Workflows
                .Include(w => w.CurrentStage)
                .FirstOrDefaultAsync(w => w.Id == id);
            if (workflow.IsCompleted || workflow.CurrentStage.AssigneeId != user.Id)
            {
                return RedirectToPage("./Index");
            }
            Input = new InputModel { InputDocuments = new List<InputDocumentModel>(), OutputDocuments = new List<OutputDocumentModel>() };
            var workflowModel = new WorkflowModel { Id = workflow.Id, Title = workflow.Title, Description = workflow.Description, TemplateName = workflow.Template.Name };
            var stageModel = new StageModel { Id = workflow.CurrentStage.Id, Title = workflow.CurrentStage.Name, CanReject = workflow.CurrentStage.CanReject };
            var nextStage = workflow.Stages.Where(s => s.Id > workflow.CurrentStageId).OrderBy(s => s.Id).FirstOrDefault();
            if (nextStage == null)
                stageModel.IsFinal = true;
            var inputDocuments = workflow.CurrentStage.InputDocuments.Where(d => d.Document.TextContent != null || d.Document.BinaryContent != null);
            foreach (var doc in inputDocuments)
            {
                string fileLink = Url.Page("Details", "GetContent", new { id = workflow.Id, documentId = doc.DocumentId });
                var inDoc = new InputDocumentModel { Id = doc.Id, Title = doc.Document.Title, TextContent = doc.Document.TextContent, BinaryContentLink = fileLink, BinaryContentFilename = doc.Document.BinaryContentFilename };
                Input.InputDocuments.Add(inDoc);
            }
            foreach (var doc in workflow.CurrentStage.OutputDocuments)
            {
                var outDoc = new OutputDocumentModel { Id = doc.Id, Title = doc.Document.Title, Type = doc.Document.DocumentType.Name };
                outDoc.Key = $"doc_{doc.Document.DocId}";
                Input.OutputDocuments.Add(outDoc);
            }
            Input.Workflow = workflowModel;
            Input.Stage = stageModel;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //throw new InvalidOperationException("This is exception message.");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            var workflow = await _context.Workflows
                .Include(w => w.CurrentStage)
                .FirstOrDefaultAsync(w => w.Id == Input.Workflow.Id);
            if (workflow.IsCompleted || workflow.CurrentStage.Id != Input.Stage.Id || workflow.CurrentStage.AssigneeId != user.Id)
            {
                return RedirectToPage("./Index");
            }
            var errors = new List<string>();
            foreach (var doc in workflow.CurrentStage.OutputDocuments)
            {
                var key = $"doc_{doc.Document.DocId}";
                var msg = $"Поле \"{doc.Document.Title}\" обязательно для заполнения.";
                if (doc.Document.DocumentType.Name == Constants.DocumentType.Text)
                {
                    string value = Request.Form[key].FirstOrDefault() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        if (doc.Document.IsRequired)
                            errors.Add(msg);
                        doc.Document.TextContent = null;
                    }
                    else
                    {
                        doc.Document.TextContent = value;
                    }
                    Input.OutputDocuments.First(d => d.Id == doc.Id).TextContent = value;
                }
                else if (doc.Document.DocumentType.Name == Constants.DocumentType.File)
                {
                    var file = Request.Form.Files[key];
                    if (file == null)
                    {
                        if (doc.Document.IsRequired)
                            errors.Add(msg);
                        doc.Document.BinaryContent = null;
                    }
                    else
                    {
                        var upload = Request.Form.Files[key];
                        string filename = Path.GetFileName(upload.FileName);
                        byte[] fileBytes = null;
                        using (var stream = new MemoryStream())
                        {
                            upload.CopyTo(stream);
                            fileBytes = stream.ToArray();
                        }
                        doc.Document.BinaryContent = fileBytes;
                        doc.Document.BinaryContentFilename = filename;
                    }
                }
            }

            var now = DateTime.Now;
            TimeSpan timeSpent = now - workflow.CurrentStage.StartedDate.Value;
            workflow.CurrentStage.TimeSpentInMinutes += (int)timeSpent.TotalMinutes;

            if (Input.IsRejected == true)
            {
                var prevStage = workflow.Stages.Where(s => s.Id < workflow.CurrentStageId).OrderByDescending(s => s.Id).FirstOrDefault();
                if (prevStage != null)
                {
                    workflow.CurrentStageId = prevStage.Id;
                    prevStage.StartedDate = now;
                }
            }
            else
            {
                var nextStage = workflow.Stages.Where(s => s.Id > workflow.CurrentStageId).OrderBy(s => s.Id).FirstOrDefault();
                if (nextStage == null)
                {
                    if (Input.Grade == null)
                    {
                        errors.Add("Поле \"Оценка\" обязательно для заполнения.");
                    }
                    else if (Input.Grade < 1 || Input.Grade > 5)
                    {
                        errors.Add("Некорректное значение в поле \"Оценка\".");
                    }
                    else
                    {
                        workflow.Grade = Input.Grade;
                        workflow.IsCompleted = true;
                    }
                }
                else
                {
                    workflow.CurrentStageId = nextStage.Id;
                    nextStage.StartedDate = now;
                }
            }

            if (errors.Count > 0)
            {
                foreach (var message in errors)
                    ModelState.AddModelError(string.Empty, message);
                return Page();
            }
            await _context.SaveChangesAsync();
            //return Page();
            return RedirectToPage("./My");
        }
    }
}
