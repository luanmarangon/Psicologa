using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;

namespace Psicologa.Domain.ProntuarioSessao.Services
{
    public class ProntuarioSessaoService : ServiceBase<Entities.ProntuarioSessao>, IServiceBase<Entities.ProntuarioSessao>
    {
        private readonly Interfaces.Repositories.IProntuarioSessaoRepository _repository;

        public ProntuarioSessaoService(Interfaces.Repositories.IProntuarioSessaoRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public bool EvoluirSessao(Entities.ProntuarioSessao sessao)
        {
            if (sessao.Id == 0)
            {
                sessao.DataCriacao = DateTime.Now;
                sessao.DataAtualizacao = DateTime.Now;
            }
            else
            {
                sessao.DataAtualizacao = DateTime.Now;
            }

            return _repository.EvoluirSessao(sessao);
        }

        public IEnumerable<Domain.ProntuarioSessao.Entities.ProntuarioSessao> Consultar(string termo, int protocoloId, int filtroTipoAtendimento, PaginacaoDados paginacao)
        {
            if (string.IsNullOrEmpty(termo))
                termo = "";
            else
                termo = termo.Replace("%", "").Replace("_", "");

            return _repository.Consultar(termo, protocoloId, filtroTipoAtendimento, paginacao);
        }

        public Entities.ProntuarioSessao ObterSessao(int id)
        {
            return _repository.ObterSessao(id);
        }

        public bool ExcluirSessao(int id)
        {
            return _repository.ExcluirSessao(id);
        }
    }
}