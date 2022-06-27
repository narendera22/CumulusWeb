using MicrofyWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MicrofyWebApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;
        private IMemoryCache _cache;
        private IConfiguration _configuration;
        string Userurl = string.Empty;
        string Usercode = string.Empty;
        string DefaultPassword = string.Empty;
        string Activityurl = string.Empty;
        string ActivityCode = string.Empty;
        string userid = string.Empty;
        string ConfigUrl = string.Empty;
        string ConfigCode = string.Empty;
        string Asseturl = string.Empty;
        string AssetCode = string.Empty;
        string bestpracFolder = string.Empty;

        public SettingsController(ILogger<SettingsController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _logger = logger;
            _cache = memoryCache;
            _configuration = configuration;

            Userurl = _configuration.GetValue<string>("Values:UsersBaseUrl");
            Usercode = _configuration.GetValue<string>("Values:UsersCode");
            DefaultPassword = _configuration.GetValue<string>("Values:DefaultPassword");
            Activityurl = _configuration.GetValue<string>("Values:ActivityBaseUrl");
            ActivityCode = _configuration.GetValue<string>("Values:ActivityCode");
            ConfigUrl = _configuration.GetValue<string>("Values:ConfigBaseUrl");
            ConfigCode = _configuration.GetValue<string>("Values:ConfigCode");
            Asseturl = _configuration.GetValue<string>("Values:AssetStrgeBaseUrl");
            AssetCode = _configuration.GetValue<string>("Values:AssetStrgeCode");
            bestpracFolder = _configuration.GetValue<string>("Values:ServiceFolder");

        }
        public IActionResult Settings(string menulevel1)
        {
            switch (menulevel1)
            {
                case "Users":
                    return RedirectToAction("ListUsers");
                default:
                    return RedirectToAction("Login");
            }
        }
        public async Task<IActionResult> ListUsersAsync()
        {
            //string username = (string)_cache.Get("_UserId");
            userid = HttpContext.Session.GetString("_userId");
            if (userid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            UserViewModel userViewModel = new UserViewModel();
            userViewModel = await ListAllUsersAsync();
            return View(userViewModel);
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
            //var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"Menu.json"}");
            //var JSON = System.IO.File.ReadAllText(folderDetails);
            //userViewModel.Menu = JsonConvert.DeserializeObject<Menus>(JSON);

            MenuViewModel PhaseModel = new MenuViewModel();
            string Phase;
            string menuapi = $"api/GetMenuListFunction?{ConfigCode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync(menuapi);
                if (Res.IsSuccessStatusCode)
                {
                    Phase = Res.Content.ReadAsStringAsync().Result;
                    PhaseModel = JsonConvert.DeserializeObject<MenuViewModel>(Phase);
                    PhaseModel.UserRole = HttpContext.Session.GetString("_UserRole");
                }
            }
            List<Module> listmod = new List<Module>();
            foreach (var menu in PhaseModel.Menu)
            {
                Module mod = new Module();
                mod.Name = menu.Name;
                mod.Visible = menu.Visible;
                listmod.Add(mod);
            }
            userViewModel.moduleAccess = listmod;
            return userViewModel;
        }
    }
}
