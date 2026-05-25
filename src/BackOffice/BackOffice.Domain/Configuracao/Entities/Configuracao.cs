using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Configuracao.Entities
{
    public class Configuracao : EntityBase
    {
        public string Nome { get; set; }
        public string CEP { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

        public string Whatsapp { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Linkedin { get; set; }

        public string Slogan { get; set; }


        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public override bool Validar()
        {
            //if (string.IsNullOrEmpty(Nome))
            //    base.ValidationResult.Add(Message.TypeMessage.InvalidField, "O nome é obrigatório.");
            //else if (Nome.Length > 200)
            //    base.ValidationResult.Add(Message.TypeMessage.InvalidField, "O nome deve conter no máximo 200 caracteres.");
         
            return base.ValidationResult.Count == 0;
        }


    }
}
