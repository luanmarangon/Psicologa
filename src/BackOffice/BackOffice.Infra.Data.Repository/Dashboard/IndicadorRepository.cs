using Psicologa.Domain.Dashboard.Entities;
using Psicologa.Domain.Dashboard.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Infra.Data.Providers;
using System;

namespace Psicologa.Infra.Data.Repository.Dashboard
{
    public class IndicadorRepository : RepositoryBase<Domain.Dashboard.Entities.Indicador>, IIndicadorRepository
    {
        private readonly ILogger<IndicadorRepository> _logger;

        public IndicadorRepository(IDBContextFactory dbContextFactory, ILogger<IndicadorRepository> logger) : base(dbContextFactory)
        {
            _logger = logger;
        }


        public int ObterQuantidadePsicologos()
        {

            try
            {
                using(var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT COUNT(*) AS Psicologos FROM pessoa p JOIN pessoatipo pt ON p.PessoaId = pt.PessoaId WHERE pt.Tipo = 3 AND p.Ativo = 1";

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de psicologos");
                return 0;
            }

        }
        public int ObterQuantidadeClientes()
        {

            try
            {
                using(var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT COUNT(*) AS Clientes FROM pessoa p JOIN pessoatipo pt ON p.PessoaId = pt.PessoaId WHERE pt.Tipo = 1 AND p.Ativo = 1";

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de clientes");
                return 0;
            }

        }
        public int ObterQuantidadeBlogs()
        {

            try
            {
                using(var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT COUNT(*) AS BlogTotal FROM BlogPost";

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de clientes");
                return 0;
            }

        }
        public int ObterQuantidadeColaboradores()
        {

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT COUNT(*) AS Colaboradores FROM pessoa p JOIN pessoatipo pt ON p.PessoaId = pt.PessoaId WHERE pt.Tipo = 2 AND p.Ativo = 1";

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de clientes");
                return 0;
            }

        }
        public int ObterQuantidadeBlogsPublicados()
        {

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT COUNT(*) AS BlogPublicados FROM BlogPost Where Date(DataPublicacao) <= Now()";

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de clientes");
                return 0;
            }

        }
        public int ObterQuantidadeUsuarios()
        {

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT COUNT(*) AS USUARIOS FROM usuario";

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de clientes");
                return 0;
            }

        }
        public int ObterQuantidadeServicosAtivos()
        {

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT COUNT(*) AS ServicosAtivos FROM Servico where Ativo = 1";

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de clientes");
                return 0;
            }

        }
        public int ObterQuantidadeAgendamentosAgendadoHoje()
        {

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT COUNT(*) FROM Agendamento WHERE StatusAgendamento = @StatusAgendamento and DATE(DataConsulta) = @Data";
                    cmd.ParameterAdd("@StatusAgendamento", 1);
                    cmd.ParameterAdd("@Data", DateTime.Now.Date);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de agendamento hoje");
                return 0;
            }

        }
        public int ObterMeusAgendamentos(int psicologoId)
        {

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT COUNT(*) FROM Agendamento WHERE StatusAgendamento = @StatusAgendamento and DATE(DataConsulta) = @Data and PsicologoId = @PsicologoId";
                    
                    cmd.ParameterAdd("@StatusAgendamento", 1);
                    cmd.ParameterAdd("@Data", DateTime.Now.Date);
                    cmd.ParameterAdd("@PsicologoId", psicologoId);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de agendamento hoje");
                return 0;
            }

        }


        public Indicador ObterIndicadoresAdmin()
        {
            Indicador t = new Indicador();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT
	                        (SELECT COUNT(*) FROM pessoa p JOIN pessoatipo pt ON p.PessoaId = pt.PessoaId WHERE pt.Tipo = 1 AND p.Ativo = 1) AS Clientes,
	                        (SELECT COUNT(*) FROM pessoa p JOIN pessoatipo pt ON p.PessoaId = pt.PessoaId WHERE pt.Tipo = 2 AND p.Ativo = 1) AS Colaboradores,
	                        (SELECT COUNT(*) FROM pessoa p JOIN pessoatipo pt ON p.PessoaId = pt.PessoaId WHERE pt.Tipo = 3 AND p.Ativo = 1) AS Psicologos,
                            (SELECT COUNT(*) FROM BlogPost ) AS BlogTotal,
                            (SELECT COUNT(*) FROM BlogPost Where Date(DataPublicacao) <= Now()) AS BlogPublicados,
                            (SELECT COUNT(*) FROM usuario) as Usuarios,
                            (SELECT COUNT(*) FROM Servico where Ativo = 1) as ServicosAtivos

                        
                        ";
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            t.QuantidadeClientes = Convert.ToInt32(dr["Clientes"]);
                            t.QuantidadeColaboradores = Convert.ToInt32(dr["Colaboradores"]);
                            t.QuantidadePsicologos = Convert.ToInt32(dr["Psicologos"]);
                            t.QuantidadeBlogs = Convert.ToInt32(dr["BlogTotal"]);
                            t.QuantidadeBlogsPublicado = Convert.ToInt32(dr["BlogPublicados"]);
                            t.QuantidadeUsuarios = Convert.ToInt32(dr["Usuarios"]);
                            t.QuantidadeServicosAtivos = Convert.ToInt32(dr["ServicosAtivos"]);
                        }
                    }
                    cmd.ParametersClear();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Indicador Dashboard Material Educativo.");
            }
            return t;
        }

        public Indicador ObterIndicadoresPsicologo()
        {
            Indicador t = new Indicador();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"SELECT
	                        (SELECT COUNT(*) FROM pessoa p JOIN pessoatipo pt ON p.PessoaId = pt.PessoaId WHERE pt.Tipo = 3 AND p.Ativo = 1) AS Psicologos,
                            (SELECT COUNT(*) FROM BlogPost Where Date(DataPublicacao) <= Now()) AS BlogPublicados,
                            (SELECT COUNT(*) FROM Servico where Ativo = 1) as ServicosAtivos

                        
                        ";
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            //t.QuantidadeClientes = Convert.ToInt32(dr["Clientes"]);
                            //t.QuantidadeColaboradores = Convert.ToInt32(dr["Colaboradores"]);
                            t.QuantidadePsicologos = Convert.ToInt32(dr["Psicologos"]);
                            //t.QuantidadeBlogs = Convert.ToInt32(dr["BlogTotal"]);
                            t.QuantidadeBlogsPublicado = Convert.ToInt32(dr["BlogPublicados"]);
                            //t.QuantidadeUsuarios = Convert.ToInt32(dr["Usuarios"]);
                            t.QuantidadeServicosAtivos = Convert.ToInt32(dr["ServicosAtivos"]);
                        }
                    }
                    cmd.ParametersClear();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Indicador Dashboard Material Educativo.");
            }
            return t;
        }
    }
}
