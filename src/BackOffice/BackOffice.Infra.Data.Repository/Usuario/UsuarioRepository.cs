using Microsoft.Extensions.Logging;
using Psicologa.Domain.Usuario.Entities;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace Psicologa.Infra.Data.Repository.Usuario
{
    public class UsuarioRepository : RepositoryBase<Domain.Usuario.Entities.Usuario>, Domain.Usuario.Interfaces.Repositories.IUsuarioRepository
    {
        private readonly ILogger<UsuarioRepository> _logger;
        Domain.Usuario.Entities.PerfilUsuario.TpPerfil[] _perfisHabilitados = new Domain.Usuario.Entities.PerfilUsuario.TpPerfil[] 
        { 
            Domain.Usuario.Entities.PerfilUsuario.TpPerfil.Master, 
            Domain.Usuario.Entities.PerfilUsuario.TpPerfil.Administrativo, 
            Domain.Usuario.Entities.PerfilUsuario.TpPerfil.Psicologo, 
            Domain.Usuario.Entities.PerfilUsuario.TpPerfil.Suporte
        };

        public UsuarioRepository(IDBContextFactory dbContextFactory, ILogger<UsuarioRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;

        }

        public bool Salvar(Domain.Usuario.Entities.Usuario usuario)
        {
            bool operacao = false;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (usuario.Id == 0)
                    {
                        cmd.CommandText = @"insert into Usuario (Nome, Senha, DtCadastro, DtSenha, PessoaId, Perfil, PrimeiroAcesso) 
                                            values (@Nome, @Senha, @DtCadastro, @DtSenha, @PessoaId, @Perfil, @PrimeiroAcesso)";
                        cmd.ParameterAdd("@PrimeiroAcesso", true);
                    }
                    else
                    {
                        cmd.CommandText = @"update Usuario 
                                            set Nome = @Nome, 
                                                PessoaId = @PessoaId,
                                                Perfil = @Perfil
                                            where UsuarioId = @Id"
                        ;
                        cmd.ParameterAdd("@Id", usuario.Id);
                    }

                    cmd.ParameterAdd("@Nome", usuario.Nome.ToLower());
                    cmd.ParameterAdd("@Senha", usuario.Senha.ToLower());
                    cmd.ParameterAdd("@DtCadastro", usuario.DataCadastro);
                    cmd.ParameterAdd("@DtSenha", usuario.DataSenha);
                    cmd.ParameterAdd("@PessoaId", usuario.Pessoa.Id);
                    cmd.ParameterAdd("@Perfil", (int)usuario.Perfil);
                    //cmd.ParameterAdd("@ClienteIdVinculo", DBUtils.ConvertIntZeroToNull(usuario.ClienteVinculado.Id));

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (usuario.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "select LAST_INSERT_ID();";
                            usuario.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar o Usuário.");
            }

            return operacao;
        }

        public Domain.Usuario.Entities.Usuario Obter(int id)
        {
            Domain.Usuario.Entities.Usuario u = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"select u.*,
                                  p.Nome as PessoaNome
                          from Usuario u
                               left join Pessoa p on p.PessoaId = u.PessoaId
                          where u.Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and
                                u.UsuarioId = @Id";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Id", id);

                    using (var dr = cmd.ExecuteReader())
                    {
                        u = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o Usuário.");
            }
           
            return u;
        }

        public Domain.Usuario.Entities.Usuario Obter(string usuarioNome, string senha)
        {
            Domain.Usuario.Entities.Usuario u = null;

            try
            {

                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"select u.*,
                                  p.Nome as PessoaNome
                          from Usuario u
                               left join Pessoa p on p.PessoaId = u.PessoaId
                          where u.Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and
                                u.Nome = @Nome and 
                                u.Senha = @Senha";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Nome", usuarioNome.Trim());
                    cmd.ParameterAdd("@Senha", senha.Trim());

                    using (var dr = cmd.ExecuteReader())
                    {
                        u = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o Usuário.");
            }
           

            return u;
        }

        public Domain.Usuario.Entities.Usuario ObterPorUsuarioNome(string usuarioNome)
        {
            Domain.Usuario.Entities.Usuario u = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"select u.*,
                                  p.Nome as PessoaNome
                          from Usuario u
                               left join Pessoa p on p.PessoaId = u.PessoaId
                          where u.Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and 
                                u.Nome = @Nome";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Nome", usuarioNome.Trim());

                    using (var dr = cmd.ExecuteReader())
                    {
                        u = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o Usuário.");
            }
            
            return u;
        }

        public Domain.Usuario.Entities.Usuario ObterPorPessoa(int pessoaId)
        {
            Domain.Usuario.Entities.Usuario u = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"select u.*,
                                  p.Nome as PessoaNome
                          from Usuario u
                               left join Pessoa p on p.PessoaId = u.PessoaId
                          where u.Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and 
                                u.PessoaId = @pessoaId";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@pessoaId", pessoaId);

                    using (var dr = cmd.ExecuteReader())
                    {
                        u = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o Usuário.");
            }
        
            return u;
        }

        public bool Excluir(int id)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"delete from Usuario
                                         where u.Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and  
                                               UsuarioId = @Id";
                    cmd.ParameterAdd("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                operacao = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o Usuário.");
            }
           

            return operacao;
        }

        public bool ExcluirPorPessoa(int pessoaId)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"delete from Usuario
                                         where u.Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and  
                                               PessoaId = @PessoaId";
                    cmd.ParameterAdd("@PessoaId", pessoaId);
                    cmd.ExecuteNonQuery();
                }
                operacao = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o Usuário.");
            }


            return operacao;
        }

        public IEnumerable<Domain.Usuario.Entities.Usuario> Consultar(string nome)
        {
            IEnumerable<Domain.Usuario.Entities.Usuario> us = new List<Domain.Usuario.Entities.Usuario>();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                          $@"select u.*,
                               p.Nome as PessoaNome
                          from Usuario u
                               left outer join Pessoa p on p.PessoaId = u.PessoaId
                          where u.Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and 
                                p.Nome Like @Nome 
                          order by p.Nome ASC";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Nome", "%" + nome.Trim() + "%");

                    using (var dr = cmd.ExecuteReader())
                    {
                        us = Map(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Usuários.");
            }
         

            return us;
        }

        public IEnumerable<Domain.Usuario.Entities.Usuario> ObterUltimos(int top)
        {
            IEnumerable<Domain.Usuario.Entities.Usuario> us = new List<Domain.Usuario.Entities.Usuario>();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                          $@"select u.*,
                               p.Nome as PessoaNome
                          from Usuario u
                               left join Pessoa p on p.PessoaId = u.PessoaId
                          where u.Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and p.Ativo = 1
                          order by p.PessoaId desc
                          limit {top}";


                    cmd.ParametersClear();

                    using (var dr = cmd.ExecuteReader())
                    {
                        us = Map(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Usuários.");
            }
            
            return us;
        }

        public void AtualizarCodigoSeguranca(int id, string codigoSeguranca)
        {
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"update Usuario set 
                                               CodigoSeguranca = @CodigoSeguranca
                                          where Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and 
                                                UsuarioId = @Id";
                    cmd.ParameterAdd("@CodigoSeguranca", codigoSeguranca);
                    cmd.ParameterAdd("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o código de segurança do usuário.", ex);
            }
          
        }

        public bool AlterarSenha(int id, string senha)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"update Usuario 
                                        set Senha = @Senha, DtSenha = @DtSenha, CodigoSeguranca = null, PrimeiroAcesso = false
                                        where Perfil in ({string.Join(',', _perfisHabilitados.Select(p => (int)p).ToArray())}) and 
                                              UsuarioId = @Id";
                    cmd.ParameterAdd("@Senha", senha);
                    cmd.ParameterAdd("@DtSenha", DateTime.Now);
                    cmd.ParameterAdd("@Id", id);
                    cmd.ExecuteNonQuery();
                    operacao = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar a senha do usuário.");
            }
          

            return operacao;
        }

        public int ObterQuantidadeUsuariosCadastrados()
        {
            int qtde = 0;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                        $@"select count(*) from Usuario";

                    qtde = Convert.ToInt32(cmd.ExecuteScalar());
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar a quantidade de usuários cadastrados.");
            }

            return qtde;
        }

        internal override IEnumerable<Domain.Usuario.Entities.Usuario> Map(IDataReader dr)
        {
            List<Domain.Usuario.Entities.Usuario> us = new List<Domain.Usuario.Entities.Usuario>();

            while (dr.Read())
            {
                int id = Convert.ToInt32(dr["UsuarioId"]);
                Domain.Usuario.Entities.Usuario u = us.Find(i => i.Id == id);

                if (u == null)
                {
                    u = new Domain.Usuario.Entities.Usuario();
                    u.Id = id;
                    u.Nome = dr["Nome"].ToString();
                    u.Senha = dr["Senha"].ToString();
                    u.DataSenha = dr["DtSenha"] != DBNull.Value ? Convert.ToDateTime(dr["DtSenha"]) : DateTime.MinValue;
                    u.DataCadastro = dr["DtCadastro"] != DBNull.Value ? Convert.ToDateTime(dr["DtCadastro"]) : DateTime.MinValue;
                    u.CodigoSeguranca = dr["CodigoSeguranca"].ToString();
                    u.Perfil = (Domain.Usuario.Entities.PerfilUsuario.TpPerfil)(Int16)dr["Perfil"];
                    //u.Perfil = dr["Perfil"].ToString();
                   
                    u.Pessoa = new Domain.Pessoa.Entities.Pessoa()
                    {
                        Id = Convert.ToInt32(dr["PessoaId"]),
                        Nome = dr["PessoaNome"].ToString()
                    };

                    us.Add(u);
                }
            }

            return us;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
