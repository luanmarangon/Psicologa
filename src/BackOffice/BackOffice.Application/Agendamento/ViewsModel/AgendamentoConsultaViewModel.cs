using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Org.BouncyCastle.Asn1.X509;
using Shared.Infra.CrossCutting.JSONConverter;


namespace Psicologa.Application.Agendamento.ViewsModel
{

    public class AgendamentoViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int PsicologoId { get; set; }
        public string PsicologoNome { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int PacienteId { get; set; }
        public string PacienteNome { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int ServicoId { get; set; }
        public string ServicoNome { get; set; }
        [JsonConverter(typeof(DateTimeJSONConverter))]
        public DateTime DataConsulta { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        [JsonConverter(typeof(Int32JSONConverter))]
        public int TempoSessao { get; set; }
        public bool Online { get; set; }
        public bool Presencial { get; set; }
        [JsonConverter(typeof(Int32JSONConverter))]
        public int StatusAgendamento { get; set; }
        [JsonConverter(typeof(Int32JSONConverter))]

        public int TipoAgendamento { get; set; }
        public bool Ativo { get; set; }
        public bool ConfirmouAgendamento { get; set; }
        public DateTime? DataConfirmacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

    }

    public class AgendamentoConsultaViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int PsicologoId { get; set; }
        public string PsicologoNome { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int PacienteId { get; set; }
        public string PacienteNome { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int ServicoId { get; set; }
        public string ServicoNome { get; set; }
        [JsonConverter(typeof(DateTimeJSONConverter))]
        public DateTime DataConsulta { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public int TempoSessao { get; set; }
        public bool Online { get; set; }
        public bool Presencial { get; set; }
        public string StatusAgendamentoDescricao { get; set; }
        public int StatusAgendamento { get; set; }
        public string TipoAgendamentoDescricao { get; set; }
        public int TipoAgendamento { get; set; }
        public bool Ativo { get; set; }
        public bool ConfirmouAgendamento { get; set; }
        public DateTime? DataConfirmacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

    }

    public class AgendamentoDisponibilidadeViewModel
    {
        public int PsicologaId { get; set; }
        public DateTime DataConsulta { get; set; }
        public List<AgendamentoHorariosDisponiveisViewModel> HorariosDisponiveis { get; set; }
    }

    public class AgendamentoHorariosDisponiveisViewModel
    {
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
    }
}