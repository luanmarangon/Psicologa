using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Paciente.Services;
using Psicologa.Application.Paciente.ViewsModel;
using Psicologa.Application.Pessoa.Services;
using Psicologa.Application.Usuario.ViewsModel;
using Psicologa.Presentation.Painel;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Areas.Administrativo.Presentation.Painel.Controllers
{
    [Permissao(Tipo = PerfilUsuarioViewModel.TpPermissao.GerenciarPacientes)]
    [Area("Administrativo")]
    public class PacienteController : BaseController
    {
        ApplicationPessoaService _pessoaService;
        ApplicationPacienteService _pacienteService;
        UsuarioAutenticado _ua;
        RequisicaoAtual _req;


        public PacienteController(ApplicationPessoaService pessoaService, 
            ApplicationPacienteService pacienteService, 
            UsuarioAutenticado ua, RequisicaoAtual req
            )
        {
            _pessoaService = pessoaService;
            _pacienteService = pacienteService;
            _ua = ua;
            _req = req;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Pesquisar(string q, int tipoPessoa, int pagina = 0, int ordenacao = 1)
        {
            IEnumerable<object> pessoas = new List<object>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 30, (PaginacaoDados.TpOrdenacao)ordenacao);

            var tp = (Domain.Pessoa.Entities.PessoaTipo.TpPessoa.Paciente);
            pessoas = _pacienteService.Consultar(q, paginacao, tp);

            var retorno = new
            {
                pessoas,
                paginacao
            };

            return DefaultJSONResponse(true, retorno);
        }
        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var servico = _pacienteService.Obter(idLimpo);
            return DefaultJSONResponse(true, servico);
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement pacienteDados)
        {
            var requisicao = _req.ToArray(_ua);

            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            PacienteViewModel paciente = null;
            try
            {
                paciente = pacienteDados.Deserialize<PacienteViewModel>();

                (operacao, vr) = _pacienteService.Salvar(paciente, requisicao);

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
                pessoaVM = _pessoaService.Obter(paciente.Id);
                AddUserMessageSuccess("Pessoa salva com sucesso.");
            }

            return DefaultJSONResponse(operacao, pessoaVM);
        }




        //Prontuario
        public IActionResult Prontuario()
        {
            return View("Areas/Administrativo/Views/Paciente/Prontuario.cshtml");
        }


        [HttpGet]
        public IActionResult ObterProntuario(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var servico = _pacienteService.Obter(idLimpo);
            return DefaultJSONResponse(true, servico);
        }

        //Sessões
        [HttpGet]
        public IActionResult ObterSessoes(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var servico = _pacienteService.Obter(idLimpo);
            return DefaultJSONResponse(true, servico);
        }




    }
}
