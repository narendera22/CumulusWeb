using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrofyWebApp.Models
{
    public class BestPracticesViewModel
    {
        public List<projects> projects { get; set; }
        public ProjectViewModel projectServices { get; set; }
        public List<BestPractices> BestPractices { get; set; }
        public string selectedProjectname { get; set; }
        public string userRole { get; set; }
    }

    

    public class BestPracticesInsertModel
    {
        public ProjectViewModel Project { get; set; }
        public List<Plan> Plans { get; set; }
        public List<Deliverables> Deliverables { get; set; }
        public string UserId { get; set; }
    }

    public class Computation
    {
        public string Service { get; set; }
        public string ImpactArea { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Remarks { get; set; }
        public string Mandatory { get; set; }
        public string Field { get; set; }
        public string ListOfValues { get; set; }
        public string disable { get; set; }
    }

    public class BestPracticesFormInputData
    {
        public List<Computation> Checklist { get; set; }
        public string ProjectName { get; set; }
        public string ProductCategory { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
    }

    public class Configurations
    {
        public List<ChecklistConfig> Configuration { get; set; }
    }

    public class ChecklistConfig
    {
        public string ProductCategory { get; set; }

        public List<Services> Services { get; set; }
    }
    public class Services
    {
        public string Name { get; set; }
        public string FileName { get; set; }
    }

}
