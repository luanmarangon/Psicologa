using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Psicologa.Domain.Pessoa.Entities.PessoaTipo;

namespace Psicologa.Domain.ProntuarioAnexo.Services
{
    public class ProntuarioAnexoService : ServiceBase<Entities.ProntuarioAnexo>, IServiceBase<Entities.ProntuarioAnexo>
    {

        public readonly Interfaces.Repositories.IProntuarioAnexoRepository _repository;
        public ProntuarioAnexoService(Interfaces.Repositories.IProntuarioAnexoRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Entities.ProntuarioAnexo anexo)
        {
            if (anexo.Id == 0)
            {
                anexo.DataCriacao = DateTime.Now;
                anexo.DataAtualizacao = DateTime.Now;
            }
            else
            {
                anexo.DataAtualizacao = DateTime.Now;
            }


            return _repository.Salvar(anexo);


        }
        public Domain.ProntuarioAnexo.Entities.ProntuarioAnexo Obter(int id)
        {
            return _repository.Obter(id);
        }
        public IEnumerable<Domain.ProntuarioAnexo.Entities.ProntuarioAnexo> Consultar(string termo, int prontuarioId, Domain.ProntuarioAnexo.Entities.ProntuarioAnexo.tpTipoAnexo tpAnexo, PaginacaoDados paginacao)
        {
            if (string.IsNullOrEmpty(termo))
                termo = "";
            else
                termo = termo.Replace("%", "").Replace("_", "");

            return _repository.Consultar(termo, prontuarioId, tpAnexo, paginacao);
        }
        public bool Excluir(int id)
        {
            return _repository.Excluir(id);
        }
    }
}
