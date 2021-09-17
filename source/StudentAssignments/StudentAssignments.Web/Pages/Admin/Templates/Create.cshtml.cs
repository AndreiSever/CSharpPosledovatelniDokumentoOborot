using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;
using StudentAssignments.Web.Services;

namespace StudentAssignments.Web.Pages.Admin.Templates
{
    public class CreateModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;
        private readonly WorkflowService _workflowService;

        public CreateModel(StudentAssignments.Web.Model.ApplicationContext context, WorkflowService workflowService)
        {
            _context = context;
            _workflowService = workflowService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "Название")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "XML-файл шаблона")]
            public IFormFile FileUpload { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingTemplate = await _context.Templates.FirstOrDefaultAsync(t => string.Equals(t.Name, Input.Name, StringComparison.OrdinalIgnoreCase));
            if (existingTemplate != null)
            {
                ModelState.AddModelError(string.Empty, "Шаблон с таким именем уже существует.");
                return Page();
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(Input.FileUpload.OpenReadStream());
            }
            catch (XmlException ex)
            {
                ModelState.AddModelError(string.Empty, "Ошибка чтения XML: " + ex.Message);
                return Page();
            }
            try
            {
                Xml.WorkflowTemplate tpl = _workflowService.DeserializeXmlTemplate(doc.OuterXml);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ошибка парсинга XML шаблона: " + ex.Message);
                return Page();
            }

            string filename = Path.GetFileName(Input.FileUpload.FileName);
            _context.Templates.Add(new Template { Name = Input.Name, Content = doc.OuterXml, Filename = filename });
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}