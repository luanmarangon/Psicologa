using Microsoft.Extensions.Logging;
using Psicologa.Domain.Agendamento.Entities;
using Psicologa.Domain.Agendamento.Interfaces.Repositories;
using Psicologa.Domain.Servico.Entities;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Psicologa.Infra.Data.Repository.Agendamento
{
    public class AgendamentoRepository : RepositoryBase<Domain.Agendamento.Entities.Agendamento>, IAgendamentoRepository
    {
        private readonly ILogger<AgendamentoRepository> _logger;

        public AgendamentoRepository(IDBContextFactory dbContextFactory, ILogger<AgendamentoRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }
        public bool Salvar(Domain.Agendamento.Entities.Agendamento agendamento)
        {
            bool operacao = false;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (agendamento.Id == 0)
                    {
                        cmd.CommandText = $@"Insert into Agendamento (PacienteId, PsicologoId, ServicoId, DataConsulta, HoraInicio, HoraFim, TempoSessao, Online, Presencial, DataCriacao, DataAtualizacao, StatusAgendamento, TipoAgendamento, Ativo, ConfirmouAgendamento, DataConfirmacao)
                                            values (@PacienteId, @PsicologoId, @ServicoId, @DataConsulta, @HoraInicio, @HoraFim, @TempoSessao, @Online, @Presencial, @DataCriacao, @DataAtualizacao, @StatusAgendamento, @TipoAgendamento, @Ativo, @ConfirmouAgendamento, @DataConfirmacao)";
                    }
                    else
                    {
                        cmd.CommandText = $@"Update Agendamento set 
                                                    PacienteId = @PacienteId, 
                                                    PsicologoId = @PsicologoId, 
                                                    ServicoId = @ServicoId, 
                                                    DataConsulta = @DataConsulta, 
                                                    HoraInicio = @HoraInicio, 
                                                    HoraFim = @HoraFim, 
                                                    TempoSessao = @TempoSessao, 
                                                    Online = @Online, 
                                                    Presencial = @Presencial, 
                                                    DataAtualizacao = @DataAtualizacao, 
                                                    StatusAgendamento = @StatusAgendamento, 
                                                    TipoAgendamento = @TipoAgendamento, 
                                                    Ativo = @Ativo,
                                                    ConfirmouAgendamento = @ConfirmouAgendamento,
                                                    DataConfirmacao = @DataConfirmacao
                                            where AgendamentoId = @AgendamentoId";
                        cmd.ParameterAdd("@AgendamentoId", agendamento.Id);
                    }
                
                    cmd.ParameterAdd("@PacienteId", agendamento.Paciente.Id);
                    cmd.ParameterAdd("@PsicologoId", agendamento.Psicologo.Id);
                    cmd.ParameterAdd("@ServicoId", agendamento.Servico.Id);
                    cmd.ParameterAdd("@DataConsulta", agendamento.DataConsulta);
                    cmd.ParameterAdd("@HoraInicio", agendamento.HoraInicio);
                    cmd.ParameterAdd("@HoraFim", agendamento.HoraFim);
                    cmd.ParameterAdd("@TempoSessao", agendamento.TempoSessao);
                    cmd.ParameterAdd("@Online", agendamento.Online);
                    cmd.ParameterAdd("@Presencial", agendamento.Presencial);
                    cmd.ParameterAdd("@DataCriacao", agendamento.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", agendamento.DataAtualizacao);
                    cmd.ParameterAdd("@StatusAgendamento", (int)agendamento.StatusAgendamento);
                    cmd.ParameterAdd("@TipoAgendamento", (int)agendamento.TipoAgendamento);
                    cmd.ParameterAdd("@Ativo", agendamento.Ativo);
                    cmd.ParameterAdd("@ConfirmouAgendamento", agendamento.ConfirmouAgendamento);
                    cmd.ParameterAdd("@DataConfirmacao", agendamento.DataConfirmacao);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (agendamento.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "SELECT LAST_INSERT_ID();";
                            agendamento.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar agendamento.");
            }

            return operacao;
        }
        public IEnumerable<Disponibilidade> ObterDisponibilidade(int psicologoId, DateTime dataConsulta)
        {
            List<Domain.Agendamento.Entities.Disponibilidade> p = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                                $@"select AgendamentoId, PsicologoId, DataConsulta, HoraInicio, HoraFim from Agendamento where PsicologoId = @psicologoId and Date(DataConsulta) = @dataConsulta;";

                    cmd.ParameterAdd("@psicologoId", psicologoId);
                    cmd.ParameterAdd("@dataConsulta", dataConsulta);
                    using (var dr = cmd.ExecuteReader())
                    {
                        p = MapDisponibilidade(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a disponibilidade.");
            }

            return p;
        }
        public IEnumerable<Domain.Agendamento.Entities.Agendamento> Consultar(string termo, Domain.Agendamento.Entities.Agendamento.tpFiltro filtro, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.Agendamento.Entities.Agendamento> agendamentos = new List<Domain.Agendamento.Entities.Agendamento>();
            try
            {
                string filtroConsulta = string.Empty;

                if(filtro == Domain.Agendamento.Entities.Agendamento.tpFiltro.Presencial)
                    filtroConsulta = "and a.Presencial = 1";
                else if(filtro == Domain.Agendamento.Entities.Agendamento.tpFiltro.Online)
                    filtroConsulta = "and a.Online = 1";


                int pular = 0;
                if (paginacao.PaginaAtual > 0)
                    pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

                if (pular < 0)
                    pular = 0;

                using (var cmd = DbContext.CreateCommand())
                {
                    string consultaPrincipal =
                                            $@"select a.AgendamentoId, a.PacienteId, a.PsicologoId, a.ServicoId, a.DataConsulta, a.HoraInicio, a.HoraFim,
                                            a.TempoSessao, a.Online, a.Presencial, a.DataCriacao, a.DataAtualizacao, a.StatusAgendamento, a.TipoAgendamento, a.Ativo, 
                                            a.ConfirmouAgendamento, a.DataConfirmacao,
                            p.Nome as PacienteNome, ps.Nome as PsicologoNome, s.Nome as ServicoNome
                            from Agendamento a
                            inner join Pessoa p on p.PessoaId = a.PacienteId
                            inner join Pessoa ps on ps.PessoaId = a.PsicologoId
                            inner join Servico s on a.ServicoId = s.ServicoId
                            where (p.Nome like @termo or ps.Nome like @termo) {filtroConsulta}
                            #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";

                    using (var dr = cmd.ExecuteReader())
                    {
                        agendamentos = Map(dr).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar agendamento.");
            }
            return agendamentos;
        }
        public Domain.Agendamento.Entities.Agendamento ObterPorId(int id)
        {
            Domain.Agendamento.Entities.Agendamento agendamento = new Domain.Agendamento.Entities.Agendamento();
            try
            {
                using(var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                                   $@"select a.AgendamentoId, a.PacienteId, a.PsicologoId, a.ServicoId, a.DataConsulta, a.HoraInicio, a.HoraFim,
                                            a.TempoSessao, a.Online, a.Presencial, a.DataCriacao, a.DataAtualizacao, a.StatusAgendamento, a.TipoAgendamento, a.Ativo,
                                            a.ConfirmouAgendamento, a.DataConfirmacao,
                            p.Nome as PacienteNome, ps.Nome as PsicologoNome, s.Nome as ServicoNome
                            from Agendamento a
                            inner join Pessoa p on p.PessoaId = a.PacienteId
                            inner join Pessoa ps on ps.PessoaId = a.PsicologoId
                            inner join Servico s on a.ServicoId = s.ServicoId
                            where AgendamentoId = @id";
                    cmd.ParameterAdd("@id", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        agendamento = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter agendamento por id.");
            }
            return agendamento;
        }
        public bool Excluir(int id)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"Delete from Agendamento where AgendamentoId = @id";
                    cmd.ParameterAdd("@id", id);
                    operacao = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir agendamento.");
            }
            return operacao;
        }
        internal override List<Domain.Agendamento.Entities.Agendamento> Map(IDataReader dr)
        {
            List<Domain.Agendamento.Entities.Agendamento> agendamentos = new();
            while (dr.Read())
            {
                agendamentos.Add(new Domain.Agendamento.Entities.Agendamento
                {
                    Id = Convert.ToInt32(dr["AgendamentoId"]),
                    DataConsulta = Convert.ToDateTime(dr["DataConsulta"]),
                    HoraInicio = dr["HoraInicio"].ToString(),
                    HoraFim = dr["HoraFim"].ToString(),
                    TempoSessao = Convert.ToInt32(dr["TempoSessao"]),
                    Online = Convert.ToBoolean(dr["Online"]),
                    Presencial = Convert.ToBoolean(dr["Presencial"]),
                    Paciente = new Domain.Pessoa.Entities.Pessoa
                    {
                        Id = Convert.ToInt32(dr["PacienteId"]),
                        Nome = dr["PacienteNome"].ToString()
                    },
                    Psicologo = new Domain.Pessoa.Entities.Pessoa
                    {
                        Id = Convert.ToInt32(dr["PsicologoId"]),
                        Nome = dr["PsicologoNome"].ToString()
                    },
                    Servico = new Domain.Servico.Entities.Servico
                    {
                        Id = Convert.ToInt32(dr["ServicoId"]),
                        Nome = dr["ServicoNome"].ToString()
                    },

                    DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"]),
                    StatusAgendamento = (Domain.Agendamento.Entities.Agendamento.TpStatusAgendamento)Convert.ToInt32(dr["StatusAgendamento"]),
                    TipoAgendamento = (Domain.Agendamento.Entities.Agendamento.TpTipoAgendamento)Convert.ToInt32(dr["TipoAgendamento"]),
                    Ativo = Convert.ToBoolean(dr["Ativo"]),
                    ConfirmouAgendamento = Convert.ToBoolean(dr["ConfirmouAgendamento"]),
                    DataConfirmacao = dr["DataConfirmacao"] != DBNull.Value ? Convert.ToDateTime(dr["DataConfirmacao"]) : (DateTime?)null
                    //DataConfirmacao = dr["DataConfirmacao"] as DateTime?,
                });
            }
            return agendamentos;
        }
        internal List<Domain.Agendamento.Entities.Disponibilidade> MapDisponibilidade(IDataReader dr)
        {
            List<Domain.Agendamento.Entities.Disponibilidade> disponibilidades = new();

            while (dr.Read())
            {
                int id = Convert.ToInt32(dr["AgendamentoId"]);

                Domain.Agendamento.Entities.Disponibilidade d =
                    disponibilidades.Find(i => i.Id == id);

                if (d == null)
                {
                    d = new Domain.Agendamento.Entities.Disponibilidade
                    {
                        Id = id,

                        Psicologo = new Domain.Pessoa.Entities.Pessoa
                        {
                            Id = Convert.ToInt32(dr["PsicologoId"]),
                        },

                        DataConsulta =
                            Convert.ToDateTime(dr["DataConsulta"]),

                        HorariosAgendados =
                            new List<HorarioAgendado>()
                    };

                    disponibilidades.Add(d);
                }

                d.HorariosAgendados.Add(
                    new HorarioAgendado
                    {
                        HoraInicio = ((TimeSpan)dr["HoraInicio"]).ToString(@"hh\:mm"),
                        HoraFim = ((TimeSpan)dr["HoraFim"]).ToString(@"hh\:mm")
                    });
            }

            return disponibilidades;
        }
    }
}