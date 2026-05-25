using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;

namespace Psicologa.Domain.Convenio.Services
{
    public class ConvenioService : ServiceBase<Entities.Convenio>, IServiceBase<Entities.Convenio>
    {
        public readonly Interfaces.Repositories.IConvenioRepository _repository;

        public ConvenioService(Interfaces.Repositories.IConvenioRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Psicologa.Domain.Convenio.Entities.Convenio convenio)
        {
            bool operacao = false;
            if (convenio.Id == 0)
            {
                convenio.DataCriacao = DateTime.Now;
                convenio.DataAtualizacao = DateTime.Now;
            }
            else
            {
                convenio.DataAtualizacao = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(convenio.Nome))
            {
                convenio.Nome = convenio.Nome.ToUpper();
            }

            operacao = _repository.Salvar(convenio);

            return operacao;
        }

        public IEnumerable<Domain.Convenio.Entities.Convenio> Consultar(string termo, PaginacaoDados paginacao)
        {
            if (string.IsNullOrEmpty(termo))
                termo = "";
            else
                termo = termo.Replace("%", "").Replace("_", "");

            return _repository.Consultar(termo, paginacao);
        }

        public IEnumerable<Domain.Convenio.Entities.Convenio> ConsultarUltimos(int quantidade)
        {
            return _repository.ConsultarUltimos(quantidade);
        }
        public IEnumerable<Domain.Convenio.Entities.Convenio> ObterDestaquesHome()
        {
            return _repository.ObterDestaquesHome();
        }

        public Entities.Convenio Obter(int convenioId)
        {
            return _repository.Obter(convenioId);
        }

        public bool Excluir(int convenioId)
        {
            return _repository.Excluir(convenioId);
        }
    }
}