using System.ComponentModel.DataAnnotations;

namespace AvaliaFatec.Models
{
    public class Coordenador
    {
        [Required]
        public Guid Id { get; set; }
        [Display(Name = "Nome Completo")]
        public string? NomeCompleto { get; set; }
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        public string? Senha { get; set; }
        [Display(Name = "Confirmar Senha")]
        public string? ConfirmeSenha { get; set; }
    }
}
