using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.ServicoContato.Entities
{
    public class ServicoContato : EntityBase
    {
        public enum TpStatusContato
        {
            [Description("Novo")]
            Novo = 0,
            [Description("Em Atendimento")]
            EmAtendimento = 1,
            [Description("Respondido")]
            Respondido = 2,
            [Description("Finalizado")]
            Finalizado = 3,
            [Description("Não Retornou")]
            NaoRetornou = 4
        }

        public enum TpPreferenciaContato
        {
            [Description("Indefinido")]
            Indefinido = 0,
            [Description("WhatsApp")]
            WhatsApp = 1,
            [Description("Ligação")]
            Ligacao = 2,
            [Description("Email")]
            Email = 3
        }

        public enum TpPrioridade
        {
            [Description("Normal")]
            Normal = 0,
            [Description("Urgente")]
            Urgente = 1,
        }

        public int Id { get; set; }
        public Domain.Servico.Entities.Servico Servico { get; set; }
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


        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
