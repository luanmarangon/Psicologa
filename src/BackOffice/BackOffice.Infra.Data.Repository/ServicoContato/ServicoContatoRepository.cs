using Microsoft.Extensions.Logging;
using Psicologa.Domain.ServicoContato.Interfaces.Repositories;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Psicologa.Infra.Data.Repository.ServicoContato
{
    public class ServicoContatoRepository : RepositoryBase<Domain.ServicoContato.Entities.ServicoContato>, IServicoContatoRepository
    {
        private readonly ILogger<ServicoContatoRepository> _logger;

        public ServicoContatoRepository(IDBContextFactory dbContextFactory, ILogger<ServicoContatoRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.ServicoContato.Entities.ServicoContato servico)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (servico.Id == 0)
                    {
                        cmd.CommandText = @" INSERT INTO ServicoContato( ServicoId, Nome,Contato,Email,Mensagem,StatusContato,EntrouContato,DataContato,DataRetorno,ObservacaoInterna,
                                                                        Origem,VirouPaciente,Prioridade,PreferenciaContato,IP,UserAgent,DataCriacao,DataAtualizacao)
                                                                    VALUES(@ServicoId,@Nome,@Contato,@Email,@Mensagem,@StatusContato,@EntrouContato,@DataContato,@DataRetorno,@ObservacaoInterna,
                                                                        @Origem,@VirouPaciente,@Prioridade,@PreferenciaContato,@IP,@UserAgent,@DataCriacao,@DataAtualizacao);";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE ServicoContato SET
                                            ServicoId = @ServicoId,
                                            Nome = @Nome,
                                            Contato = @Contato,
                                            Email = @Email,
                                            Mensagem = @Mensagem,
                                            StatusContato = @StatusContato,
                                            EntrouContato = @EntrouContato,
                                            DataContato = @DataContato,
                                            DataRetorno = @DataRetorno,
                                            ObservacaoInterna = @ObservacaoInterna,
                                            Origem = @Origem,
                                            VirouPaciente = @VirouPaciente,
                                            Prioridade = @Prioridade,
                                            PreferenciaContato = @PreferenciaContato,
                                            IP = @IP,
                                            UserAgent = @UserAgent,
                                            DataCriacao = @DataCriacao,
                                            DataAtualizacao = @DataAtualizacao
                                        WHERE ServicoContatoId = @ServicoContatoId;";

                        cmd.ParameterAdd("@ServicoContatoId", servico.Id);
                    }
                    cmd.ParameterAdd("@ServicoId", servico.Servico.Id);
                    cmd.ParameterAdd("@Nome", servico.Nome);
                    cmd.ParameterAdd("@Contato", servico.Contato);
                    cmd.ParameterAdd("@Email", servico.Email);
                    cmd.ParameterAdd("@Mensagem", servico.Mensagem);
                    cmd.ParameterAdd("@StatusContato", servico.StatusContato);
                    cmd.ParameterAdd("@EntrouContato", servico.EntrouContato);
                    cmd.ParameterAdd("@DataContato", servico.DataContato);
                    cmd.ParameterAdd("@DataRetorno", servico.DataRetorno);
                    cmd.ParameterAdd("@ObservacaoInterna", servico.ObservacaoInterna);
                    cmd.ParameterAdd("@Origem", servico.Origem);
                    cmd.ParameterAdd("@VirouPaciente", servico.VirouPaciente);
                    cmd.ParameterAdd("@Prioridade", servico.Prioridade);
                    cmd.ParameterAdd("@PreferenciaContato", servico.PreferenciaContato);
                    cmd.ParameterAdd("@IP", servico.IP);
                    cmd.ParameterAdd("@UserAgent", servico.UserAgent);
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
                _logger.LogError(ex, "Erro ao salvar o serviço contato");
            }
            return operacao;
        }

        public Domain.ServicoContato.Entities.ServicoContato Obter(int id)
        {
            Domain.ServicoContato.Entities.ServicoContato servico = new Domain.ServicoContato.Entities.ServicoContato();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT
                                            -- ServicoContato
                                            SC.ServicoContatoId, SC.ServicoId as ServicoIdContatoId, SC.Nome, SC.Contato, SC.Email, SC.Mensagem,
                                            SC.StatusContato, SC.EntrouContato, SC.DataContato, SC.DataRetorno, SC.ObservacaoInterna,
                                            SC.Origem, SC.VirouPaciente, SC.Prioridade, SC.PreferenciaContato, SC.IP, SC.UserAgent,
                                            SC.DataCriacao, SC.DataAtualizacao,
                                            -- Servico
                                            S.ServicoId, S.Nome AS ServicoNome, S.Url AS ServicoUrl, S.DescricaoCurta AS ServicoDescricaoCurta,
                                            S.Descricao AS ServicoDescricao, S.TempoSessaoMinutos AS ServicoTempoSessaoMinutos, S.ValorSessao AS ServicoValorSessao,
                                            S.ImagemCapa, S.Online AS ServicoOnline, S.Presencial AS ServicoPresencial, S.DestaqueHome AS ServicoDestaqueHome,
                                            S.OrdemExibicao AS ServicoOrdemExibicao, S.Ativo AS ServicoAtivo, S.DataCriacao AS ServicoDataCriacao,
                                            S.DataAtualizacao AS ServicoDataAtualizacao
                                        FROM ServicoContato SC
                                        INNER JOIN Servico S
                                            ON S.ServicoId = SC.ServicoId
                                        WHERE SC.ServicoContatoId = @ServicoContatoId";
                    
                    cmd.ParameterAdd("@ServicoContatoId", id);
                    
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

        //public IEnumerable<Domain.Servico.Entities.Servico> Obter(string[] filtro)
        //{
        //    string consultaFiltro = string.Empty;
        //    if (filtro == null || filtro.Length == 0)
        //        return ObterTodos().ToList();

        //    if (filtro.Contains("online"))
        //        consultaFiltro += "Online = 1 AND ";
        //    if (filtro.Contains("presencial"))
        //        consultaFiltro += "Presencial = 1 AND ";
        //    if (filtro.Contains("destaquehome"))
        //        consultaFiltro += "DestaqueHome = 1 AND ";

        //    List<Domain.Servico.Entities.Servico> servicos = new List<Domain.Servico.Entities.Servico>();
        //    try
        //    {
        //        using (var cmd = DbContext.CreateCommand())
        //        {
        //            cmd.CommandText = $"SELECT * FROM Servico WHERE {consultaFiltro} Ativo = 1 ORDER BY OrdemExibicao;";
        //            using (var dr = cmd.ExecuteReader())
        //            {
        //                servicos = Map(dr).ToList();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao obter os serviços online");
        //    }
        //    return servicos;
        //}
        //public IEnumerable<Domain.Servico.Entities.Servico> ObterTodos()
        //{
        //    List<Domain.Servico.Entities.Servico> servicos = new List<Domain.Servico.Entities.Servico>();
        //    try
        //    {
        //        using (var cmd = DbContext.CreateCommand())
        //        {
        //            cmd.CommandText = "SELECT * FROM Servico;";
        //            using (var dr = cmd.ExecuteReader())
        //            {
        //                servicos = Map(dr).ToList();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao obter todos os serviços");
        //    }
        //    return servicos;
        //}
        //public bool Excluir(int id)
        //{
        //    bool operacao = false;
        //    try
        //    {
        //        using (var cmd = DbContext.CreateCommand())
        //        {
        //            cmd.CommandText = "DELETE FROM Servico WHERE ServicoId = @ServicoId;";
        //            cmd.ParameterAdd("@ServicoId", id);
        //            operacao = cmd.ExecuteNonQuery() > 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao excluir o serviço");
        //    }
        //    return operacao;
        //}
        //public IEnumerable<Domain.Servico.Entities.Servico> ObterDestaquesHome(int limite)
        //{
        //    List<Domain.Servico.Entities.Servico> servicos = new List<Domain.Servico.Entities.Servico>();
        //    try
        //    {
        //        using (var cmd = DbContext.CreateCommand())
        //        {
        //            cmd.CommandText = $"SELECT * FROM Servico WHERE DestaqueHome = 1 AND Ativo = 1 ORDER BY RAND() LIMIT {limite};";
        //            using (var dr = cmd.ExecuteReader())
        //            {
        //                servicos = Map(dr).ToList();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao obter os serviços em destaque para a home");
        //    }
        //    return servicos;
        //}
        public IEnumerable<Domain.ServicoContato.Entities.ServicoContato> Consultar(string termo, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.ServicoContato.Entities.ServicoContato> servicos = new List<Domain.ServicoContato.Entities.ServicoContato>();
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
                                    SELECT
                                            -- ServicoContato
                                            SC.ServicoContatoId, SC.ServicoId as ServicoIdContatoId, SC.Nome, SC.Contato, SC.Email, SC.Mensagem,
                                            SC.StatusContato, SC.EntrouContato, SC.DataContato, SC.DataRetorno, SC.ObservacaoInterna,
                                            SC.Origem, SC.VirouPaciente, SC.Prioridade, SC.PreferenciaContato, SC.IP, SC.UserAgent,
                                            SC.DataCriacao, SC.DataAtualizacao,
                                            -- Servico
                                            S.ServicoId, S.Nome AS ServicoNome, S.Url AS ServicoUrl, S.DescricaoCurta AS ServicoDescricaoCurta,
                                            S.Descricao AS ServicoDescricao, S.TempoSessaoMinutos AS ServicoTempoSessaoMinutos, S.ValorSessao AS ServicoValorSessao,
                                            S.ImagemCapa, S.Online AS ServicoOnline, S.Presencial AS ServicoPresencial, S.DestaqueHome AS ServicoDestaqueHome,
                                            S.OrdemExibicao AS ServicoOrdemExibicao, S.Ativo AS ServicoAtivo, S.DataCriacao AS ServicoDataCriacao,
                                            S.DataAtualizacao AS ServicoDataAtualizacao
                                        FROM ServicoContato SC
                                        INNER JOIN Servico S
                                            ON S.ServicoId = SC.ServicoId
                                        WHERE (sc.Nome LIKE @Termo || s.Nome LIKE @Termo) 
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
                _logger.LogError(ex, "Erro ao consultar Servico Contato");
            }
            return servicos;
        }
        //public Domain.Servico.Entities.Servico ObterPorUrl(string url)
        //{
        //    Domain.Servico.Entities.Servico servico = new Domain.Servico.Entities.Servico();
        //    try
        //    {
        //        using (var cmd = DbContext.CreateCommand())
        //        {
        //            cmd.CommandText = "SELECT * FROM Servico WHERE Url = @Url;";
        //            cmd.ParameterAdd("@Url", url);
        //            using (var dr = cmd.ExecuteReader())
        //            {
        //                servico = Map(dr).FirstOrDefault();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao obter o serviço por ID");
        //    }
        //    return servico;
        //}
        //public Domain.Servico.Entities.Servico ObterPorNome(string nome)
        //{
        //    Domain.Servico.Entities.Servico servico = new Domain.Servico.Entities.Servico();
        //    try
        //    {
        //        using (var cmd = DbContext.CreateCommand())
        //        {
        //            cmd.CommandText = "SELECT * FROM Servico WHERE Nome= @Nome;";
        //            cmd.ParameterAdd("@Nome", nome);
        //            using (var dr = cmd.ExecuteReader())
        //            {
        //                servico = Map(dr).FirstOrDefault();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao obter o serviço por Nome");
        //    }
        //    return servico;
        //}
        internal override IEnumerable<Domain.ServicoContato.Entities.ServicoContato> Map(IDataReader dr)
        {
            List<Domain.ServicoContato.Entities.ServicoContato> servContato = new List<Domain.ServicoContato.Entities.ServicoContato>();
            while (dr.Read())
            {
                Domain.ServicoContato.Entities.ServicoContato serv = new Domain.ServicoContato.Entities.ServicoContato
                {
                    Id = Convert.ToInt32(dr["ServicoContatoId"]),
                    Nome = dr["Nome"].ToString(),
                    Contato = dr["Contato"].ToString(),
                    Email = dr["Email"].ToString(),
                    Mensagem = dr["Mensagem"].ToString(),
                    StatusContato = (Domain.ServicoContato.Entities.ServicoContato.TpStatusContato)Convert.ToInt32(dr["StatusContato"]),
                    EntrouContato = Convert.ToBoolean(dr["EntrouContato"]),
                    DataContato = dr["DataContato"] != DBNull.Value ? Convert.ToDateTime(dr["DataContato"]) : (DateTime?)null,
                    DataRetorno = dr["DataRetorno"] != DBNull.Value ? Convert.ToDateTime(dr["DataRetorno"]) : (DateTime?)null,
                    ObservacaoInterna = dr["ObservacaoInterna"].ToString(),
                    Origem = dr["Origem"].ToString(),
                    VirouPaciente = Convert.ToBoolean(dr["VirouPaciente"]),
                    Prioridade = (Domain.ServicoContato.Entities.ServicoContato.TpPrioridade)Convert.ToInt32(dr["Prioridade"]),
                    PreferenciaContato = (Domain.ServicoContato.Entities.ServicoContato.TpPreferenciaContato)Convert.ToInt32(dr["PreferenciaContato"]),
                    IP = dr["IP"].ToString(),
                    UserAgent = dr["UserAgent"].ToString(),
                    DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"]),

                    Servico = new Domain.Servico.Entities.Servico
                    {
                        Id = Convert.ToInt32(dr["ServicoIdContatoId"]),
                        Nome = dr["ServicoNome"].ToString(),
                        Url = dr["ServicoUrl"].ToString(),
                        DescricaoCurta = dr["ServicoDescricaoCurta"].ToString(),
                        Descricao = dr["ServicoDescricao"].ToString(),
                        TempoSessaoMinutos = Convert.ToInt32(dr["ServicoTempoSessaoMinutos"]),
                        ValorSessao = Convert.ToDecimal(dr["ServicoValorSessao"]),
                        ImagemCapa = dr["ImagemCapa"] != DBNull.Value
                                                    ? (byte[])dr["ImagemCapa"]
                                                    : null,
                        Online = Convert.ToBoolean(dr["ServicoOnline"]),
                        Presencial = Convert.ToBoolean(dr["ServicoPresencial"]),
                        DestaqueHome = Convert.ToBoolean(dr["ServicoDestaqueHome"]),
                        OrdemExibicao = Convert.ToInt32(dr["ServicoOrdemExibicao"]),
                        Ativo = Convert.ToBoolean(dr["ServicoAtivo"]),
                        DataCriacao = Convert.ToDateTime(dr["ServicoDataCriacao"]),
                        DataAtualizacao = Convert.ToDateTime(dr["ServicoDataAtualizacao"])
                    }
                };
                servContato.Add(serv);
            }
            return servContato;
        }
    }
}