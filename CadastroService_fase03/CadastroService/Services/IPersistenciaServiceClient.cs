using CadastroService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CadastroService.Services {
    public interface IPersistenciaServiceClient {
        Task<bool> EnviarContatoAsync(ContatoDto contato);
        Task<ContatoDto?> ObterContatoAsync(int id);
        Task<List<ContatoDto>> ListarContatosAsync();
        Task<bool> AtualizarContatoAsync(int id, ContatoDto contato);
        Task<bool> DeletarContatoAsync(int id);
    }
}
