using MySqlX.XDevAPI.Common;
using Psicologa.Application.Prontuario.ViewsModel;
using Psicologa.Domain.Convenio.Entities;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Application.Documentos.Services
{
    public class ApplicationDocumentosService : IDisposable
    {
        private readonly Domain.Documentos.Services.DocumentosService _service;
        private readonly IAppSettings _appSettings;

        public ApplicationDocumentosService(Domain.Documentos.Services.DocumentosService service, IAppSettings appSettings)
        {
            _service = service;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(DocumentosViewModel documentoVM, string[] requisicao)
        {
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            Domain.Documentos.Entities.Documentos doc = new Domain.Documentos.Entities.Documentos();
            doc.Id = documentoVM.Id;
            doc.Ativo = documentoVM.Ativo;
            doc.Nome = documentoVM.Nome;
            doc.Categoria = (Domain.Documentos.Entities.Documentos.TpCategoria)documentoVM.Categoria;
            doc.Conteudo = documentoVM.Conteudo;

            if (doc.Validar())
            {
                operacao = _service.Salvar(doc);
                if(operacao) 
                    documentoVM.Id = doc.Id;
            }



            return (operacao, vr);
        }

        public DocumentosConsultaViewModel Obter(int id)
        {
            var documento = _service.Obter(id);
            return FormatarRetornoConsulta(documento);
        }

        public IEnumerable<DocumentosConsultaViewModel> Consultar(string termo, int tp, PaginacaoDados paginacao)
        {
            List<DocumentosConsultaViewModel> retorno = new List<DocumentosConsultaViewModel>();
            var documentos = _service.Consultar(termo, tp, paginacao);

            foreach (var documento in documentos)
            {
                retorno.Add(FormatarRetornoConsulta(documento));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.Nome).ToList();
            }

            return retorno;
        }

        internal DocumentosConsultaViewModel FormatarRetornoConsulta(Domain.Documentos.Entities.Documentos documento)
        {
            if(documento == null)
                return null;

            DocumentosConsultaViewModel documentoVM = new DocumentosConsultaViewModel();
            documentoVM.Id = documento.Id;
            documentoVM.Nome = documento.Nome;
            documentoVM.Categoria = (int)documento.Categoria;
            documentoVM.CategoriaNome = Utils.ObterDescricaoEnum((Domain.Documentos.Entities.Documentos.TpCategoria)documento.Categoria);
            documentoVM.Ativo = documento.Ativo;
            documentoVM.Conteudo = documento.Conteudo;
            documentoVM.DataCriacao = documento.DataCriacao;
            documentoVM.DataAtualizacao = documento.DataAtualizacao;

            return documentoVM;
        }
        public void Dispose()
        {
        }
    }
}
