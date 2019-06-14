using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MVCClient.Models;
using MVCClient.Services;
using Newtonsoft.Json.Linq;

namespace MVCClient.Controllers
{
    public class HomeController : Controller
    {
        private IApi1Service _api1Svc;

        public HomeController(IApi1Service api1Svc)
        {
            _api1Svc = api1Svc;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> Api1Data()
        //{
            
        //    ViewBag.Json = await _api1Svc.GetData();

        //    return View("json");
        //}

        [Authorize]
        public async Task<IActionResult> Api1UserData()
        {
            
            ViewBag.Json = await _api1Svc.GetUserData();

            return View("json");
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Api1AdminData()
        {
            
            ViewBag.Json = await _api1Svc.GetAdminData();

            return View("json");
        }

        //public async Task<IActionResult> CreateApi1Data(string api1data)
        //{
        //    ViewBag.Json = await _api1Svc.CreateData(api1data);

        //    return View("json");
        //}

        public async Task<IActionResult> Api2Data()
        {
            
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("http://localhost:5000/api2/data");

            ViewBag.Json = (content).ToString();
            return View("json");
        }

        [Authorize]
        public async Task<IActionResult> Api2UserData()
        {
            
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("http://localhost:5000/api2/userdata");

            ViewBag.Json = (content).ToString();
            return View("json");
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Api2AdminData()
        {
           
            var accessToken = await HttpContext.GetTokenAsync("access_token");
           
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("http://localhost:5000/api2/admindata");

            ViewBag.Json = (content).ToString();
            return View("json");
        }

        public async Task<IActionResult> Api1and2Data()
        {
            
            HttpContext.GetType();

            ViewBag.Json = await _api1Svc.GetApi1and2JoinedData();
            return View("json");
        }

        public async Task<IActionResult> DelayedDataRequests()
        {
            for (int i = 0; i < 10; i++)
            {
                var client = new HttpClient();
                var content = await client.GetStringAsync("http://localhost:5000/api1/delayeddata");
            }
            

            return View("json");
        }
    }
}
