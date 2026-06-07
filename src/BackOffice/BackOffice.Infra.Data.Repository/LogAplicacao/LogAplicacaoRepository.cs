using Microsoft.Extensions.Logging;
using Psicologa.Domain.LogAplicacao.Interfaces.Repositories;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Psicologa.Infra.Data.Repository.LogAplicacao
{
    public class LogAplicacaoRepository : RepositoryBase<Domain.LogAplicacao.Entities.LogAplicacao>, ILogAplicacaoRepository
    {
        private readonly ILogger<LogAplicacaoRepository> _logger;

        public LogAplicacaoRepository(IDBContextFactory dbContextFactory, ILogger<LogAplicacaoRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.LogAplicacao.Entities.LogAplicacao log)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Log(DataCriacao, UsuarioId, UsuarioNome, Dispositivo, IP, UserAgent, Entidade, EntidadeId, Operacao,
                                                        Aplicacao, Metodo, DadosAntes, DadosDepois, DadosAlterados)
                                        VALUES(@DataCriacao, @UsuarioId, @UsuarioNome, @Dispositivo, @IP, @UserAgent, @Entidade, @EntidadeId, @Operacao,
                                               @Aplicacao, @Metodo, @DadosAntes, @DadosDepois, @DadosAlterados)";

                    cmd.ParameterAdd("@DataCriacao", log.DataCriacao);
                    cmd.ParameterAdd("@UsuarioId", log.UsuarioId);
                    cmd.ParameterAdd("@UsuarioNome", log.UsuarioNome);
                    cmd.ParameterAdd("@Dispositivo", log.Dispositivo);
                    cmd.ParameterAdd("@IP", log.IP);
                    cmd.ParameterAdd("@UserAgent", log.UserAgent);
                    cmd.ParameterAdd("@Entidade", log.Entidade);
                    cmd.ParameterAdd("@EntidadeId", log.EntidadeId);
                    cmd.ParameterAdd("@Operacao", log.Operacao);
                    cmd.ParameterAdd("@Aplicacao", log.Aplicacao);
                    cmd.ParameterAdd("@Metodo", log.Metodo);
                    cmd.ParameterAdd("@DadosAntes", log.DadosAntes);
                    cmd.ParameterAdd("@DadosDepois", log.DadosDepois);
                    cmd.ParameterAdd("@DadosAlterados", log.DadosAlterados);
                    //cmd.ParameterAdd("@Sucesso", log.Sucesso);
                    //cmd.ParameterAdd("@MensagemErro", log.MensagemErro);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (log.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "select LAST_INSERT_ID();";
                            log.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar log de aplicação.");
            }
            return operacao;
        }

        public IEnumerable<Domain.LogAplicacao.Entities.LogAplicacao> ObterUltimos(int top)
        {
            IEnumerable<Domain.LogAplicacao.Entities.LogAplicacao> logs = new List<Domain.LogAplicacao.Entities.LogAplicacao>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT l.LogId, l.DataCriacao, l.UsuarioId, l.UsuarioNome, l.Dispositivo, l.IP, l.UserAgent, l.Entidade, l.EntidadeId,
                                                l.Operacao, l.Aplicacao, l.Metodo, l.DadosAntes, l.DadosDepois, l.DadosAlterados
                                        FROM log l
                                        ORDER BY DataCriacao
                                        DESC LIMIT {top}";
                    cmd.ParametersClear();
                    using (var dr = cmd.ExecuteReader())
                    {
                        logs = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter logs de aplicação.");
            }
            return logs;
        }
        public Domain.LogAplicacao.Entities.LogAplicacao Obter(int id)
        {
            Domain.LogAplicacao.Entities.LogAplicacao log = null;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT l.LogId, l.DataCriacao, l.UsuarioId, l.UsuarioNome, l.Dispositivo, l.IP, l.UserAgent, l.Entidade, l.EntidadeId,
                                                l.Operacao, l.Aplicacao, l.Metodo, l.DadosAntes, l.DadosDepois, l.DadosAlterados
                                        FROM log l
                                        WHERE l.LogId = @Id";
                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Id", id);

                    using (var dr = cmd.ExecuteReader())
                    {
                        log = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter logs de aplicação.");
            }
            return log;
        }

        public IEnumerable<Domain.LogAplicacao.Entities.LogAplicacao> Consultar(string termo, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.LogAplicacao.Entities.LogAplicacao> logs = new List<Domain.LogAplicacao.Entities.LogAplicacao>();
            try
            {
                int pular = 0;
                if (paginacao.PaginaAtual > 0)
                    pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

                if (pular < 0)
                    pular = 0;

                using (var cmd = DbContext.CreateCommand())
                {
                    string consultaPrincipal = $@"SELECT l.LogId, l.DataCriacao, l.UsuarioId, l.UsuarioNome, l.Dispositivo, l.IP, l.UserAgent, l.Entidade, l.EntidadeId,
                                                l.Operacao, l.Aplicacao, l.Metodo, l.DadosAntes, l.DadosDepois, l.DadosAlterados
                                        FROM log l
                                        WHERE (l.UsuarioNome LIKE @Termo || l.Entidade LIKE @Termo || l.Aplicacao LIKE @Termo || l.Metodo LIKE @Termo)
                                        #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";
                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";
                    
                    using (var dr = cmd.ExecuteReader())
                    {
                        logs = Map(dr).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar logs de aplicação.");
            }
            return logs;
        }

        internal override IEnumerable<Domain.LogAplicacao.Entities.LogAplicacao> Map(IDataReader dr)
        {
            List<Domain.LogAplicacao.Entities.LogAplicacao> logs = new List<Domain.LogAplicacao.Entities.LogAplicacao>();

            while (dr.Read())
            {
                int id = Convert.ToInt32(dr["LogId"]);
                Domain.LogAplicacao.Entities.LogAplicacao log = logs.Find(l => l.Id == id);

                if (log == null)
                {
                    log = new Domain.LogAplicacao.Entities.LogAplicacao();
                    log.Id = id;
                    log.DataCriacao = Convert.ToDateTime(dr["DataCriacao"]);
                    log.UsuarioId = Convert.ToInt32(dr["UsuarioId"]);
                    log.UsuarioNome = dr["UsuarioNome"].ToString();
                    log.Dispositivo = dr["Dispositivo"].ToString();
                    log.IP = dr["IP"].ToString();
                    log.UserAgent = dr["UserAgent"].ToString();
                    log.Entidade = dr["Entidade"].ToString();
                    log.EntidadeId = Convert.ToInt32(dr["EntidadeId"]);
                    log.Operacao = dr["Operacao"].ToString();
                    log.Aplicacao = dr["Aplicacao"].ToString();
                    log.Metodo = dr["Metodo"].ToString();
                    log.DadosAntes = dr["DadosAntes"].ToString();
                    log.DadosDepois = dr["DadosDepois"].ToString();
                    log.DadosAlterados = dr["DadosAlterados"].ToString();

                    logs.Add(log);
                }
            }
            return logs;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}