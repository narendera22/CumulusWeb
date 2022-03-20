﻿using System;
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
        public string Value { get; set; }
        public DisplayAttributes DisplayAttributes { get; set; }
    }
    public class DisplayAttributes
    {
        public string Mandatory { get; set; }
        public string Field { get; set; }
        public string ListOfValues { get; set; }
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
        public string Mandatory { get; set; }
        public string Field { get; set; }
        public string ListOfValues { get; set; }
    }

    public class BestPracticesFormInputData
    {
        public List<Computation> BestPractices { get; set; }
        public string ProjectName { get; set; }
    }

}
