using System.ComponentModel.DataAnnotations;

namespace AvaliaFatec.Models
{
   
    public class UserRole
    {
        [Required(ErrorMessage = "O campo Role é obrigatório.")]
        [Display(Name = "Nome da Role")]
        public string? RoleName{ get; set; }
    }
}
