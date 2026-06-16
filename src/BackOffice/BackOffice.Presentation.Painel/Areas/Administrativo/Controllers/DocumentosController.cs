using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Documentos.Services;
using Psicologa.Application.Prontuario.ViewsModel;
using Psicologa.Application.ServicoContato.ViewsModel;
using Psicologa.Domain.Convenio.Services;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarDocumentos)]
    [Area("Administrativo")]
    public class DocumentosController : BaseController
    {
        private UsuarioAutenticado _ua;
        private RequisicaoAtual _req;
        private readonly ApplicationDocumentosService _documentosService;

        public DocumentosController(UsuarioAutenticado ua, RequisicaoAtual req, ApplicationDocumentosService documentosService)
        {
            _ua = ua;
            _req = req;
            _documentosService = documentosService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {

            var requisicao = _req.ToArray(_ua);
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            DocumentosViewModel servicoVM = null;
            try
            {
                servicoVM = dados.Deserialize<DocumentosViewModel>();
                (operacao, vr) = _documentosService.Salvar(servicoVM, requisicao);

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
                retorno = _documentosService.Obter(servicoVM.Id);
                AddUserMessageSuccess("Serviço salvo com sucesso");
            }
            return DefaultJSONResponse(operacao, retorno);
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, int filtro, int pagina = 0, int ordenacao = 1)
        {
            IEnumerable<object> documentos = new List<object>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 12, (PaginacaoDados.TpOrdenacao)ordenacao); //12


            var tp = (Domain.Documentos.Entities.Documentos.TpCategoria)Convert.ToInt32(filtro);
            documentos = _documentosService.Consultar(q, (int)tp, paginacao);
            var retorno = new
            {
                documentos,
                paginacao
            };
            return DefaultJSONResponse(true, retorno);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var documento = _documentosService.Obter(idLimpo);
            return DefaultJSONResponse(true, documento);
        }





        }
}