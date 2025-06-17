using System.ComponentModel.DataAnnotations;

namespace AvaliaFatec.Models
{
    public class Pergunta
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Conteúdo")]
        public string? Conteudo { get; set; }
        [Display(Name = "Data da Postagem")]
        public DateTime DataPostagem { get; set; }
        public string? Status { get; set; }
        public string? CoordenadorId { get; set; }
    }
}
