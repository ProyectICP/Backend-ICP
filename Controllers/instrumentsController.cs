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
    public class instrumentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnviroment;

        public instrumentsController(AppDbContext context, IWebHostEnvironment hostEnviroment)
        {
            _context = context;
            this._hostEnviroment = hostEnviroment;
        }

        [HttpGet]
        [Route("/instruments")]
        public async Task<ActionResult<IEnumerable<Instruments>>> GetInstruments()
        {

            return await _context.instruments.OrderBy(c => c.Caption)
                .Select(x => new Instruments()
                {
                    _id = x._id,
                    Laboratorie = x.Laboratorie,
                    Area = x.Area,
                    Area_analitica = x.Area_analitica,
                    Caption = x.Caption,
                    Number_serie = x.Number_serie,
                    Modelo = x.Modelo,
                    Marca = x.Marca,
                    Code_sap = x.Code_sap,
                    Existencia = x.Existencia,
                    Date_make = x.Date_make,
                    Number_part = x.Number_part,
                    Photo = x.Photo,
                    _state = x._state,
                    Caption_equipment = x.Caption_equipment,
                    PhotoSrc = String.Format("{0}://{1}{2}/Images/Instruments/{3}", Request.Scheme, Request.Host, Request.PathBase, x.Photo),
                }).AsSingleQuery().ToListAsync();
        }

        [HttpGet]
        [Route("/instruments/{id}")]
        public async Task<ActionResult<Instruments>> GetInstruments(string id)
        {
            var instruments = await _context.instruments.FindAsync(id);

            if (instruments == null)
            {
                return NotFound();
            }

            return instruments;
        }

        [HttpPut]
        [Route("/instruments/{id}")]
        public async Task<IActionResult> PutInstruments(string id, [FromForm] Instruments i)
        {
            if (id != i._id)
            {
                return BadRequest();
            }

            if (i.PhotoFile != null)
            {
                DeleteImage(i.Photo);
                i.Photo = await SaveImage(i.PhotoFile);
            }

            var updateCommand = "UPDATE instruments set  CAPTION='" + i.Caption +
                "', NUMBER_SERIE = '" + i.Number_serie +
                "', MODELO= '" + i.Modelo +
                "', MARCA = '" + i.Marca +
                "', CODE_SAP= '" + i.Code_sap +
                "', EXISTENCIA = '" + i.Existencia +
                "', NUMBER_PART = '" + i.Number_part +
                "', PHOTO = '" + i.Photo +
                "' where _ID = '" + i._id + "'";
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
                if (!InstrumentsExists(id))
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
        [Route("/instruments")]
        public async Task<ActionResult<Instruments>> PostInstruments([FromForm] Instruments instrument)
        {
            instrument.Photo = await SaveImage(instrument.PhotoFile);
            _context.instruments.Add(instrument);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (InstrumentsExists(instrument._id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetInstruments", new { id = instrument._id }, instrument);
        }


        [HttpPost]
        [Route("/instruments/delete")]
        public IActionResult instrumentsDelete([FromForm] string id)
        {
            var instrument = _context.instruments.FirstOrDefault(u => u._id == id);

            if (instrument == null) return NotFound();

            var deleteCommand = "DELETE FROM instruments WHERE _ID = '" + id + "'";
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

        private bool InstrumentsExists(string id)
        {
            return _context.instruments.Any(e => e._id == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile photoFile)
        {
            string nombreCarpeta = "/wwwroot/Images/Instruments";
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
            string nombreCarpeta = "/wwwroot/Images/Instruments";
            string rutaRaiz = _hostEnviroment.ContentRootPath;
            string rutaCompleta = rutaRaiz + nombreCarpeta;

            var imagePath = Path.Combine(rutaCompleta, photo);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
