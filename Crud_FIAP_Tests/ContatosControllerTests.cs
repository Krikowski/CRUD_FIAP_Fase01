using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Crud_FIAP_Debora_Krikowski.Controllers;
using Crud_FIAP_Debora_Krikowski.Data;
using Crud_FIAP_Debora_Krikowski.Models;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using Crud_FIAP_Debora_Krikowski.Services;


namespace Crud_FIAP_Tests.Tests {
    public class ContatosControllerTests {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ContatosController _controller;

        public ContatosControllerTests() {
            // Cria a instância do ApplicationDbContext com um banco de dados em memória
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Banco único por teste
                .Options;

            _context = new ApplicationDbContext(options);

            // Inicializar com dados de teste se necessário
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Criar a instância do IMemoryCache
            _cache = new MemoryCache(new MemoryCacheOptions());

            // Passar tanto o _context quanto o _cache para o controlador
            _controller = new ContatosController(_context, _cache);
        }

        //Validação POST - Dados iniciais
        [Fact, Trait("Category", "Unit")] //teste de unidade independente - sem parâmetros
        public async Task Create_ValidContato_ReturnsCreatedAtActionResult_Single() {
            // Arrange
            // Criação da instância do controller com o _context e _cache
            var controller = new ContatosController(_context, _cache);

            //criação do objeto válido
            var contato = new Contato {
                Nome = "Novo Contato",
                Email = "novocontato@example.com",
                Telefone = "11987654321",
                DDD = "11"
            };

            // Chama o método create
            var result = await controller.Create(contato);

            // Valida se realmente aconteceu o CreatedAtActionResult
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdAtActionResult.Value);


            //Valida retorno igual ao criado
            var returnedContato = Assert.IsType<Contato>(createdAtActionResult.Value);
            Assert.Equal(contato.Nome, returnedContato.Nome);
            Assert.Equal(contato.Email, returnedContato.Email);
            Assert.Equal(contato.Telefone, returnedContato.Telefone);
            Assert.Equal(contato.DDD, returnedContato.DDD);
        }

        //Validação POST - Dados duplicados
        [Fact, Trait("Category", "Unit")]
        public async Task Create_ContatoComEmailExistente_ReturnsBadRequest() {
            // Adiciona contato 'limpo'
            var contatoExistente = new Contato {
                Nome = "Contato Existente",
                Email = "testuser@example.com",
                Telefone = "11987654321",
                DDD = "11"
            };

            // Adiciona o contato existente no banco de dados
            _context.Contatos.Add(contatoExistente);
            await _context.SaveChangesAsync();

            //e-mail duplicado
            var novoContato = new Contato {
                Nome = "Novo Contato",
                Email = "testuser@example.com",
                Telefone = "11912345678",
                DDD = "11"
            };

            // Cria o controller com o _context e _cache
            var controller = new ContatosController(_context, _cache);

            var result = await controller.Create(novoContato);

            Assert.IsType<BadRequestObjectResult>(result);
        }


        // Teste PUT - Alterando Nome Inválido
        [Fact, Trait("Category", "Unit")]
        public async Task Update_ValidContato_ReturnsBadRequest() {
            // Arrange
            var controller = new ContatosController(_context, _cache);
            var contatoExistente = new Contato {
                Nome = "Contato Atualizado",
                Email = "updated@example.com",
                Telefone = "11987654321",
                DDD = "11"
            };

            // Adiciona o contato ao banco de dados
            _context.Contatos.Add(contatoExistente);
            await _context.SaveChangesAsync();

            // Simula um erro de validação, alterando o nome para vazio
            contatoExistente.Nome = "";

            var result = await controller.Update(contatoExistente.Id, contatoExistente);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            // Verifica se o valor retornado não é nulo
            Assert.NotNull(badRequestResult.Value);

            // Acessando a resposta e verificando a chave 'error'
            var response = badRequestResult.Value as IDictionary<string, object>;

        }

        // Teste PUT - Alterando um ID que não existe
        [Fact, Trait("Category", "Unit")]
        public async Task Update_InvalidContatoId_ReturnsNotFound() {

            var controller = new ContatosController(_context, _cache);
            var contatoExistente = new Contato {
                Nome = "Contato para Atualizar",
                Email = "toUpdate@example.com",
                Telefone = "11987654321",
                DDD = "11"
            };

            _context.Contatos.Add(contatoExistente);
            await _context.SaveChangesAsync();

            var contatoAlterado = new Contato {
                Id = 999, // ID inválido
                Nome = "Nome Alterado",
                Email = "invalid@example.com",
                Telefone = "11987654321",
                DDD = "11"
            };

            // Act
            var result = await controller.Update(contatoAlterado.Id, contatoAlterado);

            // Verifica se o objeto não foi encontrado
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // Teste DELETE - deletando com sucesso um contato
        [Fact, Trait("Category", "Unit")]
        public async Task Delete_ValidId_ReturnsOkResult() {

            var contatoExistente = new Contato {
                Nome = "Contato para Deletar",
                Email = "todelete@example.com",
                Telefone = "11987654321",
                DDD = "11"
            };

            _context.Contatos.Add(contatoExistente);
            await _context.SaveChangesAsync();

            var controller = new ContatosController(_context, _cache);

            var result = await controller.Delete(contatoExistente.Id.ToString());

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            // Deserializando a resposta para um JObject
            var jsonResponse = JObject.FromObject(okResult.Value);

            // Acessando a chave 'message' diretamente
            var message = jsonResponse["message"].ToString();

            // Verificando se a mensagem está correta
            Assert.Equal($"Contato com ID {contatoExistente.Id} foi deletado com sucesso.", message);
        }

        // TESTE DELETE - ID inválido
        [Fact, Trait("Category", "Integration")]
        public async Task Delete_InvalidId_ReturnsBadRequest() {
            // Arrange
            var controller = new ContatosController(_context, _cache);

            // Act
            var result = await controller.Delete("abc"); // ID inválido

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        //Teste GET - Todos os contatos
        [Fact, Trait("Category", "Unit")]
        public async Task GetAll_ValidDDD_ReturnsOkResult() {
            var controller = new ContatosController(_context, _cache);

            // Adiciona contatos de teste
            _context.Contatos.Add(new Contato { Nome = "Contato 1", DDD = "11", Email = "contato1@example.com", Telefone = "11987654321" });
            _context.Contatos.Add(new Contato { Nome = "Contato 2", DDD = "11", Email = "contato2@example.com", Telefone = "11912345678" });
            await _context.SaveChangesAsync();

            var result = await controller.GetAll("11"); //filtra DDD

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var contatos = okResult.Value as List<Contato>;
            Assert.NotNull(contatos);

            Assert.True(contatos.Count > 0);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Service_CreateContato_SavesInDatabase() {
            // Arrange
            var service = new ContatoService(_context);
            var contato = new Contato {
                Nome = "Teste Serviço",
                Email = "servico@example.com",
                Telefone = "11987654321",
                DDD = "11"
            };


            // Act
            await service.CreateContatoAsync(contato);

            // Assert
            var contatoSalvo = await _context.Contatos.FirstOrDefaultAsync(c => c.Email == "servico@example.com");
            Assert.NotNull(contatoSalvo);
        }


    }
}
