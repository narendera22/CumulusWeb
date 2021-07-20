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

namespace MicrofyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _cache;

        string Baseurl = "https://microfy-docfunc.azurewebsites.net/";
        string Asseturl = "https://microfy-assetstorfunc.azurewebsites.net/";
        string Phaseurl = "https://microfy-configfunc.azurewebsites.net/";



        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;
        }
        public async Task<IActionResult> MicrofyAsync()
        {

            PhaseViewModel PhaseModel = new PhaseViewModel();
            DocumentViewModel documentModel = new DocumentViewModel();
            string Phase;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Phaseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/GetPhaseListFunction?code=DSa/RrNoIxbON2obraEyBx3GK8RfoLygKk5GdIPnXMS3cfJReW2xKA==");
                if (Res.IsSuccessStatusCode)
                {
                    Phase = Res.Content.ReadAsStringAsync().Result;
                    PhaseModel = JsonConvert.DeserializeObject<PhaseViewModel>(Phase);
                    documentModel = await GetDocumentListAsync();
                    PhaseModel.documentRepository = documentModel.documentRepository;
                }
            }
            return View(PhaseModel);
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
                    var response = await client.PostAsync("api/Upload?code=cXk0i43E1NkCnFPUC9yqFRLcYrZ0kqLWqCC5f4V4pnh6T5BUrHF1dg==", multiContent).Result.Content.ReadAsStringAsync();
                    FileUploadResponse FileUploadReponseValue = JsonConvert.DeserializeObject<FileUploadResponse>(response);

                    return FileUploadReponseValue;

                }
            }

        }

        [HttpPost]
        public async Task<ActionResult> CreateDocumentAsync(CreateDocuments create)
        {
            DocumentViewModel DocModel = new DocumentViewModel();

            var createDoc = JsonConvert.SerializeObject(create);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                var result = client.PostAsync("api/Document?code=xsoMPmFwEkvOtSYDeqdI6ykfmqt6C/qJbdI8RS4IEawmxeuCG1WKlA==", new StringContent(JsonConvert.SerializeObject(create), Encoding.UTF8, "application/json")).Result;
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

            if (!_cache.TryGetValue("_GetDocList", out DocRepos))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Get?code=xsoMPmFwEkvOtSYDeqdI6ykfmqt6C/qJbdI8RS4IEawmxeuCG1WKlA==");
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
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(Asseturl);
                Task<HttpResponseMessage> response = client.GetAsync("api/Download/" + filename + "?code=cXk0i43E1NkCnFPUC9yqFRLcYrZ0kqLWqCC5f4V4pnh6T5BUrHF1dg==");
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
