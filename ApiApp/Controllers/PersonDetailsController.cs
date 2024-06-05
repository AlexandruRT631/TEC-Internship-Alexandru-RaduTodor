using ApiApp.ObjectModel;
using Internship.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Internship.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonDetailsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var db = new APIDbContext();
            var list = db.PersonDetails.Include(x => x.Person)
                .Select(x => new PersonDetailInformation()
                {
                    Id = x.Id,
                    BirthDay = x.BirthDay,
                    PersonCity = x.PersonCity,
                    PersonId = x.PersonId,
                    PersonName = x.Person.Name
                }).ToList();
            return Ok(list);
        }
        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            var db = new APIDbContext();
            PersonDetail personDetail = db.PersonDetails.FirstOrDefault(x => x.Id == Id);
            if (personDetail == null)
                return NotFound();
            else
                return Ok(personDetail);
        }
        [HttpPost]
        public IActionResult Post(PersonDetail personDetail)
        {
            if (ModelState.IsValid)
            {
                var db = new APIDbContext();
                db.PersonDetails.Add(personDetail);
                db.SaveChanges();
                return Created("", personDetail);
            }
            else
                return BadRequest();
        }
        [HttpPut]
        public IActionResult UpdatePersonDetail(PersonDetail personDetail)
        {
            if (ModelState.IsValid)
            {
                var db = new APIDbContext();
                PersonDetail updatePersonDetail = db.PersonDetails.Find(personDetail.Id);
                updatePersonDetail.BirthDay = personDetail.BirthDay;
                updatePersonDetail.PersonCity = personDetail.PersonCity;
                updatePersonDetail.PersonId = personDetail.PersonId;
                db.SaveChanges();
                return NoContent();
            }
            else
                return BadRequest();
        }
        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            var db = new APIDbContext();
            PersonDetail personDetail = db.PersonDetails.Find(Id);
            if (personDetail == null)
                return NotFound();
            else
            {
                db.PersonDetails.Remove(personDetail);
                db.SaveChanges();
                return NoContent();
            }
        }
    }
}
