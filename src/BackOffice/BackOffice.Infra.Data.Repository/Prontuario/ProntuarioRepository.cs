using Microsoft.Extensions.Logging;
using Psicologa.Domain.Prontuario.Interfaces.Repositories;
using Psicologa.Domain.Servico.Entities;
using Shared.Infra.Data.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Psicologa.Infra.Data.Repository.Prontuario
{
    public class ProntuarioRepository : RepositoryBase<Domain.Prontuario.Entities.Prontuario>, IProntuarioRepository
    {
        private readonly ILogger<ProntuarioRepository> _logger;

        public ProntuarioRepository(IDBContextFactory dbContextFactory, ILogger<ProntuarioRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.Prontuario.Entities.Prontuario prontuario)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (prontuario.Id == 0)
                    {
                        cmd.CommandText = $@"
                            INSERT INTO Prontuario (PacienteId, QueixaPrincipal, ObjetivoTratamento, HistoricoFamiliar, ObservacoesIniciais, Ativo, DataCriacao, DataAtualizacao)
                            VALUES (@PacienteId, @QueixaPrincipal, @ObjetivoTratamento, @HistoricoFamiliar, @ObservacoesIniciais, @Ativo, @DataCriacao, @DataAtualizacao);";
                    }
                    else
                    {
                        cmd.CommandText = $@"
                            UPDATE Prontuario
                            SET PacienteId = @PacienteId,
                                QueixaPrincipal = @QueixaPrincipal,
                                ObjetivoTratamento = @ObjetivoTratamento,
                                HistoricoFamiliar = @HistoricoFamiliar,
                                ObservacoesIniciais = @ObservacoesIniciais,
                                Ativo = @Ativo,
                                DataCriacao = @DataCriacao,
                                DataAtualizacao = @DataAtualizacao
                            WHERE ProntuarioId = @ProntuarioId;";

                        cmd.ParameterAdd("@ProntuarioId", prontuario.Id);
                    }

                    cmd.ParameterAdd("@PacienteId", prontuario.Paciente.Id);
                    cmd.ParameterAdd("@QueixaPrincipal", prontuario.QueixaPrincipal);
                    cmd.ParameterAdd("@ObjetivoTratamento", prontuario.ObjetivoTratamento);
                    cmd.ParameterAdd("@HistoricoFamiliar", prontuario.HistoricoFamiliar);
                    cmd.ParameterAdd("@ObservacoesIniciais", prontuario.ObservacoesIniciais);
                    cmd.ParameterAdd("@Ativo", prontuario.Ativo);
                    cmd.ParameterAdd("@DataCriacao", prontuario.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", prontuario.DataAtualizacao);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (prontuario.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "SELECT LAST_INSERT_ID();";
                            prontuario.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar prontuário para paciente com ID {PacienteId}", prontuario.Paciente.Id);
            }

            return operacao;
        }

        public Domain.Prontuario.Entities.Prontuario Obter(int prontuarioId)
        {
            //VALIDAR ESTA ERRADO O SELECT
            Domain.Prontuario.Entities.Prontuario prontuario = new Domain.Prontuario.Entities.Prontuario();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"
                            SELECT pr.ProntuarioId, pr.PacienteId as PacienteId, pr.QueixaPrincipal, pr.ObjetivoTratamento, pr.HistoricoFamiliar, pr.ObservacoesIniciais,
                                pr.Ativo As ProntuarioAtivo, pr.DataCriacao as DataCriacaoProntuario,
                                pr.DataAtualizacao as DataAtualizacaoProntuario, pr.DataEncerramento as DataEncerramentoProntuario,
                                pa.PacienteId as PacienteIdPaciente, pa.Matricula,
                                p.PessoaId as PessoaIdPessoa, p.Nome as PessoaNome
                            FROM prontuario pr
                            INNER JOIN Paciente pa on pr.PacienteId = pa.PacienteId
                            INNER JOIN Pessoa p on pa.PessoaId = p.PessoaId
                            WHERE pa.PacienteId = @Id";
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
        public Domain.Prontuario.Entities.Prontuario ObterProntuarioPorPacienteId(int pacienteId)
        {
            Domain.Prontuario.Entities.Prontuario prontuario = new Domain.Prontuario.Entities.Prontuario();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"
                            SELECT pr.ProntuarioId, pr.PacienteId as PacienteId, pr.QueixaPrincipal, pr.ObjetivoTratamento, pr.HistoricoFamiliar, pr.ObservacoesIniciais,
                                pr.Ativo As ProntuarioAtivo, pr.DataCriacao as DataCriacaoProntuario,
                                pr.DataAtualizacao as DataAtualizacaoProntuario, pr.DataEncerramento as DataEncerramentoProntuario,
                                pa.PacienteId as PacienteIdPaciente, pa.Matricula,
                                p.PessoaId as PessoaIdPessoa, p.Nome as PessoaNome
                            FROM prontuario pr
                            INNER JOIN Paciente pa on pr.PacienteId = pa.PacienteId
                            INNER JOIN Pessoa p on pa.PessoaId = p.PessoaId
                            WHERE pa.PacienteId = @Id";
                    cmd.ParameterAdd("@Id", pacienteId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        prontuario = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter prontuário com ID {ProntuarioId}", pacienteId);
            }

            return prontuario;
        }

        public IEnumerable<Domain.Prontuario.Entities.Prontuario> ObterPorPaciente(int pacienteId)
        {
            throw new NotImplementedException();
        }

        internal override IEnumerable<Domain.Prontuario.Entities.Prontuario> Map(IDataReader dr)
        {
            List<Domain.Prontuario.Entities.Prontuario> servicos = new List<Domain.Prontuario.Entities.Prontuario>();
            while (dr.Read())
            {
                Domain.Prontuario.Entities.Prontuario prontuario = new Domain.Prontuario.Entities.Prontuario
                {
                    Id = Convert.ToInt32(dr["ProntuarioId"]),
                    Paciente = new Domain.Paciente.Entities.Paciente
                    {
                        Id = Convert.ToInt32(dr["PacienteId"]),
                        Pessoa = new Domain.Pessoa.Entities.Pessoa
                        {
                            Id = Convert.ToInt32(dr["PessoaIdPessoa"]),
                            Nome = Convert.ToString(dr["PessoaNome"])
                        },
                        Matricula = dr["Matricula"].ToString(),

                    },
                    QueixaPrincipal = Convert.ToString(dr["QueixaPrincipal"]),
                    ObjetivoTratamento = Convert.ToString(dr["ObjetivoTratamento"]),
                    HistoricoFamiliar = Convert.ToString(dr["HistoricoFamiliar"]),
                    ObservacoesIniciais = Convert.ToString(dr["ObservacoesIniciais"]),
                    Ativo = Convert.ToBoolean(dr["ProntuarioAtivo"]),
                    DataCriacao = Convert.ToDateTime(dr["DataCriacaoProntuario"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacaoProntuario"]),
                    DataEncerramento = dr["DataEncerramentoProntuario"] != DBNull.Value ? Convert.ToDateTime(dr["DataEncerramentoProntuario"]) : (DateTime?)null
                };
                servicos.Add(prontuario);
            }
            return servicos;
        }
    }
}