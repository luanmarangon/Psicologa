using Microsoft.Extensions.Logging;
using Psicologa.Domain.ProntuarioSessao.Interfaces.Repositories;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Psicologa.Infra.Data.Repository.ProntuarioSessao
{
    public class ProntuarioSessaoRepository : RepositoryBase<Domain.ProntuarioSessao.Entities.ProntuarioSessao>, IProntuarioSessaoRepository
    {
        private readonly ILogger _logger;

        public ProntuarioSessaoRepository(IDBContextFactory dbContextFactory, ILogger<ProntuarioSessaoRepository> logger)
             : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool EvoluirSessao(Domain.ProntuarioSessao.Entities.ProntuarioSessao sessao)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (sessao.Id == 0)
                    {
                        cmd.CommandText = $@"
                            INSERT INTO ProntuarioSessao (ProntuarioId, AgendamentoId, DataSessao, HoraInicio, HoraFim, PsicologaId, TipoAtendimento, Evolucao, DataCriacao, DataAtualizacao)
                            VALUES (@ProntuarioId, @AgendamentoId, @DataSessao, @HoraInicio, @HoraFim, @PsicologaId, @TipoAtendimento, @Evolucao, @DataCriacao, @DataAtualizacao);";
                    }
                    else
                    {
                        cmd.CommandText = $@"
                            UPDATE ProntuarioSessao
                            SET AgendamentoId = @AgendamentoId,
                                DataSessao = @DataSessao,
                                HoraInicio = @HoraInicio,
                                HoraFim = @HoraFim,
                                PsicologaId = @PsicologaId,
                                TipoAtendimento = @TipoAtendimento,
                                Evolucao = @Evolucao,
                                DataCriacao = @DataCriacao,
                                DataAtualizacao = @DataAtualizacao
                            WHERE ProntuarioSessaoId = @ProntuarioSessaoId;";

                        cmd.ParameterAdd("@ProntuarioSessaoId", sessao.Id);
                    }

                    cmd.ParameterAdd("@ProntuarioId", sessao.Prontuario.Id);
                    cmd.ParameterAdd("@AgendamentoId", sessao.Agendamento.Id);
                    cmd.ParameterAdd("@DataSessao", sessao.DataSessao);
                    cmd.ParameterAdd("@HoraInicio", sessao.HoraInicio);
                    cmd.ParameterAdd("@HoraFim", sessao.HoraFim);
                    cmd.ParameterAdd("@PsicologaId", sessao.Psicologa.Id);
                    cmd.ParameterAdd("@TipoAtendimento", sessao.TipoAtendimento);
                    cmd.ParameterAdd("@Evolucao", sessao.Evolucao);
                    cmd.ParameterAdd("@DataCriacao", sessao.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", sessao.DataAtualizacao);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (sessao.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "SELECT LAST_INSERT_ID();";
                            sessao.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar prontuário para sessão com ID {SessaoId}", sessao.Id);
            }

            return operacao;
        }

        public IEnumerable<Domain.ProntuarioSessao.Entities.ProntuarioSessao> Consultar(string termo, int prontuarioId, int filtro, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.ProntuarioSessao.Entities.ProntuarioSessao> sessoes = new List<Domain.ProntuarioSessao.Entities.ProntuarioSessao>();
            string filtroConsulta = string.Empty;
            try
            {
                if (filtro > 0)
                    filtroConsulta = $@" AND ps.TipoAtendimento = {filtro}";

                int pular = 0;
                if (paginacao.PaginaAtual > 0)
                    pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

                if (pular < 0)
                    pular = 0;

                using (var cmd = DbContext.CreateCommand())
                {
                    string consultaPrincipal = $@"
                                    SELECT
                                        ps.ProntuarioSessaoId, ps.ProntuarioId AS ProntuarioSessaoProntuarioId, ps.AgendamentoId AS ProntuarioSessaoAgendamentoId, ps.DataSessao, ps.HoraInicio, ps.HoraFim,
                                        ps.PsicologaId, ps.TipoAtendimento,  ps.Evolucao, ps.DataCriacao AS ProntuarioSessaoDataCriacao, ps.DataAtualizacao AS ProntuarioSessaoDataAtualizacao,
                                        pr.ProntuarioId,
                                        p.PessoaId, p.Nome as PsicologoNome
                                    FROM prontuariosessao ps
                                    JOIN Prontuario pr on ps.ProntuarioId = pr.ProntuarioId
                                    JOIN Pessoa p on ps.PsicologaId = p.PessoaId
                                    LEFT JOIN Agendamento a on ps.AgendamentoId = a.AgendamentoId
                                    WHERE (p.Nome LIKE @Termo || ps.DataSessao LIKE @Termo || ps.HoraInicio LIKE @Termo || ps.HoraFim LIKE @Termo) 
                                        AND pr.ProntuarioId = @ProntuarioId {filtroConsulta}
                                    #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");
                    cmd.ParameterAdd("@ProntuarioId", prontuarioId);

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";

                    using (var dr = cmd.ExecuteReader())
                    {
                        sessoes = Map(dr).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar ProntuarioSessao");
            }
            return sessoes;
        }

        public bool ExcluirSessao(int id)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ProntuarioSessao WHERE ProntuarioSessaoId = @Id";
                    cmd.ParameterAdd("@Id", id);
                    operacao = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir sessão com ID {SessaoId}", id);
            }
            return operacao;
        }

        public Domain.ProntuarioSessao.Entities.ProntuarioSessao ObterSessao(int prontuarioId)
        {
            Domain.ProntuarioSessao.Entities.ProntuarioSessao prontuario = new Domain.ProntuarioSessao.Entities.ProntuarioSessao();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"
                            SELECT
                                        ps.ProntuarioSessaoId, ps.ProntuarioId AS ProntuarioSessaoProntuarioId, ps.AgendamentoId AS ProntuarioSessaoAgendamentoId, ps.DataSessao, ps.HoraInicio, ps.HoraFim,
                                        ps.PsicologaId, ps.TipoAtendimento,  ps.Evolucao, ps.DataCriacao AS ProntuarioSessaoDataCriacao, ps.DataAtualizacao AS ProntuarioSessaoDataAtualizacao,
                                        pr.ProntuarioId,
                                        p.PessoaId, p.Nome as PsicologoNome
                                    FROM prontuariosessao ps
                                    JOIN Prontuario pr on ps.ProntuarioId = pr.ProntuarioId
                                    JOIN Pessoa p on ps.PsicologaId = p.PessoaId
                                    LEFT JOIN Agendamento a on ps.AgendamentoId = a.AgendamentoId
                                    WHERE ps.ProntuarioSessaoId = @Id";
                    cmd.ParameterAdd("@Id", prontuarioId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        prontuario = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter prontuário com ID {ProntuarioId}", prontuarioId);
            }

            return prontuario;
        }

        //public IEnumerable<Domain.Prontuario.Entities.Prontuario> ObterPorPaciente(int pacienteId)
        //{
        //    throw new NotImplementedException();
        //}

        internal override IEnumerable<Domain.ProntuarioSessao.Entities.ProntuarioSessao> Map(IDataReader dr)
        {
            List<Domain.ProntuarioSessao.Entities.ProntuarioSessao> sessaoList = new List<Domain.ProntuarioSessao.Entities.ProntuarioSessao>();
            while (dr.Read())
            {
                Domain.ProntuarioSessao.Entities.ProntuarioSessao prontuarioSessao = new Domain.ProntuarioSessao.Entities.ProntuarioSessao
                {
                    Id = Convert.ToInt32(dr["ProntuarioSessaoId"]),
                    Prontuario = new Domain.Prontuario.Entities.Prontuario
                    {
                        Id = Convert.ToInt32(dr["ProntuarioSessaoProntuarioId"]),
                    },
                    Agendamento = dr["ProntuarioSessaoAgendamentoId"] == DBNull.Value
                    ? null
                    : new Domain.Agendamento.Entities.Agendamento
                    {
                        Id = Convert.ToInt32(dr["ProntuarioSessaoAgendamentoId"])
                    },
                    Psicologa = new Domain.Pessoa.Entities.Pessoa
                    {
                        Id = Convert.ToInt32(dr["PsicologaId"]),
                        Nome = dr["PsicologoNome"].ToString()
                    },
                    DataSessao = Convert.ToDateTime(dr["DataSessao"]),
                    HoraInicio = TimeSpan.Parse(dr["HoraInicio"].ToString()),
                    HoraFim = TimeSpan.Parse(dr["HoraFim"].ToString()),
                    TipoAtendimento = (Domain.Agendamento.Entities.Agendamento.tpFiltro)Convert.ToInt32(dr["TipoAtendimento"]),
                    Evolucao = dr["Evolucao"].ToString(),
                    DataCriacao = Convert.ToDateTime(dr["ProntuarioSessaoDataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["ProntuarioSessaoDataAtualizacao"]),
                };
                sessaoList.Add(prontuarioSessao);
            }
            return sessaoList;
        }
    }
}