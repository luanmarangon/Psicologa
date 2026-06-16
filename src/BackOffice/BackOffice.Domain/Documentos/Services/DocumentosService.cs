using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Documentos.Services
{
    public class DocumentosService : ServiceBase<Entities.Documentos>, IServiceBase<Entities.Documentos>
    {
        public readonly Interfaces.Repositories.IDocumentosRepository _repository;

        public DocumentosService(Interfaces.Repositories.IDocumentosRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Psicologa.Domain.Documentos.Entities.Documentos documento)
        {
            bool operacao = false;
            if (documento.Id == 0)
            {
                documento.DataCriacao = DateTime.Now;
                documento.DataAtualizacao = DateTime.Now;
            }
            else
            {
                documento.DataAtualizacao = DateTime.Now;
            }
            documento.Nome = documento.Nome.ToUpper();

            operacao = _repository.Salvar(documento);
            return operacao;
        }

        public Domain.Documentos.Entities.Documentos Obter(int id)
        {
            return _repository.Obter(id);
        }
        public IEnumerable<Domain.Documentos.Entities.Documentos> Consultar(string termo, int tp, PaginacaoDados paginacao)
        {
            if (string.IsNullOrEmpty(termo))
                termo = "";
            else
                termo = termo.Replace("%", "").Replace("_", "");
            return _repository.Consultar(termo, tp, paginacao);
        }
    }
}