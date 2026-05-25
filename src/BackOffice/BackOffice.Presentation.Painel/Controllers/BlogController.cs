using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Psicologa.Application;
using Psicologa.Application.BlogPost.Services;
using Psicologa.Application.BlogPost.ViewsModel;
using Psicologa.Domain.BlogPost.Entities;
using Shared.Infra.CrossCutting;

namespace Psicologa.Presentation.Painel.Controllers
{
    [AllowAnonymous]
    public class BlogController : BaseController
    {
        private readonly ApplicationBlogPostService _blogPostService;
        private readonly IAppSettings _appSettings;

        public BlogController(ApplicationBlogPostService blogPostService, IAppSettings appSettings)
        {
            _blogPostService = blogPostService;
            _appSettings = appSettings;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ObterUltimos()
        {
            IEnumerable<object> blogs = new List<object>();
            blogs = _blogPostService.ConsultarUltimos(6);
            return DefaultJSONResponse(true, blogs);
        }

        [HttpGet]
        public IActionResult ObterTodosPublicados()
        {
            IEnumerable<object> blogs = new List<object>();
            blogs = _blogPostService.ObterTodosPublicados();
            return DefaultJSONResponse(true, blogs);
        }


        public IActionResult Post()
        {
            return View("Views/Blog/Post.cshtml");
        }

        [HttpGet]
        public IActionResult ObterPorUrl(string url)
        {
            BlogPostConsultaViewModel blogPost = _blogPostService.ObterPorUrl(url);
            //return DefaultJSONResponse(true, blogs);
            return DefaultJSONResponse(blogPost != null, blogPost);
        }

        



    }
}
