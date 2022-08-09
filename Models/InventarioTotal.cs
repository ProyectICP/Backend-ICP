using System.ComponentModel.DataAnnotations;

namespace app.Models
{
  public class InventarioTotal
  {
    [Key]
    public string _id { get; set; }
    public string Laboratorio { get; set; }
    public string Area { get; set; }
    public string Caption { get; set; }
    public string Code_sap { get; set; }
  }

    public class InventarioInst
    {
        public string _id { get; set; }
        public string Caption { get; set; }
        public string Laboratorio { get; set; }
        public string Area { get; set; }
        public string Existence { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Code_sap { get; set; }
        public string Caption_equipment { get; set; }

    }
}
