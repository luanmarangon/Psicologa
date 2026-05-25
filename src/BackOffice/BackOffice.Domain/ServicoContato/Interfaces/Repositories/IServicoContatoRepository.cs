using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.ServicoContato.Interfaces.Repositories
{
    public interface IServicoContatoRepository: IRepositoryBase<Entities.ServicoContato>
    {
        bool Salvar(Entities.ServicoContato servicoContato);
        Entities.ServicoContato Obter(int servicoContatoId);
        //IEnumerable<Entities.ServicoContato> Obter(string[] filtro);
        //IEnumerable<Domain.Servico.Entities.ServicoContato> ObterTodos();
        //bool Excluir (int servicoContatoId);
        //IEnumerable<Domain.Servico.Entities.ServicoContato> ObterDestaquesHome(int limite);
        IEnumerable<Domain.ServicoContato.Entities.ServicoContato> Consultar(string termo, PaginacaoDados paginacao);
        //Entities.ServicoContato ObterPorUrl(string url);
        //Entities.ServicoContato ObterPorNome(string nome);
    }
}
