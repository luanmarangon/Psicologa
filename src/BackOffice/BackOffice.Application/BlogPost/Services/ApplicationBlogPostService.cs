using Psicologa.Application.BlogPost.ViewsModel;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Application.BlogPost.Services
{
    public class ApplicationBlogPostService : IDisposable
    {
        private readonly Domain.BlogPost.Services.BlogPostService _blogPostService;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly IAppSettings _appSettings;

        public ApplicationBlogPostService(Domain.BlogPost.Services.BlogPostService blogPostService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService, IAppSettings appSettings)
        {
            _blogPostService = blogPostService;
            _logAplicacaoService = logAplicacaoService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(BlogPostViewModel blogPost, string[] requisicao)
        {
            var dadosExistente = _blogPostService.Obter(blogPost.Id);

            bool operacao = false;
            Domain.BlogPost.Entities.BlogPost blog = new Domain.BlogPost.Entities.BlogPost();
            blog.Id = blogPost.Id;
            blog.Titulo = blogPost.Titulo;

            if (string.IsNullOrEmpty(blogPost.Url))
                blog.Url = MontarUrl(blogPost.Titulo);
            else
                blog.Url = blogPost.Url;

            blog.Conteudo = blogPost.Conteudo;

            blog.Resumo = ObterResumo(blogPost.Conteudo);

            //blog.ImagemCapa = blogPost.ImagemCapa != null
            //    ? Convert.FromBase64String(blogPost.ImagemCapa.Replace("data:image/jpeg;base64,", ""))
            //    : null;
            if (!string.IsNullOrEmpty(blogPost.ImagemCapa))
            {
                var base64 = blogPost.ImagemCapa;

                // Remove o prefixo data:image/...;base64,
                var commaIndex = base64.IndexOf(',');
                if (commaIndex >= 0)
                    base64 = base64.Substring(commaIndex + 1);

                blog.ImagemCapa = Convert.FromBase64String(base64);
            }
            else
            {
                blog.ImagemCapa = null;
            }

            blog.Autor = blogPost.Autor;
            blog.DataPublicacao = blogPost.DataPublicacao;
            blog.DataRevogacao = blogPost.DataRevogacao;
            blog.Ativo = blogPost.Ativo;
            blog.Pessoa = new Domain.Pessoa.Entities.Pessoa
            {
                Id = blogPost.PessoaId,
                Nome = blogPost.PessoaNome,
            };

            if (blog.Validar())
            {
                operacao = _blogPostService.Salvar(blog);
                if (operacao)
                {
                    blogPost.Id = blog.Id;
                }
            }

            if (operacao)
            {
                _logAplicacaoService.Registrar(blogPost.Id, requisicao, dadosExistente, blog, "BlogPost", "ApplicationBlogPostService", "Salvar");
            }

            return (operacao, blog.ValidationResult);
        }

        public IEnumerable<BlogPostConsultaViewModel> Consultar(string nome, PaginacaoDados paginacao)
        {
            List<BlogPostConsultaViewModel> retorno = new List<BlogPostConsultaViewModel>();

            var blogPosts = _blogPostService.Consultar(nome, paginacao);

            foreach (var blogPost in blogPosts)
            {
                retorno.Add(FormatarRetornoConsulta(blogPost));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.Titulo).ToList();
            }

            return retorno;
        }

        public IEnumerable<BlogPostConsultaViewModel> ConsultarUltimos(int quantidade)
        {
            List<BlogPostConsultaViewModel> retorno = new List<BlogPostConsultaViewModel>();
            var blogPosts = _blogPostService.ConsultarUltimos(quantidade);
            foreach (var blogPost in blogPosts)
            {
                retorno.Add(FormatarRetornoConsulta(blogPost));
            }
            return retorno;
        }

        public IEnumerable<BlogPostConsultaViewModel> ObterTodosPublicados()
        {
            List<BlogPostConsultaViewModel> retorno = new List<BlogPostConsultaViewModel>();
            var blogPosts = _blogPostService.ObterTodosPublicados();
            foreach (var blogPost in blogPosts)
            {
                retorno.Add(FormatarRetornoConsulta(blogPost));
            }
            return retorno;
        }

        public BlogPostConsultaViewModel Obter(int blogPostId)
        {
            BlogPostConsultaViewModel retorno = new BlogPostConsultaViewModel();
            retorno = FormatarRetornoConsulta(_blogPostService.Obter(blogPostId));
            return retorno;
        }

        public BlogPostConsultaViewModel ObterPorUrl(string blogUrl)
        {
            BlogPostConsultaViewModel retorno = new BlogPostConsultaViewModel();
            retorno = FormatarRetornoConsulta(_blogPostService.ObterPorUrl(blogUrl));
            return retorno;
        }

        internal BlogPostConsultaViewModel FormatarRetornoConsulta(Domain.BlogPost.Entities.BlogPost blogPost)
        {
            var blog = new BlogPostConsultaViewModel
            {
                Id = blogPost.Id,
                Titulo = blogPost.Titulo,
                Url = blogPost.Url,
                Conteudo = blogPost.Conteudo,
                Resumo = blogPost.Resumo,
                ImagemCapa = MontarImagem(blogPost.ImagemCapa),
                Autor = blogPost.Autor,
                DataPublicacao = blogPost.DataPublicacao ?? DateTime.MinValue,
                DataRevogacao = blogPost.DataRevogacao ?? DateTime.MinValue,
                Ativo = blogPost.Ativo,
                PessoaId = blogPost.Pessoa.Id,
                PessoaNome = blogPost.Pessoa.Nome
            };

            return blog;
        }

        private bool EhBase64(string s)
        {
            Span<byte> buffer = new Span<byte>(new byte[s.Length]);
            return Convert.TryFromBase64String(s, buffer, out _);
        }

        private string MontarImagem(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            var texto = System.Text.Encoding.UTF8.GetString(bytes);

            // 🔥 CASO 1: já é data:image
            if (texto.StartsWith("data:image"))
                return texto;

            // 🔥 CASO 2: é base64 puro (sem prefixo)
            if (EhBase64(texto))
                return $"data:image/jpeg;base64,{texto}";

            // 🔥 CASO 3: é imagem binária real
            var mime = ObterMimeType(bytes);
            return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
        }

        private string ObterMimeType(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
                return "image/jpeg";

            // JPEG
            if (bytes[0] == 0xFF && bytes[1] == 0xD8)
                return "image/jpeg";

            // PNG
            if (bytes[0] == 0x89 && bytes[1] == 0x50)
                return "image/png";

            // GIF
            if (bytes[0] == 0x47 && bytes[1] == 0x49)
                return "image/gif";

            // WEBP
            if (bytes[0] == 0x52 && bytes[1] == 0x49)
                return "image/webp";

            return "image/jpeg"; // fallback
        }

        private string MontarUrl(string titulo)
        {
            string url = titulo.ToLower().Trim();
            url = url.Replace(" ", "-");
            url = url.Replace(".", "");
            url = url.Replace(",", "");
            url = url.Replace(";", "");
            url = url.Replace(":", "");
            url = url.Replace("?", "");
            url = url.Replace("!", "");
            return url;
        }

        private string ObterResumo(string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
                return string.Empty;

            // Remove tags HTML
            string textoLimpo = Regex.Replace(conteudo, "<.*?>", string.Empty);

            // Remove espaços extras
            textoLimpo = textoLimpo.Trim();

            int maxLength = 100;

            if (textoLimpo.Length <= maxLength)
                return textoLimpo;

            return textoLimpo.Substring(0, maxLength) + "...";
        }

        public void Dispose()
        {
        }
    }
}