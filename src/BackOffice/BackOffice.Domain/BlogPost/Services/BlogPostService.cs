using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.BlogPost.Services
{
    public class BlogPostService : ServiceBase<Entities.BlogPost>, IServiceBase<Entities.BlogPost>
    {
        public readonly Interfaces.Repositories.IBlogPostRepository _repository;

        public BlogPostService(Interfaces.Repositories.IBlogPostRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Psicologa.Domain.BlogPost.Entities.BlogPost blogPost)
        {
            bool operacao = false;
            if (blogPost.Id == 0)
            {
                blogPost.DataCriacao = DateTime.Now;
                blogPost.DataAtualizacao = DateTime.Now;
            }
            else
            {
                blogPost.DataAtualizacao = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(blogPost.Titulo))
            {
                blogPost.Titulo = blogPost.Titulo.ToUpper();
            }

            if (!string.IsNullOrEmpty(blogPost.Autor))
            {
                blogPost.Autor = blogPost.Autor.ToUpper();
            }

            if (blogPost.Pessoa != null)
            {
                if (!string.IsNullOrEmpty(blogPost.Pessoa.Nome))
                {
                    blogPost.Pessoa.Nome = blogPost.Pessoa.Nome.ToUpper();
                }
            }

            operacao = _repository.Salvar(blogPost);

            return operacao;
        }

        public IEnumerable<Domain.BlogPost.Entities.BlogPost> Consultar(string termo, PaginacaoDados paginacao)
        {
            if (string.IsNullOrEmpty(termo))
                termo = "";
            else
                termo = termo.Replace("%", "").Replace("_", "");

            return _repository.Consultar(termo, paginacao);
        }

        public IEnumerable<Domain.BlogPost.Entities.BlogPost> ConsultarUltimos(int quantidade)
        {
            return _repository.ConsultarUltimos(quantidade);
        }
        public IEnumerable<Domain.BlogPost.Entities.BlogPost> ObterTodosPublicados()
        {
            return _repository.ObterTodosPublicados();
        }

        public Entities.BlogPost Obter(int blogPostId)
        {
            return _repository.Obter(blogPostId);
        }
        public Entities.BlogPost ObterPorUrl(string blogUrl)
        {
            return _repository.ObterPorUrl(blogUrl);
        }
    }
}