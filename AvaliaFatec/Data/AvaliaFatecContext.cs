using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AvaliaFatec.Models;

namespace AvaliaFatec.Data
{
    public class AvaliaFatecContext : DbContext
    {
        public AvaliaFatecContext (DbContextOptions<AvaliaFatecContext> options)
            : base(options)
        {
        }

        public DbSet<AvaliaFatec.Models.Pergunta> Pergunta { get; set; } = default!;
        public DbSet<AvaliaFatec.Models.Coordenador> Coordenador { get; set; } = default!;
        public DbSet<AvaliaFatec.Models.Avaliacao> Avaliacao { get; set; } = default!;
    }
}
