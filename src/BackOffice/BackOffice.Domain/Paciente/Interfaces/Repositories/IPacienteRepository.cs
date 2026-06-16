using Psicologa.Domain.Pessoa.Entities;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Paciente.Interfaces.Repositories
{
    public interface IPacienteRepository: IRepositoryBase<Entities.Paciente>
    {
        Entities.Paciente Obter(int id);
        Entities.Paciente ObterPorPessoaId(int pessoaId);
        bool Salvar(Entities.Paciente paciente);
        IEnumerable<Domain.Paciente.Entities.Paciente> Consultar(string nome, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido);

    }
}
