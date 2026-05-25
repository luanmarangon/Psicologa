using Psicologa.Application.BlogPost.ViewsModel;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Application.Consultar.Services
{
    public class ApplicationConsultarService : IDisposable
    {
        private readonly Domain.BlogPost.Services.BlogPostService _blogPostService;
        private readonly Domain.Servico.Services.ServicoService _servicoService;
        private readonly IAppSettings _appSettings;

        public ApplicationConsultarService(Domain.BlogPost.Services.BlogPostService blogPostService,
            Domain.Servico.Services.ServicoService servicoService, IAppSettings appSettings)
        {
            _blogPostService = blogPostService;
            _servicoService = servicoService;
            _appSettings = appSettings;
        }

        public IEnumerable<ConsultarViewModel> Consultar(string nome, PaginacaoDados paginacao)
        {
            List<ConsultarViewModel> retorno = new List<ConsultarViewModel>();

            var blogPosts = _blogPostService.Consultar(nome, paginacao);
            var servicos = _servicoService.Consultar(nome, Domain.Servico.Entities.Servico.TpFiltroServico.Indefinido, paginacao);

            foreach (var blogPost in blogPosts)
            {
                retorno.Add(FormatarRetornoConsulta(blogPost));
            }

            foreach (var servico in servicos)
            {
                retorno.Add(FormatarRetornoConsulta(servico));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.Titulo).ToList();
            }

            return retorno;
        }

        internal ConsultarViewModel FormatarRetornoConsulta(object dados)
        {
            if (dados is Domain.BlogPost.Entities.BlogPost blogPost)
            {
                return new ConsultarViewModel
                {
                    Id = blogPost.Id,
                    Titulo = blogPost.Titulo,
                    Resumo = blogPost.Resumo,
                    Url = "/Blog/Post/" + blogPost.Url,
                    ImagemCapa = MontarImagem(blogPost.ImagemCapa),
                    Tipo = "Blog",
                    DataPublicacao = blogPost.DataPublicacao
                };
            }

            if (dados is Domain.Servico.Entities.Servico servico)
            {
                return new ConsultarViewModel
                {
                    Id = servico.Id,
                    Titulo = servico.Nome,
                    Resumo = servico.DescricaoCurta,
                    Url = "/Servico/",
                    ImagemCapa = MontarImagem(servico.ImagemCapa),
                    Tipo = "Servico"
                };
            }

            return null;
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


        public void Dispose()
        {
        }
    }
}