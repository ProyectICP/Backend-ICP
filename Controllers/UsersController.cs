using app.Context;
using app.Models;
using app.Models.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Icp.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UsersController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        [Route("/user")]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.users.OrderBy(c => c.name)
              .Select(x => new Users()
              {
                  _ID = x._ID,
                  name = x.name,
                  email = x.email,
                  role = x.role,
              }).ToListAsync();
        }

        [HttpPost]
        [Route("/user/login")]
        public ActionResult<Users> Login([FromForm] LoginDto dto)
        {
            var user = GetByEmail(dto.Email);

            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.password)) return NotFound();

            return user;
        }

        //[HttpPost]
        //[Route("/user/delete")]
        //public IActionResult userDelete([FromForm] string _ID)
        //{
        //    var user = _context.users.First(u => u._ID == _ID);

        //    if (user == null) return NotFound();

        //    var deleteCommand = "DELETE FROM users WHERE _ID = '" + _ID + "'";
        //    var num = _context.Database.ExecuteSqlRaw(deleteCommand);

        //    if (num == 0)
        //    {
        //        return NotFound();
        //    }

        //    return NoContent();

        //}

        [HttpPost]
        [Route("/user/delete")]
        public IActionResult userDelete([FromForm] string _ID)
        {
            var user = _context.users.FirstOrDefault(u => u._ID == _ID);

            if (user == null) return NotFound();

            var deleteCommand = "DELETE FROM users WHERE _ID = '" + _ID + "'";
            var num = _context.Database.ExecuteSqlRaw(deleteCommand);

            if (num == 0)
            {
                return NotFound();
            }

            return Ok(new
            {
                message = "success"
            });
        }



        [HttpPost]
        [Route("/user/register")]
        public async Task<IActionResult> PostUsers([FromForm] RegisterDto dto)
        {
            Console.WriteLine(dto);
            var userR = new Users
            {
                _ID = dto.id,
                name = dto.name,
                email = dto.email,
                password = BCrypt.Net.BCrypt.HashPassword(dto.password),
                role = dto.role
            };

            _context.users.Add(userR);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UsersExists(userR._ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new
            {
                message = "success"
            });
        }

        private bool UsersExists(string email)
        {
            return _context.users.Any(e => e.email == email);
        }

        private Users GetByEmail(string email)
        {
            return _context.users.FirstOrDefault(u => u.email == email);
        }


        [HttpPut]
        [Route("/user/{id}")]
        public async Task<IActionResult> PutUser(string id, [FromForm] Users user)
        {
            if (id != user._ID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!UsersExists(user.email))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }




        private Users GetByUser(string id)
        {
            return _context.users.FirstOrDefault(u => u._ID == id);
        }
    }
}
