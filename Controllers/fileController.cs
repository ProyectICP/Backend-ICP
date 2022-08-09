using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;


namespace app.Controllers
{

    [ApiController]
    public class fileController : ControllerBase
    {

        private readonly IWebHostEnvironment _hostEnviroment;

        public fileController(IWebHostEnvironment hostEnviroment)
        {
            this._hostEnviroment = hostEnviroment;
        }

        [HttpPost]
        [Route("/file")]
        public string UploadFile()
        {
            string resultado = "";

            var inputEquipInst = Request.Form["inputEquipInst"]; //equipo o inst
            var inputOption = Request.Form["inputOption"]; // que equipo o inst
            var inputDoc = Request.Form["inputDoc"]; //tipo de documento 
            var file = Request.Form.Files[0]; //file

            string NombreCarpeta = "/Files/" + inputEquipInst + "/" + inputDoc;
            string RutaRaiz = _hostEnviroment.WebRootPath;
            string RutaCompleta = RutaRaiz + NombreCarpeta;

            if (!Directory.Exists(RutaCompleta))
            {
                Directory.CreateDirectory(RutaCompleta);
            }




            if (file.Length > 0)
            {
                string NombreArchivo = inputDoc + "-" + inputOption + ".pdf";
                string RutaFullCompleta = Path.Combine(RutaCompleta, NombreArchivo);

                if (Existfile(RutaFullCompleta))
                {
                    return "Archivo ya existe";
                }
                {
                    using (var stream = new FileStream(RutaFullCompleta, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        resultado = "Archivo guardado";
                    }
                }

                //if (Existfile(RutaCompleta))
                //{
                //    string resul = DeleteFile(RutaCompleta);
                //}

                //using (var stream = new FileStream(RutaFullCompleta, FileMode.Create))
                //{
                //    file.CopyTo(stream);
                //    resultado = "Archivo guardado";
                //}

            }


            return resultado;
        }

        [HttpPost]
        [Route("/down")]
        public ActionResult dowloadFile()
        {
            var inputEquipInst = Request.Form["inputEquipInst"]; //equipo o inst
            var inputOption = Request.Form["inputOption"]; // que equipo o inst
            var inputDoc = Request.Form["inputDoc"]; //tipo de documento 

            string NombreArchivo = inputDoc + "-" + inputOption + ".pdf";
            string ruta = "/Files/" + inputEquipInst + "/" + inputDoc;


            var path = Path.Combine(_hostEnviroment.WebRootPath, ruta, NombreArchivo);
            return File(path, "application/pdf", NombreArchivo);
        }

        [HttpPost]
        [Route("/delete")]
        public string deleteFile()
        {
            string resultado = "";

            var inputEquipInst = Request.Form["inputEquipInst"]; //equipo o inst
            var inputOption = Request.Form["inputOption"]; // que equipo o inst
            var inputDoc = Request.Form["inputDoc"]; //tipo de documento 

            string NombreArchivo = inputDoc + "-" + inputOption + ".pdf";

            string NombreCarpeta = "/Files/" + inputEquipInst + "/" + inputDoc + "/" + NombreArchivo;
            string RutaRaiz = _hostEnviroment.WebRootPath;
            string path = RutaRaiz + NombreCarpeta;

            if (Existfile(path))
            {
                return resultado = DeleteFile(path);
            }
            else
            {
                return resultado = "Archivo no existente";
            }
            return "Problemas";
        }

        private bool Existfile(string ruta)
        {
            if (System.IO.File.Exists(ruta))
            {
                return true;
            }

            return false;

        }

        [NonAction]
        public string DeleteFile(string ruta)
        {

            System.IO.File.Delete(ruta);
            return "Archivo eliminado";

        }
    }
}
