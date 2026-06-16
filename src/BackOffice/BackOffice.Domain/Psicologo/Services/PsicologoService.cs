using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Psicologo.Services
{
    public class PsicologoService : ServiceBase<Entities.Psicologo>, IServiceBase<Entities.Psicologo>
    {
        private readonly Interfaces.Repositories.IPsicologoRepository _repository;

        public PsicologoService(Interfaces.Repositories.IPsicologoRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Entities.Psicologo psicologo)
        {
            if (psicologo.Id == 0)
            {
                psicologo.DataCriacao = DateTime.Now;
                psicologo.DataAtualizacao = DateTime.Now;
            }
            else
            {
                psicologo.DataAtualizacao = DateTime.Now;
            }
            return _repository.Salvar(psicologo);
        }

        public Entities.Psicologo Obter(int id)
        {
            return _repository.Obter(id);
        }

        public Entities.Psicologo ObterPorPessoaId(int pessoaId)
        {
            return _repository.ObterPorPessoaId(pessoaId);
        }
    }
}