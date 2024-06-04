using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class SalaryController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<Salary> list = new List<Salary>();
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync("http://localhost:5229/api/Salaries");
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<Salary>>(jstring);
                return View(list);
            }
            else
                return View(list);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.DeleteAsync("http://localhost:5229/api/Salaries/" + Id);
            if (message.IsSuccessStatusCode)
                return RedirectToAction("Index");
            else
                return View();

        }
    }
}
