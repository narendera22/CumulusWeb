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

        public LoginController(ILogger<LoginController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _logger = logger;
            _cache = memoryCache;
            _configuration = configuration;
            Userurl = _configuration.GetValue<string>("Values:UsersBaseUrl");
            Usercode = _configuration.GetValue<string>("Values:UsersCode");
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            _cache.Remove("_UserId");
            _cache.Remove("_GetUseDetailsList");
            _cache.Remove("_GetDocList");
            _cache.Remove("_UserRole");
            _cache.Remove("_UserLoginResponse");
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
            string Userid = (string)_cache.Get("_UserId");

            if (Userid == null)
            {
                return RedirectToAction("Login");
            }
            UserViewModel userViewModel = new UserViewModel();
            string userResponse = string.Empty;
            string JWTResponse= (string)_cache.Get("_UserLoginResponse");
            userResponse = await GetLoginUserDetails(JWTResponse, Userid); ;
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
                            _cache.Set("_UserLoginResponse", UserResponse, cacheEntryOptions);
                            _cache.Set("_UserId", loginDetails.UserId, cacheEntryOptions);
                            LoginUserResponse = await GetLoginUserDetails(UserResponse, loginDetails.UserId);
                            _cache.Set("_GetUseDetailsList", LoginUserResponse, cacheEntryOptions);

                        }
                        userViewModel = JsonConvert.DeserializeObject<UserViewModel>(LoginUserResponse);
                        _cache.Set("_UserRole", userViewModel.userRole, cacheEntryOptions);
                        userViewModel.StatusCode = Res.IsSuccessStatusCode;
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
        public async Task<UserViewModel> UpdateUser(UserViewModel users)
        {
            string UserResponse = string.Empty;
            var createDoc = JsonConvert.SerializeObject(users);
            UserViewModel userViewModel = new UserViewModel();
            string Requestapi = $"api/UpdateUser/{users.username}?{Usercode}";
            string JWTResponse = (string)_cache.Get("_UserLoginResponse");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                client.DefaultRequestHeaders.Add("Authorization", JWTResponse);
                var result = client.PutAsync(Requestapi, new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json")).Result;

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

        public async Task<IActionResult> ListUserAsync()
        {
            string username = (string)_cache.Get("_UserId");

            if (username == null)
            {
                return RedirectToAction("Login");
            }
            UserViewModel userViewModel = new UserViewModel();
            string userResponse = string.Empty;
            string JWTResponse = (string)_cache.Get("_UserLoginResponse");
            string Requestapi = $"api/GetUserList?{Usercode}";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", JWTResponse);
                HttpResponseMessage Res = await client.GetAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    userResponse = Res.Content.ReadAsStringAsync().Result;
                    userViewModel.usersDetails = JsonConvert.DeserializeObject<List<ListUserDetails>>(value: userResponse);

                }

            }
            return View(userViewModel);
        }
        public async Task<bool> ActivateDeactivate(string username)
        {
            string userResponse = string.Empty;
            string Requestapi = $"api/Delete/{username}?{Usercode}";
            string JWTResponse = (string)_cache.Get("_UserLoginResponse");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", JWTResponse);

                HttpResponseMessage Res = await client.DeleteAsync(Requestapi);
                if (Res.IsSuccessStatusCode)
                {
                    userResponse = Res.Content.ReadAsStringAsync().Result;

                }

            }
            return true;
        }
    }
}
