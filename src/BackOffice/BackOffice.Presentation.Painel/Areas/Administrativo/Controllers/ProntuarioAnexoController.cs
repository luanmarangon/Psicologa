using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Prontuario.ViewsModel;
using Psicologa.Application.ProntuarioAnexo.Services;
using Psicologa.Domain.ProntuarioSessao.Services;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarPacientes)]
    [Area("Administrativo")]
    public class ProntuarioAnexoController : BaseController
    {
        private UsuarioAutenticado _ua;
        private RequisicaoAtual _req;
        private ApplicationProntuarioAnexoService _prontuarioAnexoService;

        public ProntuarioAnexoController(UsuarioAutenticado ua, RequisicaoAtual req, ApplicationProntuarioAnexoService prontuarioAnexoService)
        {
            _ua = ua;
            _req = req;
            _prontuarioAnexoService = prontuarioAnexoService;
        }

        //Sem Index, pois não tem tela, apenas ações para upload e download de anexos do prontuário a Tela Inicial vem do PacienteController

        [HttpGet]
        public IActionResult Obter(string id)
        {
            //int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            int idLimpo = Convert.ToInt32(id);
            var anexos = _prontuarioAnexoService.Obter(idLimpo);
            return DefaultJSONResponse(true, anexos);
        }

        [HttpGet]
        public IActionResult BaixarAnexo(string id)
        {
            //int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            //Aqui deve ser implementado a lógica para buscar o anexo do prontuário e retornar o arquivo para download
            //Exemplo:
            //var anexo = _prontuarioAnexoService.Obter(idLimpo);
            //return File(anexo.CaminhoArquivo, anexo.TipoConteudo, anexo.NomeArquivo);
            return View();
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);

            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            ProntuarioAnexoViewModel anexo = null;
            try
            {
                anexo = dados.Deserialize<ProntuarioAnexoViewModel>();
                (operacao, vr) = _prontuarioAnexoService.Salvar(anexo, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }

            object anexoVM = null;

            if (operacao)
            {
                //anexoVM = _prontuarioAnexoService.Obter(anexo.Id);
                AddUserMessageSuccess("Anexo salvo com sucesso");
            }

            return DefaultJSONResponse(operacao, anexoVM);
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, string prontuarioId, int filtroTipoAnexo = 0, int pagina = 0, int ordenacao = 1)
        {
            int idLimpo = Convert.ToInt32((prontuarioId));

            IEnumerable<object> sessoes = new List<object>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 10, (PaginacaoDados.TpOrdenacao)ordenacao);


            sessoes = _prontuarioAnexoService.Consultar(q, idLimpo, filtroTipoAnexo, paginacao);

            var retorno = new
            {
                sessoes,
                paginacao
            };

            return DefaultJSONResponse(true, retorno);
        }

        [HttpDelete]
        public IActionResult Excluir(string id)
        {
            var requisicao = _req.ToArray(_ua);

            //int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            int idLimpo = Convert.ToInt32(id);
            bool operacao = _prontuarioAnexoService.Excluir(idLimpo, requisicao);
            return DefaultJSONResponse(operacao, null);
        }
    }
}