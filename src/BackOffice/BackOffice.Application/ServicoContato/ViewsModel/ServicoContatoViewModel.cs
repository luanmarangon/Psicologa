using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Psicologa.Domain.ServicoContato.Entities.ServicoContato;

namespace Psicologa.Application.ServicoContato.ViewsModel
{
    public class ServicoContatoViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public int ServicoId { get; set; }
        public string Nome { get; set; }
        public string Contato { get; set; }
        public string Email { get; set; }
        public string Mensagem { get; set; }
        public TpStatusContato StatusContato { get; set; }
        public bool EntrouContato { get; set; }
        public DateTime? DataContato { get; set; }
        public DateTime? DataRetorno { get; set; }

        public string ObservacaoInterna { get; set; }
        public string Origem { get; set; }
        public bool VirouPaciente { get; set; }
        public TpPrioridade Prioridade { get; set; }
        public TpPreferenciaContato PreferenciaContato { get; set; }

        public string IP { get; set; }
        public string UserAgent { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class ServicoContatoConsultaViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public int ServicoId { get; set; }
        public string ServicoNome { get; set; }
        public string Nome { get; set; }
        public string Contato { get; set; }
        public string Email { get; set; }
        public string Mensagem { get; set; }
        public TpStatusContato StatusContato { get; set; }
        public bool EntrouContato { get; set; }
        public DateTime? DataContato { get; set; }
        public DateTime? DataRetorno { get; set; }

        public string ObservacaoInterna { get; set; }
        public string Origem { get; set; }
        public bool VirouPaciente { get; set; }
        public TpPrioridade Prioridade { get; set; }
        public TpPreferenciaContato PreferenciaContato { get; set; }

        public string IP { get; set; }
        public string UserAgent { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

}