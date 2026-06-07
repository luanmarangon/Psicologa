using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Application.LogAplicacao.ViewsModel
{
    public class LogAplicacaoViewModel
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
    }



}