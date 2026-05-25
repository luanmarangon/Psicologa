using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Servico.Entities
{
    public class Servico : EntityBase
    {
        public enum TpFiltroServico
        {
            [Description("Indefinido")]
            Indefinido = 0,

            [Description("DestaquesHome")]
            DestaquesHome = 1,
            [Description("Presencial")]
            Presencial = 2,
            [Description("Online")]
            Online = 3,
        }


        public int Id { get; set; }
        public string Nome { get; set; }
        public string Url { get; set; }
        public string DescricaoCurta { get; set; }
        public string Descricao { get; set; }
        public int TempoSessaoMinutos { get; set; }
        public decimal ValorSessao { get; set; }
        public byte[] ImagemCapa { get; set; }
        public bool Online { get; set; }
        public bool Presencial { get; set; }
        public bool DestaqueHome { get; set; }
        public int OrdemExibicao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
