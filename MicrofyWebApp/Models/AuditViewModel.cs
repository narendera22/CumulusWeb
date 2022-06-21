﻿using System.Collections.Generic;

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
        public List<AuditChecklistModel> AuditChecklistModel { get; set; }

    }
    public class SolutionObservationFormInputData
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
    public class BestPractices
    {
        public string Service { get; set; }
        public List<Section> Section { get; set; }
    }
    public class Section
    {
        public string ImpactArea { get; set; }
        public List<Checklist> Checklist { get; set; }
    }
    public class Checklist
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Inputvalue Input { get; set; }
        public DisplayAttributes DisplayAttributes { get; set; }

    }
    public class Inputvalue
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Remarks { get; set; }

    }
    public class DisplayAttributes
    {
        public string Mandatory { get; set; }
        public string Field { get; set; }
        public string ListOfValues { get; set; }
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
    public class Summary
    {
        public string projectname { get; set; }
        public string RequiredCount { get; set; }
        public string NotImplementedCount { get; set; }
        public string ObservationCount { get; set; }
        public string ActionItemsCount { get; set; }
    }
}
