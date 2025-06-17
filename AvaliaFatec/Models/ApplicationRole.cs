using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace AvaliaFatec.Models
{
    [CollectionName ("Roles")]
    public class ApplicationRole:MongoIdentityRole
    {

    }
}
