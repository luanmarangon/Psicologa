using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Text.Json.Serialization;

namespace Psicologa.Application.Prontuario.ViewsModel
{
    public class DocumentosViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public string Nome { get; set; }
        [JsonConverter(typeof(Int32JSONConverter))]
        public int Categoria { get; set; }
        public bool Ativo { get; set; }
        public string Conteudo {get; set;}
    }
    public class DocumentosConsultaViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public string Nome { get; set; }
        [JsonConverter(typeof(Int32JSONConverter))]
        public int Categoria { get; set; }
        public string CategoriaNome { get; set; }
        public bool Ativo { get; set; }
        public string Conteudo {get; set;}
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
