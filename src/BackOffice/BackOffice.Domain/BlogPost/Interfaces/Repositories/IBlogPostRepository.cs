using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.BlogPost.Interfaces.Repositories
{
    public interface IBlogPostRepository: IRepositoryBase<Entities.BlogPost>
    {
        bool Salvar(Entities.BlogPost blogPost);
        IEnumerable<Domain.BlogPost.Entities.BlogPost> Consultar(string termo, PaginacaoDados paginacao);
        IEnumerable<Domain.BlogPost.Entities.BlogPost> ConsultarUltimos(int quantidade);
        IEnumerable<Domain.BlogPost.Entities.BlogPost> ObterTodosPublicados();
        Entities.BlogPost Obter(int blogPostId);
        Entities.BlogPost ObterPorUrl(string blogUrl);
    }
}
