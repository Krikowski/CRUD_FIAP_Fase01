using System.Threading.Tasks;
using Crud_FIAP_Debora_Krikowski.Data;
using Crud_FIAP_Debora_Krikowski.Models;
using Microsoft.EntityFrameworkCore;


namespace Crud_FIAP_Debora_Krikowski.Services {
    public class ContatoService {
        private readonly ApplicationDbContext _context;

        public ContatoService(ApplicationDbContext context) {
            _context = context;
        }

        // Método para criar um contato
        public async Task<Contato> CreateContatoAsync(Contato contato) {
            _context.Contatos.Add(contato);
            await _context.SaveChangesAsync();
            return contato;
        }

        // Método para buscar um contato pelo ID
        public async Task<Contato> GetContatoByIdAsync(int id) {
            return await _context.Contatos.FindAsync(id);
        }

        // Método para atualizar um contato
        public async Task<bool> UpdateContatoAsync(Contato contato) {
            _context.Contatos.Update(contato);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        // Método para deletar um contato
        public async Task<bool> DeleteContatoAsync(int id) {
            var contato = await _context.Contatos.FindAsync(id);
            if (contato == null)
                return false;

            _context.Contatos.Remove(contato);
            var deleted = await _context.SaveChangesAsync();
            return deleted > 0;
        }
    }
}
