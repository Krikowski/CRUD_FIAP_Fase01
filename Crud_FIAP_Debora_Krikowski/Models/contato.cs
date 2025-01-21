using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Crud_FIAP_Debora_Krikowski.Models
{
    public class Contato
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;  // Inicializando com valor padrão
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string DDD { get; set; } = string.Empty;
    }


}
