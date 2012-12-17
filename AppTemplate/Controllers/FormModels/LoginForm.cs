using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace AppTemplate.Controllers.FormModels {
  public class LoginForm {
    [Required]
    [Email]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public bool Remember { get; set; }
  }
}