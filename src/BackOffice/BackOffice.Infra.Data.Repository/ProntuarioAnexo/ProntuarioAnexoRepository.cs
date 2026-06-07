using Microsoft.Extensions.Logging;
using Psicologa.Domain.Paciente.Entities;
using Psicologa.Domain.Pessoa.Entities;
using Psicologa.Domain.ProntuarioAnexo.Interfaces.Repositories;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Infra.Data.Repository.ProntuarioAnexo
{
    public class ProntuarioAnexoRepository : RepositoryBase<Domain.ProntuarioAnexo.Entities.ProntuarioAnexo>, IProntuarioAnexoRepository
    {
        private readonly ILogger _logger;

        public ProntuarioAnexoRepository(IDBContextFactory dbContextFactory, ILogger<ProntuarioAnexoRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public Domain.ProntuarioAnexo.Entities.ProntuarioAnexo Obter(int id)
        {
            Domain.ProntuarioAnexo.Entities.ProntuarioAnexo anexo = new Domain.ProntuarioAnexo.Entities.ProntuarioAnexo();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @$"SELECT pa.ProntuarioAnexoId, pa.TipoAnexo, pa.Nome, pa.NomeArquivo, pa.MimeType, pa.TamanhoArquivo, pa.Arquivo, pa.Observacao, pa.DataCriacao, pa.DataAtualizacao, pt.ProntuarioId as ProntuarioIdPaciente
                                   FROM prontuarioanexo pa
                                   JOIN Prontuario pt ON pa.ProntuarioId = pt.ProntuarioId
                                        WHERE pa.ProntuarioAnexoId = @Id";

                    var param = cmd.CreateParameter();

                    cmd.ParameterAdd("Id", id);

                    using (var dr = cmd.ExecuteReader())
                    {
                        anexo = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter paciente com ID {Id}", id);
                throw;
            }

            return anexo;
        }
        public IEnumerable<Domain.ProntuarioAnexo.Entities.ProntuarioAnexo> Consultar(string termo, int ProntuarioId, Domain.ProntuarioAnexo.Entities.ProntuarioAnexo.tpTipoAnexo tpAnexo, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.ProntuarioAnexo.Entities.ProntuarioAnexo> anexos = new List<Domain.ProntuarioAnexo.Entities.ProntuarioAnexo>();
            string sqlFiltroTipoAnexo = string.Empty;

            if (tpAnexo != Domain.ProntuarioAnexo.Entities.ProntuarioAnexo.tpTipoAnexo.Indefinido)
            {
                sqlFiltroTipoAnexo = "AND TipoAnexo = @TipoAnexo";
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
                                $@"SELECT pa.ProntuarioAnexoId, pa.TipoAnexo, pa.Nome, pa.NomeArquivo, pa.MimeType, pa.TamanhoArquivo, pa.Arquivo, pa.Observacao, pa.DataCriacao, pa.DataAtualizacao, pt.ProntuarioId as ProntuarioIdPaciente
                                   FROM prontuarioanexo pa
                                   JOIN Prontuario pt ON pa.ProntuarioId = pt.ProntuarioId
                                   where pa.Nome Like @Termo AND pa.ProntuarioId = @ProntuarioId {sqlFiltroTipoAnexo}
                                   order by pa.Nome desc
                                   #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");
                    cmd.ParameterAdd("@ProntuarioId", ProntuarioId);

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";

                    using (var dr = cmd.ExecuteReader())
                    {
                        anexos = Map(dr);
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

            return anexos;
        }
        public bool Salvar(Domain.ProntuarioAnexo.Entities.ProntuarioAnexo anexo)
        {
            bool operacao = false;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (anexo.Id == 0)
                    {
                        cmd.CommandText = @"INSERT INTO ProntuarioAnexo (ProntuarioId, TipoAnexo, Nome, NomeArquivo, MimeType, TamanhoArquivo, Arquivo, Observacao, DataCriacao, DataAtualizacao)
                                            VALUES (@ProntuarioId, @TipoAnexo, @Nome, @NomeArquivo, @MimeType, @TamanhoArquivo, @Arquivo, @Observacao, @DataCriacao, @DataAtualizacao);";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE ProntuarioAnexo
                                            SET ProntuarioId = @ProntuarioId,
                                                TipoAnexo = @TipoAnexo,
                                                Nome = @Nome,
                                                NomeArquivo = @NomeArquivo,
                                                MimeType = @MimeType,
                                                TamanhoArquivo = @TamanhoArquivo,
                                                Arquivo = @Arquivo,
                                                Observacao = @Observacao,
                                                DataAtualizacao = @DataAtualizacao
                                            WHERE ProntuarioAnexoId = @Id;";
                        cmd.ParameterAdd("@Id", anexo.Id);
                    }
                    cmd.ParameterAdd("@ProntuarioId", anexo.Prontuario.Id);
                    cmd.ParameterAdd("@TipoAnexo", (int)anexo.TipoAnexo);
                    cmd.ParameterAdd("@Nome", anexo.Nome);
                    cmd.ParameterAdd("@NomeArquivo", anexo.NomeArquivo);
                    cmd.ParameterAdd("@MimeType", anexo.MimeType);
                    cmd.ParameterAdd("@TamanhoArquivo", anexo.TamanhoArquivo);
                    cmd.ParameterAdd("@Arquivo", anexo.Arquivo);
                    cmd.ParameterAdd("@Observacao", anexo.Observacao);
                    cmd.ParameterAdd("@DataCriacao", anexo.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", anexo.DataAtualizacao);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (anexo.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "SELECT LAST_INSERT_ID();";
                            anexo.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar o anexo do prontuário.");
            }

            return operacao;
        }
        public bool Excluir(int id)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM ProntuarioAnexo WHERE ProntuarioAnexoId = @Id;";
                    cmd.ParameterAdd("@Id", id);
                    operacao = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o anexo do prontuário com ID {Id}", id);
            }
            return operacao;
        }
        internal override IEnumerable<Domain.ProntuarioAnexo.Entities.ProntuarioAnexo> Map(System.Data.IDataReader dr)
        {
            List<Domain.ProntuarioAnexo.Entities.ProntuarioAnexo> anexos = new List<Domain.ProntuarioAnexo.Entities.ProntuarioAnexo>();
            while (dr.Read())
            {
                var anexo = new Domain.ProntuarioAnexo.Entities.ProntuarioAnexo
                {
                    Id = Convert.ToInt32(dr["ProntuarioAnexoId"]),
                    Prontuario = new Domain.Prontuario.Entities.Prontuario
                    {
                        Id = Convert.ToInt32(dr["ProntuarioIdPaciente"])
                    },
                    TipoAnexo = (Domain.ProntuarioAnexo.Entities.ProntuarioAnexo.tpTipoAnexo)Convert.ToInt32(dr["TipoAnexo"]),
                    Nome = dr["Nome"].ToString(),
                    NomeArquivo = dr["NomeArquivo"].ToString(),
                    MimeType = dr["MimeType"].ToString(),
                    TamanhoArquivo = Convert.ToInt64(dr["TamanhoArquivo"]),
                    Arquivo = (byte[])dr["Arquivo"],
                    Observacao = dr["Observacao"].ToString(),
                    DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"])
                };
                anexos.Add(anexo);
            }
            return anexos;
        }
    }
}