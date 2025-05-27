using CadastroService.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CadastroService.Services {

    public class PersistenciaServiceClient : IPersistenciaServiceClient {
        private readonly IHttpClientFactory _httpClientFactory;

        public PersistenciaServiceClient(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> EnviarContatoAsync(ContatoDto contato) {
            var client = _httpClientFactory.CreateClient("PersistenciaService");
            var response = await client.PostAsJsonAsync("/api/Contatos", contato);
            return response.IsSuccessStatusCode;
        }

        public async Task<ContatoDto?> ObterContatoAsync(int id) {
            var client = _httpClientFactory.CreateClient("PersistenciaService");
            var response = await client.GetAsync($"/api/Contatos/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ContatoDto>();
        }

        public async Task<List<ContatoDto>> ListarContatosAsync() {
            var client = _httpClientFactory.CreateClient("PersistenciaService");
            var response = await client.GetAsync("/api/Contatos");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<ContatoDto>>();
        }

        public async Task<bool> AtualizarContatoAsync(int id, ContatoDto contato) {
            var client = _httpClientFactory.CreateClient("PersistenciaService");
            var response = await client.PutAsJsonAsync($"/api/Contatos/{id}", contato);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletarContatoAsync(int id) {
            var client = _httpClientFactory.CreateClient("PersistenciaService");
            var response = await client.DeleteAsync($"/api/Contatos/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
