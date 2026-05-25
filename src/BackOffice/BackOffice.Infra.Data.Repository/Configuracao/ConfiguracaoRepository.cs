using Microsoft.Extensions.Logging;
using Psicologa.Domain.Configuracao.Entities;
using Psicologa.Domain.Configuracao.Interfaces.Repositories;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Psicologa.Infra.Data.Repository.Configuracao
{
    public class ConfiguracaoRepository : RepositoryBase<Domain.Configuracao.Entities.Configuracao>, IConfiguracaoRepository
    {
        private readonly ILogger<ConfiguracaoRepository> _logger;

        public ConfiguracaoRepository(IDBContextFactory dbContextFactory, ILogger<ConfiguracaoRepository> logger) : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.Configuracao.Entities.Configuracao config)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (config.Id == 0)
                    {
                        cmd.CommandText = @"INSERT INTO Configuracao (Nome, CEP, Endereco, Numero, Complemento, Bairro, Cidade, Estado, Whatsapp, Email, Facebook, Instagram, Linkedin, DataCriacao, DataAtualizacao, Slogan)
                                            VALUES (@Nome, @CEP, @Endereco, @Numero, @Complemento, @Bairro, @Cidade, @Estado, @Whatsapp, @Email, @Facebook, @Instagram, @Linkedin, @DataCriacao, @DataAtualizacao, @Slogan)";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE Configuracao SET Nome = @Nome, CEP = @CEP, Endereco = @Endereco, Numero = @Numero, Complemento = @Complemento,
                                            Bairro = @Bairro, Cidade = @Cidade, Estado = @Estado, Whatsapp = @Whatsapp, Email = @Email,
                                            Facebook = @Facebook, Instagram = @Instagram, Linkedin = @Linkedin,
                                            DataAtualizacao = @DataAtualizacao, Slogan = @Slogan
                                            WHERE ConfiguracaoId = @Id";
                        cmd.ParameterAdd("@Id", config.Id);
                    }

                    cmd.ParameterAdd("@Nome", config.Nome);
                    cmd.ParameterAdd("@CEP", config.CEP);
                    cmd.ParameterAdd("@Endereco", config.Endereco);
                    cmd.ParameterAdd("@Numero", config.Numero);
                    cmd.ParameterAdd("@Complemento", config.Complemento);
                    cmd.ParameterAdd("@Bairro", config.Bairro);
                    cmd.ParameterAdd("@Cidade", config.Cidade);
                    cmd.ParameterAdd("@Estado", config.Estado);
                    cmd.ParameterAdd("@Whatsapp", config.Whatsapp);
                    cmd.ParameterAdd("@Email", config.Email);
                    cmd.ParameterAdd("@Facebook", config.Facebook);
                    cmd.ParameterAdd("@Instagram", config.Instagram);
                    cmd.ParameterAdd("@Linkedin", config.Linkedin);
                    cmd.ParameterAdd("@DataCriacao", config.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", config.DataAtualizacao);
                    cmd.ParameterAdd("@Slogan", config.Slogan);

                    operacao = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar configuração");
            }
            return operacao;
        }

        public Domain.Configuracao.Entities.Configuracao ObterConfiguracao()
        {
            Domain.Configuracao.Entities.Configuracao config = new Domain.Configuracao.Entities.Configuracao();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Configuracao";

                    cmd.ParametersClear();

                    using (var dr = cmd.ExecuteReader())
                    {
                        config = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter configuração");
            }
            return config;
        }

        public Domain.Configuracao.Entities.Configuracao ObterConfiguracao(int id)
        {
            Domain.Configuracao.Entities.Configuracao config = new Domain.Configuracao.Entities.Configuracao();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Configuracao WHERE ConfiguracaoId = @Id";
                    cmd.ParameterAdd("@ConfiguracaoId", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        config = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter configuração com Id {id}");
            }
            return config;
        }

        internal override IEnumerable<Domain.Configuracao.Entities.Configuracao> Map(IDataReader dr)
        {
            List<Domain.Configuracao.Entities.Configuracao> configuracoes = new List<Domain.Configuracao.Entities.Configuracao>();
            while (dr.Read())
            {
                Domain.Configuracao.Entities.Configuracao config = new Domain.Configuracao.Entities.Configuracao
                {
                    Id = Convert.ToInt32(dr["ConfiguracaoId"]),
                    Nome = dr["Nome"].ToString(),
                    CEP = dr["CEP"].ToString(),
                    Endereco = dr["Endereco"].ToString(),
                    Numero = dr["Numero"].ToString(),
                    Complemento = dr["Complemento"].ToString(),
                    Bairro = dr["Bairro"].ToString(),
                    Cidade = dr["Cidade"].ToString(),
                    Estado = dr["Estado"].ToString(),
                    Whatsapp = dr["Whatsapp"].ToString(),
                    Email = dr["Email"].ToString(),
                    Facebook = dr["Facebook"].ToString(),
                    Instagram = dr["Instagram"].ToString(),
                    Linkedin = dr["Linkedin"].ToString(),
                    DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"]),
                    Slogan = dr["Slogan"].ToString()
                };
                configuracoes.Add(config);
            }
            return configuracoes;
        }

        public bool SalvarFuncionamento(Domain.Configuracao.Entities.ConfiguracaoFuncionamento funcionamento)
        {
            bool operacao = false;

            try
            {
                using (var uow = DbContext.CreateUnitOfWork())
                {
                    try
                    {
                        // remove tudo
                        using (var cmdDelete = DbContext.CreateCommand())
                        {
                            cmdDelete.CommandText = @"DELETE FROM configuracaofuncionamento";
                            cmdDelete.ExecuteNonQuery();
                        }

                        // reinsere
                        foreach (var dia in funcionamento.Funcionamento)
                        {
                            if (!dia.Ativo)
                                continue;

                            int ordem = 1;

                            foreach (var periodo in dia.Periodos)
                            {
                                using (var cmd = DbContext.CreateCommand())
                                {
                                    cmd.CommandText = @"INSERT INTO configuracaofuncionamento(DiaSemana,Ativo,HoraInicio,HoraFim,Ordem,DataCriacao,DataAtualizacao)
                                                                                    VALUES(@DiaSemana,@Ativo,@HoraInicio,@HoraFim,@Ordem,@DataCriacao,@DataAtualizacao)";

                                    cmd.ParameterAdd("@DiaSemana",dia.DiaSemana);
                                    cmd.ParameterAdd("@Ativo",dia.Ativo);
                                    cmd.ParameterAdd("@HoraInicio",periodo.HoraInicio);
                                    cmd.ParameterAdd("@HoraFim",periodo.HoraFim);
                                    cmd.ParameterAdd("@Ordem",ordem);
                                    cmd.ParameterAdd("@DataCriacao",dia.DataCriacao);
                                    cmd.ParameterAdd("@DataAtualizacao",dia.DataAtualizacao);
                                    cmd.ExecuteNonQuery();
                                    ordem++;
                                }
                            }
                        }

                        uow.SaveChanges();

                        operacao = true;
                    }
                    catch (Exception ex)
                    {
                        uow.CancelChanges();

                        _logger.LogError(
                            ex,
                            "Erro ao salvar configuração de funcionamento.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro geral ao salvar funcionamento.");
            }

            return operacao;
        }

        public ConfiguracaoFuncionamento ObterFuncionamento(int id)
        {
            throw new NotImplementedException();
        }

        public Domain.Configuracao.Entities.ConfiguracaoFuncionamento ObterFuncionamento()
        {
            Domain.Configuracao.Entities.ConfiguracaoFuncionamento config = new Domain.Configuracao.Entities.ConfiguracaoFuncionamento();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM ConfiguracaoFuncionamento";

                    cmd.ParametersClear();

                    using (var dr = cmd.ExecuteReader())
                    {
                        config = MapFuncionamento(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter configuração");
            }
            return config;
        }

        internal Domain.Configuracao.Entities.ConfiguracaoFuncionamento MapFuncionamento(IDataReader dr)
        {
            
            Domain.Configuracao.Entities.ConfiguracaoFuncionamento config = new Domain.Configuracao.Entities.ConfiguracaoFuncionamento
                {
                    Funcionamento = new List<ConfiguracaoFuncionamentoDia>()
                };

            while (dr.Read())
            {
                int diaSemana = Convert.ToInt32(dr["DiaSemana"]);
                var dia =config.Funcionamento.FirstOrDefault(x => x.DiaSemana == diaSemana);

                // cria o dia se não existir
                if (dia == null)
                {
                    dia = new ConfiguracaoFuncionamentoDia
                    {
                        DiaSemana = diaSemana,
                        Ativo = Convert.ToBoolean(dr["Ativo"]),
                        Nome = Shared.Infra.CrossCutting.Utils.ObterDiaSemana(diaSemana),
                        Periodos = new List<ConfiguracaoFuncionamentoPeriodo>(),
                        DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                        DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"])
                    };

                    config.Funcionamento.Add(dia);
                }

                // adiciona período
                dia.Periodos.Add(new ConfiguracaoFuncionamentoPeriodo
                {
                    HoraInicio = ((TimeSpan)dr["HoraInicio"]).ToString(@"hh\:mm"),
                    HoraFim = ((TimeSpan)dr["HoraFim"]).ToString(@"hh\:mm")
                });
            }

            return config;
        }

    }
}