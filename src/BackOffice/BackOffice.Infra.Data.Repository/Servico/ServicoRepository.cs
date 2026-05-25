using Microsoft.Extensions.Logging;
using Psicologa.Domain.Servico.Interfaces.Repositories;
using Psicologa.Infra.Data.Repository.BlogPost;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Infra.Data.Repository.Servico
{
    public class ServicoRepository : RepositoryBase<Domain.Servico.Entities.Servico>, IServicoContatoRepository
    {
        private readonly ILogger<ServicoRepository> _logger;

        public ServicoRepository(IDBContextFactory dbContextFactory, ILogger<ServicoRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.Servico.Entities.Servico servico)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (servico.Id == 0)
                    {
                        cmd.CommandText = @"INSERT INTO Servico (Nome, Url, DescricaoCurta, Descricao, TempoSessaoMinutos, ValorSessao, ImagemCapa, Online, Presencial, DestaqueHome, OrdemExibicao, Ativo, DataCriacao, DataAtualizacao)
                                            VALUES (@Nome, @Url, @DescricaoCurta, @Descricao, @TempoSessaoMinutos, @ValorSessao, @ImagemCapa, @Online, @Presencial, @DestaqueHome, @OrdemExibicao, @Ativo, @DataCriacao, @DataAtualizacao);";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE Servico SET
                                            Nome = @Nome,
                                            Url = @Url,
                                            DescricaoCurta = @DescricaoCurta,
                                            Descricao = @Descricao,
                                            TempoSessaoMinutos = @TempoSessaoMinutos,
                                            ValorSessao = @ValorSessao,
                                            ImagemCapa = @ImagemCapa,
                                            Online = @Online,
                                            Presencial = @Presencial,
                                            DestaqueHome = @DestaqueHome,
                                            OrdemExibicao = @OrdemExibicao,
                                            Ativo = @Ativo,
                                            DataCriacao = @DataCriacao,
                                            DataAtualizacao = @DataAtualizacao
                                            WHERE ServicoId = @Id;";
                        cmd.ParameterAdd("@Id", servico.Id);
                    }
                    cmd.ParameterAdd("@Nome", servico.Nome);
                    cmd.ParameterAdd("@Url", servico.Url);
                    cmd.ParameterAdd("@DescricaoCurta", servico.DescricaoCurta);
                    cmd.ParameterAdd("@Descricao", servico.Descricao);
                    cmd.ParameterAdd("@TempoSessaoMinutos", servico.TempoSessaoMinutos);
                    cmd.ParameterAdd("@ValorSessao", servico.ValorSessao);
                    cmd.ParameterAdd("@ImagemCapa", servico.ImagemCapa);
                    cmd.ParameterAdd("@Online", servico.Online);
                    cmd.ParameterAdd("@Presencial", servico.Presencial);
                    cmd.ParameterAdd("@DestaqueHome", servico.DestaqueHome);
                    cmd.ParameterAdd("@OrdemExibicao", servico.OrdemExibicao);
                    cmd.ParameterAdd("@Ativo", servico.Ativo);
                    cmd.ParameterAdd("@DataCriacao", servico.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", servico.DataAtualizacao);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (servico.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "SELECT LAST_INSERT_ID();";
                            servico.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar o serviço");
            }
            return operacao;
        }
        public Domain.Servico.Entities.Servico Obter(int id)
        {
            Domain.Servico.Entities.Servico servico = new Domain.Servico.Entities.Servico();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Servico WHERE ServicoId = @ServicoId;";
                    cmd.ParameterAdd("@ServicoId", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        servico = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o serviço por ID");
            }
            return servico;
        }
        public IEnumerable<Domain.Servico.Entities.Servico> Obter(string[] filtro)
        {
            string consultaFiltro = string.Empty;
            if (filtro == null || filtro.Length == 0)
                return ObterTodos().ToList();

            if (filtro.Contains("online"))
                consultaFiltro += "Online = 1 AND ";
            if (filtro.Contains("presencial"))
                consultaFiltro += "Presencial = 1 AND ";
            if (filtro.Contains("destaquehome"))
                consultaFiltro += "DestaqueHome = 1 AND ";

            List<Domain.Servico.Entities.Servico> servicos = new List<Domain.Servico.Entities.Servico>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM Servico WHERE {consultaFiltro} Ativo = 1 ORDER BY OrdemExibicao;";
                    using (var dr = cmd.ExecuteReader())
                    {
                        servicos = Map(dr).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter os serviços online");
            }
            return servicos;
        }
        public IEnumerable<Domain.Servico.Entities.Servico> ObterTodos()
        {
            List<Domain.Servico.Entities.Servico> servicos = new List<Domain.Servico.Entities.Servico>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Servico;";
                    using (var dr = cmd.ExecuteReader())
                    {
                        servicos = Map(dr).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os serviços");
            }
            return servicos;
        }
        public IEnumerable<Domain.Servico.Entities.Servico> ObterTodosAtivos()
        {
            List<Domain.Servico.Entities.Servico> servicos = new List<Domain.Servico.Entities.Servico>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Servico WHERE Ativo = 1;";
                    using (var dr = cmd.ExecuteReader())
                    {
                        servicos = Map(dr).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os serviços");
            }
            return servicos;
        }
        public bool Excluir(int id)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Servico WHERE ServicoId = @ServicoId;";
                    cmd.ParameterAdd("@ServicoId", id);
                    operacao = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o serviço");
            }
            return operacao;
        }
        public IEnumerable<Domain.Servico.Entities.Servico> ObterDestaquesHome(int limite)
        {
            List<Domain.Servico.Entities.Servico> servicos = new List<Domain.Servico.Entities.Servico>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM Servico WHERE DestaqueHome = 1 AND Ativo = 1 ORDER BY RAND() LIMIT {limite};";
                    using (var dr = cmd.ExecuteReader())
                    {
                        servicos = Map(dr).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter os serviços em destaque para a home");
            }
            return servicos;
        }
        public IEnumerable<Domain.Servico.Entities.Servico> Consultar(string termo, Domain.Servico.Entities.Servico.TpFiltroServico filtro, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.Servico.Entities.Servico> servicos = new List<Domain.Servico.Entities.Servico>();
            string filtroConsulta = string.Empty;
            try
            {
                if (filtro != Domain.Servico.Entities.Servico.TpFiltroServico.Indefinido)
                {
                    filtroConsulta = filtro switch
                    {
                        Domain.Servico.Entities.Servico.TpFiltroServico.DestaquesHome => "AND DestaqueHome = 1 ",
                        Domain.Servico.Entities.Servico.TpFiltroServico.Presencial => "AND Presencial = 1 ",
                        Domain.Servico.Entities.Servico.TpFiltroServico.Online => "AND Online = 1 ",
                        _ => string.Empty
                    };
                }

                int pular = 0;
                if (paginacao.PaginaAtual > 0)
                    pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

                if (pular < 0)
                    pular = 0;

                using (var cmd = DbContext.CreateCommand())
                {
                    string consultaPrincipal = $@"
                                    SELECT s.ServicoId, s.Nome, s.Url, s.DescricaoCurta, s.Descricao, s.TempoSessaoMinutos,
                                           s.ValorSessao, s.ImagemCapa, s.Online, s.Presencial, s.DestaqueHome, s.OrdemExibicao,
                                           s.Ativo, s.DataCriacao, s.DataAtualizacao
                                    FROM Servico s
                                    WHERE (s.Nome LIKE @Termo || s.DescricaoCurta LIKE @Termo) {filtroConsulta}
                                    #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";

                    using (var dr = cmd.ExecuteReader())
                    {
                        servicos = Map(dr).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Servico");
            }
            return servicos;
        }
        public Domain.Servico.Entities.Servico ObterPorUrl(string url)
        {
            Domain.Servico.Entities.Servico servico = new Domain.Servico.Entities.Servico();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Servico WHERE Url = @Url;";
                    cmd.ParameterAdd("@Url", url);
                    using (var dr = cmd.ExecuteReader())
                    {
                        servico = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o serviço por ID");
            }
            return servico;
        }
        public Domain.Servico.Entities.Servico ObterPorNome(string nome)
        {
            Domain.Servico.Entities.Servico servico = new Domain.Servico.Entities.Servico();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Servico WHERE Nome= @Nome;";
                    cmd.ParameterAdd("@Nome", nome);
                    using (var dr = cmd.ExecuteReader())
                    {
                        servico = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o serviço por Nome");
            }
            return servico;
        }
        internal override IEnumerable<Domain.Servico.Entities.Servico> Map(IDataReader dr)
        {
            List<Domain.Servico.Entities.Servico> servicos = new List<Domain.Servico.Entities.Servico>();
            while (dr.Read())
            {
                Domain.Servico.Entities.Servico servico = new Domain.Servico.Entities.Servico
                {
                    Id = Convert.ToInt32(dr["ServicoId"]),
                    Nome = dr["Nome"].ToString(),
                    Url = dr["Url"].ToString(),
                    DescricaoCurta = dr["DescricaoCurta"].ToString(),
                    Descricao = dr["Descricao"].ToString(),
                    TempoSessaoMinutos = Convert.ToInt32(dr["TempoSessaoMinutos"]),
                    ValorSessao = Convert.ToDecimal(dr["ValorSessao"]),
                    ImagemCapa = dr["ImagemCapa"] != DBNull.Value
                                                    ? (byte[])dr["ImagemCapa"]
                                                    : null,
                    Online = Convert.ToBoolean(dr["Online"]),
                    Presencial = Convert.ToBoolean(dr["Presencial"]),
                    DestaqueHome = Convert.ToBoolean(dr["DestaqueHome"]),
                    OrdemExibicao = Convert.ToInt32(dr["OrdemExibicao"]),
                    Ativo = Convert.ToBoolean(dr["Ativo"]),
                    DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"])
                };
                servicos.Add(servico);
            }
            return servicos;
        }
    }
}