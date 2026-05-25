using Microsoft.Extensions.Logging;
using Psicologa.Domain.LogAplicacao.Interfaces.Repositories;
using Shared.Infra.Data.Providers;
using System;

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
                    cmd.ParameterAdd("@Dispositivo", log.Dispostivo);
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
    }
}