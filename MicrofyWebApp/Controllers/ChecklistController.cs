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
        string ChecklistPlanfile = string.Empty;
        string ChecklistDeliverablesfile = string.Empty;
        string BestPractices = string.Empty;
        string Projecturl = string.Empty;
        string ProjectCode = string.Empty;
        string Checklisturl = string.Empty;
        string ChecklistCode = string.Empty;
        private IConfiguration _configuration;
        string Userurl = string.Empty;
        string Usercode = string.Empty;
        string userid = string.Empty;
        string bestpracFolder = string.Empty;
        string ChecklistconfigUrl = string.Empty;
        string ChecklistconfigCode = string.Empty;

        public ChecklistController(ILogger<ChecklistController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Asseturl = _configuration.GetValue<string>("Values:AssetStrgeBaseUrl");
            AssetCode = _configuration.GetValue<string>("Values:AssetStrgeCode");
            ChecklistPlanfile = _configuration.GetValue<string>("Values:ChecklistPlan");
            ChecklistDeliverablesfile = _configuration.GetValue<string>("Values:ChecklistDeliverables");
            BestPractices = _configuration.GetValue<string>("Values:BestPractices");
            Projecturl = _configuration.GetValue<string>("Values:ProjectBaseUrl");
            ProjectCode = _configuration.GetValue<string>("Values:ProjectCode");
            Checklisturl = _configuration.GetValue<string>("Values:ChecklistUrl");
            ChecklistCode = _configuration.GetValue<string>("Values:ChecklistCode");
            Userurl = _configuration.GetValue<string>("Values:UsersBaseUrl");
            Usercode = _configuration.GetValue<string>("Values:UsersCode");
            bestpracFolder = _configuration.GetValue<string>("Values:ServiceFolder");
            ChecklistconfigUrl = _configuration.GetValue<string>("Values:ChecklistconfigUrl");
            ChecklistconfigCode = _configuration.GetValue<string>("Values:ChecklistconfigCode");

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

        public async Task<IActionResult> ChecklistPlanAsync(string projectname = null)
        {
            string UserDet = HttpContext.Session.GetString("_UserDet");
            string role = HttpContext.Session.GetString("_UserRole");
            if (UserDet == null)
            {
                return RedirectToAction("Login", "Login");
            }
            UserViewModel userViewModel = new UserViewModel();
            ChecklistPlanViewModel checklistPlanView = new ChecklistPlanViewModel();
            ChecklistPlanViewModel checklist = new ChecklistPlanViewModel();

            checklistPlanView = JsonConvert.DeserializeObject<ChecklistPlanViewModel>(GetMasterTemplate(ChecklistPlanfile));

            List<projects> projectlist = new List<projects>();
            if (role == "Administrator")
            {
                userViewModel = await ListAllUsersAsync();
                foreach (var usr in userViewModel.usersDetails)
                {
                    foreach (var prj in usr.projects)
                    {
                        if (!(projectlist.Any(pro => pro.projectName == prj.projectName)))
                        {
                            projects prjmodel = new projects();
                            prjmodel.projectName = prj.projectName;
                            prjmodel.customerName = prj.customerName;
                            projectlist.Add(prjmodel);
                        }
                    }
                }
                checklistPlanView.projects = projectlist;
            }
            else
            {
                userViewModel = JsonConvert.DeserializeObject<UserViewModel>(UserDet);
                checklistPlanView.projects = userViewModel.projects;
            }


            if (projectname == null)
            {
                var project = checklistPlanView.projects.FirstOrDefault();
                projectname = project.projectName;
            }

            string checklistdet = GetChecklistDetailsAsync(projectname);
            if (checklistdet != "")
                checklist = JsonConvert.DeserializeObject<ChecklistPlanViewModel>(checklistdet);
            if (checklist.Plans != null)
            {
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
            }
            checklistPlanView.selectedProjectname = projectname;
            checklistPlanView.userRole = role;

            return View(checklistPlanView);
        }

        public string GetMasterTemplate(string mastertemplate, string foldername = null)
        {
            string folder = $"folder={foldername}";
            string template = string.Empty;
            string Requestapi = $"api/GetTemplate/{mastertemplate}?{AssetCode}&{folder}";
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

        public async Task<IActionResult> ChecklistDeliverableAsync(string projectname = null)
        {

            string UserDet = HttpContext.Session.GetString("_UserDet");
            string role = HttpContext.Session.GetString("_UserRole");
            if (UserDet == null)
            {
                return RedirectToAction("Login", "Login");
            }
            UserViewModel userViewModel = new UserViewModel();
            ChecklistPlanViewModel checklistPlanView = new ChecklistPlanViewModel();
            ChecklistPlanViewModel checklist = new ChecklistPlanViewModel();

            checklistPlanView = JsonConvert.DeserializeObject<ChecklistPlanViewModel>(GetMasterTemplate(ChecklistDeliverablesfile));

            List<projects> projectlist = new List<projects>();
            if (role == "Administrator")
            {
                userViewModel = await ListAllUsersAsync();
                foreach (var usr in userViewModel.usersDetails)
                {
                    foreach (var prj in usr.projects)
                    {
                        if (!(projectlist.Any(pro => pro.projectName == prj.projectName)))
                        {
                            projects prjmodel = new projects();
                            prjmodel.projectName = prj.projectName;
                            prjmodel.customerName = prj.customerName;
                            projectlist.Add(prjmodel);
                        }
                    }
                }
                checklistPlanView.projects = projectlist;
            }
            else
            {
                userViewModel = JsonConvert.DeserializeObject<UserViewModel>(UserDet);
                checklistPlanView.projects = userViewModel.projects;
            }

            if (projectname == null)
            {
                var project = checklistPlanView.projects.FirstOrDefault();
                projectname = project.projectName;
            }

            string checklistdet = GetChecklistDetailsAsync(projectname);
            if (checklistdet != "")
                checklist = JsonConvert.DeserializeObject<ChecklistPlanViewModel>(checklistdet);
            if (checklist.Deliverables != null)
            {
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
            }
            checklistPlanView.selectedProjectname = projectname;
            checklistPlanView.userRole = role;

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
            var projdet = GetProjectDetails(dataele.ProjectName);
            if (projdet != string.Empty)
            {
                checklistPlanInsert.Project = JsonConvert.DeserializeObject<ProjectViewModel>(projdet);
            }
            else
            {
                checklistPlanInsert.Project = new ProjectViewModel();
                checklistPlanInsert.Project.ProjectName = dataele.ProjectName;
            }
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
                if (resp.IsSuccessStatusCode)
                    projDet = resp.Content.ReadAsStringAsync().Result;
            }

            return projDet;
        }

        public string GetAllProjectDetails()
        {
            string projDet = string.Empty;
            string Requestapi = $"api/GetAllProjects?{ProjectCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Projecturl);
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
        public string GetChecklistConfig()
        {
            string config = string.Empty;
            string Requestapi = $"api/GetChecklistConfig?{ChecklistconfigCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ChecklistconfigUrl);
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

            var projdet = GetProjectDetails(dataele.ProjectName);
            if (projdet != string.Empty)
            {
                checklistPlanInsert.Project = JsonConvert.DeserializeObject<ProjectViewModel>(projdet);
            }
            else
            {
                checklistPlanInsert.Project = new ProjectViewModel();
                checklistPlanInsert.Project.ProjectName = dataele.ProjectName;
            }
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

        public async Task<IActionResult> ChecklistBestPracticesAsync(string projectname = null)
        {
            string UserDet = HttpContext.Session.GetString("_UserDet");
            string role = HttpContext.Session.GetString("_UserRole");

            if (UserDet == null)
            {
                return RedirectToAction("Login", "Login");
            }
            UserViewModel userViewModel = new UserViewModel();
            BestPracticesViewModel bestPracticesView = new BestPracticesViewModel();
            ChecklistPlanInsertModel checklist = new ChecklistPlanInsertModel();
            Configurations configurations = new Configurations();
            List<BestPractices> bplist = new List<BestPractices>();

            //var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"ChecklistConfig.json"}");
            //var JSON = System.IO.File.ReadAllText(folderDetails);
            configurations = JsonConvert.DeserializeObject<Configurations>(GetChecklistConfig());

            //foreach (var config in configurations.Services)
            //{
            //    BestPractices bp = new BestPractices();
            //    var bestPrac = GetMasterTemplate(config.File, bestpracFolder);
            //    bp = JsonConvert.DeserializeObject<BestPractices>(bestPrac);
            //    bplist.Add(bp);
            //}
            bestPracticesView.BestPractices = bplist;
            //var bestPractices = GetMasterTemplate(BestPractices);
            //bestPracticesView = JsonConvert.DeserializeObject<BestPracticesViewModel>(bestPractices);

            List<projects> projectlist = new List<projects>();
            if (role == "Administrator")
            {
                userViewModel = await ListAllUsersAsync();
                foreach (var usr in userViewModel.usersDetails)
                {
                    foreach (var prj in usr.projects)
                    {
                        if (!(projectlist.Any(pro => pro.projectName == prj.projectName)))
                        {
                            projects prjmodel = new projects();
                            prjmodel.projectName = prj.projectName;
                            prjmodel.customerName = prj.customerName;
                            projectlist.Add(prjmodel);
                        }
                    }
                }
                bestPracticesView.projects = projectlist;
            }
            else
            {
                userViewModel = JsonConvert.DeserializeObject<UserViewModel>(UserDet);
                bestPracticesView.projects = userViewModel.projects;
            }

            if (projectname == null)
            {
                var project = bestPracticesView.projects.FirstOrDefault();
                projectname = project.projectName;
            }
            string checklistdet = GetChecklistDetailsAsync(projectname);

            //if (checklistdet != "")
            //{
            //    checklist = JsonConvert.DeserializeObject<ChecklistPlanInsertModel>(checklistdet);
            //    if (checklist.BestPractices.Count > 0)
            //    {
            //        //bestPracticesView.BestPractices.Clear();
            //        foreach (var bp in bestPracticesView.BestPractices)
            //        {
            //            foreach (var chk in checklist.BestPractices)
            //            {
            //                if (bp.Service == chk.Service)
            //                {
            //                    foreach (var sec in bp.Section)
            //                    {
            //                        foreach (var chksec in chk.Section)
            //                        {
            //                            if (sec.ImpactArea == chksec.ImpactArea)
            //                            {
            //                                foreach (var chklist in sec.Checklist)
            //                                {
            //                                    foreach (var check in chksec.Checklist)
            //                                    {
            //                                        if (chklist.Title == check.Title)
            //                                        {
            //                                            chklist.Value = check.Value;
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        //bestPracticesView.BestPractices = checklist.BestPractices;
            //    }
            //}


            bestPracticesView.selectedProjectname = projectname;
            bestPracticesView.projectServices = JsonConvert.DeserializeObject<ProjectViewModel>(GetProjectDetails(projectname));

            bestPracticesView.userRole = role;

            return View(bestPracticesView);
        }


        [HttpPost]
        public async Task<ServiceResponse> InsertBestPractices(string data)
        {
            BestPracticesFormInputData dataele = JsonConvert.DeserializeObject<BestPracticesFormInputData>(data);
            AuditChecklistModel auditChecklist = new AuditChecklistModel();
            ServiceResponse resp = new ServiceResponse();
            AuditViewModel audit = new AuditViewModel();
            Configurations configurations = new Configurations();
            List<BestPractices> bplist = new List<BestPractices>();


            configurations = JsonConvert.DeserializeObject<Configurations>(GetChecklistConfig());
            var configfile = string.Empty;
            foreach (var con in configurations.Configuration)
            {
                if (con.ProductCategory == dataele.ProductCategory)
                {
                    foreach (var ser in con.Services)
                    {
                        if (ser.Name == dataele.Service)
                        {
                            configfile = ser.FileName;
                        }
                    }
                }
            }
            BestPractices bp = new BestPractices();
            var bestPrac = GetMasterTemplate(configfile, bestpracFolder);
            bp = JsonConvert.DeserializeObject<BestPractices>(bestPrac);
            bplist.Add(bp);

            string checklist = GetChecklistDetailsAsync(dataele.ProjectName);

            string Requestapi = $"api/CreateChecklist?{ChecklistCode}";

            if (checklist != "")
            {
                auditChecklist = JsonConvert.DeserializeObject<AuditChecklistModel>(checklist);
                Requestapi = $"api/UpdateChecklist?{ChecklistCode}";
            }
            //if (dataele.Checklist != null)
            //{
            //    auditChecklist.ProjectDetails = dataele.ProjectDetails;
            //}

            foreach (var BestPrac in bplist)
            {
                foreach (var sec in BestPrac.Section.Where(x => x.Checklist != null).ToList())
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

            if (auditChecklist.Checklist != null)
            {
                foreach (var ser in auditChecklist.Checklist)
                {
                    //if (ser.Service.Contains(dataele.Service))
                    //{
                    //    auditChecklist.Checklist.Remove(ser);
                    //    auditChecklist.Checklist = audit.BestPractices;
                    //}
                    //else if (!ser.Service.Contains(dataele.Service))
                    //{
                    //    auditChecklist.Checklist = audit.BestPractices;
                    //}
                    if (!ser.Service.Equals(dataele.Service))
                    {
                        BestPractices bestPractices = new BestPractices();
                        bestPractices = ser;
                        bplist.Add(bestPractices);
                    }
                }
            }
            audit.BestPractices = bplist;

            auditChecklist.Checklist = audit.BestPractices;

            auditChecklist.UserId = HttpContext.Session.GetString("_userId");
            if (auditChecklist.Status == "Partially Completed")
                auditChecklist.Status = dataele.Status;
            string json = JsonConvert.SerializeObject(auditChecklist);


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Checklisturl);
                if (checklist != "")
                {
                    var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(auditChecklist), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }
                else
                {
                    var result = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(auditChecklist), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }

            }
            return resp;
        }
        public async Task<IActionResult> AuditAsync(string projectname = null)
        {
            AuditViewModel audit = new AuditViewModel();
            Configurations configurations = new Configurations();
            configurations = JsonConvert.DeserializeObject<Configurations>(GetChecklistConfig());
            audit.projectname = projectname;
            var projdet = GetProjectDetails(projectname);
            audit.project = JsonConvert.DeserializeObject<ProjectViewModel>(projdet);
            AuditChecklistModel auditChecklist = new AuditChecklistModel();
            string checklist = GetChecklistDetailsAsync(projectname);
            if (checklist != "")
            {
                auditChecklist = JsonConvert.DeserializeObject<AuditChecklistModel>(checklist);
            }
            else
            {
                auditChecklist = new AuditChecklistModel();
            }
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
                foreach (var usr in userViewModel.usersDetails.Where(q => q.userRole == "Auditor"))
                {
                    auditor.Add(usr.fullName.ToString());
                }
            }
            ChecklistPlanViewModel checklistPlanView = new ChecklistPlanViewModel();
            checklistPlanView = JsonConvert.DeserializeObject<ChecklistPlanViewModel>(GetMasterTemplate(ChecklistDeliverablesfile));

            audit.ListUsers = users;
            audit.ListAuditor = auditor;
            audit.Deliverables = checklistPlanView.Deliverables;
            audit.Configuration = configurations.Configuration;
            audit.ProjectDetails = auditChecklist.ProjectDetails;
            audit.Observations = auditChecklist.Observations;
            audit.ActionItems = auditChecklist.ActionItems;
            return View(audit);
        }
        public async Task<IActionResult> ProjectAsync()
        {
            ProjectView projRespon = new ProjectView();
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
                foreach (var usr in userViewModel.usersDetails.Where(q => q.userRole == "Auditor"))
                {
                    auditor.Add(usr.fullName.ToString());
                }
            }

            string projectResponse = string.Empty;
            string Requestapi = string.Empty;
            Requestapi = $"api/GetAllProjects?{ProjectCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Projecturl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    projectResponse = Res.Content.ReadAsStringAsync().Result;
                    projRespon.ProjectsList = JsonConvert.DeserializeObject<List<ProjectViewModel>>(value: projectResponse);
                }

            }
            projRespon.ListUsers = users;
            projRespon.ListAuditor = auditor;
            return View(projRespon);
        }
        [HttpPost]
        public async Task<ServiceResponse> InsertChecklist(string data)
        {
            AuditViewModel audit = new AuditViewModel();
            ChecklistFormInputData dataele = JsonConvert.DeserializeObject<ChecklistFormInputData>(data);
            ServiceResponse resp = new ServiceResponse();
            AuditChecklistModel auditChecklist = new AuditChecklistModel();
            string checklist = GetChecklistDetailsAsync(dataele.projectname);

            string Requestapi = $"api/CreateChecklist?{ChecklistCode}";

            if (checklist != "")
            {
                auditChecklist = JsonConvert.DeserializeObject<AuditChecklistModel>(checklist);
                Requestapi = $"api/UpdateChecklist?{ChecklistCode}";
            }
            if (dataele.ProjectDetails != null)
            {
                auditChecklist.ProjectDetails = dataele.ProjectDetails;
            }
            else if (dataele.observation != null)
            {
                auditChecklist.Observations = dataele.observation;
            }
            else if (dataele.ActionItems != null)
            {
                auditChecklist.ActionItems = dataele.ActionItems;
            }
            auditChecklist.UserId = HttpContext.Session.GetString("_userId");
            if(auditChecklist.Status== "Partially Completed")
                auditChecklist.Status = dataele.Status;
            string json = JsonConvert.SerializeObject(auditChecklist);


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Checklisturl);
                if (checklist != "")
                {
                    var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(auditChecklist), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }
                else
                {
                    var result = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(auditChecklist), Encoding.UTF8, "application/json")).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resp.isSuccess = result.IsSuccessStatusCode;
                        resp.data = await result.Content.ReadAsStringAsync();
                    }
                }

            }
            return resp;

        }
        public async Task<ActionResult> LoadChecklist(string projectname)
        {
            AuditViewModel audit = new AuditViewModel();
            AuditChecklistModel auditChecklist = new AuditChecklistModel();
            string checklist = GetChecklistDetailsAsync(projectname);
            auditChecklist = JsonConvert.DeserializeObject<AuditChecklistModel>(checklist);

            audit.ProjectDetails = auditChecklist.ProjectDetails;
            List<Azuretech> services = new List<Azuretech>();
            foreach (var pro in audit.ProjectDetails.AzureServicesUsed)
            {
                var ser = pro.Services.Split(",");
                foreach (var item in ser)
                {
                    Azuretech azuretech = new Azuretech();
                    azuretech.Services = item;
                    azuretech.ProductCategory = pro.ProductCategory;
                    services.Add(azuretech);
                }
            }
            audit.AzureServicesUsed = services;
            Configurations configurations = new Configurations();
            List<BestPractices> bplist = new List<BestPractices>();

            configurations = JsonConvert.DeserializeObject<Configurations>(GetChecklistConfig());

            var firstservice = audit.AzureServicesUsed.FirstOrDefault();
            var configfile = string.Empty;
            foreach (var con in configurations.Configuration)
            {
                if (con.ProductCategory == firstservice.ProductCategory)
                {
                    foreach (var ser in con.Services)
                    {
                        if (ser.Name == firstservice.Services)
                        {
                            configfile = ser.FileName;
                        }
                    }
                }
            }

            BestPractices bp = new BestPractices();
            var bestPrac = GetMasterTemplate(configfile, bestpracFolder);
            bp = JsonConvert.DeserializeObject<BestPractices>(bestPrac);
            bplist.Add(bp);
            audit.BestPractices = bplist;


            if (auditChecklist.Checklist != null)
            {
                foreach (var auditchk in auditChecklist.Checklist)
                {
                    if (auditchk.Service.Equals(firstservice.Services))
                    {
                        foreach (var auditchksec in auditchk.Section.Where(x => x.Checklist != null).ToList())
                        {
                            foreach (var BestPrac in audit.BestPractices)
                            {
                                foreach (var sec in BestPrac.Section.Where(x => x.Checklist != null).ToList())
                                {
                                    var section = from x in sec.Checklist
                                                  join y in auditchksec.Checklist
                                                      on new { a = BestPrac.Service, b = sec.ImpactArea, c = x.Title } equals new { a = auditchk.Service, b = auditchksec.ImpactArea, c = y.Title }
                                                  select new { x, y };

                                    foreach (var match in section)
                                    {
                                        match.x.Input.Value = match.y.Input.Value;
                                        match.x.Input.Remarks = match.y.Input.Remarks;
                                    }
                                }
                            }
                        }
                    }
                }

            }



            return PartialView("VW_CHECKLIST", audit);

        }
        public async Task<ActionResult> LoadService(string productcat, string service, string projectname)
        {
            AuditViewModel audit = new AuditViewModel();
            AuditChecklistModel auditChecklist = new AuditChecklistModel();

            Configurations configurations = new Configurations();
            List<BestPractices> bplist = new List<BestPractices>();

            string checklist = GetChecklistDetailsAsync(projectname);
            configurations = JsonConvert.DeserializeObject<Configurations>(GetChecklistConfig());
            auditChecklist = JsonConvert.DeserializeObject<AuditChecklistModel>(checklist);

            audit.ProjectDetails = auditChecklist.ProjectDetails;
            var configfile = string.Empty;
            foreach (var con in configurations.Configuration)
            {
                if (con.ProductCategory == productcat)
                {
                    foreach (var ser in con.Services)
                    {
                        if (ser.Name == service)
                        {
                            configfile = ser.FileName;
                        }
                    }
                }
            }

            BestPractices bp = new BestPractices();
            var bestPrac = GetMasterTemplate(configfile, bestpracFolder);
            bp = JsonConvert.DeserializeObject<BestPractices>(bestPrac);
            bplist.Add(bp);
            audit.BestPractices = bplist;

            if (auditChecklist.Checklist != null)
            {
                foreach (var auditchk in auditChecklist.Checklist)
                {
                    if (auditchk.Service.Equals(service))
                    {
                        foreach (var auditchksec in auditchk.Section.Where(x => x.Checklist != null).ToList())
                        {
                            foreach (var BestPrac in audit.BestPractices)
                            {
                                foreach (var sec in BestPrac.Section.Where(x => x.Checklist != null).ToList())
                                {
                                    var section = from x in sec.Checklist
                                                  join y in auditchksec.Checklist
                                                      on new { a = BestPrac.Service, b = sec.ImpactArea, c = x.Title } equals new { a = auditchk.Service, b = auditchksec.ImpactArea, c = y.Title }
                                                  select new { x, y };

                                    foreach (var match in section)
                                    {
                                        match.x.Input.Value = match.y.Input.Value;
                                        match.x.Input.Remarks = match.y.Input.Remarks;
                                    }
                                }
                            }
                        }
                    }
                }

            }



            return PartialView("VW_SERVICES", audit);

        }
    }
}
