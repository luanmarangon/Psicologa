using Microsoft.Extensions.Logging;
using Psicologa.Domain.Convenio.Entities;
using Psicologa.Domain.Documentos.Interfaces.Repositories;
using Psicologa.Infra.Data.Repository.Agendamento;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Infra.Data.Repository.Documentos
{
    public class DocumentosRepository : RepositoryBase<Domain.Documentos.Entities.Documentos>, IDocumentosRepository
    {
        private readonly ILogger<DocumentosRepository> _logger;

        public DocumentosRepository(IDBContextFactory dbContextFactory, ILogger<DocumentosRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Psicologa.Domain.Documentos.Entities.Documentos documento)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if(documento.Id == 0)
                    {
                        cmd.CommandText = @"INSERT INTO Documentos (Nome, Categoria, Conteudo, Ativo, DataCriacao, DataAtualizacao)
                                            VALUES (@Nome, @Categoria, @Conteudo, @Ativo, @DataCriacao, @DataAtualizacao);";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE Documentos SET
                                                    Nome = @Nome,
                                                    Categoria = @Categoria,     
                                                    Conteudo = @Conteudo,
                                                    Ativo = @Ativo,
                                                    DataAtualizacao = @DataAtualizacao
                                            WHERE DocumentosId = @Id;";
                        cmd.ParameterAdd("@Id", documento.Id);
                    }
                    cmd.ParameterAdd("@Nome", documento.Nome);
                    cmd.ParameterAdd("@Categoria", (int)documento.Categoria);
                    cmd.ParameterAdd("@Conteudo", documento.Conteudo);
                    cmd.ParameterAdd("@DataCriacao", documento.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", documento.DataAtualizacao);
                    cmd.ParameterAdd("@Ativo", documento.Ativo);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (documento.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "select LAST_INSERT_ID();";
                            documento.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch
            (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar documento");
            }
            return operacao;
        }

        public Domain.Documentos.Entities.Documentos Obter(int id)
        {
                       Domain.Documentos.Entities.Documentos documento = null;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * FROM Documentos WHERE DocumentosId = @Id;";
                    cmd.ParameterAdd("@Id", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        documento = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter documento");
            }
            return documento;
        }
        public IEnumerable<Domain.Documentos.Entities.Documentos> Consultar(string termo, int tp, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.Documentos.Entities.Documentos> documentos = new List<Domain.Documentos.Entities.Documentos>();
         
            string categoriaFiltro = "";
            if (tp > 0)
            {
                categoriaFiltro = $" AND Categoria = {tp} ";
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
                    string consultaPrincipal = $@"
                                    SELECT * FROM Documentos
                                    WHERE Nome LIKE @Termo {categoriaFiltro}
                                    #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";

                    using (var dr = cmd.ExecuteReader())
                    {
                        documentos = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar documentos");
            }
            return documentos;
        }




        internal override IEnumerable<Domain.Documentos.Entities.Documentos> Map(IDataReader dr)
        {
            List<Domain.Documentos.Entities.Documentos> documentos = new List<Domain.Documentos.Entities.Documentos>();
            while (dr.Read())
            {
                documentos.Add(new Domain.Documentos.Entities.Documentos
                {
                    Id = Convert.ToInt32(dr["DocumentosId"]),
                    Nome = dr["Nome"].ToString(),
                    Categoria = (Domain.Documentos.Entities.Documentos.TpCategoria)Convert.ToInt32(dr["Categoria"]),
                    Conteudo = dr["Conteudo"].ToString(),
                    Ativo = Convert.ToBoolean(dr["Ativo"]),
                    DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"])
                });
            }
            return documentos;
        }


    }
}