using Psicologa.Domain.Pessoa.Entities;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Paciente.Services
{
    public class PacienteService : ServiceBase<Entities.Paciente>, IServiceBase<Entities.Paciente>
    {
        public readonly Interfaces.Repositories.IPacienteRepository _repository;

        public PacienteService(Interfaces.Repositories.IPacienteRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<Psicologa.Domain.Paciente.Entities.Paciente> Consultar(string nome, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            if (string.IsNullOrEmpty(nome))
                nome = "";
            else
                nome = nome.Replace("%", "").Replace("_", "");

            return _repository.Consultar(nome, paginacao, tpPessoa);
        }

        public bool Salvar(Psicologa.Domain.Paciente.Entities.Paciente paciente)
        {
            bool operacao = false;
            if (paciente.Id == 0)
            {
                paciente.DataCriacao = DateTime.Now;
                paciente.DataAtualizacao = DateTime.Now;
            }
            else
            {
                paciente.DataAtualizacao = DateTime.Now;
            }
         
            operacao = _repository.Salvar(paciente);
            return operacao;
        }

        public Entities.Paciente Obter(int id)
        {
            return _repository.Obter(id);
        }
    }
}