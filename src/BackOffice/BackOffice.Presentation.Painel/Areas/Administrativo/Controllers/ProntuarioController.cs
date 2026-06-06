using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Prontuario.Services;
using Psicologa.Application.Prontuario.ViewsModel;
using Psicologa.Application.ProntuarioSessao.Services;
using Psicologa.Application.ProntuarioSessao.ViewsModel;
using Psicologa.Application.Usuario.ViewsModel;
using Psicologa.Domain.Paciente.Entities;
using Psicologa.Domain.Paciente.Services;
using Psicologa.Domain.Pessoa.Services;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = PerfilUsuarioViewModel.TpPermissao.GerenciarPacientes)]
    [Area("Administrativo")]
    public class ProntuarioController : BaseController
    {
        private UsuarioAutenticado _ua;
        private ApplicationProntuarioService _prontuarioService;
        private ApplicationProntuarioSessaoService _prontuarioSessaoService;
        private RequisicaoAtual _req;

        public ProntuarioController(ApplicationProntuarioService prontuarioService, ApplicationProntuarioSessaoService prontuarioSessaoService, UsuarioAutenticado ua, RequisicaoAtual req)
        {
            _prontuarioService = prontuarioService;
            _ua = ua;
            _req = req;
            _prontuarioSessaoService = prontuarioSessaoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var prontuario = _prontuarioService.Obter(idLimpo);
            return DefaultJSONResponse(true, prontuario);
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);

            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            ProntuarioViewModel prontuario = null;
            try
            {
                prontuario = dados.Deserialize<ProntuarioViewModel>();

                (operacao, vr) = _prontuarioService.Salvar(prontuario, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }

            object prontuarioVM = null;
            if (operacao)
            {
                prontuarioVM = _prontuarioService.Obter(prontuario.Id);
                AddUserMessageSuccess("Pessoa salva com sucesso.");
            }

            return DefaultJSONResponse(operacao, prontuarioVM);
        }

        [HttpGet]
        public IActionResult ObterProntuarioPorPacienteId(string pacienteId)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(pacienteId));
            var servico = _prontuarioService.ObterProntuarioPorPacienteId(idLimpo);
            return DefaultJSONResponse(true, servico);
        }
        
        
        #region ProntuarioSessao
        [HttpPost]
        public IActionResult EvoluirSessao([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);

            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            ProntuarioSessaoViewModel prontuario = null;
            try
            {
                prontuario = dados.Deserialize<ProntuarioSessaoViewModel>();
                prontuario.PsicologaId = _ua.PessoaId;
                (operacao, vr) = _prontuarioSessaoService.EvoluirSessao(prontuario, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }

            object prontuarioVM = null;
            if (operacao)
            {
                prontuarioVM = _prontuarioService.Obter(prontuario.Id);
                AddUserMessageSuccess("Pessoa salva com sucesso.");
            }

            return DefaultJSONResponse(operacao, prontuarioVM);
        }
        [HttpGet]
        public IActionResult ObterSessao(string id)
        {
            int idLimpo = Convert.ToInt32(id);
            var servico = _prontuarioSessaoService.ObterSessao(idLimpo);
            return DefaultJSONResponse(true, servico);
        }
        [HttpGet]
        public IActionResult PesquisarSessao(string q, string protocoloId, int filtroTipoAtendimento = 0, int pagina = 0, int ordenacao = 1)
        {
            int idLimpo = Convert.ToInt32((protocoloId));
            
            IEnumerable<object> sessoes = new List<object>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 10, (PaginacaoDados.TpOrdenacao)ordenacao);

                        
            sessoes = _prontuarioSessaoService.Consultar(q, idLimpo, filtroTipoAtendimento, paginacao);

            var retorno = new
            {
                sessoes,
                paginacao
            };

            return DefaultJSONResponse(true, retorno);
        }
        [HttpDelete]
        public IActionResult ExcluirSessao(string id)
        {
            int idLimpo = Convert.ToInt32(id);
            var operacao = _prontuarioSessaoService.ExcluirSessao(idLimpo);
            if (operacao)
                AddUserMessageSuccess("Sessão excluída com sucesso.");
            else
                AddUserMessageError("Não foi possível excluir a sessão. Tente novamente.");
            return DefaultJSONResponse(operacao, null);
        }
        #endregion ProntuarioSessao
    }
}