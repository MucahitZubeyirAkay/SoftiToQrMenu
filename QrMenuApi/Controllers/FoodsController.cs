using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

namespace QrMenuApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly QrMenuApiContext _context;

        public FoodsController(IMapper mapper, QrMenuApiContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Foods
        [HttpGet]
        public ActionResult<List<Food>> GetFoods()
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            List<Food> food = _context.Foods!.ToList();

            return food;
        }

        // GET: api/Foods/5
        [HttpGet("{id}")]
        public ActionResult<Food> GetFood(int id)
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            Food? food = _context.Foods.Find(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }

        // PUT: api/Foods/5
        [HttpPut("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public ActionResult PutFood(int id, FoodDto foodDto)
        {
            var restaurantId = User.Claims.FirstOrDefault(c => c.Type == "RestaurantId")?.Value;

            var categoryRestaurantId = _context.Foods!.Include(c => c.Category).FirstOrDefault(f => f.Id == id)?.Category?.RestraurantId.ToString();

            if (restaurantId != categoryRestaurantId)
            {
                return Unauthorized();
            }

            var existingFood = _context.Foods!.Find(id);
            if (existingFood == null)
            {
                return NotFound();
            }

            // AutoMapper kullanarak DTO'yu model nesnesine eşleme
            var food = _mapper.Map(foodDto, existingFood);

            //Eşlenen modeli veritabanı ile güncelleme
            _context.Entry(food).State = EntityState.Modified;

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

        // POST: api/Foods
        [HttpPost]
        [Authorize(Roles = "RestaurantAdministrator")]
        public ActionResult<Food> PostFood(FoodDto foodDto)
        {
            var restaurantId = User.Claims.FirstOrDefault(c => c.Type == "RestaurantId")?.Value;

            var categoryRestaurantId = _context.Foods!.Include(c => c.Category).FirstOrDefault(f => f.Id == foodDto.CategoryId)?.Category?.RestraurantId.ToString();

            if (restaurantId != categoryRestaurantId)
            {
                return Unauthorized();
            }

            if (foodDto == null)
            {
                return NotFound();
            }

            var food = _mapper.Map<Food>(foodDto);
            _context.Foods!.Add(food);
            _context.SaveChanges();
            return Ok();
        }

        // DELETE: api/Foods/5
        [HttpDelete("{id}")]
        public ActionResult FoodStateChange(int id, byte stateId)
        {
            if (stateId != 0 && stateId != 1 && stateId != 2)
            {
                return BadRequest("Yanlış stateId girdiniz!");
            }
            var food = _context.Foods!.FirstOrDefault(f => f.Id == id);

            if (food == null || food.StateId == 0)
            {
                return NotFound("Silmek istediğiniz yiyecek bulunamadı. Ya daha önceden silinmiş yada hiç var olmamış.");
            }

            //Food'un Id sini aldığımız parametreye göre değiştirme.
            food.StateId = stateId;

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

        private bool FoodExists(int id)
        {
            return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
