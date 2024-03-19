using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QrMenuApi.Data.Context;
using QrMenuApi.Data.Models;

namespace QrMenuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public ActionResult<List<ApplicationUser>> GetAllAplciationUsers()
        { 
            return _signInManager.UserManager.Users.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<ApplicationUser> GetAplicationUser(string id)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;
            if (applicationUser == null)
            {
                return NotFound();
            }
            return Ok(applicationUser);
        }

        [HttpPost]
        public ActionResult CreateApplicationUser(ApplicationUser applicationUser)
        {
            var result = _signInManager.UserManager.CreateAsync(applicationUser).Result;
            if (result.Succeeded)
            {
                return Ok("Kullanıcı başarıyla oluşturuldu");
            }
            else
            {
                return BadRequest("Kullanıcı oluşturma işlemi başarısız oldu: " + "," + result.Errors);
            }
        }


        [HttpPut("{id}")]
        public ActionResult PutApplicationUser(ApplicationUser applicationUser)
        {
            try
            {
                ApplicationUser changeApplicationUser = _signInManager.UserManager.FindByIdAsync(applicationUser.Id).Result;

                changeApplicationUser.UserName = applicationUser.UserName;
                changeApplicationUser.Name = applicationUser.Name;
                changeApplicationUser.SurName = applicationUser.SurName;
                changeApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
                changeApplicationUser.Email = applicationUser.Email;
                changeApplicationUser.StateId = applicationUser.StateId;

                _signInManager.UserManager.UpdateAsync(changeApplicationUser);
                return Ok();
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteApplicationUser(string id)
        {
          ApplicationUser applicationUser   = _signInManager.UserManager.FindByIdAsync(id).Result;

            if(applicationUser != null)
            {
                applicationUser.StateId = 0;

                _signInManager.UserManager.UpdateAsync(applicationUser);
                return Ok();
            }

            return NotFound();
           

        }


    }
}
