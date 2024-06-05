using Microsoft.AspNetCore.Http;
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
    public class PersonDetailController : Controller
    {

        private readonly IConfiguration _config;
        private readonly string _api;
        public PersonDetailController(IConfiguration config)
        {
            _config = config;
            _api = _config.GetValue<string>("ApiSettings:ApiUrl") + "persondetails/";
        }

        public async Task<IActionResult> Index()
        {
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
                List<PersonDetailInformation> list = JsonConvert.DeserializeObject<List<PersonDetailInformation>>(jstring);
                return View(list);
            }
            else
                return View(new List<PersonDetailInformation>());
        }
        public IActionResult Add()
        {
            PersonDetail personDetail = new PersonDetail();
            return View(personDetail);
        }
        [HttpPost]
        public async Task<IActionResult> Add(PersonDetail personDetail)
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
                var jsonPersonDetail = JsonConvert.SerializeObject(personDetail);
                StringContent content = new StringContent(jsonPersonDetail, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync(_api, content);
                if (message.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "There is an API Error");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(personDetail);
                }
            }
            else
            {
                return View(personDetail);
            }
        }
        public async Task<IActionResult> Update(int Id)
        {
            HttpClient client = new HttpClient();
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Login");
            }
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage message = await client.GetAsync(_api + Id);
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                PersonDetail personDetail = JsonConvert.DeserializeObject<PersonDetail>(jstring);
                return View(personDetail);
            }
            else
                return RedirectToAction("Add");
        }
        [HttpPost]
        public async Task<IActionResult> Update(PersonDetail personDetail) 
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
                var jsonPersonDetail = JsonConvert.SerializeObject(personDetail);
                StringContent content = new StringContent(jsonPersonDetail, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync(_api, content);
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(personDetail);
                }
            }
            else
                return View(personDetail);
        }
        [HttpPost]
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
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "There is an API Error");
                return RedirectToAction("Index");
            }
        }
    }
}
