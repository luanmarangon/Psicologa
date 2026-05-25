using Microsoft.Extensions.Logging;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace Psicologa.Infra.Data.Repository.Usuario
{
    public class PerfilUsuarioRepository : RepositoryBase<Domain.Usuario.Entities.PerfilUsuario>, Domain.Usuario.Interfaces.Repositories.IPerfilUsuarioRepository
    {
        private readonly ILogger<PerfilUsuarioRepository> _logger;

        public PerfilUsuarioRepository(IDBContextFactory dbContextFactory, ILogger<PerfilUsuarioRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.Usuario.Entities.PerfilUsuario pu)
        {
            bool operacao = false;
            using (var uow = DbContext.CreateUnitOfWork())
            {
                try
                {
                    using (var cmd = DbContext.CreateCommand())
                    {
                        cmd.CommandText = "delete from PerfilUsuarioPermissao where PerfilUsuarioId = @PerfilUsuarioId";
                        cmd.ParameterAdd("@PerfilUsuarioId", (int)pu.Id);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"insert into PerfilUsuarioPermissao (PerfilUsuarioId, Permissao) 
                                            values (@PerfilUsuarioId, @Permissao)";

                        foreach (var p in pu.Permissoes)
                        {
                            cmd.ParametersClear();
                            cmd.ParameterAdd("@PerfilUsuarioId", pu.Id);
                            cmd.ParameterAdd("@Permissao", (int)p);
                            cmd.ExecuteNonQuery();
                        }

                        operacao = true;
                    }
                    uow.SaveChanges();

                }
                catch (Exception ex)
                {
                    uow.CancelChanges();
                    _logger.LogError(ex, "Erro ao salvar a Permissão.");
                }
            }
           
            return operacao;
        }
        public Domain.Usuario.Entities.PerfilUsuario Obter(int id)
        {
            Domain.Usuario.Entities.PerfilUsuario p = new Domain.Usuario.Entities.PerfilUsuario();
            p.Perfil = (Domain.Usuario.Entities.PerfilUsuario.TpPerfil)id;
            p.Permissoes = new List<Domain.Usuario.Entities.PerfilUsuario.TpPermissao>();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                          $@"select *
                             from PerfilUsuario up 
                                  left outer join PerfilUsuarioPermissao upp on up.PerfilUsuarioId = upp.PerfilUsuarioId
                             where up.PerfilUsuarioId = @Id";

                    cmd.ParameterAdd("@Id", id);

                    using (var dr = cmd.ExecuteReader())
                    {
                        p = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o perfil.");
            }
            finally
            {
            }

            return p;
        }
        public IEnumerable<Domain.Usuario.Entities.PerfilUsuario> ObterTodos()
        {
            IEnumerable<Domain.Usuario.Entities.PerfilUsuario> pus = new List<Domain.Usuario.Entities.PerfilUsuario>();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                          $@"select *
                             from PerfilUsuario up 
                                  left outer join PerfilUsuarioPermissao upp on up.PerfilUsuarioId = upp.PerfilUsuarioId
                             order by up.Nome";

                    using (var dr = cmd.ExecuteReader())
                    {
                        pus = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o perfil.");
            }
            finally
            {
            }

            return pus;
        }
        internal override IEnumerable<Domain.Usuario.Entities.PerfilUsuario> Map(IDataReader dr)
        {
            List<Domain.Usuario.Entities.PerfilUsuario> pus = new List<Domain.Usuario.Entities.PerfilUsuario>();

            while (dr.Read())
            {
                int id = (Int32)dr["PerfilUsuarioId"];
                Domain.Usuario.Entities.PerfilUsuario pu = pus.Find(i => i.Id  == id);

                if (pu == null)
                {
                    pu = new Domain.Usuario.Entities.PerfilUsuario();
                    pu.Id = id;
                    pu.Perfil = (Domain.Usuario.Entities.PerfilUsuario.TpPerfil)id;
                    //pu.Perfil = dr["Nome"].ToString();
                    pu.Permissoes = new List<Domain.Usuario.Entities.PerfilUsuario.TpPermissao>();
                    
                    pus.Add(pu);

                }

                //relacionamentos * para *
                if (dr["Permissao"] != DBNull.Value)
                {
                    pu.Permissoes.Add((Domain.Usuario.Entities.PerfilUsuario.TpPermissao)Convert.ToInt16(dr["Permissao"]));
                }
            }

            return pus;
        }
        public bool SalvarPerfil(Domain.Usuario.Entities.PerfilUsuarioCadastro pu)
        {
            bool operacao = false;
            using (var ow = DbContext.CreateUnitOfWork())
            {
                try
                {
                    using (var cmd = DbContext.CreateCommand())
                    {
                        if (pu.Id == 0)
                        {
                            cmd.CommandText = "select max(PerfilUsuarioId) from PerfilUsuario";
                            var result = cmd.ExecuteScalar();
                            int maxId = (result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                            pu.Id = maxId + 1;

                            cmd.Parameters.Clear();
                            cmd.CommandText = "INSERT INTO PerfilUsuario (PerfilUsuarioId, Nome) values (@PerfilUsuarioId, @Nome);";
                            cmd.ParameterAdd("@PerfilUsuarioId", pu.Id);
                            cmd.ParameterAdd("@Nome", pu.Nome);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = "update PerfilUsuario set Nome = @Nome where PerfilUsuarioId = @Id";
                            cmd.ParameterAdd("@Nome", pu.Nome);
                            cmd.ParameterAdd("@Id", pu.Id);
                            cmd.ExecuteNonQuery();
                        }
                        operacao = true;
                    }
                    ow.SaveChanges();
                }
                catch (Exception ex)
                {
                    ow.CancelChanges();
                    _logger.LogError(ex, "Erro ao salvar o perfil.");
                }
            }
            return operacao;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
