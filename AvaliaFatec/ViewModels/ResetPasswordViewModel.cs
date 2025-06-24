using System.ComponentModel.DataAnnotations;

namespace AvaliaFatec.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Nova Senha")]
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirmar Nova Senha")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "A senha e a confirmação não conferem")]
        public string ConfirmPassword { get; set; }
    }
}
