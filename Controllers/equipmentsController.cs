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
    public class equipmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnviroment;

        public equipmentsController(AppDbContext context, IWebHostEnvironment hostEnviroment)
        {
            _context = context;
            this._hostEnviroment = hostEnviroment;
        }

        [HttpGet]
        [Route("/equipments")]
        public async Task<ActionResult<IEnumerable<Equipments>>> GetEquipments()
        {

            return await _context.equipments.OrderBy(c => c.Caption)
                .Include(c => c.components)
                .Include(c => c.reagents)
                .Include(c => c.consumables)
                .Select(x => new Equipments()
                {
                    _id = x._id,
                    Laboratorio = x.Laboratorio,
                    Area = x.Area,
                    Area_analitica = x.Area_analitica,
                    Caption = x.Caption,
                    Fabricante = x.Fabricante,
                    Proveedor = x.Proveedor,
                    Code_serie = x.Code_serie,
                    Model = x.Model,
                    Software = x.Software,
                    Consecutivo = x.Consecutivo,
                    Code_sap = x.Code_sap,
                    Code_stock = x.Code_stock,
                    Date_make = x.Date_make,
                    Ubicacion = x.Ubicacion,
                    Ceco = x.Ceco,
                    _state = x._state,
                    Ambientales = x.Ambientales,
                    Electricas = x.Electricas,
                    Dimensiones = x.Dimensiones,
                    Peso = x.Peso,
                    Photo = x.Photo,
                    components = x.components,
                    consumables = x.consumables,
                    reagents = x.reagents,
                    PhotoSrc = String.Format("{0}://{1}{2}/Images/Equipments/{3}", Request.Scheme, Request.Host, Request.PathBase, x.Photo),
                }).AsSingleQuery().ToListAsync();
        }

        [HttpGet]
        [Route("/equipments/{id}")]
        public async Task<ActionResult<Equipments>> GetEquipment(string id)
        {
            var equipments = await _context.equipments.FindAsync(id);

            if (equipments == null)
            {
                return NotFound();
            }

            return equipments;
        }

        [HttpPut]
        [Route("/equipments/{id}")]
        public async Task<IActionResult> PutEquipments(string id, [FromForm] Equipments e)
        {
            if (id != e._id)
            {
                return BadRequest();
            }

            if (e.PhotoFile != null)
            {
                DeleteImage(e.Photo);
                e.Photo = await SaveImage(e.PhotoFile);
            }

            var updateCommand = "UPDATE equipments set  CAPTION='" + e.Caption +
                "', FABRICANTE = '" + e.Fabricante +
                "', PROVEEDOR= '" + e.Proveedor +
                "', CODE_SERIE = '" + e.Code_serie +
                "', MODEL= '" + e.Model +
                "', SOFTWARE = '" + e.Software +
                "', CONSECUTIVO = '" + e.Consecutivo +
                "', CODE_SAP = '" + e.Code_sap +
                "', UBICACION = '" + e.Ubicacion +
                "', CECO = '" + e.Ceco +
                "', PHOTO = '" + e.Photo +
                "', AMBIENTALES = '" + e.Ambientales +
                "', ELECTRICAS = '" + e.Electricas +
                "', DIMENSIONES = '" + e.Dimensiones +
                "', PESO = '" + e.Peso +
                "' where _ID = '" + e._id + "'";
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
                if (!EquipmentsExists(id))
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
        [Route("/equipments")]
        public async Task<ActionResult<Equipments>> PostEquipments([FromForm] Equipments equipment)
        {
            try
            {
                equipment.Photo = await SaveImage(equipment.PhotoFile);
                _context.equipments.Add(equipment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EquipmentsExists(equipment._id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEquipments", new { id = equipment._id }, equipment);
        }

        [HttpPost]
        [Route("/equipments/delete")]
        public IActionResult equipmentsDelete([FromForm] string id)
        {
            var equipment = _context.equipments.FirstOrDefault(u => u._id == id);

            if (equipment == null) return NotFound();

            var deleteCommand = "DELETE FROM equipments WHERE _ID = '" + id + "'";
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

        private bool EquipmentsExists(string id)
        {
            return _context.equipments.Any(e => e._id == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile photoFile)
        {
            string nombreCarpeta = "/wwwroot/Images/Equipments";
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
            string nombreCarpeta = "/wwwroot/Images/Equipments";
            string rutaRaiz = _hostEnviroment.ContentRootPath;
            string rutaCompleta = rutaRaiz + nombreCarpeta;

            var imagePath = Path.Combine(rutaCompleta, photo);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}
