using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuApi.Data.Context;
using QrMenuApi.Data.DtoModels;
using QrMenuApi.Data.Models;

namespace QrMenuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly QrMenuApiContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompaniesController(IMapper mapper, QrMenuApiContext context, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Companies
        [HttpGet]
        public ActionResult<List<Company>> GetCompanies()
        {
            if(_context.Companies == null)
            {
                return NotFound();
            }

          List<Company> company= _context.Companies!.ToList();

          return company;
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public ActionResult<Company> GetCompany(int id)
        {
            if(_context.Companies == null)
            {
                return NotFound();
            }
            Company? company = _context.Companies.Find(id);
            if(company == null)
            {
                return NotFound();
            }
            return company;
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        public ActionResult PutCompany(int id, CompanyDto companyDto)
        {
            var existingCompany = _context.Companies!.Find(id);

            if (existingCompany == null)
            {
                return NotFound();
            }

            // AutoMapper kullanarak DTO'yu model nesnesine eşleme
            var company = _mapper.Map(companyDto, existingCompany);

            //eşlenen modeli veritabanı ile güncelleme
            _context.Entry(company).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("Güncelleme başarılı");
            }
            catch
            {
                return NoContent();
            }
            
        }

        // POST: api/Companies
        [HttpPost]
        public ActionResult<Company> PostCompany(CompanyDto companyDto, ApplicationUserDto applicationUserDto, string password)// 3 parametre alamıyoruz
        {
            if(companyDto == null)
            {
                return NotFound(); 
            }

            Claim claim;

            var company = _mapper.Map<Company>(companyDto);

            _context.Companies!.Add(company);
            _context.SaveChanges();

            var applicationUser = _mapper.Map<ApplicationUser>(applicationUserDto);

            _userManager.CreateAsync(applicationUser, password).Wait();
            claim = new Claim("CompanyId", company.Id.ToString());
            _userManager.AddClaimAsync(applicationUser, claim).Wait();
            _userManager.AddToRoleAsync(applicationUser, "CompanyAdministrator").Wait();

            return Ok();
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public ActionResult CompanyStateChange(int id, byte stateId)
        {
            if(stateId !=0 && stateId != 1 && stateId != 2)
            {
                return BadRequest("Yanlış stateId girdiniz!");
            }
            var company = _context.Companies!
                .Include(c => c.Restaurants)!
                    .ThenInclude(r => r.Categories)!
                        .ThenInclude(cat => cat.Foods)
                .FirstOrDefault(c => c.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            // Şirketin StateId'sini değiştirme
            company.StateId = stateId;

            // Bağlı restoranlar, kategoriler ve yiyeceklerin StateId'lerini sıfırla
            foreach (var restaurant in company.Restaurants!)
            {
                restaurant.StateId = stateId;

                foreach (var category in restaurant.Categories!)
                {
                    category.StateId = stateId;

                    foreach (var food in category.Foods!)
                    {
                        food.StateId = stateId;
                    }
                }
            }

            try
            {
                _context.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
