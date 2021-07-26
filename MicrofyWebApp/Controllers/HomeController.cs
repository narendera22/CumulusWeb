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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace MicrofyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _cache;

        string Docurl = string.Empty;
        string Asseturl = string.Empty;
        string Phaseurl = string.Empty;
        string DocCode = string.Empty;
        string AssetCode = string.Empty;
        string PhaseCode = string.Empty;

        private IConfiguration _configuration;


        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _logger = logger;
            _cache = memoryCache;
            _configuration = configuration;
            Docurl = _configuration.GetValue<string>("Values:DocumentBaseUrl");
            DocCode = _configuration.GetValue<string>("Values:DocumentCode");
            Asseturl = _configuration.GetValue<string>("Values:AssetStrgeBaseUrl");
            AssetCode = _configuration.GetValue<string>("Values:AssetStrgeCode");
            Phaseurl = _configuration.GetValue<string>("Values:ConfigBaseUrl");
            PhaseCode = _configuration.GetValue<string>("Values:ConfigCode");
        }
        public async Task<IActionResult> MicrofyAsync()
        {
            string username = (string)_cache.Get("_UserId");

            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }

            PhaseViewModel PhaseModel = new PhaseViewModel();
            DocumentViewModel documentModel = new DocumentViewModel();
            string Phase;
            string Requestapi = $"api/GetPhaseListFunction?{PhaseCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Phaseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    Phase = Res.Content.ReadAsStringAsync().Result;
                    PhaseModel = JsonConvert.DeserializeObject<PhaseViewModel>(Phase);
                    documentModel = await GetDocumentListAsync();
                    PhaseModel.documentRepository = documentModel.documentRepository;
                    PhaseModel.UserRole = (string)_cache.Get("_UserRole");

                }
            }
            return View(PhaseModel);
        }
        [HttpPost]
        public async Task<FileUploadResponse> UploadAsync(string phase, string subphase, IFormFile file)
        {
            using (var client = new HttpClient())
            {
                byte[] data;
                string Phase = "Phase=" + phase;
                string SubPhase = "SubPhase=" + subphase;
                string Requestapi = $"api/Upload?{AssetCode}&{Phase}&{SubPhase}";
                FileUploadResponse FileUploadReponseValue = new FileUploadResponse();
                using (var br = new BinaryReader(file.OpenReadStream()))
                {
                    data = br.ReadBytes((int)file.OpenReadStream().Length);
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    MultipartFormDataContent multiContent = new MultipartFormDataContent();
                    multiContent.Add(bytes, "file", file.FileName);
                    client.BaseAddress = new Uri(Asseturl);
                    var response = client.PostAsync(Requestapi, multiContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        FileUploadReponseValue.statuscode = response.IsSuccessStatusCode;
                        FileUploadReponseValue.url = await response.Content.ReadAsStringAsync();
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
                    DocModel.UserRole = (string)_cache.Get("_UserRole");
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
            DocModel.UserRole = (string)_cache.Get("_UserRole");

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
            DocModel = JsonConvert.DeserializeObject<DocumentViewModel>(value: DocRepos);
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
        public ActionResult GetNewUploadPartial()
        {
            return PartialView("VW_Upload_NewDoc_Partial");
        }

        public FileResult DownloadDocument(string url)
        {
            string filename = Path.GetFileName(url);
            string Requestapi = $"api/Download/{filename}?{AssetCode}";

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(Asseturl);
                Task<HttpResponseMessage> response = client.GetAsync(Requestapi);
                HttpResponseMessage file = new HttpResponseMessage();
                file = response.Result;

                return File(file.Content.ReadAsByteArrayAsync().Result, "application/octet-stream", filename);
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
