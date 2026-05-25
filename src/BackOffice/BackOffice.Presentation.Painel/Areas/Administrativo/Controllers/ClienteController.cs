using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Pessoa.Services;
using Psicologa.Application.Pessoa.ViewsModel;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.Backoffice)]
    [Area("Administrativo")]
    public class ClienteController: BaseController
    {
        private UsuarioAutenticado _ua;
        private ApplicationPessoaService _pessoaService;

        public ClienteController(UsuarioAutenticado ua, ApplicationPessoaService pessoaService)
        {
            _ua = ua;
            _pessoaService = pessoaService;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SalvarCliente([FromBody] System.Text.Json.JsonElement pessoaDados)
        {
            //Obter Dados de Navegação
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ipReal = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                         ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var isMobile = userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase);
            var isTablet = userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase);
            var dispositivo = isMobile ? "Mobile" : isTablet ? "Tablet" : "Desktop";

            string[] requisicao = new string[] { ipReal, userAgent, dispositivo, _ua.Nome, _ua.Id.ToString() };

            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            PessoaViewModel pessoa = null;
            try
            {
                pessoa = pessoaDados.Deserialize<PessoaViewModel>();

                (operacao, vr) = _pessoaService.Salvar(pessoa, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }

            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }

            object pessoaVM = null;
            if (operacao)
            {
                pessoaVM = _pessoaService.Obter(pessoa.Dados.Id);
                AddUserMessageSuccess("Dados salvo com sucesso.");
            }

            return DefaultJSONResponse(operacao, pessoaVM);
        }

        [HttpGet]
        public IActionResult ObterCliente(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            PessoaViewModel pessoa = _pessoaService.Obter(idLimpo);
            return DefaultJSONResponse(pessoa != null, pessoa);
        }

        [HttpGet]
        public IActionResult Pesquisar()
        {
            int idLimpo = _ua.PessoaId;
            PessoaViewModel pessoa = _pessoaService.Obter(idLimpo);
            return DefaultJSONResponse(pessoa != null, pessoa);
        }




        #region Servicos
        public IActionResult Servicos()
        {
            return View("Areas/Administrativo/Views/Cliente/Servicos.cshtml");
        }
        //[HttpGet]
        //public IActionResult ObterServicos()
        //{
        //    int idLimpo = _ua.PessoaId;
        //    IEnumerable<ServicoConsultaViewModel> servicos = new List<ServicoConsultaViewModel>();
        //    servicos = _servicoService.ObterServicos();

        //    return DefaultJSONResponse(true, servicos);
        //}



        #endregion
    }
}
