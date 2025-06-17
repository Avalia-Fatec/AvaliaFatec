using System.ComponentModel.DataAnnotations;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace AvaliaFatec.Models
{
    [CollectionName ("Roles")]
    public class ApplicationRole:MongoIdentityRole
    {
        [Required(ErrorMessage = "O campo Role é obrigatório.")]
        [Display(Name = "Nome da Role")]
        public string? RoleName { get; set; }
    }
}
