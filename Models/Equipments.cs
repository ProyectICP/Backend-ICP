using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
  public class Equipments
  {
    [Key]
    public string _id { get; set; }
    public string Laboratorio { get; set; }
    public string Area { get; set; }
    public string Area_analitica { get; set; }
    public string Caption { get; set; }
    public string Fabricante { get; set; }
    public string Proveedor { get; set; }
    public string Code_serie { get; set; }
    public string Model { get; set; }
    public string Software { get; set; }
    public string Consecutivo { get; set; }
    public string Code_sap { get; set; }
    public string Code_stock { get; set; }
    public DateTime Date_make { get; set; }
    public string Ubicacion { get; set; }
    public string Ceco { get; set; }
    public string Photo { get; set; }
    public string _state { get; set; }
    public string Ambientales { get; set; }
    public string Electricas { get; set; }
    public string Dimensiones { get; set; }
    public string Peso { get; set; }
    public virtual ICollection<Components> components { get; set; }

    public virtual ICollection<Consumables> consumables { get; set; }

    public virtual ICollection<Reagents> reagents { get; set; }


    [NotMapped]
    public IFormFile PhotoFile { get; set; }

    [NotMapped]
    public string PhotoSrc { get; set; }
        
    }
}
