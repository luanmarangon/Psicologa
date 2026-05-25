using Psicologa.Domain.Configuracao.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Psicologa.Domain.Configuracao.Services
{
    public class ConfiguracaoService : ServiceBase<Entities.Configuracao>
    {
        private readonly IConfiguracaoRepository _repository;

        public ConfiguracaoService(IConfiguracaoRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Entities.Configuracao config)
        {
            if (config.Id == 0)
            {
                config.DataCriacao = DateTime.Now;
                config.DataAtualizacao = DateTime.Now;
            }
            else
            {
                config.DataAtualizacao = DateTime.Now;
            }

            return _repository.Salvar(config);
        }

        public Entities.Configuracao ObterConfiguracao(int id)
        {
            return _repository.ObterConfiguracao(id);
        }
        public Entities.Configuracao ObterConfiguracao()
        {
            return _repository.ObterConfiguracao();
        }

      








        public bool SalvarFuncionamento(Entities.ConfiguracaoFuncionamento config)
        {
            foreach (var dia in config.Funcionamento)
            {
                if (dia.Id == 0)
                {
                    dia.DataCriacao = DateTime.Now;
                    dia.DataAtualizacao = DateTime.Now;
                }
                else
                {
                    dia.DataAtualizacao = DateTime.Now;
                }
            }

            return _repository.SalvarFuncionamento(config);
        }

        public Entities.ConfiguracaoFuncionamento ObterFuncionamento(int id)
        {
            return _repository.ObterFuncionamento(id);
        }
        public Entities.ConfiguracaoFuncionamento ObterFuncionamento()
        {
            return _repository.ObterFuncionamento();
        }


    }
}