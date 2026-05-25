using Microsoft.Extensions.Logging;
using Psicologa.Domain.BlogPost.Entities;
using Psicologa.Domain.BlogPost.Interfaces.Repositories;
using Psicologa.Domain.Convenio.Interfaces.Repositories;
using Psicologa.Infra.Data.Repository.Pessoa;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Psicologa.Infra.Data.Repository.Convenio
{
    public class ConvenioRepository : RepositoryBase<Domain.Convenio.Entities.Convenio>, IConvenioRepository
    {
        private readonly ILogger<ConvenioRepository> _logger;

        public ConvenioRepository(IDBContextFactory dbContextFactory, ILogger<ConvenioRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.Convenio.Entities.Convenio convenio)
        {
            bool operacao = false;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (convenio.Id == 0)
                    {
                        cmd.CommandText = @"INSERT INTO Convenio (Nome, Icon, DestaqueHome, Ativo, DataCriacao, DataAtualizacao)
                                            VALUES (@Nome, @Icon, @DestaqueHome, @Ativo, @DataCriacao, @DataAtualizacao);";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE Convenio SET
                                                    Nome = @Nome,
                                                    Icon = @Icon,
                                                    DestaqueHome = @DestaqueHome,
                                                    Ativo = @Ativo,
                                                    DataAtualizacao = @DataAtualizacao
                                            WHERE ConvenioId = @Id;";
                        cmd.ParameterAdd("@Id", convenio.Id);
                    }

                    cmd.ParameterAdd("@Nome", convenio.Nome);
                    cmd.ParameterAdd("@Icon", convenio.Icon);
                    cmd.ParameterAdd("@DestaqueHome", convenio.DestaqueHome);
                    cmd.ParameterAdd("@DataCriacao", convenio.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", convenio.DataAtualizacao);
                    cmd.ParameterAdd("@Ativo", convenio.Ativo);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (convenio.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "select LAST_INSERT_ID();";
                            convenio.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar Convenio");
            }

            return operacao;
        }

        public IEnumerable<Domain.Convenio.Entities.Convenio> Consultar(string termo, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.Convenio.Entities.Convenio> convenios = new List<Domain.Convenio.Entities.Convenio>();

            try
            {
                int pular = 0;
                if (paginacao.PaginaAtual > 0)
                    pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

                if (pular < 0)
                    pular = 0;

                using (var cmd = DbContext.CreateCommand())
                {
                    string consultaPrincipal = $@"
                                    SELECT c.ConvenioId, c.Nome, c.Icon, c.DestaqueHome, c.Ativo, c.DataCriacao, c.DataAtualizacao
                                    FROM Convenio c
                                    WHERE c.Nome LIKE @Termo
                                    #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";

                    using (var dr = cmd.ExecuteReader())
                    {
                        convenios = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Convenio");
            }
            return convenios;
        }

        public IEnumerable<Domain.Convenio.Entities.Convenio> ConsultarUltimos(int quantidade)
        {
            IEnumerable<Domain.Convenio.Entities.Convenio> convenios = new List<Domain.Convenio.Entities.Convenio>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"
                                    SELECT c.ConvenioId, c.Nome, c.Icon, c.DestaqueHome, c.Ativo, c.DataCriacao, c.DataAtualizacao
                                    FROM Convenio c
                                    WHERE c.Ativo = 1
                                    ORDER BY c.DataCriacao DESC
                                    LIMIT {quantidade};";
                    cmd.ParametersClear();
                    using (var dr = cmd.ExecuteReader())
                    {
                        convenios = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar últimos Convenios");
            }
            return convenios;
        }
        public IEnumerable<Domain.Convenio.Entities.Convenio> ObterDestaquesHome()
        {
            IEnumerable<Domain.Convenio.Entities.Convenio> convenios = new List<Domain.Convenio.Entities.Convenio>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"
                                    SELECT c.ConvenioId, c.Nome, c.Icon, c.DestaqueHome, c.Ativo, c.DataCriacao, c.DataAtualizacao
                                    FROM Convenio c
                                    WHERE c.Ativo = 1 and c.DestaqueHome = 1
                                    ORDER BY c.DataCriacao DESC";
                    cmd.ParametersClear();
                    using (var dr = cmd.ExecuteReader())
                    {
                        convenios = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar últimos Convenios");
            }
            return convenios;
        }

        public Domain.Convenio.Entities.Convenio Obter(int convenioId)
        {
            Domain.Convenio.Entities.Convenio convenio = new Domain.Convenio.Entities.Convenio();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT c.ConvenioId, c.Nome, c.Icon, c.DestaqueHome, c.Ativo, c.DataCriacao, c.DataAtualizacao
                                    FROM Convenio c
                                    WHERE c.ConvenioId = @Id;";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Id", convenioId);

                    using (var dr = cmd.ExecuteReader())
                    {
                        convenio = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter Convenio");
            }

            return convenio;
        }

        public bool Excluir(int id)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Convenio WHERE ConvenioId = @ConvenioId;";
                    cmd.ParameterAdd("@ConvenioId", id);
                    operacao = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o serviço");
            }
            return operacao;
        }

        internal override IEnumerable<Domain.Convenio.Entities.Convenio> Map(IDataReader dr)
        {
            List<Domain.Convenio.Entities.Convenio> convenios = new List<Domain.Convenio.Entities.Convenio>();

            while (dr.Read())
            {
                Domain.Convenio.Entities.Convenio convenio = new Domain.Convenio.Entities.Convenio
                {
                    Id = Convert.ToInt32(dr["ConvenioId"]),
                    Nome = dr["Nome"].ToString(),
                    Icon = dr["Icon"].ToString(),
                    DestaqueHome = Convert.ToBoolean(dr["DestaqueHome"]),
                    Ativo = Convert.ToBoolean(dr["Ativo"]),
                    DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"]),
                };
                convenios.Add(convenio);
            }
            return convenios;
        }
    }
}