using Microsoft.Extensions.Logging;
using Psicologa.Domain.Paciente.Interfaces.Repositories;
using Psicologa.Domain.Pessoa.Entities;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Infra.Data.Repository.Paciente
{
    public class PacienteRepository : RepositoryBase<Domain.Paciente.Entities.Paciente>, IPacienteRepository
    {
        private readonly ILogger<PacienteRepository> _logger;

        public PacienteRepository(IDBContextFactory dbContextFactory, ILogger<PacienteRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.Paciente.Entities.Paciente paciente)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (paciente.Id == 0)
                    {
                        cmd.CommandText = @"INSERT INTO Paciente (PessoaId, DataPrimeiraSessao, Ativo, Observacoes, ContatoEmergenciaNome, ContatoEmergenciaTelefone, Matricula, ResponsavelId, DataCriacao, DataAtualizacao)
                                            VALUES (@PessoaId, @DataPrimeiraSessao, @Ativo, @Observacoes, @ContatoEmergenciaNome, @ContatoEmergenciaTelefone, @Matricula, @ResponsavelId, @DataCriacao, @DataAtualizacao)";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE Paciente SET PessoaId = @PessoaId, DataPrimeiraSessao = @DataPrimeiraSessao, Ativo = @Ativo, Observacoes = @Observacoes,
                                            ContatoEmergenciaNome = @ContatoEmergenciaNome, ContatoEmergenciaTelefone = @ContatoEmergenciaTelefone, Matricula = @Matricula,
                                            ResponsavelId = @ResponsavelId, DataAtualizacao = @DataAtualizacao
                                            WHERE PacienteId = @PacienteId";
                        cmd.ParameterAdd("@PacienteId", paciente.Id);
                    }
                    cmd.ParameterAdd("PessoaId", paciente.Pessoa.Id);
                    cmd.ParameterAdd("DataPrimeiraSessao", paciente.DataPrimeiraSessao);
                    cmd.ParameterAdd("Ativo", paciente.Ativo);
                    cmd.ParameterAdd("Observacoes", paciente.Observacoes);
                    cmd.ParameterAdd("ContatoEmergenciaNome", paciente.ContatoEmergenciaNome);
                    cmd.ParameterAdd("ContatoEmergenciaTelefone", paciente.ContatoEmergenciaTelefone);
                    cmd.ParameterAdd("Matricula", paciente.Matricula);
                    cmd.ParameterAdd("ResponsavelId", paciente.Responsavel.Id);
                    cmd.ParameterAdd("DataCriacao", paciente.DataCriacao);
                    cmd.ParameterAdd("DataAtualizacao", paciente.DataAtualizacao);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (paciente.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "SELECT LAST_INSERT_ID()";
                            paciente.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar paciente.");
            }

            return operacao;
        }

        public IEnumerable<Domain.Paciente.Entities.Paciente> Consultar(string nome, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            IEnumerable<Domain.Paciente.Entities.Paciente> pessoas = new List<Domain.Paciente.Entities.Paciente>();

            string sqlFiltroPessoaTipo = "";

            if (tpPessoa != PessoaTipo.TpPessoa.Indefinido)
            {
                sqlFiltroPessoaTipo = " and pt.Tipo = " + (int)tpPessoa;
            }

            try
            {
                int pular = 0;
                if (paginacao.PaginaAtual > 0)
                    pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

                if (pular < 0)
                    pular = 0;

                using (var cmd = DbContext.CreateCommand())
                {
                    string consultaPrincipal =
                                $@"SELECT p.PacienteId, p.PessoaId, p.DataPrimeiraSessao, p.Ativo as PacienteAtivo, p.Observacoes, p.ContatoEmergenciaNome, p.ContatoEmergenciaTelefone, p.Matricula, p.ResponsavelId, p.DataCriacao, p.DataAtualizacao,
                                          pe.Nome, pe.DocIdNro,
                                          pt.Tipo AS PessoaTipo,
                                          r.PessoaId as ResponsavelPessoaId, r.Nome as ResponsavelPessoaNome, pr.ProntuarioId as ProntuarioIdPaciente
                                FROM Paciente p
                                INNER JOIN Pessoa pe ON pe.PessoaId = p.PessoaId
                                LEFT JOIN Pessoa r on p.ResponsavelId = r.PessoaId
                                LEFT JOIN PessoaTipo pt ON pt.PessoaId = pe.PessoaId
                                LEFT JOIN Prontuario pr ON p.PacienteId = pr.PacienteId
                                where pe.Nome Like @Nome {sqlFiltroPessoaTipo}
                                order by pe.Nome desc
                                #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Nome", "%" + nome.Trim() + "%");

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";

                    using (var dr = cmd.ExecuteReader())
                    {
                        pessoas = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar pessoas.");
            }
            finally
            {
            }

            return pessoas;
        }

        public Domain.Paciente.Entities.Paciente Obter(int id)
        {
            Domain.Paciente.Entities.Paciente paciente = new Domain.Paciente.Entities.Paciente();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @$"SELECT p.PacienteId, p.PessoaId, p.DataPrimeiraSessao, p.Ativo as PacienteAtivo, p.Observacoes, p.ContatoEmergenciaNome, p.ContatoEmergenciaTelefone, p.Matricula, p.ResponsavelId, p.DataCriacao, p.DataAtualizacao,
                                          pe.Nome, pe.DocIdNro,
                                          pt.Tipo AS PessoaTipo,
                                          r.PessoaId as ResponsavelPessoaId, r.Nome as ResponsavelPessoaNome, pr.ProntuarioId as ProntuarioIdPaciente
                                        FROM Paciente p
                                        INNER JOIN Pessoa pe ON pe.PessoaId = p.PessoaId
                                        LEFT JOIN Pessoa r on p.ResponsavelId = r.PessoaId
                                        LEFT JOIN PessoaTipo pt ON pt.PessoaId = pe.PessoaId
                                        LEFT JOIN Prontuario pr ON p.PacienteId = pr.PacienteId
                                        WHERE p.PacienteId = @Id";

                    var param = cmd.CreateParameter();

                    cmd.ParameterAdd("Id", id);

                    using (var dr = cmd.ExecuteReader())
                    {
                        paciente = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter paciente com ID {Id}", id);
                throw;
            }
            return paciente;
        }

        internal override IEnumerable<Domain.Paciente.Entities.Paciente> Map(IDataReader dr)
        {
            List<Domain.Paciente.Entities.Paciente> pacientes = new();

            while (dr.Read())
            {
                var paciente = new Domain.Paciente.Entities.Paciente
                {
                    Id = dr["PacienteId"] != DBNull.Value ? Convert.ToInt32(dr["PacienteId"]) : 0,

                    DataPrimeiraSessao = dr["DataPrimeiraSessao"] != DBNull.Value
                        ? Convert.ToDateTime(dr["DataPrimeiraSessao"])
                        : DateTime.MinValue,

                    Ativo = dr["PacienteAtivo"] != DBNull.Value
                        ? Convert.ToBoolean(dr["PacienteAtivo"])
                        : false,

                    Observacoes = dr["Observacoes"] != DBNull.Value
                        ? dr["Observacoes"].ToString()
                        : null,

                    ContatoEmergenciaNome = dr["ContatoEmergenciaNome"] != DBNull.Value
                        ? dr["ContatoEmergenciaNome"].ToString()
                        : null,

                    ContatoEmergenciaTelefone = dr["ContatoEmergenciaTelefone"] != DBNull.Value
                        ? dr["ContatoEmergenciaTelefone"].ToString()
                        : null,
                    Matricula = dr["Matricula"] != DBNull.Value
                        ? dr["Matricula"].ToString()
                        : null,
                    DataCriacao = dr["DataCriacao"] != DBNull.Value
                        ? Convert.ToDateTime(dr["DataCriacao"])
                        : DateTime.MinValue,

                    DataAtualizacao = dr["DataAtualizacao"] != DBNull.Value
                        ? Convert.ToDateTime(dr["DataAtualizacao"])
                        : DateTime.MinValue,

                    Pessoa = new Domain.Pessoa.Entities.Pessoa
                    {
                        Id = dr["PessoaId"] != DBNull.Value ? Convert.ToInt32(dr["PessoaId"]) : 0,
                        Nome = dr["Nome"]?.ToString(),
                        DocIdNro = dr["DocIdNro"]?.ToString()
                    },

                    Responsavel = dr["ResponsavelPessoaId"] != DBNull.Value ? new Domain.Pessoa.Entities.Pessoa
                    {
                        Id = Convert.ToInt32(dr["ResponsavelPessoaId"]),
                        Nome = dr["ResponsavelPessoaNome"]?.ToString()
                    } : null, 

                    Prontuario = dr["ProntuarioIdPaciente"] != DBNull.Value ? new Domain.Prontuario.Entities.Prontuario
                    {
                        Id = Convert.ToInt32(dr["ProntuarioIdPaciente"])
                    } : null

                };

                pacientes.Add(paciente);
            }

            return pacientes;
        }
    }
}