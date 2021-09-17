using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using StudentAssignments.Web.Xml;

namespace StudentAssignments.Web.Services
{
    public class WorkflowService
    {
        private readonly ApplicationContext _context;

        public WorkflowService(ApplicationContext context)
        {
            _context = context;
        }

        public WorkflowTemplate DeserializeXmlTemplate(string templateXml)
        {
            WorkflowTemplate template;
            XmlSerializer serializer = new XmlSerializer(typeof(WorkflowTemplate));
            using (var reader = new StringReader(templateXml))
            {
                template = (WorkflowTemplate)serializer.Deserialize(reader);
            }
            return template;
        }
    }
}
