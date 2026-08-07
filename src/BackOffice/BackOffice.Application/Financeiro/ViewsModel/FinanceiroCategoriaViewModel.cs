using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Psicologa.Domain.Financeiro.Entities.FinanceiroCategoria;

namespace Psicologa.Application.Financeiro.ViewsModel
{
    public class FinanceiroCategoriaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Tipo { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
    public class FinanceiroCategoriaConsultaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Tipo { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
