using MicrofyWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MicrofyWebApp.Controllers
{
    public class ActivityController : Controller
    {
        private readonly ILogger<ActivityController> _logger;
        private IMemoryCache _cache;

        
        string Activityurl = string.Empty;
        string ActivityCode = string.Empty;
        string userid = string.Empty;
        private IConfiguration _configuration;
        string Userurl = string.Empty;
        string Usercode = string.Empty;


        public ActivityController(ILogger<ActivityController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _logger = logger;
            _cache = memoryCache;
            _configuration = configuration;
            
            Activityurl = _configuration.GetValue<string>("Values:ActivityBaseUrl");
            ActivityCode = _configuration.GetValue<string>("Values:ActivityCode");
            Userurl = _configuration.GetValue<string>("Values:UsersBaseUrl");
            Usercode = _configuration.GetValue<string>("Values:UsersCode");

        }
        public async Task<IActionResult> ActivityAsync(string userid = null)
        {
            userid = (userid != null ? userid : HttpContext.Session.GetString("_userId"));
            if (userid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ActivityViewModel docmod = new ActivityViewModel();
            var activitylist = string.Empty;
            string Requestapi = $"api/GetActivity?{ActivityCode}";
            var resp = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Activityurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    activitylist = Res.Content.ReadAsStringAsync().Result;
                }
                docmod.activity = JsonConvert.DeserializeObject<List<ActivityTracker>>(value: activitylist);
            }
            docmod.currentUser = HttpContext.Session.GetString("_userId");

            docmod.users = await ListAllUsersAsync();
            docmod.topModel = await ListTopModel();
            return View(docmod);
        }
        public async Task<TopModel> ListTopModel()
        {
            TopModel topModel = new TopModel();
            string Requestapi = $"api/GetTopActivity?{ActivityCode}";
            string resp = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Activityurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    resp = Res.Content.ReadAsStringAsync().Result;
                }
                topModel = JsonConvert.DeserializeObject<TopModel>(value: resp);
            }
            return topModel;
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
    }
}
