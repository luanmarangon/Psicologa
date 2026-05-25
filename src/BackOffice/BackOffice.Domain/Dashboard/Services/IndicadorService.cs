using Psicologa.Domain.Dashboard.Interfaces.Repositories;

namespace Psicologa.Domain.Dashboard.Services
{
    public class IndicadorService : ServiceBase<Dashboard.Entities.Indicador>
    {
        private readonly IIndicadorRepository _repository;

        public IndicadorService(IIndicadorRepository repository) : base(repository)
        {
            _repository = repository;
        }

        //public Dashboard.Entities.Indicador ObterIndicador()
        //{
        //    return _repository.ObterIndicador();
        //}
        //public Dashboard.Entities.Indicador ObterIndicadoresAdmin()
        //{
        //    return _repository.ObterIndicadoresAdmin();
        //}

        //public Dashboard.Entities.Indicador ObterIndicadoresPsicologo()
        //{
        //    return _repository.ObterIndicadoresPsicologo();
        //}

        public int ObterQuantidadePsicologos()
        {
            return _repository.ObterQuantidadePsicologos();
        }
        public int ObterQuantidadeClientes()
        {
            return _repository.ObterQuantidadeClientes();
        }
        public int ObterQuantidadeBlogs()
        {
            return _repository.ObterQuantidadeBlogs ();
        }
        public int ObterQuantidadeColaboradores()
        {
            return _repository.ObterQuantidadeColaboradores();
        }
        public int ObterQuantidadeBlogsPublicados()
        {
            return _repository.ObterQuantidadeBlogsPublicados();
        }
        public int ObterQuantidadeUsuarios()
        {
            return _repository.ObterQuantidadeUsuarios();
        }
        public int ObterQuantidadeServicosAtivos()
        {
            return _repository.ObterQuantidadeServicosAtivos();
        }
        public int ObterQuantidadeAgendamentosAgendadoHoje()
        {
            return _repository.ObterQuantidadeAgendamentosAgendadoHoje();
        }
        public int ObterMeusAgendamentos(int psicologoId)
        {
            return _repository.ObterMeusAgendamentos(psicologoId);
        }



    }
}
