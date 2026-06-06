using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Psicologa.Application.Paciente.ViewsModel
{
    public class PacienteViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public string PessoaNome { get; set; }
        public DateTime DataPrimeiraSessao { get; set; }
        public bool Ativo { get; set; }
        public string? Observacoes { get; set; }
        public string? ContatoEmergenciaNome { get; set; }
        public string? ContatoEmergenciaTelefone { get; set; }
        public int? ResponsavelId { get; set; }
        public string? ResponsavelNome { get; set; }
        public string Matricula { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class PacienteConsultaViewModel
    {

        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public string PessoaNome { get; set; }
        public DateTime? DataPrimeiraSessao { get; set; }
        public bool Ativo { get; set; }
        public string? Observacoes { get; set; }
        public string? ContatoEmergenciaNome { get; set; }
        public string? ContatoEmergenciaTelefone { get; set; }
        public int? ResponsavelId { get; set; }
        public string? ResponsavelNome { get; set; }
        public string Matricula { get; set; } 
        public int ProntuarioId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }


}
