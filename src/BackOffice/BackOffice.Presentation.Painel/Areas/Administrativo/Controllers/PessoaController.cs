using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Pessoa.Services;
using Psicologa.Application.Pessoa.ViewsModel;
using Psicologa.Application.Usuario.ViewsModel;
using Psicologa.Presentation.Painel;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;
using static Psicologa.Domain.Pessoa.Entities.PessoaTipo;

namespace Psicologa.Areas.Administrativo.Presentation.Painel.Controllers
{
    [Permissao(Tipo = PerfilUsuarioViewModel.TpPermissao.GerenciarPessoas)]
    [Area("Administrativo")]
    public class PessoaController : BaseController
    {
        ApplicationPessoaService _pessoaService;
        UsuarioAutenticado _ua;
        RequisicaoAtual _req;

        public PessoaController(ApplicationPessoaService pessoaService, UsuarioAutenticado ua, RequisicaoAtual req)
        {
            _pessoaService = pessoaService;
            _ua = ua;
            _req = req;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement pessoaDados)
        {
            var requisicao = _req.ToArray(_ua);

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
                AddUserMessageSuccess("Pessoa salva com sucesso.");
            }

            return DefaultJSONResponse(operacao, pessoaVM);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            PessoaViewModel pessoa = _pessoaService.Obter(idLimpo);
            return DefaultJSONResponse(pessoa != null, pessoa);
        }

        [HttpDelete]
        public IActionResult Excluir(string id)
        {
            var requisicao = _req.ToArray(_ua);
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            bool operacao;

            operacao = _pessoaService.Excluir(idLimpo, requisicao);

            if (operacao)
            {
                AddUserMessageSuccess("Pessoa excluída com sucesso.");
            }
            else
            {
                AddUserMessageError("Erro ao excluir pessoa");
            }

            return DefaultJSONResponse(operacao);
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, int tipoPessoa, int pagina = 0, int ordenacao = 1)
        {
            
            IEnumerable<object> pessoas = new List<object>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 30, (PaginacaoDados.TpOrdenacao)ordenacao);


            var tp = (Domain.Pessoa.Entities.PessoaTipo.TpPessoa)tipoPessoa;
            pessoas = _pessoaService.Consultar(q, paginacao, tp);

            var retorno = new
            {
                pessoas,
                paginacao
            };

            return DefaultJSONResponse(true, retorno);
        }


        [HttpGet]
        public IActionResult ConsultarClienteAutoComplete(string q)
        {
           IEnumerable<PessoaConsultaViewModel> clientes = new List<PessoaConsultaViewModel>();

            if (!string.IsNullOrEmpty(q) && q.Trim() != "")
            {
                var tp = (Domain.Pessoa.Entities.PessoaTipo.TpPessoa)TpPessoa.Paciente;
                clientes = _pessoaService.Consultar(q, new PaginacaoDados { TamanhoPagina = 20 }, tp);
            }

            return Json(clientes);
        }
        [HttpGet]
        public IActionResult ConsultarPsicologoAutoComplete(string q)
        {
            IEnumerable<PessoaConsultaViewModel> clientes = new List<PessoaConsultaViewModel>();

            if (!string.IsNullOrEmpty(q) && q.Trim() != "")
            {
                var tp = (Domain.Pessoa.Entities.PessoaTipo.TpPessoa)TpPessoa.Psicologo;
                clientes = _pessoaService.Consultar(q, new PaginacaoDados { TamanhoPagina = 20 }, tp);
            }

            return Json(clientes);
        }

        [HttpGet]
        public IActionResult ConsultarPessoaAutoComplete(string q)
        {
            IEnumerable<PessoaConsultaViewModel> clientes = new List<PessoaConsultaViewModel>();

            if (!string.IsNullOrEmpty(q) && q.Trim() != "")
            {
                var tp = (Domain.Pessoa.Entities.PessoaTipo.TpPessoa)TpPessoa.Indefinido;
                clientes = _pessoaService.Consultar(q, new PaginacaoDados { TamanhoPagina = 20 }, tp);
            }

            return Json(clientes);
        }

    }
}
