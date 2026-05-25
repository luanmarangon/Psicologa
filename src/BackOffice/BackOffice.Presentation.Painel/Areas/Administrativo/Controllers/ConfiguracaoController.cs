using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Configuracao.Services;
using Psicologa.Application.Configuracao.ViewsModel;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarConfiguracoes)]
    [Area("Administrativo")]

    public class ConfiguracaoController : BaseController
    {
        ApplicationConfiguracaoService _configService;
        UsuarioAutenticado _ua;
        RequisicaoAtual _req;

        public ConfiguracaoController(ApplicationConfiguracaoService configService, UsuarioAutenticado ua, RequisicaoAtual req)
        {
            _configService = configService;
            _ua = ua;
            _req = req;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region CONFIGURACAO

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);

            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            ConfiguracaoViewModel configVM = null;
            try
            {
                configVM = dados.Deserialize<ConfiguracaoViewModel>();
                (operacao, vr) = _configService.Salvar(configVM, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }

            object configSalva = null;
            if (operacao)
            {
                configSalva = _configService.ObterConfiguracao(configVM.Id);
                AddUserMessageSuccess("Configuração salva com sucesso.");
            }
            return DefaultJSONResponse(operacao, configSalva);
        }

        [HttpGet]
        public IActionResult ObterConfiguracao()
        {
            //int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var config = _configService.ObterConfiguracao();
            return DefaultJSONResponse(true, config);

        }
       

        #endregion

        #region CONFIGURACAO FUNCIONAMENTO

        [HttpPost]
        public IActionResult SalvarFuncionamento([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);

            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            ConfiguracaoFuncionamentoViewModel configVM = null;
            try
            {
                configVM = dados.Deserialize<ConfiguracaoFuncionamentoViewModel>();
                (operacao, vr) = _configService.SalvarFuncionamento(configVM, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }

            object configSalva = null;
            if (operacao)
            {
                //configSalva = _configService.ObterConfiguracao(configVM.Id);
                AddUserMessageSuccess("Configuração salva com sucesso.");
            }
            return DefaultJSONResponse(operacao, configSalva);
        }

        [HttpGet]
        public IActionResult ObterConfiguracaoFuncionamento()
        {
            //int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var config = _configService.ObterConfiguracaoFuncionamento();
            return DefaultJSONResponse(true, config);

        }



        #endregion



    }
}
