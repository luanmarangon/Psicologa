using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Psicologa.Application.BlogPost.Services;
using Psicologa.Application.BlogPost.ViewsModel;
using Psicologa.Application.Pessoa.ViewsModel;
using Psicologa.Application.Usuario.ViewsModel;
using Psicologa.Domain.Pessoa.Services;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.AcessarConteudoBlog)]
    [Area("Administrativo")]

    public class BlogPostController: BaseController
    {
 
        ApplicationBlogPostService _blogPostService;
        UsuarioAutenticado _ua;
        RequisicaoAtual _req;   

        public BlogPostController(ApplicationBlogPostService blogPostService, UsuarioAutenticado ua, RequisicaoAtual req)
        {
            _blogPostService = blogPostService;
            _ua = ua;
            _req = req;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, int pagina = 0, int ordenacao = 1)
        {

            IEnumerable<object> blogs = new List<object>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 12, (PaginacaoDados.TpOrdenacao)ordenacao); //12


            blogs = _blogPostService.Consultar(q, paginacao);

            var retorno = new
            {
                blogs,
                paginacao
            };

            return DefaultJSONResponse(true, retorno);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            BlogPostConsultaViewModel blogPost = _blogPostService.Obter(idLimpo);
            return DefaultJSONResponse(blogPost != null, blogPost);
        }

        public IActionResult NovoPost()
        {
            return View("Areas/Administrativo/Views/BlogPost/NovoPost.cshtml");
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);
            
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            BlogPostViewModel blogVM = null;
            try
            {
                blogVM = dados.Deserialize<BlogPostViewModel>();
                blogVM.PessoaId = _ua.PessoaId;
                blogVM.PessoaNome = _ua.Nome;

                (operacao, vr) = _blogPostService.Salvar(blogVM, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }

            object ucVM = null;
            if (operacao)
            {
                ucVM = _blogPostService.Obter(blogVM.Id);
                ucVM = new
                {
                    id = blogVM.Id,
                    titulo = blogVM.Titulo
                };
                AddUserMessageSuccess("Blog salvo com sucesso.");
            }

            return DefaultJSONResponse(operacao, ucVM);
        }

       
    

    }
}
