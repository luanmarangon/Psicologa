using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.ServicoContato.Services
{
    public class ServicoContatoService : ServiceBase<Entities.ServicoContato>, IServiceBase<Entities.ServicoContato>
    {
        public readonly Interfaces.Repositories.IServicoContatoRepository _repository;
        public ServicoContatoService(Interfaces.Repositories.IServicoContatoRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Psicologa.Domain.ServicoContato.Entities.ServicoContato servico)
        {
            bool operacao = false;
            if (servico.Id == 0)
            {
                servico.DataCriacao = DateTime.Now;
                servico.DataAtualizacao = DateTime.Now;
            }
            else
            {
                servico.DataAtualizacao = DateTime.Now;
            }
           

            operacao = _repository.Salvar(servico);

            return operacao;
        }
        
        public IEnumerable<Domain.ServicoContato.Entities.ServicoContato> Consultar(string termo, PaginacaoDados paginacao)
        {
            if (string.IsNullOrEmpty(termo))
                termo = "";
            else
                termo = termo.Replace("%", "").Replace("_", "");

            return _repository.Consultar(termo, paginacao);
        }
        
        //public IEnumerable<Domain.Servico.Entities.Servico> ObterTodos()
        //{
        //    return _repository.ObterTodos();
        //}
        public Entities.ServicoContato Obter(int servicoId)
        {
            return _repository.Obter(servicoId);
        }
        //public IEnumerable<Entities.Servico> Obter(string[] filtro)
        //{
        //    return _repository.Obter(filtro);
        //}
        //public bool Excluir(int servicoId)
        //{
        //    return _repository.Excluir(servicoId);
        //}
        //public IEnumerable<Domain.Servico.Entities.Servico> ObterDestaquesHome(int limite)
        //{
        //    return _repository.ObterDestaquesHome(limite);
        //}
        //public Domain.Servico.Entities.Servico ObterPorUrl(string url)
        //{
        //    return _repository.ObterPorUrl(url);
        //}
        //public Domain.Servico.Entities.Servico ObterPorNome(string nome)
        //{
        //    return _repository.ObterPorNome(nome);
        //}
    }
}