using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{

    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _api;
        public LoginController(IConfiguration config)
        {
            _config = config;
            _api = _config.GetValue<string>("ApiSettings:ApiUrl") + "login/";
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Admin admin)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var jsonLogin = JsonConvert.SerializeObject(admin);
                StringContent content = new StringContent(jsonLogin, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync(_api + "login", content);
                if (message.IsSuccessStatusCode)
                {
                    var token = await message.Content.ReadAsStringAsync();
                    HttpContext.Session.SetString("Token", token);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "There is an Login Error");
                    return View();
                }
            }
            else
                return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Admin admin)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var jsonLogin = JsonConvert.SerializeObject(admin);
                StringContent content = new StringContent(jsonLogin, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync(_api + "register", content);
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "There is an Register Error");
                    return View();
                }
            }
            else
                return View();
        }
        public async Task<IActionResult> Disconnect()
        {
            HttpClient client = new HttpClient();
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Login");
            
        }
        public async Task<IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login");
            }
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            HttpResponseMessage message = await client.GetAsync(_api);
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                List<Admin> list = JsonConvert.DeserializeObject<List<Admin>>(jstring);
                return View(list);
            }
            else
                return View(new List<Admin>());
        }
        public async Task<IActionResult> Update(int Id)
        {
            HttpClient client = new HttpClient();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login");
            }
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            HttpResponseMessage message = await client.GetAsync(_api + Id);
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                Admin admin = JsonConvert.DeserializeObject<Admin>(jstring);
                return View(admin);
            }
            else
                return RedirectToAction("Register");
        }
        [HttpPost]
        public async Task<IActionResult> Update(Admin admin)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var token = HttpContext.Session.GetString("Token");
                if (token == null)
                {
                    return RedirectToAction("Login");
                }
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var jsonLogin = JsonConvert.SerializeObject(admin);
                StringContent content = new StringContent(jsonLogin, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync(_api, content);
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "There is an Update Error");
                    return View(admin);
                }
            }
            else
                return View(admin);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            HttpClient client = new HttpClient();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login");
            }
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            HttpResponseMessage message = await client.DeleteAsync(_api + Id);
            if (message.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");
        }
    }
}
