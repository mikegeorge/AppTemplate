using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace AppTemplate.Controllers.FormModels {
  public class RegisterForm {
    [Required]
    public string Username { get; set; }
    [Required, Email]
    public string Email { get; set; }
  }
}