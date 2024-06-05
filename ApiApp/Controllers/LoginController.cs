using ApiApp.ObjectModel;
using Internship.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var db = new APIDbContext();
            if (ModelState.IsValid)
            {
                Admin admin = db.Admins.FirstOrDefault(x => x.UserName == loginModel.UserName);
                if (admin != null && BCrypt.Net.BCrypt.Verify(loginModel.Password, admin.Password) == true)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return Ok(tokenHandler.WriteToken(token));
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
                return BadRequest();
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginModel loginModel)
        {
            var db = new APIDbContext();
            if (ModelState.IsValid)
            {
                Admin admin = new Admin();
                Admin existingAdmin = db.Admins.FirstOrDefault(x => x.UserName == loginModel.UserName);
                if (existingAdmin != null)
                    return Conflict();
                admin.UserName = loginModel.UserName;
                admin.Password = BCrypt.Net.BCrypt.HashPassword(loginModel.Password);
                db.Admins.Add(admin);
                db.SaveChanges();
                return Created();
            }
            else
                return BadRequest();
        }
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            var db = new APIDbContext();
            var list = db.Admins.ToList();
            return Ok(list);
        }
        [Authorize]
        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            var db = new APIDbContext();
            Admin admin = db.Admins.FirstOrDefault(x => x.Id == Id);
            if (admin == null)
                return NotFound();
            else
                return Ok(admin);
        }
        [Authorize]
        [HttpPut]
        public IActionResult UpdateAdmin(Admin admin)
        {
            if (ModelState.IsValid)
            {
                var db = new APIDbContext();
                Admin updateAdmin = db.Admins.Find(admin.Id);
                updateAdmin.UserName = admin.UserName;
                updateAdmin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);
                db.SaveChanges();
                return Ok(updateAdmin);
            }
            else
                return BadRequest();
        }
        [Authorize]
        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            var db = new APIDbContext();
            Admin admin = db.Admins.Find(Id);
            if (admin == null)
                return NotFound();
            else
            {
                db.Admins.Remove(admin);
                db.SaveChanges();
                return NoContent();
            }
        }
    }
}
