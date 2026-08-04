using Psicologa.Application.Paciente.ViewsModel;
using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Psicologa.Application.Prontuario.ViewsModel
{
    public class ProntuarioViewModel
    {
            [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
            [JsonConverter(typeof(EncryptIdJSONConverter))]

        public int PacienteId { get; set; }
        public string PacienteNome { get; set; }
        public string PacienteMatricula { get; set; }
        public string QueixaPrincipal { get; set; }
        public string ObjetivoTratamento { get; set; }
        public string HistoricoFamiliar { get; set; }
        public string ObservacoesIniciais { get; set; }
        public bool Ativo { get; set; }
       // public DateTime DataCriacao { get; set; }
        //public DateTime DataAtualizacao { get; set; }
        public DateTime? DataEncerramento { get; set; }
    }

    public class ProntuarioConsultaViewModel
    {
            [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]

        public int PacienteId { get; set; }
        public string PacienteNome { get; set; }
        public string PacienteMatricula { get; set; }
        public string QueixaPrincipal { get; set; }
        public string ObjetivoTratamento { get; set; }
        public string HistoricoFamiliar { get; set; }
        public string ObservacoesIniciais { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataEncerramento { get; set; }



    }
}
