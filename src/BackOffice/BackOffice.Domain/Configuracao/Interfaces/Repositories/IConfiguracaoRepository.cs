using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Configuracao.Interfaces.Repositories
{
    public interface IConfiguracaoRepository : IRepositoryBase<Entities.Configuracao>
    {
        bool Salvar(Entities.Configuracao config);
        Entities.Configuracao ObterConfiguracao(int id);
        Entities.Configuracao ObterConfiguracao();


        bool SalvarFuncionamento(Entities.ConfiguracaoFuncionamento config);
        Entities.ConfiguracaoFuncionamento ObterFuncionamento(int id);
        Domain.Configuracao.Entities.ConfiguracaoFuncionamento ObterFuncionamento();    

    }
}
