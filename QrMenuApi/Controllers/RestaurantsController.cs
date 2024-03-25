using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class RestaurantsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly QrMenuApiContext _context;

        public RestaurantsController(IMapper mapper, QrMenuApiContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Restaurants
        [HttpGet]
        [Authorize(Roles = "CompanyAdministrator,Administrator")]
        public ActionResult<List<Restaurant>> GetRestaurants()
        {

            if (User.IsInRole("Administrator"))
            {
                List<Restaurant> allrestaurant = _context.Restaurants!.ToList();

                if (allrestaurant == null)
                {
                    return NotFound();
                }

                return allrestaurant;
            }

            int companyId = int.Parse(User.Claims.First(c => c.Type == "CompanyId").Value);

            List<Restaurant> restaurant = _context.Restaurants!.Where(r => r.CompanyId == companyId).ToList();

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        [Authorize(Roles = "CompanyAdministrator,Administrator")]
        public ActionResult<Restaurant> GetRestaurant(int id)
        {
            Restaurant? restaurant = _context.Restaurants!.Find(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            if (User.IsInRole("CompanyAdministrator"))
            {
                if (User.HasClaim("CompanyId", restaurant.CompanyId.ToString()) == false)
                {
                    return Unauthorized("Sadece kendi şirketinizin verilerini getirebilirsiniz!");
                }

            }
            return restaurant;
        }

        // PUT: api/Restaurants/5
        [HttpPut("{id}")]
        [Authorize(Roles = "CompanyAdministrator,RestaurantAdministrator")]
        public ActionResult PutRestaurant(int id, RestaurantDto restaurantDto)
        {
            if (User.IsInRole("CompanyAdministrator")) //SorKısayolunu
            {
                if (User.HasClaim("CompanyId", restaurantDto.CompanyId.ToString()) == false)
                {
                    return Unauthorized();
                }
            }
            else
            {
                if (User.HasClaim("RestaurantId", id.ToString()) == false)
                {
                    return Unauthorized();
                }
            }

            var existingRestaurant = _context.Restaurants!.Find(id);

            if (existingRestaurant == null)
            {
                return NotFound();
            }

            // AutoMapper kullanarak DTO'yu model nesnesine eşleme
            var restaurant = _mapper.Map(restaurantDto, existingRestaurant);

            //eşlenen modeli veritabanı ile güncelleme
            _context.Entry(restaurant).State = EntityState.Modified;

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

        // POST: api/Restaurants
        [HttpPost]
        [Authorize(Roles = "CompanyAdministrator,Administrator")]
        public ActionResult<Restaurant> PostRestaurant(RestaurantDto restaurantDto)
        {
            if (User.HasClaim("CompanyId", restaurantDto.CompanyId.ToString()) == false)
            {
                return Unauthorized();
            }


            if (restaurantDto == null)
            {
                return NotFound();
            }

            var restaurant = _mapper.Map<Restaurant>(restaurantDto);
            _context.Restaurants!.Add(restaurant);
            _context.SaveChanges();
            return Ok();
        }

        // DELETE: api/Restaurants/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public ActionResult RestaurantStateChange(int id, byte stateId)
        {
            if (stateId != 0 && stateId != 1 && stateId != 2)
            {
                return BadRequest("Yanlış stateId girdiniz!");
            }

            var companyId = User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            var restaurantCompanyId = _context.Restaurants!.FirstOrDefault(r => r.Id == id)?.ToString();

            if (companyId != restaurantCompanyId)
            {
                return Unauthorized();
            }

            var restaurant = _context.Restaurants!
                .Include(c => c.Categories)!
                    .ThenInclude(r => r.Foods)!
                .FirstOrDefault(c => c.Id == id);

            if (restaurant == null || restaurant.StateId == 0)
            {
                return NotFound("Silmek istediğiniz restaurant bulunamadı veya daha önceden silinmiş.");
            }

            // Şirketin StateId'sini sıfırla
            restaurant.StateId = stateId;

            // Bağlı restoranlar, kategoriler ve yiyeceklerin StateId'lerini sıfırla
            foreach (var category in restaurant.Categories!)
            {
                category.StateId = stateId;

                foreach (var food in category.Foods!)
                {
                    food.StateId = stateId;

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

        private bool RestaurantExists(int id)
        {
            return (_context.Restaurants?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
