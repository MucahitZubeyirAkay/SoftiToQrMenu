using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly QrMenuApiContext _context;

        public CategoriesController(IMapper mapper, QrMenuApiContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public ActionResult<List<Category>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }

            List<Category> category = _context.Categories!.ToList();

            return category;
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory(int id)
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            Category? category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public ActionResult PutCategory(int id, CategoryDto categoryDto)
        {
            var existingCategory = _context.Categories!.Find(id);

           if(existingCategory == null)
            {
                return NotFound();
            }

            var category = _mapper.Map(categoryDto, existingCategory);
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                 _context.SaveChangesAsync();
                return Ok("Güncelleme başarılı");
            }
            catch (DbUpdateConcurrencyException)
            {
                return NoContent();
            }

        }

        // POST: api/Categories
        [HttpPost]
        public ActionResult<Category> PostCategory(Category categoryDto)
        {
          if (_context.Categories == null)
          {
                return NotFound();
          }
            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public ActionResult CategoryStateChange(int id, byte stateId)
        {
            if (stateId != 0 && stateId != 1 && stateId != 2)
            {
                return BadRequest("Yanlış stateId girdiniz!");
            }
            var category = _context.Categories!
                .Include(ct => ct.Foods)!.FirstOrDefault(ct => ct.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            //Category Id'sini sıfırla
            category.StateId = stateId;

            //Bağlı foodların StateId'lerini sıfırla
            foreach(var food in category.Foods!)
            {
                food.StateId = stateId;
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

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
