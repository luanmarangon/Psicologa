using Microsoft.Extensions.Logging;
using Psicologa.Domain.Convenio.Entities;
using Psicologa.Domain.Financeiro.Interfaces.Repositories;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using static Psicologa.Domain.Financeiro.Entities.Financeiro;

namespace Psicologa.Infra.Data.Repository.Financeiro
{
    public class FinanceiroRepository : RepositoryBase<Domain.Financeiro.Entities.Financeiro>, IFinanceiroRepository
    {
        private readonly ILogger<FinanceiroRepository> _logger;

        public FinanceiroRepository(IDBContextFactory dbContextFactory, ILogger<FinanceiroRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.Financeiro.Entities.Financeiro lancamento)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (lancamento.Id == 0)
                    {
                        cmd.CommandText = $@"
                                    INSERT INTO Financeiro (Tipo, Descricao, CategoriaId, Valor, DataLancamento, Observacao, Ativo, Quitado, DataQuitacao, DataCriacao, DataAtualizacao)
                                    VALUES (@Tipo, @Descricao, @CategoriaId, @Valor, @DataLancamento, @Observacao, @Ativo, @Quitado, @DataQuitacao, @DataCriacao, @DataAtualizacao)";
                    }
                    else
                    {
                        cmd.CommandText = $@"
                                    UPDATE Financeiro
                                    SET Tipo = @Tipo,
                                        Descricao = @Descricao,
                                        CategoriaId = @CategoriaId,
                                        Valor = @Valor,
                                        DataLancamento = @DataLancamento,
                                        Observacao = @Observacao,
                                        Ativo = @Ativo,
                                        Quitado = @Quitado,
                                        DataQuitacao = @DataQuitacao,
                                        DataAtualizacao = @DataAtualizacao
                                    WHERE FinanceiroId = @Id;";
                        cmd.ParameterAdd("@Id", lancamento.Id);
                    }

                    cmd.ParameterAdd("@Tipo", (int)lancamento.Tipo);
                    cmd.ParameterAdd("@Descricao", lancamento.Descricao);
                    cmd.ParameterAdd("@CategoriaId", lancamento.Categoria.Id);
                    cmd.ParameterAdd("@Valor", lancamento.Valor);
                    cmd.ParameterAdd("@DataLancamento", lancamento.DataLancamento);
                    cmd.ParameterAdd("@Observacao", lancamento.Observacao);
                    cmd.ParameterAdd("@Ativo", lancamento.Ativo);
                    cmd.ParameterAdd("@Quitado", lancamento.Quitado);
                    cmd.ParameterAdd("@DataQuitacao", lancamento.DataQuitacao != DateTime.MinValue ? lancamento.DataQuitacao : DBNull.Value);
                    cmd.ParameterAdd("@DataCriacao", lancamento.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", lancamento.DataAtualizacao);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (lancamento.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "select LAST_INSERT_ID();";
                            lancamento.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
             }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar lançamento financeiro");
            }
            return operacao;
        }

        public Domain.Financeiro.Entities.Financeiro Obter(int id)
        {
            Domain.Financeiro.Entities.Financeiro financeiro = null;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"
                                    SELECT f.FinanceiroId, f.Tipo, f.Descricao, f.CategoriaId, f.Valor, f.DataLancamento, f.Observacao, f.Ativo, f.Quitado, f.DataQuitacao, f.DataCriacao, f.DataAtualizacao, 
                                           fc.Nome AS CategoriaNome
                                        FROM Financeiro f
                                        JOIN FinanceiroCategoria fc on f.CategoriaId = fc.FinanceiroCategoriaId
                                        WHERE FinanceiroId = @Id";
                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Id", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        financeiro = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Convenio");
            }
            return financeiro;
        }

        public IEnumerable<Domain.Financeiro.Entities.Financeiro> Consultar(string termo, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.Financeiro.Entities.Financeiro> financas = new List<Domain.Financeiro.Entities.Financeiro>();

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
                                    SELECT f.FinanceiroId, f.Tipo, f.Descricao, f.CategoriaId, f.Valor, f.DataLancamento, f.Observacao, f.Ativo, f.Quitado, f.DataQuitacao, f.DataCriacao, f.DataAtualizacao, 
                                           fc.Nome AS CategoriaNome
                                        FROM Financeiro f
                                        JOIN FinanceiroCategoria fc on f.CategoriaId = fc.FinanceiroCategoriaId
                                    WHERE f.Descricao LIKE @Termo
                                    #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";

                    using (var dr = cmd.ExecuteReader())
                    {
                        financas = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Convenio");
            }
            return financas;
        }

        internal override IEnumerable<Domain.Financeiro.Entities.Financeiro> Map(System.Data.IDataReader dr)
        {
            List<Domain.Financeiro.Entities.Financeiro> financas = new List<Domain.Financeiro.Entities.Financeiro>();
            while (dr.Read())
            {
                int id = Convert.ToInt32(dr["FinanceiroId"]);
                Domain.Financeiro.Entities.Financeiro financeiro = financas.Find(c => c.Id == id);
                if (financeiro == null)
                {
                    financeiro = new Domain.Financeiro.Entities.Financeiro
                    {
                        Id = id,
                        Tipo = (Domain.Financeiro.Entities.Financeiro.TpTipoLancamento)Convert.ToInt32(dr["Tipo"]),
                        Descricao = dr["Descricao"].ToString(),
                        Categoria = new Domain.Financeiro.Entities.FinanceiroCategoria
                        {
                            Id = Convert.ToInt32(dr["CategoriaId"]),
                            Nome = dr["CategoriaNome"].ToString()
                        },
                        Valor = Convert.ToDecimal(dr["Valor"]),
                        DataLancamento = Convert.ToDateTime(dr["DataLancamento"]),
                        Observacao = dr["Observacao"].ToString(),
                        Ativo = Convert.ToBoolean(dr["Ativo"]),
                        Quitado = Convert.ToBoolean(dr["Quitado"]),
                        DataQuitacao = dr["DataQuitacao"] != DBNull.Value ? Convert.ToDateTime(dr["DataQuitacao"]) : DateTime.MinValue,
                        DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                        DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"])
                    };
                    financas.Add(financeiro);
                }
            }
            return financas;
        }


        public bool Excluir(int id)
        {
            bool operacao = false;
            try
            {
                using(var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"DELETE FROM Financeiro WHERE FinanceiroId = @Id";
                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Id", id);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        operacao = true;
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir Lançamento Financeiro");
            }


            return operacao;
        }

        public Domain.Financeiro.Entities.ResumoFinanceiro ObterResumo(DateTime dataInicio, DateTime dataFim)
        {
            Domain.Financeiro.Entities.ResumoFinanceiro resumo = new Domain.Financeiro.Entities.ResumoFinanceiro();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"
                            SELECT 
                                COALESCE(SUM(CASE WHEN Tipo = @TipoReceita THEN Valor ELSE 0 END), 0) AS TotalReceita,
                                COALESCE(SUM(CASE WHEN Tipo = @TipoDespesa THEN Valor ELSE 0 END), 0) AS TotalDespesa,
                                COALESCE(SUM(CASE WHEN Tipo = @TipoReceita THEN Valor ELSE 0 END), 0) 
                                    - COALESCE(SUM(CASE WHEN Tipo = @TipoDespesa THEN Valor ELSE 0 END), 0) AS Saldo
                            FROM Financeiro
                            WHERE Ativo = 1
                              AND DataLancamento BETWEEN @DataInicio AND @DataFim";
                    cmd.ParametersClear();
                    cmd.ParameterAdd("@DataInicio", dataInicio);
                    cmd.ParameterAdd("@DataFim", dataFim);
                    cmd.ParameterAdd("@TipoReceita", (int)TpTipoLancamento.Receita);
                    cmd.ParameterAdd("@TipoDespesa", (int)TpTipoLancamento.Despesa);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            resumo.TotalReceita = dr["TotalReceita"] != DBNull.Value ? Convert.ToDecimal(dr["TotalReceita"]) : 0;
                            resumo.TotalDespesa = dr["TotalDespesa"] != DBNull.Value ? Convert.ToDecimal(dr["TotalDespesa"]) : 0;
                            resumo.Saldo = dr["Saldo"] != DBNull.Value ? Convert.ToDecimal(dr["Saldo"]) : 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Resumo Financeiro");
            }
            return resumo;
        }

    }
}