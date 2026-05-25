using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Agendamento.Entities
{
    public class Agendamento : EntityBase
    {

        public enum TpStatusAgendamento
        {
            [Description("Indefinido")]
            Indefinido = 0,
            [Description("Agendado")]
            Agendado = 1,
            [Description("Cancelado")]
            Cancelado = 2,
            [Description("Realizado")]
            Realizado = 3,
            [Description("Faltou")]
            Faltou = 4,
            [Description("Remarcado")]
            Remarcado = 5
        }

        public enum TpTipoAgendamento
        {
            [Description("Indefinido")]
            Indefinido = 0,
            [Description("Consulta")]
            Consulta = 1,
            [Description("Retorno")]
            Retorno = 2,
            [Description("Avaliação")]
            Avaliacao = 3
        }

        public enum tpFiltro
        {
            [Description("Indefinido")]
            Indefinido = 0,
            [Description("Presencial")]
            Presencial = 1,
            [Description("Online")]
            Online = 2,
        }


        public int Id { get; set; }
        public Domain.Pessoa.Entities.Pessoa Psicologo { get; set; }
        public Domain.Pessoa.Entities.Pessoa  Paciente { get; set; }
        public Domain.Servico.Entities.Servico Servico { get; set; }
        public DateTime DataConsulta { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public int TempoSessao { get; set; }
        public bool Online { get; set; }
        public bool Presencial { get; set; }
        public TpStatusAgendamento StatusAgendamento { get; set; }
        public TpTipoAgendamento TipoAgendamento { get; set; }
        public bool Ativo { get; set; }
        public bool ConfirmouAgendamento { get; set; }
        public DateTime? DataConfirmacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}