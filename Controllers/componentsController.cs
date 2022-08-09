using app.Context;
using app.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace app.Controllers
{

    [ApiController]
    public class componentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnviroment;

        public componentsController(AppDbContext context, IWebHostEnvironment hostEnviroment)
        {
            _context = context;
            this._hostEnviroment = hostEnviroment;
        }

        [HttpGet]
        [Route("/components")]
        public async Task<ActionResult<IEnumerable<Components>>> Getcomponents()
        {
            return await _context.components
                .OrderBy(c => c.Caption)
                .Select(x => new Components()
                {
                    _id = x._id,
                    Equipment_id = x.Equipment_id,
                    Caption = x.Caption,
                    Description = x.Description,
                    Number_part = x.Number_part,
                    Code_sap = x.Code_sap,
                    Existence = x.Existence,
                    _state = x._state,
                    Photo = x.Photo,
                    PhotoSrc = String.Format("{0}://{1}{2}/Images/Components/{3}", Request.Scheme, Request.Host, Request.PathBase, x.Photo)
                })
                .ToListAsync();
        }

        [HttpGet]
        [Route("/components/{id}")]
        public async Task<ActionResult<Components>> GetComponents(string id)
        {
            var components = await _context.components.FindAsync(id);

            if (components == null)
            {
                return NotFound();
            }

            return components;
        }

        [HttpPut]
        [Route("/components/{id}")]
        public async Task<IActionResult> PutComponents(string id, [FromForm] Components c)
        {
            if (id != c._id)
            {
                return BadRequest();
            }

            if (c.PhotoFile != null)
            {
                DeleteImage(c.Photo);
                c.Photo = await SaveImage(c.PhotoFile);
            }

            //_context.Entry(components).State = EntityState.Modified;
            var updateCommand = "UPDATE components set  CAPTION='" + c.Caption + "', DESCRIPTION = '" + c.Description + "', NUMBER_PART= '" + c.Number_part + "', CODE_SAP = '" + c.Code_sap + "', EXISTENCE= '" + c.Existence + "', PHOTO = '" + c.Photo + "' where _ID = '"+c._id+"'" ;
            try
            {
                //await _context.SaveChangesAsync();
                var num = _context.Database.ExecuteSqlRaw(updateCommand);

                if(num == 0)
                {
                    return NotFound();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComponentsExists(id))
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

        [HttpPost]
        [Route("/components")]
        public async Task<ActionResult<Components>> PostComponents([FromForm] Components components)
        {
            components.Photo = await SaveImage(components.PhotoFile);
            _context.components.Add(components);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ComponentsExists(components._id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(201);
        }

        [HttpPost]
        [Route("/components/delete")]
        public IActionResult componentsDelete([FromForm] string id)
        {
            var component = _context.components.FirstOrDefault(u => u._id == id);

            if (component == null) return NotFound();

            var deleteCommand = "DELETE FROM components WHERE _ID = '" + id + "'";
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

        private bool ComponentsExists(string id)
        {
            return _context.components.Any(e => e._id == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile photoFile)
        {
            string nombreCarpeta = "/wwwroot/Images/Components";
            string rutaRaiz = _hostEnviroment.ContentRootPath;
            string rutaCompleta = rutaRaiz + nombreCarpeta;

            string photo = new String(Path.GetFileNameWithoutExtension(photoFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            photo = photo + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(photoFile.FileName);
            string photoPath = Path.Combine(rutaCompleta, photo);
            using (var fileStream = new FileStream(photoPath, FileMode.Create))
            {
               await photoFile.CopyToAsync(fileStream);
            }
            return photo;
        }

        [NonAction]
        public void DeleteImage(string photo)
        {
            string nombreCarpeta = "/wwwroot/Images/Components";
            string rutaRaiz = _hostEnviroment.ContentRootPath;
            string rutaCompleta = rutaRaiz + nombreCarpeta;

            var imagePath = Path.Combine(rutaCompleta, photo);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
