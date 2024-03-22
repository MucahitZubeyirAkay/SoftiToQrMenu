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

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
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
        public void PostApplicationRole(string name)
        {

            IdentityRole applicationRole = new IdentityRole(name);
            _roleManager.CreateAsync(applicationRole).Wait();
        }
    }
}

