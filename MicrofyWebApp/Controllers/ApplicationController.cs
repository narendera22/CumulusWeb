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
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;

namespace MicrofyWebApp.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ILogger<ApplicationController> _logger;

        string Asseturl = string.Empty;
        string AssetCode = string.Empty;
        string SolutionObserBaseUrl = string.Empty;
        string SolutionObserCode = string.Empty;
        private IConfiguration _configuration;
        string Userurl = string.Empty;
        string Usercode = string.Empty;
        string userid = string.Empty;
        string bestpracFolder = string.Empty;
        string configUrl = string.Empty;
        string configCode = string.Empty;
        string ChecklistDeliverablesfile = string.Empty;
        string ApplicationContainer = string.Empty;

        public ApplicationController(ILogger<ApplicationController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Asseturl = _configuration.GetValue<string>("Values:AssetStrgeBaseUrl");
            AssetCode = _configuration.GetValue<string>("Values:AssetStrgeCode");
            SolutionObserBaseUrl = _configuration.GetValue<string>("Values:SolutionObserBaseUrl");
            SolutionObserCode = _configuration.GetValue<string>("Values:SolutionObserCode");
            Userurl = _configuration.GetValue<string>("Values:UsersBaseUrl");
            Usercode = _configuration.GetValue<string>("Values:UsersCode");
            bestpracFolder = _configuration.GetValue<string>("Values:ServiceFolder");
            configUrl = _configuration.GetValue<string>("Values:ConfigBaseUrl");
            configCode = _configuration.GetValue<string>("Values:ConfigCode");
            ChecklistDeliverablesfile = _configuration.GetValue<string>("Values:ChecklistDeliverables");
            ApplicationContainer = _configuration.GetValue<string>("Values:ApplicationContainer");


        }
        public async Task<UserViewModel> ListAllUsersAsync()
        {
            userid = HttpContext.Session.GetString("_userId");
            string authKey = HttpContext.Session.GetString("_AuthKey");

            UserViewModel userViewModel = new UserViewModel();
            string userResponse = string.Empty;
            string Requestapi = $"api/GetUserList?{Usercode}";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", authKey);
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    userResponse = Res.Content.ReadAsStringAsync().Result;
                    userViewModel.usersDetails = JsonConvert.DeserializeObject<List<ListUserDetails>>(value: userResponse);
                }

            }
            return userViewModel;
        }



        public string GetDeliverables()
        {
            string deliverables;
            string Requestapi = $"api/GetDeliverableListFunction?{configCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage file = new HttpResponseMessage();
                file = response.Result;
                deliverables = file.Content.ReadAsStringAsync().Result;
            }

            return deliverables;
        }

        public string GetSolutionObsDetailsAsync(string projectname, string customername)
        {
            string ProjectName = "ProjectName=" + projectname;
            string CustomerName = "CustomerName=" + customername;
            string Requestapi = $"api/GetProjectDetails?{SolutionObserCode}&{ProjectName}&{CustomerName}";

            string SolObs = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SolutionObserBaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = response.Result;
                if (resp.IsSuccessStatusCode)
                {
                    SolObs = resp.Content.ReadAsStringAsync().Result;
                }
            }
            return SolObs;
        }



        public string GetProjectDetails(string Projectname, string customername)
        {
            string ProjectName = "ProjectName=" + Projectname;
            string CustomerName = "CustomerName=" + customername;
            string projDet = string.Empty;
            string Requestapi = $"api/GetProjectDetails?{SolutionObserCode}&{ProjectName}&{customername}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SolutionObserBaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = response.Result;
                if (resp.IsSuccessStatusCode)
                    projDet = resp.Content.ReadAsStringAsync().Result;
            }

            return projDet;
        }

        public string GetAllProjectDetails()
        {
            string projDet = string.Empty;
            string UserId = "UserId=" + HttpContext.Session.GetString("_username");
            string UserRole = "UserRole=" + HttpContext.Session.GetString("_UserRole");
            string Requestapi = $"api/GetAllProjects?{SolutionObserCode}&{UserId}&{UserRole}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SolutionObserBaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = response.Result;
                if (resp.IsSuccessStatusCode)
                    projDet = resp.Content.ReadAsStringAsync().Result;
            }

            return projDet;
        }
        public string GetAzureProductsAndServicesList()
        {
            string config = string.Empty;
            string Requestapi = $"api/GetAzureProductsAndServicesListFunction?{configCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = response.Result;
                if (resp.IsSuccessStatusCode)
                    config = resp.Content.ReadAsStringAsync().Result;
            }

            return config;
        }

        public string GetBestPractices(string service)
        {
            string config = string.Empty;
            string Requestapi = $"api/GetBestPracticesByServiceFunction/{service}?{configCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = response.Result;
                if (resp.IsSuccessStatusCode)
                    config = resp.Content.ReadAsStringAsync().Result;
            }

            return config;
        }

        public string GetServiceByProductCategory(string prodcat, string flag = null)
        {
            string Flag = "Flag=" + flag;
            string config = string.Empty;
            string Requestapi = $"api/GetServiceByProductCategory/{prodcat}?{configCode}&{Flag}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = response.Result;
                if (resp.IsSuccessStatusCode)
                    config = resp.Content.ReadAsStringAsync().Result;
            }

            return config;
        }



        [HttpPost]
        public async Task<ServiceResponse> InsertBestPractices(string data)
        {
            BestPracticesFormInputData dataele = JsonConvert.DeserializeObject<BestPracticesFormInputData>(data);
            ProjectViewModel auditChecklist = new ProjectViewModel();
            ServiceResponse resp = new ServiceResponse();
            AuditViewModel audit = new AuditViewModel();
            Configurations configurations = new Configurations();
            List<BestPractice> bplist = new List<BestPractice>();


            configurations = JsonConvert.DeserializeObject<Configurations>(GetAzureProductsAndServicesList());

            BestPractice bp = new BestPractice();
            var bestPrac = GetBestPractices(dataele.Service);
            bp = JsonConvert.DeserializeObject<BestPractice>(bestPrac);
            bplist.Add(bp);

            string checklist = GetSolutionObsDetailsAsync(dataele.ProjectName, dataele.customername);

            string Requestapi = $"api/UpdateProject?{SolutionObserCode}";

            if (checklist != "")
            {
                auditChecklist = JsonConvert.DeserializeObject<ProjectViewModel>(checklist);
                if (auditChecklist.SolutionObservations.FirstOrDefault().Checklist != null)
                {
                    foreach (var ser in auditChecklist.SolutionObservations.FirstOrDefault().Checklist)
                    {
                        if (ser.Service.Equals(dataele.Service))
                        {
                            bplist.Clear();
                            bp = ser;
                            bplist.Add(bp);
                        }
                    }
                }
            }


            foreach (var BestPrac in bplist)
            {
                foreach (var sec in BestPrac.BestPractices.Where(x => x.Checklist != null).ToList())
                {
                    var section = from x in sec.Checklist
                                  join y in dataele.Checklist
                                      on new { a = BestPrac.Service, b = sec.ImpactArea, c = x.Title } equals new { a = y.Service, b = y.ImpactArea, c = y.Title }
                                  select new { x, y };

                    foreach (var match in section)
                    {
                        match.x.Input.Value = match.y.Value;
                        match.x.Input.Remarks = match.y.Remarks;
                    }
                }
            }

            if (auditChecklist.SolutionObservations.FirstOrDefault().Checklist != null)
            {
                foreach (var ser in auditChecklist.SolutionObservations.FirstOrDefault().Checklist)
                {

                    if (!ser.Service.Equals(dataele.Service))
                    {
                        BestPractice bestPractices = new BestPractice();
                        bestPractices = ser;
                        bplist.Add(bestPractices);
                    }
                }
            }
            audit.BestPractices = bplist;

            auditChecklist.SolutionObservations.FirstOrDefault().Checklist = audit.BestPractices;

            auditChecklist.CreatedUserId = HttpContext.Session.GetString("_userId");
            if (auditChecklist.Status == "In Progress" || auditChecklist.Status == string.Empty || auditChecklist.Status == null || auditChecklist.Status == "New")
                auditChecklist.Status = dataele.Status;
            string json = JsonConvert.SerializeObject(auditChecklist);


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SolutionObserBaseUrl);

                var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(auditChecklist), Encoding.UTF8, "application/json")).Result;
                if (result.IsSuccessStatusCode)
                {
                    resp.isSuccess = result.IsSuccessStatusCode;
                    resp.data = await result.Content.ReadAsStringAsync();
                }


            }
            return resp;
        }
        public async Task<IActionResult> ViewAuditAsync(string projectname = null, string customername = null)
        {
            string userid = HttpContext.Session.GetString("_userId");
            if (userid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            AuditViewModel audit = new AuditViewModel();
            Configurations configurations = new Configurations();
            configurations = JsonConvert.DeserializeObject<Configurations>(GetAzureProductsAndServicesList());
            audit.projectname = projectname;
            //var projdet = GetProjectDetails(projectname);
            //audit.project = JsonConvert.DeserializeObject<ProjectViewModel>(projdet);
            ProjectViewModel auditChecklist = new ProjectViewModel();
            string checklist = GetSolutionObsDetailsAsync(projectname, customername);
            if (checklist != "")
            {
                auditChecklist = JsonConvert.DeserializeObject<ProjectViewModel>(checklist);
            }
            else
            {
                auditChecklist = new ProjectViewModel();
            }
            UserViewModel userViewModel = new UserViewModel();
            userViewModel = await ListAllUsersAsync();
            List<string> users = new List<string>();
            List<string> auditor = new List<string>();
            if (userViewModel.usersDetails != null)
            {
                if (userViewModel.usersDetails.Count > 0)
                {
                    foreach (var usr in userViewModel.usersDetails.Where(q => q.userRole == "User"))
                    {
                        users.Add(usr.fullName.ToString());
                    }
                    foreach (var usr in userViewModel.usersDetails.Where(q => (q.userRole == "Auditor" || q.userRole == "Administrator")))
                    {
                        auditor.Add(usr.fullName.ToString());
                    }
                }
            }
            var deliverables = JsonConvert.DeserializeObject<DeliverablesList>(GetDeliverables());
            audit.Deliverables = deliverables.Deliverables;
            audit.ListUsers = users;
            audit.ListAuditor = auditor;
            audit.Configuration = configurations.Configuration;
            audit.project = auditChecklist;
            foreach (var solobs in auditChecklist.SolutionObservations)
            {
                audit.ProjectDetails = solobs.ProjectDetails;
                audit.Observations = solobs.Observations;
                audit.ActionItems = solobs.ActionItems;
                audit.BestPractices = solobs.Checklist;
            }

            return View(audit);
        }
        public async Task<IActionResult> AuditAsync(string projectname = null, string customername = null)
        {
            string userid = HttpContext.Session.GetString("_userId");
            if (userid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            AuditViewModel audit = new AuditViewModel();
            Configurations configurations = new Configurations();
            configurations = JsonConvert.DeserializeObject<Configurations>(GetAzureProductsAndServicesList());
            audit.projectname = projectname;
            //var projdet = GetProjectDetails(projectname);
            //audit.project = JsonConvert.DeserializeObject<ProjectViewModel>(projdet);
            ProjectViewModel auditChecklist = new ProjectViewModel();
            string checklist = GetSolutionObsDetailsAsync(projectname, customername);
            if (checklist != "")
            {
                auditChecklist = JsonConvert.DeserializeObject<ProjectViewModel>(checklist);
            }
            else
            {
                auditChecklist = new ProjectViewModel();
            }
            UserViewModel userViewModel = new UserViewModel();
            userViewModel = await ListAllUsersAsync();
            List<string> users = new List<string>();
            List<string> auditor = new List<string>();
            if (userViewModel.usersDetails != null)
            {
                if (userViewModel.usersDetails.Count > 0)
                {
                    foreach (var usr in userViewModel.usersDetails.Where(q => q.userRole == "User"))
                    {
                        users.Add(usr.fullName.ToString());
                    }
                    foreach (var usr in userViewModel.usersDetails.Where(q => (q.userRole == "Auditor" || q.userRole == "Administrator")))
                    {
                        auditor.Add(usr.fullName.ToString());
                    }
                }
            }
            var deliverables = JsonConvert.DeserializeObject<DeliverablesList>(GetDeliverables());
            audit.Deliverables = deliverables.Deliverables;
            audit.ListUsers = users;
            audit.ListAuditor = auditor;
            audit.Configuration = configurations.Configuration;
            audit.project = auditChecklist;
            foreach (var solobs in auditChecklist.SolutionObservations)
            {
                audit.ProjectDetails = solobs.ProjectDetails;
                audit.Observations = solobs.Observations;
                audit.ActionItems = solobs.ActionItems;
            }

            return View(audit);
        }
        public async Task<IActionResult> ApplicationAsync()
        {
            string userid = HttpContext.Session.GetString("_userId");
            if (userid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ProjectView projRespon = new ProjectView();
            UserViewModel userViewModel = new UserViewModel();
            AuditViewModel auditViewModel = new AuditViewModel();
            //auditViewModel = await ListAllChecklistAsync();
            userViewModel = await ListAllUsersAsync();
            List<string> users = new List<string>();
            List<string> auditor = new List<string>();
            if (userViewModel.usersDetails != null)
            {
                if (userViewModel.usersDetails.Count > 0)
                {
                    foreach (var usr in userViewModel.usersDetails.Where(q => q.userRole == "User"))
                    {
                        users.Add(usr.fullName.ToString());
                    }
                    foreach (var usr in userViewModel.usersDetails.Where(q => (q.userRole == "Auditor" || q.userRole == "Administrator")))
                    {
                        auditor.Add(usr.fullName.ToString());
                    }
                }
            }
            string projectResponse = string.Empty;
            string Requestapi = string.Empty;
            string UserId = "UserId=" + HttpContext.Session.GetString("_username");
            string UserRole = "UserRole=" + HttpContext.Session.GetString("_UserRole");
            Requestapi = $"api/GetAllProjects?{SolutionObserCode}&{UserId}&{UserRole}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SolutionObserBaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    projectResponse = Res.Content.ReadAsStringAsync().Result;
                    if (projectResponse != null || projectResponse != "")
                        projRespon.ProjectsList = JsonConvert.DeserializeObject<List<ProjectViewModel>>(value: projectResponse);
                }

            }
            projRespon.ListUsers = users;
            projRespon.ListAuditor = auditor;

            return View(projRespon);
        }

        [HttpPost]
        public async Task<ProjectViewModel> UpdateProject(ProjectViewModel project)
        {

            string UserResponse = string.Empty;
            project.CreatedUserId = HttpContext.Session.GetString("_userId");
            var createDoc = JsonConvert.SerializeObject(project);
            ProjectViewModel projectViewModel = new ProjectViewModel();
            string Requestapi = $"api/CreateProject?{SolutionObserCode}";
            string checklist = GetSolutionObsDetailsAsync(project.ProjectName, project.CustomerName);

            if (project.flag == "Update")
            {
                Requestapi = $"api/UpdateProject?{SolutionObserCode}";
                if (checklist != "")
                {
                    projectViewModel = JsonConvert.DeserializeObject<ProjectViewModel>(checklist);
                }
                projectViewModel.ProjectName = projectViewModel.ProjectName;
                projectViewModel.CustomerName = projectViewModel.CustomerName;
                projectViewModel.Status = projectViewModel.Status;
            }
            else
            {
                if (checklist != "")
                {
                    projectViewModel.StatusCode = false;
                    projectViewModel.responseMessage = "Application Name already exists !!!";
                    return projectViewModel;
                }
                projectViewModel.ProjectName = project.ProjectName;
                projectViewModel.CustomerName = project.CustomerName;
                projectViewModel.Status = "New";
            }

            projectViewModel.Auditor = project.Auditor;
            projectViewModel.Users = project.Users;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SolutionObserBaseUrl);

                if (project.flag == "Update")
                {
                    var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(projectViewModel), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        projectViewModel.StatusCode = result.IsSuccessStatusCode;
                        projectViewModel.responseMessage = await result.Content.ReadAsStringAsync();

                    }
                    else
                    {
                        projectViewModel.StatusCode = result.IsSuccessStatusCode;
                        projectViewModel.responseMessage = await result.Content.ReadAsStringAsync();

                    }
                }
                else
                {
                    var result = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(projectViewModel), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        projectViewModel.StatusCode = result.IsSuccessStatusCode;
                        projectViewModel.responseMessage = await result.Content.ReadAsStringAsync();

                    }
                    else
                    {
                        projectViewModel.StatusCode = result.IsSuccessStatusCode;
                        projectViewModel.responseMessage = await result.Content.ReadAsStringAsync();

                    }
                }


            }
            return projectViewModel;
        }

        [HttpPost]
        public async Task<ServiceResponse> InsertChecklist(string data)
        {
            AuditViewModel audit = new AuditViewModel();
            SolutionObservationFormInputData dataele = JsonConvert.DeserializeObject<SolutionObservationFormInputData>(data);
            ServiceResponse resp = new ServiceResponse();
            ProjectViewModel auditChecklist = new ProjectViewModel();
            string checklist = GetSolutionObsDetailsAsync(dataele.projectname, dataele.customername);

            string Requestapi = $"api/UpdateProject?{SolutionObserCode}";

            if (checklist != "")
            {
                auditChecklist = JsonConvert.DeserializeObject<ProjectViewModel>(checklist);
            }


            if (auditChecklist.SolutionObservations.Count <= 0)
            {
                AuditChecklistModel auditChecklistModel = new AuditChecklistModel();
                if (dataele.ProjectDetails != null)
                {
                    auditChecklistModel.ProjectDetails = dataele.ProjectDetails;
                }
                else if (dataele.observation != null)
                {
                    auditChecklistModel.Observations = dataele.observation;
                }
                else if (dataele.ActionItems != null)
                {
                    auditChecklistModel.ActionItems = dataele.ActionItems;
                }
                auditChecklist.SolutionObservations.Add(auditChecklistModel);
            }
            else
            {
                foreach (var solobs in auditChecklist.SolutionObservations)
                {
                    if (dataele.ProjectDetails != null)
                    {
                        solobs.ProjectDetails = dataele.ProjectDetails;
                    }
                    else if (dataele.observation != null)
                    {
                        solobs.Observations = dataele.observation;
                    }
                    else if (dataele.ActionItems != null)
                    {
                        solobs.ActionItems = dataele.ActionItems;
                    }
                }
            }
            auditChecklist.CreatedUserId = HttpContext.Session.GetString("_userId");
            if (auditChecklist.Status == "In Progress" || auditChecklist.Status == string.Empty || auditChecklist.Status == null || auditChecklist.Status == "New")
                auditChecklist.Status = dataele.Status;
            string json = JsonConvert.SerializeObject(auditChecklist);


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SolutionObserBaseUrl);
                var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(auditChecklist), Encoding.UTF8, "application/json")).Result;
                if (result.IsSuccessStatusCode)
                {
                    resp.isSuccess = result.IsSuccessStatusCode;
                    resp.data = await result.Content.ReadAsStringAsync();
                }
            }
            return resp;

        }
        public async Task<ActionResult> LoadChecklist(string projectname, string customername)
        {
            AuditViewModel audit = new AuditViewModel();
            ProjectViewModel auditChecklist = new ProjectViewModel();
            string checklist = GetSolutionObsDetailsAsync(projectname, customername);
            auditChecklist = JsonConvert.DeserializeObject<ProjectViewModel>(checklist);

            audit.ProjectDetails = auditChecklist.SolutionObservations.FirstOrDefault().ProjectDetails;
            List<Azuretech> services = new List<Azuretech>();
            foreach (var pro in audit.ProjectDetails.AzureServicesUsed)
            {
                var configserv = GetServiceByProductCategory(pro.ProductCategory, "");
                var servicelist = JsonConvert.DeserializeObject<Servicelist>(configserv);
                var ser = pro.Services.Split(",");
                foreach (var item in ser)
                {
                    foreach (var s in servicelist.services)
                    {
                        if (item == s.ServiceName)
                        {
                            Azuretech azuretech = new Azuretech();
                            azuretech.Services = item;
                            azuretech.ProductCategory = pro.ProductCategory;
                            services.Add(azuretech);
                        }
                    }
                }
            }
            audit.AzureServicesUsed = services;
            Configurations configurations = new Configurations();
            List<BestPractice> bplist = new List<BestPractice>();

            configurations = JsonConvert.DeserializeObject<Configurations>(GetAzureProductsAndServicesList());

            var firstservice = audit.AzureServicesUsed.FirstOrDefault();


            BestPractice bp = new BestPractice();
            var bestPrac = GetBestPractices(firstservice.Services);
            bp = JsonConvert.DeserializeObject<BestPractice>(bestPrac);
            bplist.Add(bp);
            audit.BestPractices = bplist;


            if (auditChecklist.SolutionObservations.FirstOrDefault().Checklist != null)
            {
                foreach (var auditchk in auditChecklist.SolutionObservations.FirstOrDefault().Checklist)
                {
                    if (auditchk.Service.Equals(firstservice.Services))
                    {
                        //foreach (var auditchksec in auditchk.BestPractices.Where(x => x.Checklist != null).ToList())
                        //{
                        //    foreach (var BestPrac in audit.BestPractices)
                        //    {
                        //        foreach (var sec in BestPrac.BestPractices.Where(x => x.Checklist != null).ToList())
                        //        {
                        //            var section = from x in sec.Checklist
                        //                          join y in auditchksec.Checklist
                        //                              on new { a = BestPrac.Service, b = sec.ImpactArea, c = x.Title } equals new { a = auditchk.Service, b = auditchksec.ImpactArea, c = y.Title }
                        //                          select new { x, y };

                        //            foreach (var match in section)
                        //            {
                        //                match.x.Input.Value = match.y.Input.Value;
                        //                match.x.Input.Remarks = match.y.Input.Remarks;
                        //            }
                        //        }
                        //    }
                        //}
                        audit.BestPractices.Clear();
                        bplist.Add(auditchk);
                        audit.BestPractices = bplist;
                    }
                }

            }

            audit.projectname = auditChecklist.ProjectName;
            audit.customername = auditChecklist.CustomerName;
            audit.Status = auditChecklist.Status;

            return PartialView("VW_CHECKLIST", audit);

        }
        public async Task<ActionResult> LoadService(string productcat, string service, string projectname, string customername)
        {
            AuditViewModel audit = new AuditViewModel();
            ProjectViewModel auditChecklist = new ProjectViewModel();

            Configurations configurations = new Configurations();
            List<BestPractice> bplist = new List<BestPractice>();

            string checklist = GetSolutionObsDetailsAsync(projectname, customername);
            configurations = JsonConvert.DeserializeObject<Configurations>(GetAzureProductsAndServicesList());
            auditChecklist = JsonConvert.DeserializeObject<ProjectViewModel>(checklist);

            audit.ProjectDetails = auditChecklist.SolutionObservations.FirstOrDefault().ProjectDetails;

            BestPractice bp = new BestPractice();
            var bestPrac = GetBestPractices(service);
            bp = JsonConvert.DeserializeObject<BestPractice>(bestPrac);
            bplist.Add(bp);
            audit.BestPractices = bplist;

            if (auditChecklist.SolutionObservations.FirstOrDefault().Checklist != null)
            {
                foreach (var auditchk in auditChecklist.SolutionObservations.FirstOrDefault().Checklist)
                {
                    if (auditchk.Service.Equals(service))
                    {
                        //foreach (var auditchksec in auditchk.BestPractices.Where(x => x.Checklist != null).ToList())
                        //{
                        //    foreach (var BestPrac in audit.BestPractices)
                        //    {
                        //        foreach (var sec in BestPrac.BestPractices.Where(x => x.Checklist != null).ToList())
                        //        {
                        //            var section = from x in sec.Checklist
                        //                          join y in auditchksec.Checklist
                        //                              on new { a = BestPrac.Service, b = sec.ImpactArea, c = x.Title } equals new { a = auditchk.Service, b = auditchksec.ImpactArea, c = y.Title }
                        //                          select new { x, y };

                        //            foreach (var match in section)
                        //            {
                        //                match.x.Input.Value = match.y.Input.Value;
                        //                match.x.Input.Remarks = match.y.Input.Remarks;
                        //            }
                        //        }
                        //    }
                        //}
                        audit.BestPractices.Clear();
                        bplist.Add(auditchk);
                        audit.BestPractices = bplist;
                    }
                }

            }

            audit.projectname = auditChecklist.ProjectName;
            audit.Status = auditChecklist.Status;

            return PartialView("VW_SERVICES", audit);

        }
        public async Task<ActionResult> LoadSummary(string projectname, string customername)
        {
            ProjectViewModel auditChecklist = new ProjectViewModel();
            Summary summary = new Summary();
            string checklist = GetSolutionObsDetailsAsync(projectname, customername);
            auditChecklist = JsonConvert.DeserializeObject<ProjectViewModel>(checklist);
            var countimp = 0;
            var countnotimp = 0;
            var obsCount = 0;
            var actionCount = 0;
            foreach (var sol in auditChecklist.SolutionObservations)
            {
                foreach (var check in sol.Checklist)
                {
                    foreach (var ser in check.BestPractices)
                    {
                        foreach (var chk in ser.Checklist.Where(q => (q.Input.Value == "Required – Implemented" || q.Input.Value == "Required – Alternative Implemented")))
                        {
                            countimp++;
                        }
                    }
                }
                foreach (var check in sol.Checklist)
                {
                    foreach (var ser in check.BestPractices)
                    {
                        foreach (var chk in ser.Checklist.Where(q => q.Input.Value == "Required – Not Implemented"))
                        {
                            countnotimp++;
                        }
                    }
                }
                obsCount = sol.Observations.Count();
                actionCount = sol.ActionItems.Count();
            }


            summary.projectname = auditChecklist.ProjectName;
            summary.customername=auditChecklist.CustomerName;
            summary.RequiredCount = countimp.ToString();
            summary.NotImplementedCount = countnotimp.ToString();
            summary.ObservationCount = obsCount.ToString();
            summary.ActionItemsCount = actionCount.ToString();

            return PartialView("VW_CHECKLIST_SUMMARY", summary);

        }
        public async Task<IActionResult> EditProject(string projectname = null, string customername = null)
        {
            string userid = HttpContext.Session.GetString("_userId");
            if (userid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            AuditViewModel audit = new AuditViewModel();
            var projdet = GetSolutionObsDetailsAsync(projectname, customername);
            audit.project = JsonConvert.DeserializeObject<ProjectViewModel>(projdet);
            UserViewModel userViewModel = new UserViewModel();
            userViewModel = await ListAllUsersAsync();
            List<string> users = new List<string>();
            List<string> auditor = new List<string>();
            if (userViewModel.usersDetails.Count > 0)
            {
                foreach (var usr in userViewModel.usersDetails.Where(q => q.userRole == "User"))
                {
                    users.Add(usr.fullName.ToString());
                }
                foreach (var usr in userViewModel.usersDetails.Where(q => (q.userRole == "Auditor" || q.userRole == "Administrator")))
                {
                    auditor.Add(usr.fullName.ToString());
                }
            }

            audit.ListUsers = users;
            audit.ListAuditor = auditor;
            return View(audit);
        }
        public async Task<bool> DeleteProject(string ApplicationName, string CustomerName)
        {
            string Container ="containername=" + ApplicationContainer;
            string Phase = "Phase=" + CustomerName;
            string SubPhase = "SubPhase=" + ApplicationName;
            string flag = "flag=Application";
            string RequestAssertapi = $"api/Delete/ ?{AssetCode}&{Phase}&{SubPhase}&{Container}&{flag}";

            var resp = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Asseturl);
                var response = client.DeleteAsync(RequestAssertapi).Result;

                if (response.IsSuccessStatusCode)
                {
                    resp = response.IsSuccessStatusCode;
                }

            }
            bool projectresp = false;
            string ProjectName = "ProjectName=" + ApplicationName;
            string customerName = "CustomerName=" + CustomerName;
            
            string Requestapi = $"api/DeleteProject?{SolutionObserCode}&{ProjectName}&{customerName}";
            //string JWTResponse = (string)_cache.Get("_UserLoginResponse");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SolutionObserBaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.DeleteAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    projectresp = Res.IsSuccessStatusCode;
                    //bool Activity = ActivityTracker("DeactivateUser", $"User {userid} has successfully deactivated {username}");

                }

            }
            return projectresp;
        }
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }
        public async Task<IActionResult> PrintView()
        {
            return View();
        }
        //public void OnPostGeneratePDF()
        //{
        //    IronPdf.ChromePdfRenderer Renderer = new IronPdf.ChromePdfRenderer();
        //    using var pdf = Renderer.RenderUrlAsPdf("https://www.nuget.org/packages/IronPdf");
        //    pdf.WatermarkAllPages("<h2 style='color:red'>SAMPLE</h2>", IronPdf.Editing.WaterMarkLocation.MiddleCenter, 50, -45, "https://www.nuget.org/packages/IronPdf");
        //    pdf.SaveAs(@"C:\Path\To\Watermarked.pdf");
        //    pdf.SaveAs(@"c:\Downloads\Watermarked.pdf");
        //}
        [HttpPost]
        //[ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            }
        }
    }
}
