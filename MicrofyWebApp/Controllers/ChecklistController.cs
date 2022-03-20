using MicrofyWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MicrofyWebApp.Controllers
{
    public class ChecklistController : Controller
    {
        private readonly ILogger<ChecklistController> _logger;

        string Asseturl = string.Empty;
        string AssetCode = string.Empty;
        string masterfile = string.Empty;
        string checklistfile = string.Empty;
        string Projecturl = string.Empty;
        string ProjectCode = string.Empty;
        string Checklisturl = string.Empty;
        string ChecklistCode = string.Empty;
        private IConfiguration _configuration;

        public ChecklistController(ILogger<ChecklistController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Asseturl = _configuration.GetValue<string>("Values:AssetStrgeBaseUrl");
            AssetCode = _configuration.GetValue<string>("Values:AssetStrgeCode");
            masterfile = _configuration.GetValue<string>("Values:MasterTemplateFile");
            checklistfile = _configuration.GetValue<string>("Values:ChecklistMasterTemplateFile");
            Projecturl = _configuration.GetValue<string>("Values:ProjectBaseUrl");
            ProjectCode = _configuration.GetValue<string>("Values:ProjectCode");
            Checklisturl = _configuration.GetValue<string>("Values:ChecklistUrl");
            ChecklistCode = _configuration.GetValue<string>("Values:ChecklistCode");

        }

        public IActionResult ChecklistPlan(string projectname = null)
        {
            UserViewModel userViewModel = new UserViewModel();
            ChecklistPlanViewModel checklistPlanView = new ChecklistPlanViewModel();
            ChecklistPlanViewModel checklist = new ChecklistPlanViewModel();

            checklistPlanView = JsonConvert.DeserializeObject<ChecklistPlanViewModel>(GetMasterTemplate(masterfile));

            string UserDet = HttpContext.Session.GetString("_UserDet");
            userViewModel = JsonConvert.DeserializeObject<UserViewModel>(UserDet);
            checklistPlanView.projects = userViewModel.projects;

            if (projectname == null)
            {
                var project = checklistPlanView.projects.FirstOrDefault();
                projectname = project.projectName;
            }

            string checklistdet = GetChecklistDetailsAsync(projectname);
            if (checklistdet != null || checklistdet != "")
                checklist = JsonConvert.DeserializeObject<ChecklistPlanViewModel>(checklistdet);

            if (checklist.Plans.Count > 0)
            {
                var planval = from x in checklistPlanView.Plans
                              join y in checklist.Plans
                                     on new { a = x.PhaseName } equals new { a = y.PhaseName }
                              select new { x, y };

                foreach (var match in planval)
                {
                    match.y.HasValue = true;
                    match.x.HasValue = true;
                    match.x.Start = match.y.Start;
                    match.x.End = match.y.End;
                }
                foreach (var chk in checklist.Plans.Where(p => p.HasValue != true))
                {
                    chk.HasValue = true;
                    //checklistPlanView.Plans.Add(chk);
                }
                checklistPlanView.Plans.Clear();
                checklistPlanView.Plans = checklist.Plans;
            }

            checklistPlanView.selectedProjectname = projectname;

            return View(checklistPlanView);
        }

        public string GetMasterTemplate(string mastertemplate)
        {
            //string folder = $"folder={foldername}";
            string template = string.Empty;
            string Requestapi = $"api/GetTemplate/{mastertemplate}?{AssetCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Asseturl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage file = new HttpResponseMessage();
                file = response.Result;
                template = file.Content.ReadAsStringAsync().Result;
            }

            return template;
        }

        public IActionResult ChecklistDeliverable(string projectname = null)
        {
            UserViewModel userViewModel = new UserViewModel();
            ChecklistPlanViewModel checklistPlanView = new ChecklistPlanViewModel();
            ChecklistPlanViewModel checklist = new ChecklistPlanViewModel();

            checklistPlanView = JsonConvert.DeserializeObject<ChecklistPlanViewModel>(GetMasterTemplate(masterfile));

            string UserDet = HttpContext.Session.GetString("_UserDet");
            userViewModel = JsonConvert.DeserializeObject<UserViewModel>(UserDet);
            checklistPlanView.projects = userViewModel.projects;

            if (projectname == null)
            {
                var project = checklistPlanView.projects.FirstOrDefault();
                projectname = project.projectName;
            }

            string checklistdet = GetChecklistDetailsAsync(projectname);
            if (checklistdet != null || checklistdet != "")
                checklist = JsonConvert.DeserializeObject<ChecklistPlanViewModel>(checklistdet);

            if (checklist.Deliverables.Count > 0)
            {
                var delval = from x in checklistPlanView.Deliverables
                             join y in checklist.Deliverables
                                    on new { a = x.Name } equals new { a = y.Name }
                             select new { x, y };

                foreach (var match in delval)
                {
                    match.y.HasValue = true;
                    match.x.HasValue = true;
                    match.x.PlannedDeliveryDate = match.y.PlannedDeliveryDate;
                    match.x.Owner = match.y.Owner;
                }
                foreach (var chk in checklist.Deliverables.Where(p => p.HasValue != true))
                {
                    chk.HasValue = true;
                    //checklistPlanView.Deliverables.Add(chk);
                }

                checklistPlanView.Deliverables.Clear();
                checklistPlanView.Deliverables = checklist.Deliverables;
            }

            checklistPlanView.selectedProjectname = projectname;
            return View(checklistPlanView);
        }

        public string GetChecklistDetailsAsync(string ProjectName)
        {
            string Requestapi = $"api/GetChecklistDetails/{ProjectName}?{ChecklistCode}";
            string checklist = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Checklisturl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = response.Result;
                if (resp.IsSuccessStatusCode)
                {
                    checklist = resp.Content.ReadAsStringAsync().Result;
                }
            }
            return checklist;
        }

        [HttpPost]
        public async Task<ServiceResponse> InsertChecklistPlan(string data)
        {
            FormInputData dataele = JsonConvert.DeserializeObject<FormInputData>(data);
            ChecklistPlanInsertModel checklistPlanInsert = new ChecklistPlanInsertModel();
            ServiceResponse resp = new ServiceResponse();

            string checklist = GetChecklistDetailsAsync(dataele.ProjectName);

            string Requestapi = $"api/CreateChecklist?{ChecklistCode}";

            if (checklist != "")
            {
                checklistPlanInsert = JsonConvert.DeserializeObject<ChecklistPlanInsertModel>(checklist);
                Requestapi = $"api/UpdateChecklist?{ChecklistCode}";
            }

            checklistPlanInsert.Project = JsonConvert.DeserializeObject<ProjectViewModel>(GetProjectDetails(dataele.ProjectName));
            checklistPlanInsert.Plans = dataele.Plan;
            checklistPlanInsert.UserId = HttpContext.Session.GetString("_userId"); ;

            string json = JsonConvert.SerializeObject(checklistPlanInsert);



            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Checklisturl);
                if (checklist != "")
                {
                    var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(checklistPlanInsert), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }
                else
                {
                    var result = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(checklistPlanInsert), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }

            }
            return resp;
        }

        public string GetProjectDetails(string Projectname)
        {
            string projDet = string.Empty;
            string Requestapi = $"api/GetProjectDetails/{Projectname}?{ProjectCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Projecturl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = response.Result;
                projDet = resp.Content.ReadAsStringAsync().Result;
            }

            return projDet;
        }

        [HttpPost]
        public async Task<ServiceResponse> InsertChecklistDeliverable(string data)
        {
            FormInputData dataele = JsonConvert.DeserializeObject<FormInputData>(data);
            ChecklistPlanInsertModel checklistPlanInsert = new ChecklistPlanInsertModel();
            ServiceResponse resp = new ServiceResponse();

            string checklist = GetChecklistDetailsAsync(dataele.ProjectName);

            string Requestapi = $"api/CreateChecklist?{ChecklistCode}";

            if (checklist != "")
            {
                checklistPlanInsert = JsonConvert.DeserializeObject<ChecklistPlanInsertModel>(checklist);
                Requestapi = $"api/UpdateChecklist?{ChecklistCode}";
            }

            checklistPlanInsert.Project = JsonConvert.DeserializeObject<ProjectViewModel>(GetProjectDetails(dataele.ProjectName));
            checklistPlanInsert.Deliverables = dataele.Deliverables;
            checklistPlanInsert.UserId = HttpContext.Session.GetString("_userId"); ;

            string json = JsonConvert.SerializeObject(checklistPlanInsert);



            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Checklisturl);
                if (checklist != "")
                {
                    var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(checklistPlanInsert), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }
                else
                {
                    var result = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(checklistPlanInsert), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }

            }
            return resp;
        }

        public IActionResult ChecklistBestPractices(string projectname = null)
        {
            UserViewModel userViewModel = new UserViewModel();
            BestPracticesViewModel bestPracticesView = new BestPracticesViewModel();
            ChecklistPlanInsertModel checklist = new ChecklistPlanInsertModel();

            //var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"BestPractices.json"}");
            //var JSON = System.IO.File.ReadAllText(folderDetails);
            var bestPractices = GetMasterTemplate(checklistfile);
            bestPracticesView = JsonConvert.DeserializeObject<BestPracticesViewModel>(bestPractices);

            string UserDet = HttpContext.Session.GetString("_UserDet");
            userViewModel = JsonConvert.DeserializeObject<UserViewModel>(UserDet);
            bestPracticesView.projects = userViewModel.projects;

            if (projectname == null)
            {
                var project = bestPracticesView.projects.FirstOrDefault();
                projectname = project.projectName;
            }
            string checklistdet = GetChecklistDetailsAsync(projectname);

            if (checklistdet != null || checklistdet != "")
            {
                checklist = JsonConvert.DeserializeObject<ChecklistPlanInsertModel>(checklistdet);
                if (checklist.BestPractices.Count > 0)
                {
                    bestPracticesView.BestPractices.Clear();
                    bestPracticesView.BestPractices = checklist.BestPractices;
                }
            }


            bestPracticesView.selectedProjectname = projectname;
            bestPracticesView.projectServices = JsonConvert.DeserializeObject<ProjectViewModel>(GetProjectDetails(projectname));



            return View(bestPracticesView);
        }


        [HttpPost]
        public async Task<ServiceResponse> InsertBestPractices(string data)
        {
            BestPracticesFormInputData dataele = JsonConvert.DeserializeObject<BestPracticesFormInputData>(data);
            ChecklistPlanInsertModel checklistPlanInsert = new ChecklistPlanInsertModel();
            ServiceResponse resp = new ServiceResponse();
            BestPracticesViewModel bestPracticesView = new BestPracticesViewModel();

            string checklist = GetChecklistDetailsAsync(dataele.ProjectName);

            string Requestapi = $"api/CreateChecklist?{ChecklistCode}";

            if (checklist != "")
            {
                checklistPlanInsert = JsonConvert.DeserializeObject<ChecklistPlanInsertModel>(checklist);
                Requestapi = $"api/UpdateChecklist?{ChecklistCode}";
            }

            //var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"BestPractices.json"}");
            //var JSON = System.IO.File.ReadAllText(folderDetails);
            bestPracticesView = JsonConvert.DeserializeObject<BestPracticesViewModel>(GetMasterTemplate(checklistfile));


            foreach (var BestPrac in bestPracticesView.BestPractices)
            {
                foreach (var sec in BestPrac.Section.Where(x => x.Checklist != null).ToList())
                {
                    var section = from x in sec.Checklist
                                  join y in dataele.BestPractices
                                      on new { a = BestPrac.Service, b = sec.ImpactArea, c = x.Title } equals new { a = y.Service, b = y.ImpactArea, c = y.Title }
                                  select new { x, y };

                    foreach (var match in section)
                        match.x.Value = match.y.Value;
                }
            }

            checklistPlanInsert.Project = JsonConvert.DeserializeObject<ProjectViewModel>(GetProjectDetails(dataele.ProjectName));
            checklistPlanInsert.BestPractices = bestPracticesView.BestPractices;
            checklistPlanInsert.UserId = HttpContext.Session.GetString("_userId");

            string json = JsonConvert.SerializeObject(checklistPlanInsert);


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Checklisturl);
                if (checklist != "")
                {
                    var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(checklistPlanInsert), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }
                else
                {
                    var result = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(checklistPlanInsert), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }

            }
            return resp;
        }
    }
}
