using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages.Assignment
{
    public class MyModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public MyModel(StudentAssignments.Web.Model.ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<WorkflowModel> Workflows { get;set; }

        public class WorkflowModel
        {
            public int Id { get; set; }
            public string TemplateName { get; set; }
            public string Title { get; set; }
            public StageModel CurrentStage { get; set; }
        }

        public class StageModel
        {
            public string Title { get; set; }
        }

        public async Task OnGetAsync()
        {
            Workflows = new List<WorkflowModel>();
            var user = await _userManager.GetUserAsync(User);
            var workflows = await _context.Workflows
                .Include(w => w.CreatedBy)
                .Include(w => w.CurrentStage)
                .Include(w => w.Group)
                .Include(w => w.Template).Where(w => !w.IsCompleted && w.CurrentStage.AssigneeId == user.Id).ToListAsync();
            foreach (var workflow in workflows)
            {
                var workflowModel = new WorkflowModel { Id = workflow.Id, Title = workflow.Title, TemplateName = workflow.Template.Name };
                workflowModel.CurrentStage = new StageModel { Title = workflow.CurrentStage.Name };
                Workflows.Add(workflowModel);
            }
        }
    }
}
