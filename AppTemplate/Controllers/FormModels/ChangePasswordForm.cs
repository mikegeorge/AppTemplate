using System.ComponentModel.DataAnnotations;

namespace AppTemplate.Controllers.FormModels {
  public class ChangePasswordForm {
    [Required]
    public string CurrentPassword { get; set; }

    [Required][Compare("NewPassword2")]
    public string NewPassword { get; set; }
    [Required]
    public string NewPassword2 { get; set; }
  }
}