using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Documentos.Entities
{
    public class Documentos : EntityBase
    {
        public enum TpCategoria
        {

            [Description("Declaração")]
            Declaracao = 1,

            [Description("Atestado")]
            Atestado = 2,

            [Description("Relatório")]
            Relatorio = 3,

            [Description("Laudo")]
            Laudo = 4,

            [Description("Parecer")]
            Parecer = 5,

            [Description("Termo")]
            Termo = 6,

            [Description("Encaminhamento")]
            Encaminhamento = 7,

            [Description("Outro")]
            Outro = 8
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public TpCategoria Categoria { get; set; }
        public bool Ativo { get; set; }
        public string Conteudo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}