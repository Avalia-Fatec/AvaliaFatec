using System.ComponentModel.DataAnnotations;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace AvaliaFatec.Models
{
    [CollectionName("Coordenador")]
    public class ApplicationUser:MongoIdentityUser
    {
        [Display(Name = "Nome Completo")]
        public string? NomeCompleto { get; set; }
    }
}
