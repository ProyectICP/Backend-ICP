using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace app.Models
{
  public class Components
  {
    [Key]
    public string _id { get; set; }

    public string Equipment_id { get; set; }

    public string Caption { get; set; }

    public string Description { get; set; }

    public string Number_part { get; set; }

    public string Code_sap { get; set; }

    public int Existence { get; set; }

    public string _state { get; set; }

    public string Photo { get; set; }

    [JsonIgnore]
    public Equipments equipment { get; set; }

    [NotMapped]
    public IFormFile PhotoFile { get; set; }

    [NotMapped]
    public string PhotoSrc { get; set; }

  }
}
