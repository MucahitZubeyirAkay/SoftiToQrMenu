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
    public class RestaurantsUsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly QrMenuApiContext _context;

        public RestaurantsUsersController(IMapper mapper, QrMenuApiContext context)
        {
            _mapper = mapper;
            _context = context;
        }


        [HttpGet("GetUsers/{id}")]
        public ActionResult<List<string>> GetRestaurantUsers(int id)
        {
            if (_context.RestaurantUser!.Any(ru => ru.RestaurantId != id))
            {
                return NotFound();
            }

            var restaurantUsers = _context.RestaurantUser!.Where(ru => ru.RestaurantId == id).Select(ru=> ru.ApplicationUserId).ToList();

            if(restaurantUsers==null)
            {
                return NotFound();
            }
            
            return restaurantUsers!;
            
                
        }

        [HttpGet("GetRestaurants/{id}")]
        public ActionResult<List<int>> GetUserRestaurants(string id)
        {
            if (!_context.RestaurantUser!.Any(ru => ru.ApplicationUserId == id))
            {
                return NotFound();
            }

            var userRestaurants = _context.RestaurantUser!.Where(ru => ru.ApplicationUserId == id).Select(ru => ru.RestaurantId).ToList();


            return userRestaurants;

        }

        [HttpPost]
        public ActionResult<RestaurantUser> PostRestaurantsUsers(RestaurantUserDto restaurantUserDto)
        {
            if(restaurantUserDto== null)
            {
                return NotFound();
            }

            var restaurantuser = _mapper.Map<RestaurantUser>(restaurantUserDto);
            _context.RestaurantUser!.Add(restaurantuser);
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("DeleteRestaurantUser")]
        public ActionResult DeleteRestaurantUser(int? restaurantId, string? userId)
        {
            if(restaurantId==null && userId==null)
            {
                return NotFound();
            }

            var restaurantUser = _context.RestaurantUser!.FirstOrDefault(ru => ru.RestaurantId == restaurantId && ru.ApplicationUserId == userId);

                if (restaurantUser == null)
                {
                    return NotFound();
                }

            _context.RestaurantUser!.Remove(restaurantUser);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("DeleteRestaurantUsers")]
        public ActionResult DeleteRestaurantUser(int restaurantId)
        {
           
            var restaurantUsers = _context.RestaurantUser!.FirstOrDefault(ru => ru.RestaurantId == restaurantId);

            if (restaurantUsers == null)
            {
                return NotFound();
            }

            _context.RestaurantUser!.Remove(restaurantUsers);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("DeleteUserRestaurants")]
        public ActionResult DeleteUserResaurants(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var userRestaurants = _context.RestaurantUser!.FirstOrDefault(ru => ru.ApplicationUserId == userId);

            if (userRestaurants == null)
            {
                return NotFound();
            }

            _context.RestaurantUser!.Remove(userRestaurants);
            _context.SaveChanges();

            return NoContent();
        }



        private bool RestaurantUserExists(int id)
        {
            return (_context.RestaurantUser?.Any(e => e.RestaurantId == id)).GetValueOrDefault();
        }
    }
}
