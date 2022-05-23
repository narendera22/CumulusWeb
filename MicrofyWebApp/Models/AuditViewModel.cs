using System.Collections.Generic;

namespace MicrofyWebApp.Models
{
    public class AuditViewModel
    {
        public string projectname { get; set; }
        public ProjectViewModel project { get; set; }
        public List<string> ListUsers { get; set; }
        public List<string> ListAuditor { get; set; }
        public List<ChecklistConfig> Configuration { get; set; }
        public ProjectDetails ProjectDetails { get; set; }
        public List<BestPractices> BestPractices { get; set; }
        public List<Azuretech> AzureServicesUsed { get; set; }
        public List<Observations> Observations { get; set; }
        public List<ActionItems> ActionItems { get; set; }
        public List<Deliverables> Deliverables { get; set; }
        public string Status { get; set; }


    }
    public class ChecklistFormInputData
    {
        public ProjectDetails ProjectDetails { get; set; }
        public string UserId { get; set; }
        public string projectname { get; set; }
        public List<Observations> observation { get; set; }
        public List<ActionItems> ActionItems { get; set; }
        public List<BestPractices> Checklist { get; set; }
        public string Status { get; set; }
        public string ProductCategory { get; set; }
        public string Service { get; set; }

    }
    public class Observations
    {
        public string ImpactArea { get; set; }
        public string Observation { get; set; }
    }
    public class ActionItems
    {
        public string ImpactArea { get; set; }
        public string Severity { get; set; }
        public string Concern { get; set; }
        public string RecommendedMitigation { get; set; }
    }
    public class ProjectDetails
    {
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOverview { get; set; }
        public string ProjectOverview { get; set; }
        public string ProjectDeliverables { get; set; }
        public string ProjectStartDate { get; set; }
        public string TentativeEndDate { get; set; }
        public string ApplicationArchitecture { get; set; }
        public string TechnologiesInUse { get; set; }
        public List<Azuretech> AzureServicesUsed { get; set; }
    }
    public class Azuretech
    {
        public string ProductCategory { get; set; }
        public string Services { get; set; }
    }
    public class AuditChecklistModel
    {
        public ProjectDetails ProjectDetails { get; set; }
        public List<BestPractices> Checklist { get; set; }
        public List<Observations> Observations { get; set; }
        public List<ActionItems> ActionItems { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
    }
    public class NavtabParam
    {
        public string projectName { get; set; }
        public string prevProdCat { get; set; }
        public string prevService { get; set; }
        public string prevDivid { get; set; }
        public string currentProdCat { get; set; }
        public string currentService { get; set; }
        public string currentDivid { get; set; }
    }
}
