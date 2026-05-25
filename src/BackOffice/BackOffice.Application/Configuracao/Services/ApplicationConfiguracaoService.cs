using Psicologa.Application.Configuracao.ViewsModel;
using Psicologa.Domain.BlogPost.Entities;
using Psicologa.Domain.BlogPost.Services;
using Psicologa.Domain.Configuracao.Entities;
using Psicologa.Domain.Servico.Entities;
using Psicologa.Domain.Servico.Services;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Application.Configuracao.Services
{
    public class ApplicationConfiguracaoService : IDisposable
    {
        private readonly Domain.Configuracao.Services.ConfiguracaoService _configuracaoService;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly IAppSettings _appSettings;

        public ApplicationConfiguracaoService(Domain.Configuracao.Services.ConfiguracaoService configuracaoService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService, IAppSettings appSettings)
        {
            _configuracaoService = configuracaoService;
            _logAplicacaoService = logAplicacaoService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(ConfiguracaoViewModel configVW, string[] requisicao)
        {
            var dadosExistente = _configuracaoService.ObterConfiguracao(configVW.Id);

            bool operacao = false;
            Domain.Configuracao.Entities.Configuracao config = new Domain.Configuracao.Entities.Configuracao();

            config.Id = configVW.Id;
            config.Nome = configVW.Nome;
            config.CEP = configVW.CEP;
            config.Endereco = configVW.Endereco;
            config.Numero = configVW.Numero;
            config.Complemento = configVW.Complemento;
            config.Bairro = configVW.Bairro;
            config.Cidade = configVW.Cidade;
            config.Estado = configVW.Estado;
            config.Whatsapp = configVW.Whatsapp;
            config.Email = configVW.Email;
            config.Facebook = configVW.Facebook;
            config.Instagram = configVW.Instagram;
            config.Linkedin = configVW.Linkedin;
            config.Slogan = configVW.Slogan;

            if (config.Validar())
            {
                operacao = _configuracaoService.Salvar(config);
            }

            if (operacao)
                RegistrarLogConfiguracao(config.Id, requisicao, dadosExistente, "Configuração");

            return (operacao, config.ValidationResult);
        }

        public ConfiguracaoViewModel ObterConfiguracao(int id)
        {
            ConfiguracaoViewModel retorno = new ConfiguracaoViewModel();
            retorno = FormatarRetornoConsulta(_configuracaoService.ObterConfiguracao(id));
            return retorno;
        }

        public ConfiguracaoViewModel ObterConfiguracao()
        {
            ConfiguracaoViewModel retorno = new ConfiguracaoViewModel();
            retorno = FormatarRetornoConsulta(_configuracaoService.ObterConfiguracao());
            return retorno;
        }

        internal ConfiguracaoViewModel FormatarRetornoConsulta(Domain.Configuracao.Entities.Configuracao config)
        {
            ConfiguracaoViewModel retorno = new ConfiguracaoViewModel();
            retorno.Id = config.Id;
            retorno.Nome = config.Nome;
            retorno.CEP = config.CEP;
            retorno.Endereco = config.Endereco;
            retorno.Numero = config.Numero;
            retorno.Complemento = config.Complemento;
            retorno.Bairro = config.Bairro;
            retorno.Cidade = config.Cidade;
            retorno.Estado = config.Estado;
            retorno.Whatsapp = config.Whatsapp;
            retorno.Email = config.Email;
            retorno.Facebook = config.Facebook;
            retorno.Instagram = config.Instagram;
            retorno.Linkedin = config.Linkedin;
            retorno.Slogan = config.Slogan;

            return retorno;
        }

        private void RegistrarLogConfiguracao(int servicoId, string[] requisicao, Domain.Configuracao.Entities.Configuracao dadosExistente, string nomeClasse)
        {
            var dadosAtualizado = _configuracaoService.ObterConfiguracao(servicoId);
            var (retorno, dadosAlterados) = _logAplicacaoService.ObterDiferencas(dadosExistente, dadosAtualizado);

            if (dadosAlterados.Any())
            {
                var log = _logAplicacaoService.Criar(
                    requisicao: requisicao,
                    entidade: nomeClasse,
                    entidadeId: servicoId,
                    operacao: retorno,
                    dadosAntes: dadosExistente,
                    dadosDepois: dadosAtualizado,
                    dadosAlterados: dadosAlterados,
                    aplicacao: MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                    metodo: MethodBase.GetCurrentMethod()?.Name
                );
                _logAplicacaoService.Salvar(log);
            }
        }

        public (bool, ValidationResult) SalvarFuncionamento(ConfiguracaoFuncionamentoViewModel configVW, string[] requisicao)
        {
            bool operacao = false;

            ConfiguracaoFuncionamento confFuncionamento = new ConfiguracaoFuncionamento
            {
                Id = configVW.Id,

                Funcionamento = configVW.Funcionamento
                    .Select(dia => new ConfiguracaoFuncionamentoDia
                    {
                        DiaSemana = dia.DiaSemana,
                        Nome = dia.Nome,
                        Ativo = dia.Ativo,

                        DataCriacao = DateTime.Now,
                        DataAtualizacao = DateTime.Now,

                        Periodos = dia.Periodos
                            .Select(periodo => new ConfiguracaoFuncionamentoPeriodo
                            {
                                HoraInicio = periodo.HoraInicio,
                                HoraFim = periodo.HoraFim
                            })
                            .ToList()
                    })
                    .ToList()
            };

            if (confFuncionamento.Validar())
            {
                operacao = _configuracaoService.SalvarFuncionamento(confFuncionamento);
            }

            //if (operacao)
            //    RegistrarLogConfiguracao(confFuncionamento.Id, requisicao, dadosExistente, "Configuração");

            return (operacao, confFuncionamento.ValidationResult);
        }

        //public ConfiguracaoFuncionamentoViewModel ObterFuncionamento(int id)
        //{
        //    ConfiguracaoFuncionamentoViewModel retorno = new ConfiguracaoFuncionamentoViewModel();
        //    retorno = FormatarRetornoConsulta(_configuracaoService.ObterFuncionamento(id));
        //    return retorno;
        //}

        public ConfiguracaoFuncionamentoViewModel ObterConfiguracaoFuncionamento()
        {
            ConfiguracaoFuncionamentoViewModel retorno = new ConfiguracaoFuncionamentoViewModel();
            retorno = FormatarRetornoFuncionamentoConsulta(_configuracaoService.ObterFuncionamento());
            return retorno;
        }

        internal ConfiguracaoFuncionamentoViewModel FormatarRetornoFuncionamentoConsulta(Domain.Configuracao.Entities.ConfiguracaoFuncionamento config)
        {
            ConfiguracaoFuncionamentoViewModel retorno = new ConfiguracaoFuncionamentoViewModel();

            retorno.Id = config.Id;
            retorno.Funcionamento = config.Funcionamento.Select(d => new FuncionamentoDiaViewModel
            {
                DiaSemana = d.DiaSemana,
                Nome = d.Nome,
                Ativo = d.Ativo,
                Periodos = d.Periodos.Select(p => new FuncionamentoPeriodoViewModel
                {
                    HoraInicio = p.HoraInicio,
                    HoraFim = p.HoraFim
                }).ToList()
            }).ToList();

            return retorno;
        }

        private void RegistrarLogFuncionanmento(int servicoId, string[] requisicao, Domain.Configuracao.Entities.ConfiguracaoFuncionamento dadosExistente, string nomeClasse)
        {
            var dadosAtualizado = _configuracaoService.ObterFuncionamento();
            var (retorno, dadosAlterados) = _logAplicacaoService.ObterDiferencas(dadosExistente, dadosAtualizado);

            if (dadosAlterados.Any())
            {
                var log = _logAplicacaoService.Criar(
                    requisicao: requisicao,
                    entidade: nomeClasse,
                    entidadeId: servicoId,
                    operacao: retorno,
                    dadosAntes: dadosExistente,
                    dadosDepois: dadosAtualizado,
                    dadosAlterados: dadosAlterados,
                    aplicacao: MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                    metodo: MethodBase.GetCurrentMethod()?.Name
                );
                _logAplicacaoService.Salvar(log);
            }
        }

        public void Dispose()
        {
        }
    }
}