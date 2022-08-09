using app.Context;
using app.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Controllers
{

    [ApiController]
    public class inventarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnviroment;

        public inventarioController(AppDbContext context, IWebHostEnvironment hostEnviroment)
        {
            _context = context;
            this._hostEnviroment = hostEnviroment;
        }

        [HttpGet]
        [Route("/inventarioEquip")]
        public async Task<ActionResult<IEnumerable<InventarioTotal>>> GetInventarios()
        {
            
            return await _context.equipments.OrderBy(c => c.Caption)
                .Select(x => new InventarioTotal()
                {
                    _id = x._id,
                    Laboratorio = x.Laboratorio,
                    Area = x.Area,
                    Caption = x.Caption,
                    Code_sap = x.Code_sap
                }).AsSingleQuery().ToListAsync();
        }

        [HttpPost]
        [Route("/comp")]
        public async Task<ActionResult<IEnumerable<Components>>> GetComponentslIMInv([FromForm]  string lab)
        {
            if (lab == "fieh")
            {
                return await _context.components.FromSqlRaw("select c._ID,c.CAPTION, c.DESCRIPTION,c.EXISTENCE,c.NUMBER_PART,c.CODE_SAP,c.EQUIPMENT_ID,c._STATE,C.PHOTO from components as c, equipments as e where e._ID = c.EQUIPMENT_ID and e.LABORATORIO = 'LABORATORIO FENOMENOS INTERFACIALES Y EVALUACIÓN DE HIDROCARBUROS'").ToListAsync();

            }

            return await _context.components.FromSqlRaw("select c._ID,c.CAPTION, c.DESCRIPTION,c.EXISTENCE,c.NUMBER_PART,c.CODE_SAP,c.EQUIPMENT_ID,c._STATE,C.PHOTO from components as c, equipments as e where e._ID = c.EQUIPMENT_ID and e.LABORATORIO = 'LABORATORIO INGENIERIA E INTEGRIDAD DE MATERIALES'").ToListAsync();
        }


        [HttpPost]
        [Route("/cons")]
        public async Task<ActionResult<IEnumerable<Consumables>>> GetConsumableslIMInv([FromForm] string lab)
        {
            if (lab == "fieh")
            {
                return await _context.consumables.FromSqlRaw("select c._ID,c.CAPTION, c.DESCRIPTION,c.EXISTENCE,c.NUMBER_PART,c.CODE_SAP,c.EQUIPMENT_ID,c._STATE,c.PHOTO from consumables as c, equipments as e where e._ID = c.EQUIPMENT_ID and e.LABORATORIO = 'LABORATORIO FENOMENOS INTERFACIALES Y EVALUACIÓN DE HIDROCARBUROS'").ToListAsync();
            }

            return await _context.consumables.FromSqlRaw("select c._ID,c.CAPTION, c.DESCRIPTION,c.EXISTENCE,c.NUMBER_PART,c.CODE_SAP,c.EQUIPMENT_ID,c._STATE,c.PHOTO from consumables as c, equipments as e where e._ID = c.EQUIPMENT_ID and e.LABORATORIO = 'LABORATORIO INGENIERIA E INTEGRIDAD DE MATERIALES'").ToListAsync();
        }

        [HttpPost]
        [Route("/reag")]
        public async Task<ActionResult<IEnumerable<Reagents>>> GetReagentslIMInv([FromForm] string lab)
        {
            if (lab == "fieh")
            {
                return await _context.reagents.FromSqlRaw("select r._ID,r.CAPTION, r.DESCRIPTION,r.EXISTENCE,r.NUMBER_PART,r.CODE_SAP,r.EQUIPMENT_ID,r.CODE_CAS,r._STATE,r.PHOTO from reagents as r, equipments as e where e._ID = r.EQUIPMENT_ID and e.LABORATORIO = 'LABORATORIO FENOMENOS INTERFACIALES Y EVALUACIÓN DE HIDROCARBUROS'").ToListAsync();
            }

            return await _context.reagents.FromSqlRaw("select r._ID,r.CAPTION, r.DESCRIPTION,r.EXISTENCE,r.NUMBER_PART,r.CODE_SAP,r.EQUIPMENT_ID,r.CODE_CAS,r._STATE,r.PHOTO from reagents as r, equipments as e where e._ID = r.EQUIPMENT_ID and e.LABORATORIO = 'LABORATORIO INGENIERIA E INTEGRIDAD DE MATERIALES'").ToListAsync();
        }

        [HttpGet]
        [Route("/inventarioIns")]
        public async Task<ActionResult<IEnumerable<InventarioInst>>> GetInstrumentslIMInv()
        {

            return await _context.instruments.OrderBy(c => c.Caption)
                            .Select(x => new InventarioInst()
                            {
                                _id = x._id,
                                Laboratorio = x.Laboratorie,
                                Area = x.Area,
                                Caption = x.Caption,
                                Code_sap = x.Code_sap,
                                Existence = x.Existencia,
                                Marca = x.Marca,
                                Modelo = x.Modelo,
                                Caption_equipment = x.Caption_equipment
                            }).AsSingleQuery().ToListAsync();
        }

      
    }
}
