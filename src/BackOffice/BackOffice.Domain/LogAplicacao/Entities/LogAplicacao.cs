using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Psicologa.Domain.LogAplicacao.Entities
{
    public class LogAplicacao: EntityBase
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public string Dispositivo { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }
        public string Entidade { get; set; }
        public int EntidadeId { get; set; }
        public string Operacao { get; set; }
        public string Aplicacao { get; set; }
        public string Metodo { get; set; }
        public string DadosAntes { get; set; }
        public string DadosDepois { get; set; }
        public string DadosAlterados { get; set; }
        //public bool Sucesso { get; set; }
        //public string MensagemErro { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
