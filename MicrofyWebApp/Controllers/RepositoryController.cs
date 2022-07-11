using MicrofyWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace MicrofyWebApp.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly ILogger<RepositoryController> _logger;
        private IMemoryCache _cache;

        string Docurl = string.Empty;
        string Asseturl = string.Empty;
        string ConfigUrl = string.Empty;
        string DocCode = string.Empty;
        string AssetCode = string.Empty;
        string ConfigCode = string.Empty;
        string Activityurl = string.Empty;
        string ActivityCode = string.Empty;
        string userid = string.Empty;
        string Searchurl = string.Empty;
        string SearchCode = string.Empty;
        private IConfiguration _configuration;
        string Userurl = string.Empty;
        string Usercode = string.Empty;
        string ApplicationContainer = string.Empty;
        string DocumentContainer = string.Empty;

        public RepositoryController(ILogger<RepositoryController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _logger = logger;
            _cache = memoryCache;
            _configuration = configuration;
            Docurl = _configuration.GetValue<string>("Values:DocumentBaseUrl");
            DocCode = _configuration.GetValue<string>("Values:DocumentCode");
            Asseturl = _configuration.GetValue<string>("Values:AssetStrgeBaseUrl");
            AssetCode = _configuration.GetValue<string>("Values:AssetStrgeCode");
            ConfigUrl = _configuration.GetValue<string>("Values:ConfigBaseUrl");
            ConfigCode = _configuration.GetValue<string>("Values:ConfigCode");
            Activityurl = _configuration.GetValue<string>("Values:ActivityBaseUrl");
            ActivityCode = _configuration.GetValue<string>("Values:ActivityCode");
            Searchurl = _configuration.GetValue<string>("Values:SearchBaseUrl");
            SearchCode = _configuration.GetValue<string>("Values:SearchCode");
            Userurl = _configuration.GetValue<string>("Values:UsersBaseUrl");
            Usercode = _configuration.GetValue<string>("Values:UsersCode");
            DocumentContainer = _configuration.GetValue<string>("Values:DocumentContainer");
            ApplicationContainer = _configuration.GetValue<string>("Values:ApplicationContainer");

        }
        public async Task<IActionResult> DashboardAsync()
        {
            //string username = (string)_cache.Get("_UserId");
            userid = HttpContext.Session.GetString("_userId");
            if (userid == null)
            {
                return RedirectToAction("Login", "Login");
            }

            MenuViewModel PhaseModel = new MenuViewModel();
            DocumentViewModel documentModel = new DocumentViewModel();
            string Phase;
            string Requestapi = $"api/GetMenuListFunction?{ConfigCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    Phase = Res.Content.ReadAsStringAsync().Result;
                    PhaseModel = JsonConvert.DeserializeObject<MenuViewModel>(Phase);
                    documentModel = await GetDocumentListAsync();
                    PhaseModel.documentRepository = documentModel.documentRepository;
                    PhaseModel.UserRole = HttpContext.Session.GetString("_UserRole");

                }
            }
            HttpContext.Session.SetString("_selPhase", "Dashboard");
            HttpContext.Session.SetString("_selSubPhase", string.Empty);
            return View(PhaseModel);
        }

        public async Task<IActionResult> RepositoryAsync(string menulevel1, string menulevel2)
        {
            DocumentViewModel DocModel = new DocumentViewModel();

            DocModel = await GetDocumentListAsync();
            if (menulevel2 == "" || menulevel2 == string.Empty || menulevel2 == null)
            {
                menulevel2 = string.Empty;
            }
            DocModel.selectedPhase = menulevel1;
            DocModel.selectedSubPhases = menulevel2;
            DocModel.UserRole = HttpContext.Session.GetString("_UserRole");
            HttpContext.Session.SetString("_selPhase", menulevel1);
            HttpContext.Session.SetString("_selSubPhase", menulevel2);

            return View(DocModel);
        }

        [HttpPost]
        public async Task<FileUploadResponse> UploadAsync(string flag,string phase, string subphase, IFormFile file, string tags, string displaytags)
        {
            string Container = string.Empty;
            if (flag== "Document")
            {
                Container = "containername=" + DocumentContainer;
            }
            else if(flag== "Application")
            {
                Container = "containername=" + ApplicationContainer;
            }
            using (var client = new HttpClient())
            {
                byte[] data;
                string Phase = "Phase=" + Encoder(phase);
                string SubPhase = "SubPhase=" + Encoder(subphase);
                string Requestapi = $"api/Upload?{AssetCode}&{Phase}&{SubPhase}&{Container}";
                FileUploadResponse FileUploadReponseValue = new FileUploadResponse();

                using (var br = new BinaryReader(file.OpenReadStream()))
                {
                    data = br.ReadBytes((int)file.OpenReadStream().Length);
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    MultipartFormDataContent multiContent = new MultipartFormDataContent();
                    multiContent.Add(bytes, "file", file.FileName);
                    client.BaseAddress = new Uri(Asseturl);
                    client.DefaultRequestHeaders.Add("DisplayTags", displaytags);
                    client.DefaultRequestHeaders.Add("UserTags", tags);
                    var response = client.PostAsync(Requestapi, multiContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        FileUploadReponseValue.statuscode = response.IsSuccessStatusCode;
                        FileUploadReponseValue.url = await response.Content.ReadAsStringAsync();
                        //bool Activity = ActivityTracker("UploadDocument", $"New Document uploaded in url {FileUploadReponseValue.url}");

                    }
                    else
                    {
                        FileUploadReponseValue.statuscode = response.IsSuccessStatusCode;
                        FileUploadReponseValue = JsonConvert.DeserializeObject<FileUploadResponse>(await response.Content.ReadAsStringAsync());

                    }
                    return FileUploadReponseValue;

                }
            }

        }

        [HttpPost]
        public async Task<ActionResult> CreateDocumentAsync(CreateDocuments create)
        {
            DocumentViewModel DocModel = new DocumentViewModel();

            create.UploadedOn = DateTime.Now.ToString("dd/MM/yyyy");
            create.UploadedBy = HttpContext.Session.GetString("_username");

            var createDoc = JsonConvert.SerializeObject(create);
            string Requestapi = $"api/Document?{DocCode}";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Docurl);
                var result = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(create), Encoding.UTF8, "application/json")).Result;
                if (result.IsSuccessStatusCode)
                {
                    _cache.Remove("_GetDocList");
                    DocModel = await GetDocumentListAsync();
                    DocModel.selectedPhase = create.Phase;
                    DocModel.selectedSubPhases = create.SubPhase;
                    DocModel.UserRole = HttpContext.Session.GetString("_UserRole");
                    userid = HttpContext.Session.GetString("_userId");
                    //bool Activity = ActivityTracker("NewDocument", $"User {userid} has posted a new document {Path.GetFileName(create.URL)} at {create.URL}");
                }
            }
            return PartialView("VW_Document_Repos_Partial", DocModel);

        }



        [HttpGet]
        public async Task<ActionResult> GetDocumentRepositoryAsync(string Phase, string SubPhase)
        {
            DocumentViewModel DocModel = new DocumentViewModel();

            DocModel = await GetDocumentListAsync();

            DocModel.selectedPhase = Phase;
            DocModel.selectedSubPhases = SubPhase;
            DocModel.UserRole = HttpContext.Session.GetString("_UserRole");

            return PartialView("VW_Document_Repos_Partial", DocModel);
        }

        public async Task<DocumentViewModel> GetDocumentListAsync()
        {
            string DocRepos = string.Empty;
            DocumentViewModel DocModel = new DocumentViewModel();
            string Requestapi = $"api/Get?{DocCode}";

            if (!_cache.TryGetValue("_GetDocList", out DocRepos))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Docurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync(Requestapi);
                    if (Res.IsSuccessStatusCode)
                    {
                        DocRepos = Res.Content.ReadAsStringAsync().Result;
                        if (DocRepos != null || DocRepos != string.Empty)
                        {
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromSeconds(8000));

                            _cache.Set("_GetDocList", DocRepos, cacheEntryOptions);

                        }
                    }

                }
            }
            //var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"document.json"}");
            //var JSON = System.IO.File.ReadAllText(folderDetails);
            //DocModel = JsonConvert.DeserializeObject<DocumentViewModel>(JSON);
            if (DocRepos != null)
            {
                DocModel = JsonConvert.DeserializeObject<DocumentViewModel>(value: DocRepos);
            }
            return DocModel;
        }

        public async Task<ActionResult> GetUploadPartialAsync(string phase, string subphase, string documentname)
        {
            DocumentViewModel DocModel = new DocumentViewModel();
            string DocRepos = string.Empty;

            DocModel = await GetDocumentListAsync();

            DocModel.selectedPhase = phase;
            DocModel.selectedSubPhases = subphase;
            DocModel.selectedDocName = documentname;

            return PartialView("VW_Upload_Partial", DocModel);

        }

        public async Task<ActionResult> SearchDocumentPartial(string Search)
        {
            DocumentViewModel DocModel = new DocumentViewModel();
            string serachResp = string.Empty;
            string searchRes = "search=" + Search;
            string Requestapi = $"api/Search?{SearchCode}&{searchRes}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Searchurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    serachResp = Res.Content.ReadAsStringAsync().Result;
                }
            }

            DocModel.searchResults = JsonConvert.DeserializeObject<List<SearchResult>>(value: serachResp);
            return PartialView("VW_Search_Partial", DocModel);

        }

        public async Task<ActionResult> DocumentSearch(string Search)
        {
            DocumentViewModel DocModel = new DocumentViewModel();
            string serachResp = string.Empty;
            string searchRes = "search=" + Search;
            string Requestapi = $"api/Search?{SearchCode}&{searchRes}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Searchurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    serachResp = Res.Content.ReadAsStringAsync().Result;
                }
            }

            DocModel.searchResults = JsonConvert.DeserializeObject<List<SearchResult>>(value: serachResp);
            return View(DocModel);

        }
        public ActionResult GetNewUploadPartial()
        {
            return PartialView("VW_Upload_NewDoc_Partial");
        }

        public FileResult DownloadDocument(string url, string phase, string subphase,string flag)
        {
            string Container = string.Empty;
            if (flag == "Document")
            {
                Container = "containername=" + DocumentContainer;
            }
            else if (flag == "Application")
            {
                Container = "containername=" + ApplicationContainer;
            }
            string filename = Path.GetFileName(url);
            string Phase = "Phase=" + Encoder(phase);
            string SubPhase = "SubPhase=" + Encoder(subphase);
            string Requestapi = $"api/Download/{filename}?{AssetCode}&{Phase}&{SubPhase}&{Container}";
            bool Activity;
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(Asseturl);
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage file = new HttpResponseMessage();
                file = response.Result;
                Activity = ActivityTracker("DownloadDocument", $"{Path.GetFileName(Path.GetDirectoryName(url))}/{Path.GetFileName(url)}");
                return File(file.Content.ReadAsByteArrayAsync().Result, "application/octet-stream", filename);
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        public bool ActivityTracker(string ActivityType, string ActivityDetails)
        {
            userid = HttpContext.Session.GetString("_userId");
            ActivityTracker activity = new ActivityTracker();
            activity.UserName = userid;
            activity.ActivityType = ActivityType;
            activity.ActivityDetails = ActivityDetails;

            string Requestapi = $"api/TrackActivity?{ActivityCode}";
            var resp = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Activityurl);
                var result = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(activity), Encoding.UTF8, "application/json")).Result;
                if (result.IsSuccessStatusCode)
                {
                    resp = result.IsSuccessStatusCode;
                }
            }

            return resp;
        }



        [HttpPost]
        public bool UpdateMetadata(UpdateMetadata metadata)
        {
            string Container = string.Empty;
            if (metadata.flag == "Document")
            {
                Container = "containername=" + DocumentContainer;
            }
            else if (metadata.flag == "Application")
            {
                Container = "containername=" + ApplicationContainer;
            }
            string Phase = "Phase=" + Encoder(metadata.phase);
            string SubPhase = "SubPhase=" + Encoder(metadata.subphase);
            string file = Path.GetFileName(metadata.filename);
            string Requestapi = $"api/UpdateMetaData/{file}?{AssetCode}&{Phase}&{SubPhase}&{Container}";
            var resp = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Asseturl);
                client.DefaultRequestHeaders.Add("DisplayTags", metadata.displaytags);
                client.DefaultRequestHeaders.Add("UserTags", metadata.usertags);
                var response = client.GetAsync(Requestapi).Result;

                if (response.IsSuccessStatusCode)
                {
                    resp = response.IsSuccessStatusCode;
                }

            }
            return resp;
        }
        [HttpPost]
        public async Task<ActionResult> DeleteDocumentAsync(DeleteDoc deletedoc)
        {
            string Container = string.Empty;
            if (deletedoc.flag == "Document")
            {
                Container = "containername=" + DocumentContainer;
            }
            else if (deletedoc.flag == "Application")
            {
                Container = "containername=" + ApplicationContainer;
            }
            string Phase = "Phase=" + Encoder(deletedoc.phase);
            string SubPhase = "SubPhase=" + Encoder(deletedoc.subphase);
            string file = Path.GetFileName(deletedoc.filename);
            string RequestAssertapi = $"api/Delete/{file}?{AssetCode}&{Phase}&{SubPhase}&{Container}";
            string RequestDocapi = $"api/Delete/{deletedoc.documentname}?{DocCode}&{Phase}&{SubPhase}";
            DocumentViewModel DocModel = new DocumentViewModel();
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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Docurl);
                var response = client.DeleteAsync(RequestDocapi).Result;

                if (response.IsSuccessStatusCode)
                {
                    _cache.Remove("_GetDocList");
                    DocModel = await GetDocumentListAsync();
                    DocModel.selectedPhase = deletedoc.phase;
                    DocModel.selectedSubPhases = deletedoc.subphase;
                    DocModel.UserRole = HttpContext.Session.GetString("_UserRole");
                }
            }
            return PartialView("VW_Document_Repos_Partial", DocModel);
        }

        public string Encoder(string value)
        {
            string encodeRes = HttpUtility.UrlEncode(value);
            return encodeRes;
        }
        public string Decoder(string value)
        {
            string decodeRes = HttpUtility.UrlDecode(value);
            return decodeRes;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
