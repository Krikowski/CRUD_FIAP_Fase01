using Microsoft.AspNetCore.Mvc;
using PersistenciaService.Data;
using PersistenciaService.Models;
using Microsoft.EntityFrameworkCore;

namespace PersistenciaService.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ContatosController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public ContatosController(ApplicationDbContext context) {
            _context = context;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contato contato) {
            _context.Contatos.Add(contato);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = contato.Id }, contato);
        }

        // READ - Get all
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var contatos = await _context.Contatos.ToListAsync();
            return Ok(contatos);
        }

        // READ - Get by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) {
            var contato = await _context.Contatos.FindAsync(id);
            if (contato == null) return NotFound();
            return Ok(contato);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contato contatoAtualizado) {
            if (id != contatoAtualizado.Id)
                return BadRequest("ID do contato incompatível.");

            var contatoExistente = await _context.Contatos.FindAsync(id);
            if (contatoExistente == null)
                return NotFound();

            // Atualiza os campos
            contatoExistente.Nome = contatoAtualizado.Nome;
            contatoExistente.Email = contatoAtualizado.Email;
            contatoExistente.Telefone = contatoAtualizado.Telefone;
            contatoExistente.DDD = contatoAtualizado.DDD;

            _context.Contatos.Update(contatoExistente);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 - atualização bem-sucedida sem conteúdo
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var contato = await _context.Contatos.FindAsync(id);
            if (contato == null)
                return NotFound();

            _context.Contatos.Remove(contato);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 - exclusão bem-sucedida sem conteúdo
        }
    }
}
