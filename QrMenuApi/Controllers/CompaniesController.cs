using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuApi.Data.Context;
using QrMenuApi.Data.DtoModels;
using QrMenuApi.Data.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        [Authorize(Roles ="Administrator")]
        // GET: api/Companies
        [HttpGet]
        public ActionResult<List<Company>> GetCompanies()
        {
            List<Company> company = _context.Companies!.ToList();

            if (company == null)
              {
                    return NotFound();
              }

            return company;
        }

        [Authorize(Roles ="CompaynAdministrator, Administrator")]
        // GET: api/Companies/5
        [HttpGet("{id}")]
        public ActionResult<Company> GetCompany(int id)
        {
            Company? company = _context.Companies!.Find(id);

            if (company == null)
            {
                return NotFound();
            }

            if (User.IsInRole("CompanyAdministrator"))
            {
                if (User.HasClaim("CompanyId", company.Id.ToString()) == false)
                {
                    return Unauthorized("Sadece kendi şirketiniz verilerini getirebilirsiniz!");
                }
               
            }
                return company;
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,CompanyAdministrator")]
        public ActionResult PutCompany(int id, CompanyDto companyDto)
        {
            if (User.IsInRole("CompanyAdministrator")) //SorKısayolunu
            {
                if (User.HasClaim("CompanyId", id.ToString()) == false)
                {
                   return Unauthorized();
                }
            }


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
        [Authorize(Roles ="Administrator")]
        public async Task<ActionResult<Company>> PostCompany([FromBody] CompanyUserDto companyUserDto)//, string password)
        {
            if (companyUserDto == null)
            {
                return NotFound();
            }

            // Şirketi oluştur
            var company = _mapper.Map<Company>(companyUserDto.CompanyDto);
            _context.Companies!.Add(company);
            await _context.SaveChangesAsync();

            // Kullanıcıyı oluştur ve şirketin ID'sini ata
            var applicationUser = _mapper.Map<ApplicationUser>(companyUserDto.ApplicationUserDto);
            applicationUser.CompanyId = company.Id; // Şirketin ID'sini kullanıcıya sen atama. Kendi otomatik oluşturulan Companysini alsın.
            var result = await _userManager.CreateAsync(applicationUser, companyUserDto.Password);

            if (!result.Succeeded)
            {
                // Kullanıcı oluşturma başarısız olursa uygun bir hata döndür
                return BadRequest("Kullanıcı oluşturma başarısız: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Kullanıcıya rolleri ve yetkileri ata
            //var role = _context.Roles.FirstOrDefault(r => r.Id == "775f609c - 6bd3 - 47cf - 9d0b - 846c3d18d6df");
            await _userManager.AddToRoleAsync(applicationUser, "CompanyAdministrator");
            await _userManager.AddClaimAsync(applicationUser, new Claim("CompanyId", company.Id.ToString()));

            return Ok();
        }


        // DELETE: api/Companies/5
        [HttpPut("{id}")]
        [Authorize(Roles ="Administrator")]
        public ActionResult CompanyStateChange(int id, byte stateId)
        {

           
            if(stateId !=0 && stateId != 1 && stateId != 2)
            {
                return BadRequest("Yanlış stateId girdiniz!");
            }


            if(_context.Companies!.Any(c=> c.Id!=id || c.StateId==0||c.ParentCompanyId==null))
            {
                return BadRequest("Daha önce silinen veya hiç var olmayan bir verinin ya da kendi şirketinizin durumunu değiştiremezsiniz.");
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
