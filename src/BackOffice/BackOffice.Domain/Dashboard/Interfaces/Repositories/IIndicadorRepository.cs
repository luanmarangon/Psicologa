using System;

namespace Psicologa.Domain.Dashboard.Interfaces.Repositories
{
    public interface IIndicadorRepository : IRepositoryBase<Dashboard.Entities.Indicador>
    {
        //Dashboard.Entities.Indicador ObterIndicador();
        //Dashboard.Entities.Indicador ObterIndicadoresAdmin();
        //Dashboard.Entities.Indicador ObterIndicadoresPsicologo();


        int ObterQuantidadePsicologos();
        int ObterQuantidadeClientes();
        int ObterQuantidadeBlogs();
        int ObterQuantidadeColaboradores();
        int ObterQuantidadeBlogsPublicados();
        int ObterQuantidadeUsuarios();
        int ObterQuantidadeServicosAtivos();

        int ObterQuantidadeAgendamentosAgendadoHoje();
        int ObterMeusAgendamentos(int psicologoId);
    }
}
