using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrofyWebApp.Models
{
    public class ChecklistPlanViewModel
    {
        public List<projects> projects { get; set; }
        public List<Plan> Plans { get; set; }
        public List<Deliverables> Deliverables { get; set; }
        public string selectedProjectname { get; set; }
    }
    public class Plan
    {
        public string PhaseName { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public bool HasValue { get; set; }
    }

    public class Deliverables
    {
        public string Name { get; set; }
        public string PlannedDeliveryDate { get; set; }
        public string Owner { get; set; }
        public bool HasValue { get; set; }
    }

    public class ChecklistPlanInsertModel
    {
        public ProjectViewModel Project { get; set; }
        public List<Plan> Plans { get; set; }
        public List<Deliverables> Deliverables { get; set; }
        public List<BestPractices> BestPractices { get; set; }
        public string UserId { get; set; }
    }

    public class ServiceResponse
    {
        public bool isSuccess { get; set; }
        public string data { get; set; }
        public string exception { get; set; }
    }

    public class FormInputData
    {
        public List<Plan> Plan { get; set; }
        public List<Deliverables> Deliverables { get; set; }
        public string ProjectName { get; set; }
    }

}
