using Microsoft.EntityFrameworkCore;
using PersistenciaService.Models;

namespace PersistenciaService.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Contato> Contatos { get; set; }
    }
}
