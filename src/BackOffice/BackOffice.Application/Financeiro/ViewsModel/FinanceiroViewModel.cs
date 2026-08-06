using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Psicologa.Application.Financeiro.ViewsModel
{
    public class FinanceiroViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        [JsonConverter(typeof(Int32JSONConverter))]
        public int Tipo { get; set; } // TpTipoLancamento: 1=Despesa, 2=Receita
        public string Descricao { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; }
        public decimal Valor { get; set; }
        [JsonConverter(typeof(DateTimeJSONConverter))]
        public DateTime DataLancamento { get; set; }
        public string? Observacao { get; set; }
        public bool Ativo { get; set; }
        public bool Quitado { get; set; }
        [JsonConverter(typeof(DateTimeJSONConverter))]
        public DateTime? DataQuitacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class FinanceiroConsultaViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public int Tipo { get; set; }
        public string TipoDescricao { get; set; }
        public string Descricao { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataLancamento { get; set; }
        public string Observacao { get; set; }
        public bool Ativo { get; set; }
        public bool Quitado { get; set; }
        public DateTime DataQuitacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
