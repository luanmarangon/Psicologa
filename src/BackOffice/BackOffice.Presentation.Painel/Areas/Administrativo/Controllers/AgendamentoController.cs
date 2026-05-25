using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Psicologa.Application.Agendamento.Services;
using Psicologa.Application.Agendamento.ViewsModel;
using Psicologa.Application.Servico.ViewsModel;
using Psicologa.Application.ServicoContato.ViewsModel;
using Psicologa.Domain.Servico.Services;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarAgendamentos)]
    [Area("Administrativo")]
    public class AgendamentoController : BaseController
    {
        private UsuarioAutenticado _ua;
        private RequisicaoAtual _req;
        private ApplicationAgentamentoService _agendamentoSevice;

        public AgendamentoController(UsuarioAutenticado usuarioAutenticado, RequisicaoAtual requisicaoAtual, ApplicationAgentamentoService agendamentoService)
        {
            _ua = usuarioAutenticado;
            _req = requisicaoAtual;
            _agendamentoSevice = agendamentoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Obter Disponibilidades
        [HttpGet]
        public IActionResult ObterDisponibilidadeHorario(string psicologoId, string dataConsulta)
        {
            int psicologoIdLimpo = Convert.ToInt32(Criptografia.Descriptografar(psicologoId));
            DateTime dataConsultaLimpa = DateTime.Parse(dataConsulta);
            var dispo = _agendamentoSevice.ObterDisponibilidade(psicologoIdLimpo, dataConsultaLimpa);
            return DefaultJSONResponse(true, dispo);
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, int filtro, int pagina = 0, int ordenacao = 1)
        {
            IEnumerable<AgendamentoConsultaViewModel> agendamentos = new List<AgendamentoConsultaViewModel>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 12, (PaginacaoDados.TpOrdenacao)ordenacao); //12

            var filtroFinal = (Domain.Agendamento.Entities.Agendamento.tpFiltro)filtro;
            agendamentos = _agendamentoSevice.Consultar(q, filtroFinal, paginacao);
            var retorno = new
            {
                agendamentos,
                paginacao
            };
            return DefaultJSONResponse(true, retorno);
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            AgendamentoViewModel agendamentoVM = null;
            try
            {
                agendamentoVM = dados.Deserialize<AgendamentoViewModel>();
                (operacao, vr) = _agendamentoSevice.Salvar(agendamentoVM, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }
            object retorno = null;
            if (operacao)
            {
                retorno = _agendamentoSevice.ObterPorId(agendamentoVM.Id);
                AddUserMessageSuccess("Serviço salvo com sucesso");
            }
            return DefaultJSONResponse(operacao, retorno);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var agendamento = _agendamentoSevice.ObterPorId(idLimpo);
            return DefaultJSONResponse(true, agendamento);
        }

        [HttpDelete]
        public IActionResult Excluir(string id)
        {
            var requisicao = _req.ToArray(_ua);
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var agendamento = _agendamentoSevice.Excluir(idLimpo, requisicao);
            return DefaultJSONResponse(true, agendamento);
        }
    }
}