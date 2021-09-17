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
    public class IndexModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(StudentAssignments.Web.Model.ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<WorkflowModel> Workflows { get;set; }
        public bool CanCreate { get; set; }

        public class WorkflowModel
        {
            public int Id { get; set; }
            public string TemplateName { get; set; }
            public string Title { get; set; }
            public List<StageModel> Stages { get; set; }
            public int? Grade { get; set; }
        }

        public class StageModel
        {
            public string Title { get; set; }
            public string AssigneeName { get; set; }
            public bool IsLate { get; set; }
            public bool IsCurrent { get; set; }
        }

        public async Task OnGetAsync()
        {
            Workflows = new List<WorkflowModel>();
            var user = await _userManager.GetUserAsync(User);
            CanCreate = user.Group != null && user.Group.AssignmentsAllowed;
            var workflows = await _context.Workflows
                .Include(w => w.Template).Where(w => w.CreatedById == user.Id).ToListAsync();
            foreach (var workflow in workflows)
            {
                var workflowModel = new WorkflowModel { Id = workflow.Id, Title = workflow.Title, TemplateName = workflow.Template.Name, Grade = workflow.Grade };
                workflowModel.Stages = new List<StageModel>();
                var stages = workflow.Stages.OrderBy(s => s.Id);
                foreach (var stage in stages)
                {
                    TimeSpan timeSpent = TimeSpan.FromMinutes(stage.TimeSpentInMinutes);
                    bool isLate = timeSpent.TotalDays > stage.DurationInDays;
                    var stageModel = new StageModel { Title = stage.Name, AssigneeName = stage.Assignee.ShortName, IsLate = isLate, IsCurrent = stage.Id == workflow.CurrentStageId };
                    workflowModel.Stages.Add(stageModel);
                }
                Workflows.Add(workflowModel);
            }
        }
    }
}
