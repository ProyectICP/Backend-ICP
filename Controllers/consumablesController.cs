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
    public class consumablesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnviroment;

        public consumablesController(AppDbContext context, IWebHostEnvironment hostEnviroment)
        {
            _context = context;
            this._hostEnviroment = hostEnviroment;
        }

        [HttpGet]
        [Route("/consumables")]
        public async Task<ActionResult<IEnumerable<Consumables>>> Getconsumables()
        {
            return await _context.consumables
                .OrderBy(c => c.Caption)
                .Select(x => new Consumables()
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
                    PhotoSrc = String.Format("{0}://{1}{2}/Images/Consumables/{3}", Request.Scheme, Request.Host, Request.PathBase, x.Photo)
                })
                .ToListAsync();
        }

        [HttpGet]
        [Route("/consumables/{id}")]
        public async Task<ActionResult<Consumables>> GetConsumables(string id)
        {
            var consumables = await _context.consumables.FindAsync(id);

            if (consumables == null)
            {
                return NotFound();
            }

            return consumables;
        }

        [HttpPut]
        [Route("/consumables/{id}")]
        public async Task<IActionResult> PutConsumables(string id, [FromForm] Consumables c)
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

            //_context.Entry(consumables).State = EntityState.Modified;
            var updateCommand = "UPDATE consumables set  CAPTION='" + c.Caption + "', DESCRIPTION = '" + c.Description + "', NUMBER_PART= '" + c.Number_part + "', CODE_SAP = '" + c.Code_sap + "', EXISTENCE= '" + c.Existence + "', PHOTO = '" + c.Photo + "' where _ID = '" + c._id + "'";

            try
            {
                //await _context.SaveChangesAsync();
                var num = _context.Database.ExecuteSqlRaw(updateCommand);

                if (num == 0)
                {
                    return NotFound();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsumablesExists(id))
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
        [Route("/consumables")]
        public async Task<ActionResult<Consumables>> PostConsumables([FromForm] Consumables consumables)
        {
            consumables.Photo = await SaveImage(consumables.PhotoFile);
            _context.consumables.Add(consumables);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ConsumablesExists(consumables._id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetConsumables", new { id = consumables._id }, consumables);
        }

        [HttpPost]
        [Route("/consumables/delete")]
        public IActionResult consumablesDelete([FromForm] string id)
        {
            var consumable = _context.consumables.FirstOrDefault(u => u._id == id);

            if (consumable == null) return NotFound();

            var deleteCommand = "DELETE FROM consumables WHERE _ID = '" + id + "'";
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

        private bool ConsumablesExists(string id)
        {
            return _context.consumables.Any(e => e._id == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile photoFile)
        {
            string nombreCarpeta = "/wwwroot/Images/Consumables";
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
            string nombreCarpeta = "/wwwroot/Images/Consumables";
            string rutaRaiz = _hostEnviroment.ContentRootPath;
            string rutaCompleta = rutaRaiz + nombreCarpeta;

            var imagePath = Path.Combine(rutaCompleta, photo);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
