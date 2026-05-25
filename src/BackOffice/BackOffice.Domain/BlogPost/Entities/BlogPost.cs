using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.BlogPost.Entities
{
    public class BlogPost : EntityBase
    {
        public string Titulo { get; set; }
        public string Url { get; set; }
        public string Conteudo { get; set; }
        public string Resumo { get; set; }
        public byte[] ImagemCapa { get; set; }
        public string? Autor { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public DateTime? DataRevogacao { get; set; }

        public bool Ativo { get; set; }
        public Domain.Pessoa.Entities.Pessoa Pessoa { get; set; }

        public override bool Validar()
        {

            if(string.IsNullOrEmpty(Titulo))
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "O título é obrigatório.");
            else if (Titulo.Length > 200)
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "O título deve conter no máximo 200 caracteres.");
            else if(ImagemCapa == null)
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "A imagem de capa é obrigatória.");
            else if (Conteudo.Length > 50000)
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "O conteúdo deve conter no máximo 50000 caracteres.");
            else if(DataRevogacao <= DateTime.Now && DataRevogacao != DateTime.MinValue)
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "A data de revogacao não pode ser menor que a data atual.");
            else if(string.IsNullOrEmpty(Autor))
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "O autor é obrigatório.");

            return base.ValidationResult.Count == 0;
        }
    }
}
