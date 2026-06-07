using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.ProntuarioAnexo.Entities
{
    public class ProntuarioAnexo : EntityBase
    {
        public enum tpTipoAnexo
        {
            [Description("Indefinido")]
            Indefinido = 0,

            [Description("Documento Pessoal")]
            DocumentoPessoal = 1,

            [Description("Convênio")]
            Convenio = 2,

            [Description("Contrato")]
            Contrato = 3,

            [Description("Termo de Consentimento")]
            TermoConsentimento = 4,

            [Description("Receita Médica")]
            ReceitaMedica = 5,

            [Description("Encaminhamento")]
            Encaminhamento = 6,

            [Description("Exame")]
            Exame = 7,

            [Description("Avaliação Psicológica")]
            AvaliacaoPsicologica = 8,

            [Description("Relatório")]
            Relatorio = 9,

            [Description("Declaração")]
            Declaracao = 10,

            [Description("Outros")]
            Outro = 99
        }

        public int Id { get; set; }
        public Domain.Prontuario.Entities.Prontuario Prontuario { get; set; }
        public tpTipoAnexo TipoAnexo { get; set; }
        public string Nome { get; set; }
        public string NomeArquivo { get; set; }
        public string MimeType { get; set; }
        public long TamanhoArquivo { get; set; }
        public byte[] Arquivo { get; set; }
        public string Observacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}