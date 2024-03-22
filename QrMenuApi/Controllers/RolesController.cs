using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QrMenuApi.Data.Context;
using QrMenuApi.Data.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QrMenuApi.Controllers
{
    [Authorize(Roles = "Adminstrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;



        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/Roles
        [HttpGet]
        public ActionResult GetRoles()
        { 
            var roles = _roleManager.Roles.ToList();

            return Ok(roles);
        }

        //// GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound(); 
            }

            return Ok(role); 
        }


        // POST: api/Roles
        [HttpPost]
        [Authorize]
        public void PostApplicationRole(string name)
        {

            IdentityRole applicationRole = new IdentityRole(name);
            _roleManager.CreateAsync(applicationRole).Wait();
        }


        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteRol(string id)
        {
           
            var applicationRole = await _roleManager.FindByIdAsync(id);
            if (applicationRole == null)
            {
                return NotFound("Rol bulunamadı.");
            }

            if(applicationRole.Name=="Administrator")
            {
                return BadRequest("Administrator rolü silinemez.");
            }

            if(User.IsInRole(applicationRole.Name))
            {
                return BadRequest("Kendi rolünü silemezseiniz");
            }

            var result = await _roleManager.DeleteAsync(applicationRole);
            if (result.Succeeded)
            {
                return Ok($"{id} Id'li rol silindi!");
            }
            else
            {
                return BadRequest("Rol silme işlemi başarısız.");
            }
        }
    }
}

