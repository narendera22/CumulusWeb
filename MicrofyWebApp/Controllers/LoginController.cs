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
        public async Task<UserViewModel> UpdateUser(UserViewModel users,ActivityTracker activityTracker)
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
                userViewModel.DefaultPassword = DefaultPassword;
            }
            return userViewModel;
        }

        public async Task<IActionResult> ListUserAsync()
        {
            //string username = (string)_cache.Get("_UserId");
            userid = HttpContext.Session.GetString("_userId");
            if (userid == null)
            {
                return RedirectToAction("Login");
            }
            UserViewModel userViewModel = new UserViewModel();
            userViewModel= await ListAllUsersAsync();
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

    }
}
