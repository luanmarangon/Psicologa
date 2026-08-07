using Psicologa.Application.Convenio.ViewsModel;
using Psicologa.Application.Financeiro.ViewsModel;
using Psicologa.Domain.Convenio.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Application.Financeiro.Services
{
    public class ApplicationFinanceiroCategoriaService : IDisposable
    {
        private readonly Domain.Financeiro.Services.FinanceiroCategoriaService _fcService;

        public ApplicationFinanceiroCategoriaService(Domain.Financeiro.Services.FinanceiroCategoriaService fcService)
        {
            _fcService = fcService;
        }


        public IEnumerable<FinanceiroCategoriaConsultaViewModel> ObterTodasCategoria(int tipo)
        {
            List<FinanceiroCategoriaConsultaViewModel> retorno = new List<FinanceiroCategoriaConsultaViewModel>();

            var categorias = _fcService.ObterTodasCategoria(tipo);

            foreach (var categoria in categorias)
            {
                retorno.Add(FormatarRetornoConsulta(categoria));
            }

            return retorno;
        }


        internal FinanceiroCategoriaConsultaViewModel FormatarRetornoConsulta(Domain.Financeiro.Entities.FinanceiroCategoria categoria)
        {
            if(categoria == null)
                return null;

            FinanceiroCategoriaConsultaViewModel retorno = new FinanceiroCategoriaConsultaViewModel
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Tipo = (int)categoria.Tipo,
                Ativo = categoria.Ativo,
            };

            return retorno;
        }

        public void Dispose()
        {
        }
}
}
