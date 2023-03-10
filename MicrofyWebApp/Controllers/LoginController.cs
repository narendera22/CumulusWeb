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
using Microsoft.Extensions.Configuration;

namespace MicrofyWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
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

        public LoginController(ILogger<LoginController> logger, IMemoryCache memoryCache, IConfiguration configuration)
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

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            //_cache.Remove("_UserId");
            _cache.Remove("_GetUseDetailsList");
            _cache.Remove("_GetDocList");
            HttpContext.Session.Remove("_UserRole");
            HttpContext.Session.Remove("_AuthKey");
            HttpContext.Session.Remove("_userId");
            HttpContext.Session.Remove("_username");
            HttpContext.Session.Remove("_UserDet");
            HttpContext.Session.Remove("_config");
            HttpContext.Session.Remove("_selMainMenu");
            HttpContext.Session.Remove("_selPhase");
            HttpContext.Session.Remove("_selSubPhase");

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<UserViewModel> signup(UserViewModel users)
        {

            string UserResponse = string.Empty;
            var createDoc = JsonConvert.SerializeObject(users);
            UserViewModel userViewModel = new UserViewModel();
            string Requestapi = $"api/User?{Usercode}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                var result = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json")).Result;
                if (result.IsSuccessStatusCode)
                {
                    userViewModel.StatusCode = result.IsSuccessStatusCode;
                    userViewModel.responseMessage = await result.Content.ReadAsStringAsync();

                }
                else
                {
                    userViewModel.StatusCode = result.IsSuccessStatusCode;
                    userViewModel.responseMessage = await result.Content.ReadAsStringAsync();

                }
            }
            return userViewModel;
        }

        public IActionResult Registration()
        {
            return View();
        }
        public async Task<IActionResult> MyProfileAsync()
        {
            string authKey = HttpContext.Session.GetString("_AuthKey");
            userid = HttpContext.Session.GetString("_userId");

            if (userid == null)
            {
                return RedirectToAction("Login");
            }
            UserViewModel userViewModel = new UserViewModel();
            string userResponse = string.Empty;

            userResponse = await GetLoginUserDetails(authKey, userid); ;
            userViewModel = JsonConvert.DeserializeObject<UserViewModel>(userResponse);
            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{"Menu.json"}");
            var JSON = System.IO.File.ReadAllText(folderDetails);
            userViewModel.Menu = JsonConvert.DeserializeObject<Menus>(JSON);
            return View(userViewModel);
        }

        public async Task<UserViewModel> UserLoginAsync(LoginDetails loginDetails)
        {

            UserViewModel userViewModel = new UserViewModel();
            string UserResponse = string.Empty;
            try
            {
                var logindet = JsonConvert.SerializeObject(loginDetails);
                string Requestapi = $"api/Authenticate?{Usercode}";
                string LoginUserResponse = string.Empty;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Userurl);
                    HttpResponseMessage Res = client.PostAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(loginDetails), Encoding.UTF8, "application/json")).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        UserResponse = await Res.Content.ReadAsStringAsync();
                        var cacheEntryOptions = new MemoryCacheEntryOptions();

                        if (UserResponse != null || UserResponse != string.Empty)
                        {
                            //_cache.Set("_AuthKey", UserResponse, cacheEntryOptions);
                            //_cache.Set("_UserId", loginDetails.UserId, cacheEntryOptions);
                            HttpContext.Session.SetString("_userId", loginDetails.UserId);

                            HttpContext.Session.SetString("_AuthKey", UserResponse);
                            LoginUserResponse = await GetLoginUserDetails(UserResponse, loginDetails.UserId);
                            _cache.Set("_GetUseDetailsList", LoginUserResponse, cacheEntryOptions);

                        }
                        userViewModel = JsonConvert.DeserializeObject<UserViewModel>(LoginUserResponse);
                        HttpContext.Session.SetString("_username", userViewModel.fullName);
                        HttpContext.Session.SetString("_UserRole", userViewModel.userRole);
                        HttpContext.Session.SetString("_UserDet", LoginUserResponse);

                        HttpContext.Session.SetString("_selMainMenu", string.Empty);
                        HttpContext.Session.SetString("_selPhase", string.Empty);
                        HttpContext.Session.SetString("_selSubPhase", string.Empty);

                        //_cache.Set("_UserRole", userViewModel.userRole, cacheEntryOptions);
                        userViewModel.StatusCode = Res.IsSuccessStatusCode;
                        //bool Activity = ActivityTracker("LogIn", $"User {loginDetails.UserId} logged in");
                    }
                    else
                    {
                        userViewModel.StatusCode = Res.IsSuccessStatusCode;
                        userViewModel.responseMessage = await Res.Content.ReadAsStringAsync();
                    }
                }
                MenuViewModel PhaseModel = new MenuViewModel();
                DocumentViewModel documentModel = new DocumentViewModel();
                string Phase;
                string phaseRequestapi = $"api/GetMenuListFunction?{ConfigCode}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync(phaseRequestapi);
                    if (Res.IsSuccessStatusCode)
                    {
                        Phase = Res.Content.ReadAsStringAsync().Result;
                        PhaseModel = JsonConvert.DeserializeObject<MenuViewModel>(Phase);
                        //PhaseModel.documentRepository = documentModel.documentRepository;
                        PhaseModel.UserRole = HttpContext.Session.GetString("_UserRole");
                        HttpContext.Session.SetString("_config", Phase);
                    }
                }
            }
            catch (Exception e)
            {

            }
            return userViewModel;
        }

        public async Task<string> GetLoginUserDetails(string JWTResponse, string Userid)
        {
            string LoginUserDetails = string.Empty;

            string Requestapi = $"api/GetUserDetails/{Userid}?{Usercode}";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", JWTResponse);
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    LoginUserDetails = Res.Content.ReadAsStringAsync().Result;
                }

            }
            return LoginUserDetails;
        }

        [HttpPost]
        public async Task<UserViewModel> UpdateUser(UserViewModel users, ActivityTracker activityTracker)
        {
            string authKey = HttpContext.Session.GetString("_AuthKey");

            string UserResponse = string.Empty;
            var createDoc = JsonConvert.SerializeObject(users);
            UserViewModel userViewModel = new UserViewModel();
            string Requestapi = $"api/UpdateUser/{users.username}?{Usercode}";
            if (users.password == string.Empty || users.password == null)
            {
                users.password = DefaultPassword;
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                client.DefaultRequestHeaders.Add("Authorization", authKey);
                var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json")).Result;

                if (result.IsSuccessStatusCode)
                {
                    userViewModel.StatusCode = result.IsSuccessStatusCode;
                    userViewModel.responseMessage = await result.Content.ReadAsStringAsync();
                    if (activityTracker.ActivityType != "ChangePassword")
                        activityTracker.ActivityDetails = $"User {userid} {activityTracker.ActivityDetails}";
                    //bool Activity = ActivityTracker(activityTracker.ActivityType,activityTracker.ActivityDetails );

                }
                else
                {
                    userViewModel.StatusCode = result.IsSuccessStatusCode;
                    userViewModel.responseMessage = await result.Content.ReadAsStringAsync();

                }
                userid = HttpContext.Session.GetString("_userId");
                if (userid == users.username)
                {
                    string LoginUserResponse = await GetLoginUserDetails(authKey, userid);
                    HttpContext.Session.SetString("_UserDet", LoginUserResponse);
                }
                userViewModel.DefaultPassword = DefaultPassword;
            }
            return userViewModel;
        }

        public async Task<bool> ActivateDeactivate(string username)
        {
            string authKey = HttpContext.Session.GetString("_AuthKey");

            string userResponse = string.Empty;
            string Requestapi = $"api/Delete/{username}?{Usercode}";
            //string JWTResponse = (string)_cache.Get("_UserLoginResponse");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", authKey);

                HttpResponseMessage Res = await client.DeleteAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    userResponse = Res.Content.ReadAsStringAsync().Result;
                    //bool Activity = ActivityTracker("DeactivateUser", $"User {userid} has successfully deactivated {username}");

                }

            }
            return true;
        }

        public async Task<bool> DeleteUser(string username)
        {
            string authKey = HttpContext.Session.GetString("_AuthKey");

            bool userResponse = false;
            string Requestapi = $"api/DeleteUser/{username}?{Usercode}";
            //string JWTResponse = (string)_cache.Get("_UserLoginResponse");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", authKey);

                HttpResponseMessage Res = await client.DeleteAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    userResponse = Res.IsSuccessStatusCode;
                    //bool Activity = ActivityTracker("DeactivateUser", $"User {userid} has successfully deactivated {username}");

                }

            }
            return userResponse;
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




        public string GetMasterTemplate(string mastertemplate, string foldername = null)
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

    }
}
