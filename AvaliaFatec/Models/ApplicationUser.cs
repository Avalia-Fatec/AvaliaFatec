using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace AvaliaFatec.Models
{
    [CollectionName("User")]
    public class ApplicationUser:MongoIdentityUser
    {
        [Display(Name = "Nome Completo")]
        public string? NomeCompleto { get; set; }
        [Display(Name = "Senha")]
        public string? Senha { get; set; }

        [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
        public string? ConfirmeSenha { get; set; }
    }
}
