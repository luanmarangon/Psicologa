using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Prontuario.Services
{
    public class ProntuarioService: ServiceBase<Entities.Prontuario>, IServiceBase<Entities.Prontuario>
    {
        public readonly Interfaces.Repositories.IProntuarioRepository _repository;

        public ProntuarioService(Interfaces.Repositories.IProntuarioRepository repository) 
            : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Entities.Prontuario prontuario)
        {
            if (prontuario.Id == 0)
            {
                prontuario.DataCriacao = DateTime.Now;
                prontuario.DataAtualizacao = DateTime.Now;
            }
            else
            {
                prontuario.DataAtualizacao = DateTime.Now;
            }


            return _repository.Salvar(prontuario);


        }

        public Domain.Prontuario.Entities.Prontuario Obter(int prontuarioId)
        {
            return _repository.Obter(prontuarioId);
        }
        public Domain.Prontuario.Entities.Prontuario ObterProntuarioPorPacienteId(int pacienteId)
        {
            return _repository.ObterProntuarioPorPacienteId(pacienteId);
        }

    }
}
