using Crud_FIAP_Debora_Krikowski.Data;
using Crud_FIAP_Debora_Krikowski.Models;
using Crud_FIAP_Debora_Krikowski.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Crud_FIAP_Debora_Krikowski.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ContatosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public ContatosController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        //[POST] /api/contatos
        [HttpPost]
        public async Task<IActionResult> Create(Contato contato)
        {

            // Chama o validator para validar o contato
            var errors = ContatoValidator.Validate(contato);
            if (errors.Any())
            {
                return BadRequest(errors);
            }

            // Verifica se o e-mail já existe
            if (_context.Contatos.Any(c => c.Email == contato.Email))
            {
                return BadRequest("O e-mail já está cadastrado.");
            }

            _context.Contatos.Add(contato);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = contato.Id }, contato);
        }

        //[GET] /api/contatos + DDD
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? ddd)
        {
            var cacheKey = string.IsNullOrEmpty(ddd) ? "all_contatos" : $"contatos_ddd_{ddd}";

            if (_cache.TryGetValue(cacheKey, out List<Contato> contatos))
            {
                return Ok(contatos);
            }

            var contatosQuery = _context.Contatos.AsQueryable();

            if (!string.IsNullOrEmpty(ddd))
            {
                contatosQuery = contatosQuery.Where(c => c.DDD == ddd);
            }

            contatos = await contatosQuery.ToListAsync();

            if (!contatos.Any())
            {
                return NotFound(new { error = "Nenhum contato encontrado com o DDD informado." });
            }

            _cache.Set(cacheKey, contatos, TimeSpan.FromMinutes(10));

            return Ok(contatos);
        }


        // GET: api/contatos/{id} - getAll
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!int.TryParse(id, out int idNumerico))
            {
                return BadRequest(new { error = "O ID fornecido deve ser um número válido." });
            }

            string cacheKey = $"contato_{idNumerico}";

            // Tenta obter o contato do cache
            if (!_cache.TryGetValue(cacheKey, out Contato contato))
            {
                contato = await _context.Contatos.FindAsync(idNumerico);

                if (contato == null)
                {
                    return NotFound(new { error = $"Contato com o ID {idNumerico} não encontrado." });
                }

                _cache.Set(cacheKey, contato, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) 
                });
            }

            // Retorna o contato, seja do banco de dados ou do cache
            return Ok(contato);
        }




        // PUT: api/contatos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contato contato)
        {
            // Verificar se o ID do contato corresponde ao ID da URL
            if (id != contato.Id)
            {
                return BadRequest(new { error = "O ID do contato não corresponde ao ID da URL." });
            }

            // Validar o contato
            var validationErrors = ContatoValidator.Validate(contato);

            // Verifica se existem erros de validação
            if (validationErrors.Any())
            {
                return BadRequest(new { error = "Validações falharam.", details = validationErrors });
            }

            // Procurar o contato no banco de dados
            var contatoExistente = await _context.Contatos.FindAsync(id);
            if (contatoExistente == null)
            {
                return NotFound(new { error = $"Contato com o ID {id} não encontrado." });
            }

            // Atualizar o contato no banco de dados
            _context.Entry(contatoExistente).CurrentValues.SetValues(contato);
            await _context.SaveChangesAsync();

            // Retorno de sucesso
            return Ok(new { message = $"Contato com ID {id} atualizado com sucesso." });
        }

        // DELETE: api/contatos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // Validação manual do ID para garantir que seja um número inteiro válido
            if (!int.TryParse(id, out int idNumerico))
            {
                return BadRequest(new { error = "O ID fornecido deve ser um número válido." });
            }

            var contato = await _context.Contatos.FindAsync(idNumerico);

            if (contato == null)
            {
                return NotFound(new { error = $"Contato com o ID {idNumerico} não encontrado." });
            }

            _context.Contatos.Remove(contato);
            await _context.SaveChangesAsync();

            // Retorno de sucesso com mensagem confirmando a exclusão
            return Ok(new { message = $"Contato com ID {id} foi deletado com sucesso." });

        }
    }
}