using Crud_FIAP_Debora_Krikowski.Models;
using System.Collections.Generic;
using System.Linq;  

namespace Crud_FIAP_Debora_Krikowski.Validators
{
    public static class ContatoValidator
    {
        public static List<string> Validate(Contato contato)
        {
            var errors = new List<string>();

            // Valida nome
            if (string.IsNullOrWhiteSpace(contato.Nome))
            {
                errors.Add("O nome é obrigatório.");
            }
            else if (contato.Nome.Length > 100)
            {
                errors.Add("O nome não pode ter mais de 100 caracteres.");
            }

            //Valida e-mail
            if (string.IsNullOrWhiteSpace(contato.Email))
            {
                errors.Add("O e-mail é obrigatório.");
            }
            else if (!contato.Email.Contains("@"))
            {
                errors.Add("O e-mail informado é inválido.");
            }
            else if (contato.Email.Length > 255)
            {
                errors.Add("O e-mail não pode ter mais de 255 caracteres.");
            }

            //Valida DDD 
            if (string.IsNullOrWhiteSpace(contato.DDD) || contato.DDD.Length != 2)
            {
                errors.Add("O DDD está nulo, vazio ou possui tamanho diferente de 2 caracteres.");
            }
            else if (!int.TryParse(contato.DDD, out _))
            {
                errors.Add("O DDD precisa ser numérico.");
            }

            //valida telefone
            if (string.IsNullOrWhiteSpace(contato.Telefone) || contato.Telefone.Length < 10 || contato.Telefone.Length > 11)
            {
                errors.Add("O telefone deve ter entre 10 e 11 caracteres.");
            }

            // Se houver erros, retornar a lista de erros
            return errors;
        }
    }
}
