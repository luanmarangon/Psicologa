using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Psicologa.Application.Servico.ViewsModel
{
    public class ServicoViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Url { get; set; }
        public string DescricaoCurta { get; set; }
        public string Descricao { get; set; }
        [JsonConverter(typeof(Int32JSONConverter))]
        public int TempoSessaoMinutos { get; set; }
        [JsonConverter(typeof(DecimalJSONConverter))]
        public decimal ValorSessao { get; set; }
        public string ImagemCapa { get; set; }
        public bool Online { get; set; }
        public bool Presencial { get; set; }
        public bool DestaqueHome { get; set; }
        [JsonConverter(typeof(Int32JSONConverter))]
        public int OrdemExibicao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}