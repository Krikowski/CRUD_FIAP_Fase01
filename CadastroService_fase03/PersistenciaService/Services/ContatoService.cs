using PersistenciaService.Data;
using PersistenciaService.Models;

namespace PersistenciaService.Services {
    public class ContatoService {
        private readonly ApplicationDbContext _context;

        public ContatoService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<Contato> AdicionarContatoAsync(Contato contato) {
            _context.Contatos.Add(contato);
            await _context.SaveChangesAsync();
            return contato;
        }
    }
}
