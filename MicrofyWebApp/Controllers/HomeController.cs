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

namespace MicrofyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _cache;

        string Baseurl = "https://microfy-docfunc.azurewebsites.net/";
        string Asseturl = "https://microfy-assetstorfunc.azurewebsites.net/";
        string Phaseurl= "https://microfy-configfunc.azurewebsites.net/";
        string Userurl = "https://microfy-userfunc.azurewebsites.net/";

        public HomeController(ILogger<HomeController> logger,IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;
        }

        [HttpPost]
        public async Task<FileUploadResponse> UploadAsync(IFormFile file)
        {
            using (var client = new HttpClient())
            {
                byte[] data;
                using (var br = new BinaryReader(file.OpenReadStream()))
                {
                    data = br.ReadBytes((int)file.OpenReadStream().Length);
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    MultipartFormDataContent multiContent = new MultipartFormDataContent();
                    multiContent.Add(bytes, "file", file.FileName);
                    client.BaseAddress = new Uri(Asseturl);
                    var response = await client.PostAsync("api/UploadStorageFunction?code=pTrea7/PaHpQ8TH173XmKL4A32ulcr5huhbP0iV0xFaYFCMlYts0FQ==", multiContent).Result.Content.ReadAsStringAsync();
                    FileUploadResponse FileUploadReponseValue = JsonConvert.DeserializeObject<FileUploadResponse>(response);

                    return FileUploadReponseValue;

                }
            }

        }

        [HttpPost]
        public async Task<ActionResult> CreateDocumentAsync(CreateDocuments create)
        {
            DocumentViewModel DocModel = new DocumentViewModel();

            string DocRepos;
            var createDoc = JsonConvert.SerializeObject(create);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                var result = client.PostAsync("api/Document?code=xsoMPmFwEkvOtSYDeqdI6ykfmqt6C/qJbdI8RS4IEawmxeuCG1WKlA==", new StringContent(JsonConvert.SerializeObject(create), Encoding.UTF8, "application/json")).Result;
                if (result.IsSuccessStatusCode)
                {
                    DocRepos = await GetDocumentListAsync();
                    if (DocRepos != null || DocRepos != string.Empty)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(8000));

                        _cache.Set("_GetDocList", DocRepos, cacheEntryOptions);
                        DocModel = JsonConvert.DeserializeObject<DocumentViewModel>(value: DocRepos);
                        DocModel.selectedPhase = create.Phase;
                        DocModel.selectedSubPhases = create.SubPhase;
                    }
                }
            }

            //----------------------Need to Remove-----------------------//
            //var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"document.json"}");
            //var JSON = System.IO.File.ReadAllText(folderDetails);
            //var myJsonObject = JsonConvert.DeserializeObject<DocumentViewModel>(JSON);
            //myJsonObject.selectedPhase = create.phase;
            //-------------------------------------------------------------//

            return PartialView("VW_Document_Repos_Partial", DocModel); //Change myJsonObject to DocModel

        }

   

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> signup(UserViewModel users)
        {

            string UserResponse=string.Empty;
            var createDoc = JsonConvert.SerializeObject(users);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                var result = client.PostAsync("api/User?code=4lXNzClPbyzl9pQDE/cPAcS0yNumPv4Dpxm/Xpv/rPkGMfq5f8LaNw==", new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json")).Result;
                if (result.IsSuccessStatusCode)
                {
                    UserResponse = await result.Content.ReadAsStringAsync();
                }
            }
            return UserResponse;
        }

            [HttpGet]
        public async Task<ActionResult> GetDocumentRepositoryAsync(string Phase , string SubPhase)
        {
            DocumentViewModel DocModel = new DocumentViewModel();
            string DocRepos=string.Empty;
            if (!_cache.TryGetValue("_GetDocList", out DocRepos))
            {
                DocRepos = await GetDocumentListAsync();
            }
            if (DocRepos != null || DocRepos!=string.Empty)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(8000));

                _cache.Set("_GetDocList", DocRepos, cacheEntryOptions);
                DocModel = JsonConvert.DeserializeObject<DocumentViewModel>(value: DocRepos);
                DocModel.selectedPhase = Phase;
                DocModel.selectedSubPhases = SubPhase;
            }

           

            //----------------------Need to Remove-----------------------//
           
            //if (!_cache.TryGetValue("_GetDocListJson", out DocRepos))
            //{
            //    var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"document.json"}");
            //    var JSON = System.IO.File.ReadAllText(folderDetails);
            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //                .SetSlidingExpiration(TimeSpan.FromSeconds(1000));

            //    _cache.Set("_GetDocListJson", JSON, cacheEntryOptions);
            //}
            //var jsondoc = _cache.Get("_GetDocListJson");
            //var myJsonObject = JsonConvert.DeserializeObject<DocumentViewModel>(DocRepos);
            //myJsonObject.selectedPhase = Phase;
            //myJsonObject.selectedSubPhases = SubPhase;
            //-------------------------------------------------------------//

            return PartialView("VW_Document_Repos_Partial", DocModel); //Change myJsonObject to DocModel
        }

        public async Task<string> GetDocumentListAsync()
        {
            string DocRepos=string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Get?code=xsoMPmFwEkvOtSYDeqdI6ykfmqt6C/qJbdI8RS4IEawmxeuCG1WKlA==");
                if (Res.IsSuccessStatusCode)
                {
                    DocRepos = Res.Content.ReadAsStringAsync().Result;
                    
                }
            }
            return DocRepos;
        }

        public async Task<ActionResult> GetUploadPartialAsync(string phase,string subphase,string documentname)
        {
            DocumentViewModel DocModel = new DocumentViewModel();
            string DocRepos = string.Empty;
            if (!_cache.TryGetValue("_GetDocList", out DocRepos))
            {
                DocRepos = await GetDocumentListAsync();
            }
            if (DocRepos != null || DocRepos != string.Empty)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(8000));

                _cache.Set("_GetDocList", DocRepos, cacheEntryOptions);
                DocModel = JsonConvert.DeserializeObject<DocumentViewModel>(value: DocRepos);
                DocModel.selectedPhase = phase;
                DocModel.selectedSubPhases = subphase;
                DocModel.selectedDocName = documentname;
            }
            return PartialView("VW_Upload_Partial", DocModel);

        }
        public ActionResult GetNewUploadPartial()
        {
            return PartialView("VW_Upload_NewDoc_Partial");
        }

        public async Task<IActionResult> MicrofyAsync()
        {

            PhaseViewModel PhaseModel = new PhaseViewModel();
            string Phase;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Phaseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/GetPhaseListFunction?code=J6NFgMxPtrzjRdgRLfrl45WRShXAF9akAcQDSPScBAM7dwa3Q6RUEw==");
                if (Res.IsSuccessStatusCode)
                {
                    Phase = Res.Content.ReadAsStringAsync().Result;
                    PhaseModel = JsonConvert.DeserializeObject<PhaseViewModel>(Phase);
                }
            }
            return View(PhaseModel);
        }
        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Upload()
        {
            return View();
        }
        public IActionResult MyProfile()
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
