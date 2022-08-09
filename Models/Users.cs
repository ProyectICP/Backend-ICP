using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace app.Models
{
  public class Users
  {
    [Key]
    public string _ID { get; set; }

    public string name { get; set; }

    public string email { get; set; }

    public string role { get; set; }

    [JsonIgnore]
    public string password { get; set; }
  }
}
