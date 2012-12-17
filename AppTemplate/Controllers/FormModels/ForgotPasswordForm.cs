using System.ComponentModel.DataAnnotations;

namespace AppTemplate.Controllers.FormModels {
  public class ForgotPasswordForm {
    [Required]
    public string Email { get; set; }
  }
}