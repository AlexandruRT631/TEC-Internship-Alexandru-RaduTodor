using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace WebApp.Controllers
{
    public class SalaryController : Controller
    {

        private readonly IConfiguration _config;
        private readonly string _api;
        public SalaryController(IConfiguration config)
        {
            _config = config;
            _api = _config.GetValue<string>("ApiSettings:ApiUrl") + "salaries/";
        }

        public async Task<IActionResult> Index()
        {
            List<Salary> list = new List<Salary>();
            HttpClient client = new HttpClient();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Login");
            }
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage message = await client.GetAsync(_api);
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<Salary>>(jstring);
                return View(list);
            }
            else
                return View(list);
        }
        public IActionResult Add()
        {
            Salary salary = new Salary();
            return View(salary);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Salary salary)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var token = HttpContext.Session.GetString("Token");
                if (token == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var jsonSalary = JsonConvert.SerializeObject(salary);
                StringContent content = new StringContent(jsonSalary, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync(_api, content);
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "There is an API Error");
                    return View(salary);
                }
            }
            else
                return View(salary);
        }
        public async Task<IActionResult> Update(int id)
        {
            HttpClient client = new HttpClient();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Login");
            }
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage message = await client.GetAsync(_api + id);
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                Salary salary = JsonConvert.DeserializeObject<Salary>(jstring);
                return View(salary);
            }
            else
                return RedirectToAction("Add");
        }
        [HttpPost]
        public async Task<IActionResult> Update(Salary salary)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                var token = HttpContext.Session.GetString("Token");
                if (token == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var jsonSalary = JsonConvert.SerializeObject(salary);
                StringContent content = new StringContent(jsonSalary, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync(_api, content);
                if (message.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                else
                {
                    return View(salary);
                }
            }
            else
                return View(salary);
        }
        public async Task<IActionResult> Delete(int Id)
        {
            HttpClient client = new HttpClient();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Login");
            }
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage message = await client.DeleteAsync(_api + Id);
            if (message.IsSuccessStatusCode)
                return RedirectToAction("Index");
            else
                return View();

        }
    }
}
