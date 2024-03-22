using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QrMenuApi.Data.Context;
using QrMenuApi.Data.Models;
using System.Security.Claims;

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
        [Authorize(Roles ="Administrator,CompanyAdministrator")]
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
        public string PostApplicationUser(ApplicationUser applicationUser, string passWord)
        {
            var result = _signInManager.UserManager.CreateAsync(applicationUser, passWord).Result;
            if (result.Succeeded)
            {
                Claim claim;
                
                claim = new Claim("CompanyId", applicationUser.CompanyId.ToString());
                _signInManager.UserManager.AddClaimAsync(applicationUser, claim).Wait();
                
                return applicationUser.Id;
            }
            else
            {
                return ("Kullanıcı oluşturma işlemi başarısız oldu: " + "," + result.Errors);
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
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;
            if (applicationUser != null)
            {
                applicationUser.StateId = 0;

                _signInManager.UserManager.UpdateAsync(applicationUser);
                return Ok();
            }

            return NotFound();


        }

        [HttpPost("LogIn")]
        public bool LogIn(string userName, string passWord)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                return false;
            }
            signInResult = _signInManager.PasswordSignInAsync(applicationUser, passWord, false, false).Result;

            
            return signInResult.Succeeded;
        }

        [HttpPost("ReSetPassWord")]
        public void ReSetPassWord(string userName, string passWord)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                return;
            }
            _signInManager.UserManager.RemovePasswordAsync(applicationUser).Wait();
            _signInManager.UserManager.AddPasswordAsync(applicationUser, passWord);
        }

        [HttpPost("PassWordReSet")]
        public string? PassWordReSet(string userName)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                return null;
            }
            return _signInManager.UserManager.GeneratePasswordResetTokenAsync(applicationUser).Result;
        }

        [HttpPost("ValidateToken")]
        public ActionResult<string?> ValidateToken(string userName, string token, string newPassWord)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                return NotFound();
            }
            IdentityResult identityResult = _signInManager.UserManager.ResetPasswordAsync(applicationUser, token, newPassWord).Result;
            if (identityResult.Succeeded == false)
            {
                return identityResult.Errors.First().Description;
            }
            return Ok();
        }

        [HttpPost("AssignRole")]
        public void AssignRole(string userId, string roleId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            IdentityRole identityRole = _roleManager.FindByIdAsync(roleId).Result;

            _signInManager.UserManager.AddToRoleAsync(applicationUser, identityRole.Name).Wait();
        }


    }
}
