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
    public class LoginController : Controller
    {
        string Userurl = "https://microfy-userfunc.azurewebsites.net/";
        private readonly ILogger<LoginController> _logger;
        private IMemoryCache _cache;


        public LoginController(ILogger<LoginController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;
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
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<string> signup(UserViewModel users)
        {

            string UserResponse = string.Empty;
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

        public IActionResult Registration()
        {
            return View();
        }
        public async Task<IActionResult> MyProfileAsync()
        {
            UserViewModel userViewModel = new UserViewModel();
            string userResponse = string.Empty;
            string Userid = (string)_cache.Get("_UserId");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/GetUserDetails/" + Userid + "?code=4lXNzClPbyzl9pQDE/cPAcS0yNumPv4Dpxm/Xpv/rPkGMfq5f8LaNw==");
                if (Res.IsSuccessStatusCode)
                {
                    userResponse = Res.Content.ReadAsStringAsync().Result;
                    userViewModel = JsonConvert.DeserializeObject<UserViewModel>(value: userResponse);

                }

            }
            return View(userViewModel);
        }
        public async Task<UserViewModel> UserLoginAsync(LoginDetails loginDetails)
        {

            UserViewModel userViewModel = new UserViewModel();
            string UserResponse = string.Empty;
            try
            {
                var logindet = JsonConvert.SerializeObject(loginDetails);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Userurl);
                    HttpResponseMessage Res = client.PostAsync("api/AuthenticateUser?code=4lXNzClPbyzl9pQDE/cPAcS0yNumPv4Dpxm/Xpv/rPkGMfq5f8LaNw==", new StringContent(JsonConvert.SerializeObject(loginDetails), Encoding.UTF8, "application/json")).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        UserResponse = await Res.Content.ReadAsStringAsync();
                        var cacheEntryOptions = new MemoryCacheEntryOptions();

                        if (UserResponse != null || UserResponse != string.Empty)
                        {

                            _cache.Set("_UserId", loginDetails.UserId, cacheEntryOptions);
                            _cache.Set("_GetUseDetailsList", UserResponse, cacheEntryOptions);

                        }
                        userViewModel = JsonConvert.DeserializeObject<UserViewModel>(UserResponse);
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


        [HttpPost]
        public async Task<UserViewModel> UpdateUser(UserViewModel users)
        {
            string UserResponse = string.Empty;
            var createDoc = JsonConvert.SerializeObject(users);
            UserViewModel userViewModel = new UserViewModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Userurl);
                var result = client.PutAsync("api/UpdateUser/" + users.username + "?code=4lXNzClPbyzl9pQDE/cPAcS0yNumPv4Dpxm/Xpv/rPkGMfq5f8LaNw==", new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json")).Result;

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


    }
}
