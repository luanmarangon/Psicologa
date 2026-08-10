using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Org.BouncyCastle.Asn1.X509;
using Shared.Infra.CrossCutting.JSONConverter;

namespace Psicologa.Application.ProntuarioSessao.ViewsModel
{
    public class ProntuarioSessaoViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int ProntuarioId { get; set; }
        //[JsonConverter(typeof(EncryptIdJSONConverter))]
        public int? AgendamentoId { get; set; }
        public DateTime DataSessao { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        //[JsonConverter(typeof(EncryptIdJSONConverter))]
        public int PsicologaId { get; set; }
        public int TipoAtendimento { get; set; }
        public string Evolucao { get; set; }
    }
    public class ProntuarioConsultaSessaoViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        //[JsonConverter(typeof(EncryptIdJSONConverter))]
        public int ProntuarioId { get; set; }
        //[JsonConverter(typeof(EncryptIdJSONConverter))]
        public int? AgendamentoId { get; set; }
        public DateTime DataSessao { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public int PsicologaId { get; set; }
        public string PsicologaNome { get; set; }
        [JsonConverter(typeof(Int32JSONConverter))]
        public int TipoAtendimento { get; set; }
        public string TipoAtendimentoNome { get; set; }
        public string Evolucao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
