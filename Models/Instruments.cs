using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
  public class Instruments
  {
    [Key]
    public string _id { get; set; }

    public string Laboratorie { get; set; }

    public string Area { get; set; }

    public string Area_analitica { get; set; }

    public string Caption { get; set; }

    public string Number_serie { get; set; }

    public string Modelo { get; set; }

    public string Marca { get; set; }

    public string Code_sap { get; set; }

    public string Existencia { get; set; }

    public string Number_part { get; set; }

    public DateTime Date_make { get; set; }

    public string Photo { get; set; }

    public string _state { get; set; }

    public string Caption_equipment { get; set; }

    [NotMapped]
    public IFormFile PhotoFile { get; set; }

    [NotMapped]
    public string PhotoSrc { get; set; }

  }
}
