using CadastroService.Models;
using CadastroService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CadastroService.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CadastroController : ControllerBase {
        private readonly IPersistenciaServiceClient _persistenciaClient;

        public CadastroController(IPersistenciaServiceClient persistenciaClient) {
            _persistenciaClient = persistenciaClient;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CriarContato([FromBody] ContatoDto contato) {
            var sucesso = await _persistenciaClient.EnviarContatoAsync(contato);
            if (!sucesso)
                return StatusCode(500, "Erro ao criar o contato.");

            return Created("", contato);
        }

        // READ - Listar todos
        [HttpGet]
        public async Task<ActionResult<List<ContatoDto>>> ListarContatos() {
            var contatos = await _persistenciaClient.ListarContatosAsync();
            return Ok(contatos);
        }

        // READ - Obter por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ContatoDto>> ObterContato(int id) {
            var contato = await _persistenciaClient.ObterContatoAsync(id);
            if (contato == null)
                return NotFound();

            return Ok(contato);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarContato(int id, [FromBody] ContatoDto contato) {
            var sucesso = await _persistenciaClient.AtualizarContatoAsync(id, contato);
            if (!sucesso)
                return NotFound();

            return NoContent(); // 204
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarContato(int id) {
            var sucesso = await _persistenciaClient.DeletarContatoAsync(id);
            if (!sucesso)
                return NotFound();

            return NoContent(); // 204
        }
    }
}
