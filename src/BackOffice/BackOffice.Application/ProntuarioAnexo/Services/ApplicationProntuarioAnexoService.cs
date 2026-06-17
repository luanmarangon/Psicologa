using Psicologa.Application.Prontuario.ViewsModel;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Psicologa.Application.ProntuarioAnexo.Services
{
    public class ApplicationProntuarioAnexoService : IDisposable
    {
        //private readonly Domain.Prontuario.Services.ProntuarioService _service;
        private readonly Domain.ProntuarioAnexo.Services.ProntuarioAnexoService _anexoService;

        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly IAppSettings _appSettings;

        public ApplicationProntuarioAnexoService(Domain.ProntuarioAnexo.Services.ProntuarioAnexoService anexoService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService, IAppSettings appSettings)
        {
            _anexoService = anexoService;
            _logAplicacaoService = logAplicacaoService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(ProntuarioAnexoViewModel anexoVM, string[] requisicao)
        {
             var dadosExistente = _anexoService.Obter(anexoVM.Id);

            ValidationResult vr = new();
            bool operacao = false;

            Domain.ProntuarioAnexo.Entities.ProntuarioAnexo anexo = new Domain.ProntuarioAnexo.Entities.ProntuarioAnexo();
            anexo.Id = anexoVM.Id;
            anexo.Prontuario = new Domain.Prontuario.Entities.Prontuario
            {
                Id = anexoVM.ProntuarioId
            };
            anexo.TipoAnexo = (Domain.ProntuarioAnexo.Entities.ProntuarioAnexo.tpTipoAnexo)anexoVM.TipoAnexo;
            anexo.Nome = string.IsNullOrEmpty(anexoVM.Nome) ? anexoVM.NomeArquivo : anexoVM.Nome;
            anexo.NomeArquivo = anexoVM.NomeArquivo;
            anexo.MimeType = ObterMimeType(anexoVM.NomeArquivo);
            anexo.TamanhoArquivo = anexoVM.TamanhoArquivo != null ? anexoVM.TamanhoArquivo : 0;
            anexo.Arquivo = anexoVM.Arquivo;
            anexo.Observacao = anexoVM.Observacao;

            if (anexo.Validar())
            {
                operacao = _anexoService.Salvar(anexo);
                if (operacao)
                    anexoVM.Id = anexo.Id;
            }

            // Log
            if(operacao)
            {
                _logAplicacaoService.Registrar(anexoVM.Id, requisicao, dadosExistente, anexo, "ProntuarioAnexo", "ApplicationProntuarioAnexoService", "Salvar");
            }

            return (true, vr);
        }
        public IEnumerable<ProntuarioAnexoConsultaViewModel> Consultar(string termo, int prontuarioId, int tpAnexo, PaginacaoDados paginacao)
        {
            List<ProntuarioAnexoConsultaViewModel> retorno = new List<ProntuarioAnexoConsultaViewModel>();
            var anexos = _anexoService.Consultar(termo, prontuarioId, (Domain.ProntuarioAnexo.Entities.ProntuarioAnexo.tpTipoAnexo)tpAnexo, paginacao);

            foreach (var anexo in anexos)
            {
                retorno.Add(FormatarRetornoConsulta(anexo));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == Shared.Infra.CrossCutting.PaginacaoDados.TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.DataCriacao).ToList();
            }

            return retorno;
        }
        public ProntuarioAnexoConsultaViewModel Obter(int id)
        {
            var anexo = _anexoService.Obter(id);
            return FormatarRetornoConsulta(anexo);
        }
        public bool Excluir(int id, string[] requisicao)
        {
            var dadosExistente = _anexoService.Obter(id);
            bool operacao = _anexoService.Excluir(id);
         
            if (operacao)
            {
                _logAplicacaoService.Registrar(id, requisicao, dadosExistente, null, "ProntuarioAnexo", "ApplicationProntuarioAnexoService", "Excluir");
            }

            return operacao;
        }
        internal ProntuarioAnexoConsultaViewModel FormatarRetornoConsulta(Domain.ProntuarioAnexo.Entities.ProntuarioAnexo anexo)
        {
            if (anexo == null)
                return null;

            ProntuarioAnexoConsultaViewModel ret = new ProntuarioAnexoConsultaViewModel();
            ret.Id = anexo.Id;
            ret.ProntuarioId = anexo.Prontuario.Id;
            ret.TipoAnexo = (int)anexo.TipoAnexo;
            ret.TipoAnexoDescricao = Utils.ObterDescricaoEnum(anexo.TipoAnexo);
            ret.Nome = anexo.Nome;
            ret.NomeArquivo = anexo.NomeArquivo;
            ret.MimeType = anexo.MimeType;
            ret.TipoArquivo = ObterTipoArquivo(anexo.MimeType);
            ret.TamanhoArquivo = anexo.TamanhoArquivo;
            ret.Arquivo = anexo.Arquivo;
            ret.Observacao = anexo.Observacao;
            ret.DataCriacao = anexo.DataCriacao;
            ret.DataAtualizacao = anexo.DataAtualizacao;
            return ret;
        }
        private string ObterTipoArquivo(string mimeType)
        {
            string retorno = string.Empty;

            switch (mimeType)
            {
                case "application/pdf":
                    retorno = "Arquivo PDF";
                    break;
                case "application/msword":
                    retorno = "Arquivo DOC";
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    retorno = "Arquivo DOCX";
                    break;
                case "image/jpeg":
                    retorno = "Imagem JPEG";
                    break;
                case "image/png":
                    retorno = "Imagem PNG";
                    break;
                default:
                    retorno = "Desconhecido";
                    break;
            }

            return retorno;
        }
        private string ObterMimeType(string nomeArquivo)
        {
            string extensao = System.IO.Path.GetExtension(nomeArquivo).ToLower();
            string mimeType = extensao switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
            return mimeType;
        }
        public void Dispose()
        {
        }
    }
}