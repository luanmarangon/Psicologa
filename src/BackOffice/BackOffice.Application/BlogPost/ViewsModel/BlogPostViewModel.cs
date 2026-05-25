using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Psicologa.Application.BlogPost.ViewsModel
{
    public class BlogPostViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Url { get; set; }
        public string Conteudo { get; set; }
        public string Resumo { get; set; }
        public string ImagemCapa { get; set; }
        public string? Autor { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public DateTime? DataRevogacao { get; set; }

        public bool Ativo { get; set; }

        public int PessoaId { get; set; }
        public string PessoaNome { get; set; }
    }

    public class BlogPostConsultaViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Url { get; set; }
        public string Conteudo { get; set; }
        public string Resumo { get; set; }
        public string ImagemCapa { get; set; }
        public string Autor { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime DataPublicacao { get; set; }
        public DateTime DataRevogacao { get; set; }

        public bool Ativo { get; set; }

        public int PessoaId { get; set; }
        public string PessoaNome { get; set; }
    }
}
