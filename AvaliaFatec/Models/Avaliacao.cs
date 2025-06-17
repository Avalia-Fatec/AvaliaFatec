using System.ComponentModel.DataAnnotations;

namespace AvaliaFatec.Models
{
    public class Avaliacao
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string? Emoji { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        [Display(Name = "ID da Pergunta")]
        public Guid PerguntaId { get; set; }
    }
}
