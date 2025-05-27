using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CadastroService.Models;
using CadastroService.Services;
using Moq;
using Moq.Protected;
using Xunit;

public class PersistenciaServiceClientTests {
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

    public PersistenciaServiceClientTests() {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
    }

    private HttpClient CreateHttpClient(HttpResponseMessage response) {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        return new HttpClient(handlerMock.Object) {
            BaseAddress = new Uri("http://localhost")
        };
    }

    [Fact]
    public async Task EnviarContatoAsync_DeveRetornarTrue_QuandoSucesso() {
        var contato = new ContatoDto { Id = 1, Nome = "Teste", Email = "teste@teste.com", Telefone = "123456", DDD = "11" };

        var response = new HttpResponseMessage(HttpStatusCode.Created);
        var httpClient = CreateHttpClient(response);

        _httpClientFactoryMock.Setup(f => f.CreateClient("PersistenciaService"))
                              .Returns(httpClient);

        var service = new PersistenciaServiceClient(_httpClientFactoryMock.Object);

        var result = await service.EnviarContatoAsync(contato);

        Assert.True(result);
    }

    [Fact]
    public async Task ObterContatoAsync_DeveRetornarContato_QuandoEncontrado() {
        var contato = new ContatoDto { Id = 1, Nome = "Teste", Email = "teste@teste.com", Telefone = "123456", DDD = "11" };

        var response = new HttpResponseMessage(HttpStatusCode.OK) {
            Content = JsonContent.Create(contato)
        };
        var httpClient = CreateHttpClient(response);

        _httpClientFactoryMock.Setup(f => f.CreateClient("PersistenciaService"))
                              .Returns(httpClient);

        var service = new PersistenciaServiceClient(_httpClientFactoryMock.Object);

        var result = await service.ObterContatoAsync(1);

        Assert.NotNull(result);
        Assert.Equal(contato.Id, result.Id);
    }

    [Fact]
    public async Task ListarContatosAsync_DeveRetornarListaDeContatos() {
        var contatos = new List<ContatoDto> {
            new ContatoDto { Id = 1, Nome = "Teste1", Email = "t1@teste.com", Telefone = "111111", DDD = "11" },
            new ContatoDto { Id = 2, Nome = "Teste2", Email = "t2@teste.com", Telefone = "222222", DDD = "22" }
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK) {
            Content = JsonContent.Create(contatos)
        };
        var httpClient = CreateHttpClient(response);

        _httpClientFactoryMock.Setup(f => f.CreateClient("PersistenciaService"))
                              .Returns(httpClient);

        var service = new PersistenciaServiceClient(_httpClientFactoryMock.Object);

        var result = await service.ListarContatosAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task AtualizarContatoAsync_DeveRetornarTrue_QuandoSucesso() {
        var contato = new ContatoDto { Id = 1, Nome = "Atualizado", Email = "atualizado@teste.com", Telefone = "123456", DDD = "11" };

        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        var httpClient = CreateHttpClient(response);

        _httpClientFactoryMock.Setup(f => f.CreateClient("PersistenciaService"))
                              .Returns(httpClient);

        var service = new PersistenciaServiceClient(_httpClientFactoryMock.Object);

        var result = await service.AtualizarContatoAsync(1, contato);

        Assert.True(result);
    }

    [Fact]
    public async Task DeletarContatoAsync_DeveRetornarTrue_QuandoSucesso() {
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        var httpClient = CreateHttpClient(response);

        _httpClientFactoryMock.Setup(f => f.CreateClient("PersistenciaService"))
                              .Returns(httpClient);

        var service = new PersistenciaServiceClient(_httpClientFactoryMock.Object);

        var result = await service.DeletarContatoAsync(1);

        Assert.True(result);
    }
}
