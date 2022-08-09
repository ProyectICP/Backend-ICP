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
    public class reagentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnviroment;

        public reagentsController(AppDbContext context, IWebHostEnvironment hostEnviroment)
        {
            _context = context;
            this._hostEnviroment = hostEnviroment;
        }

        [HttpGet]
        [Route("/reagents")]
        public async Task<ActionResult<IEnumerable<Reagents>>> Getreagents()
        {
            return await _context.reagents
                .OrderBy(c => c.Caption)
                .Select(x => new Reagents()
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
                    Code_cas = x.Code_cas,
                    PhotoSrc = String.Format("{0}://{1}{2}/Images/Reagents/{3}", Request.Scheme, Request.Host, Request.PathBase, x.Photo)
                })
                .ToListAsync();
        }

        [HttpGet]
        [Route("/reagents/{id}")]
        public async Task<ActionResult<Reagents>> GetReagents(string id)
        {
            var reagents = await _context.reagents.FindAsync(id);

            if (reagents == null)
            {
                return NotFound();
            }

            return reagents;
        }

        [HttpPut]
        [Route("/reagents/{id}")]
        public async Task<IActionResult> PutReagents(string id, [FromForm] Reagents r)
        {
            if (id != r._id)
            {
                return BadRequest();
            }

            if (r.PhotoFile != null)
            {
                DeleteImage(r.Photo);
                r.Photo = await SaveImage(r.PhotoFile);
            }

            var updateCommand = "UPDATE reagents set  CAPTION='" + r.Caption + "', DESCRIPTION = '" + r.Description + "', NUMBER_PART= '" + r.Number_part + "', CODE_SAP = '" + r.Code_sap + "', CODE_CAS ='"+r.Code_cas+"', EXISTENCE= '" + r.Existence + "', PHOTO = '" + r.Photo + "' where _ID = '" + r._id + "'";
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
                if (!ReagentsExists(id))
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
        [Route("/reagents")]
        public async Task<ActionResult<Reagents>> PostReagents([FromForm] Reagents reagents)
        {
            reagents.Photo = await SaveImage(reagents.PhotoFile);
            _context.reagents.Add(reagents);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ReagentsExists(reagents._id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetReagents", new { id = reagents._id }, reagents);
        }

        [HttpPost]
        [Route("/reagents/delete")]
        public IActionResult reagentsDelete([FromForm] string id)
        {
            var reagent = _context.reagents.FirstOrDefault(u => u._id == id);

            if (reagent == null) return NotFound();

            var deleteCommand = "DELETE FROM reagents WHERE _ID = '" + id + "'";
            var num = _context.Database.ExecuteSqlRaw(deleteCommand);

            if (num == 0)
            {
                return NotFound();
            }

            DeleteImage(reagent.Photo);

            return Ok(new
            {
                message = "success"
            });
        }

        private bool ReagentsExists(string id)
        {
            return _context.reagents.Any(e => e._id == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile photoFile)
        {
            string nombreCarpeta = "/wwwroot/Images/Reagents";
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
            string nombreCarpeta = "/wwwroot/Images/Reagents";
            string rutaRaiz = _hostEnviroment.ContentRootPath;
            string rutaCompleta = rutaRaiz + nombreCarpeta;

            var imagePath = Path.Combine(rutaCompleta, photo);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
