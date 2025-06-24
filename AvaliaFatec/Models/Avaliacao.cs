using System.ComponentModel.DataAnnotations;

namespace AvaliaFatec.Models
{
    public class Avaliacao
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string? Emoji { get; set; }
        public DateTime? Data { get; set; }
        public Guid PerguntaId { get; set; }
        public Guid UsuarioId { get; set; }
    }
}
