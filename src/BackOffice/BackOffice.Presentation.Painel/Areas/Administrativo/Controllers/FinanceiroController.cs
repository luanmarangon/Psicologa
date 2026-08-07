using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Agendamento.ViewsModel;
using Psicologa.Application.Financeiro.Services;
using Psicologa.Application.Financeiro.ViewsModel;
using Psicologa.Application.Pessoa.ViewsModel;
using Psicologa.Domain.Agendamento.Services;
using Psicologa.Domain.Pessoa.Services;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarFinanceiro)]
    [Area("Administrativo")]
    public class FinanceiroController : BaseController
    {
        private UsuarioAutenticado _ua;
        private RequisicaoAtual _req;
        private ApplicationFinanceiroService _applicationFinanceiro;
        private ApplicationFinanceiroCategoriaService _applicationFinanceiroCategoria;

        public FinanceiroController(UsuarioAutenticado ua, RequisicaoAtual req, ApplicationFinanceiroService applicationFinanceiro, ApplicationFinanceiroCategoriaService applicationFinanceiroCategoria)
        {
            _ua = ua;
            _req = req;
            _applicationFinanceiro = applicationFinanceiro;
            _applicationFinanceiroCategoria = applicationFinanceiroCategoria;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, int filtro, int pagina = 0, int ordenacao = 1)
        {
            IEnumerable<FinanceiroConsultaViewModel> lancamentos = new List<FinanceiroConsultaViewModel>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 12, (PaginacaoDados.TpOrdenacao)ordenacao); //12

            var filtroFinal = (Domain.Agendamento.Entities.Agendamento.tpFiltro)filtro;
            lancamentos = _applicationFinanceiro.Consultar(q, paginacao);
            var resumo = _applicationFinanceiro.ObterResumo(DateTime.MinValue, DateTime.MinValue);
            var retorno = new
            {
                lancamentos,
                resumo,
                paginacao
            };
            return DefaultJSONResponse(true, retorno);
        }


        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement pessoaDados)
        {
            var requisicao = _req.ToArray(_ua);

            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            FinanceiroViewModel lancamento = null;
            try
            {
                lancamento = pessoaDados.Deserialize<FinanceiroViewModel>();

                (operacao, vr) = _applicationFinanceiro.Salvar(lancamento, requisicao);

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
                pessoaVM = _applicationFinanceiro.Obter(lancamento.Id);
                AddUserMessageSuccess("Financeiro salvo com sucesso.");
            }

            return DefaultJSONResponse(operacao, pessoaVM);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            FinanceiroConsultaViewModel financeiro = _applicationFinanceiro.Obter(idLimpo);
            return DefaultJSONResponse(financeiro != null, financeiro);
        }

        [HttpDelete]
        public IActionResult Excluir(string id)
        {
            var requisicao = _req.ToArray(_ua);
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            bool operacao;

            operacao = _applicationFinanceiro.Excluir(idLimpo, requisicao);

            if (operacao)
            {
                AddUserMessageSuccess("Lançamento excluído com sucesso.");
            }
            else
            {
                AddUserMessageError("Erro ao excluir lançamento");
            }

            return DefaultJSONResponse(operacao);
        }


        //Financeiro Categoria
        [HttpGet]
        public IActionResult ObterCategorias(string tipo)
        {
            var categorias = _applicationFinanceiroCategoria.ObterTodasCategoria(Convert.ToInt32(tipo));
            return DefaultJSONResponse(true, categorias);
        }


        //Resumo Financeiro
        [HttpGet]
        public IActionResult ObterResumo(string dataInicio = null, string dataFim = null)
        {
            DateTime dtInicio = Convert.ToDateTime(dataInicio);
            DateTime dtFim = Convert.ToDateTime(dataFim);
            var resumo = _applicationFinanceiro.ObterResumo(dtInicio, dtFim);
            return DefaultJSONResponse(true, resumo);
        }
    }
}