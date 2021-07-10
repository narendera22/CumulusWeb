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

namespace MicrofyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        string Baseurl = "https://microfy-docfunc.azurewebsites.net/";
        string Asseturl = "https://microfy-assetstorfunc.azurewebsites.net/";
        string Phaseurl= "https://microfy-configfunc.azurewebsites.net/";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> PhaseAsync()
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

        [HttpPost]
        public async Task<string> UploadAsync(IFormFile file)
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
                    return response;

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
                var result = client.PostAsync("api/CreateDocument?code=FwIH7Enq4yweKZVeWNwgtueM7WtnuZAAK9AA5vSqPaZlKMuJTumKtw==", new StringContent(JsonConvert.SerializeObject(create), Encoding.UTF8, "application/json")).Result;
                if (result.IsSuccessStatusCode)
                {
                    using (var Baseclient = new HttpClient())
                    {
                        Baseclient.BaseAddress = new Uri(Baseurl);
                        Baseclient.DefaultRequestHeaders.Clear();
                        Baseclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage Res = await Baseclient.GetAsync("api/GetDocumentListFunction?code=2CUJAefFX9UgUFTyDu3wMAEzZI0PtYBldkCSFlBxasjTaa8BKWCR/A==");
                        if (Res.IsSuccessStatusCode)
                        {
                            DocRepos = Res.Content.ReadAsStringAsync().Result;
                            DocModel = JsonConvert.DeserializeObject<DocumentViewModel>(DocRepos);
                        }
                    }
                }
            }

            //----------------------Need to Remove-----------------------//
            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"document.json"}");
            var JSON = System.IO.File.ReadAllText(folderDetails);
            var myJsonObject = JsonConvert.DeserializeObject<DocumentViewModel>(JSON);
            myJsonObject.selectedPhase = create.phase;
            //-------------------------------------------------------------//

            return PartialView("VW_Document_Repos_Partial", myJsonObject); //Change myJsonObject to DocModel

        }

   

        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> GetResultByAjaxAsync(string Phase)
        {
            DocumentViewModel DocModel = new DocumentViewModel();
            string DocRepos;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/GetDocumentListFunction?code=2CUJAefFX9UgUFTyDu3wMAEzZI0PtYBldkCSFlBxasjTaa8BKWCR/A==");
                if (Res.IsSuccessStatusCode)
                {
                    DocRepos = Res.Content.ReadAsStringAsync().Result;
                    DocModel = JsonConvert.DeserializeObject<DocumentViewModel>(DocRepos);
                }
            }
            DocModel.selectedPhase = Phase;

            //----------------------Need to Remove-----------------------//
            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"document.json"}");
            var JSON = System.IO.File.ReadAllText(folderDetails);
            var myJsonObject = JsonConvert.DeserializeObject<DocumentViewModel>(JSON);
            myJsonObject.selectedPhase = Phase;
            //-------------------------------------------------------------//

            return PartialView("VW_Document_Repos_Partial", myJsonObject); //Change myJsonObject to DocModel
        }

        public ActionResult GetUploadPartial()
        {
            return PartialView("VW_Upload_Partial");

        }

        public IActionResult Microfy()
        {
            return View();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
