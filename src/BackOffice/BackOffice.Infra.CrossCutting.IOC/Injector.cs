using Microsoft.Extensions.DependencyInjection;
using System;

namespace Psicologa.Infra.CrossCutting.IOC
{
    public static class Injector
    {
        public static void RegisterDomainServices(this IServiceCollection services)
        {
            #region Serviços e Repositórios do Domain.

            services.AddScoped(typeof(Domain.Dashboard.Services.IndicadorService));
            services.AddScoped<Domain.Dashboard.Interfaces.Repositories.IIndicadorRepository, Infra.Data.Repository.Dashboard.IndicadorRepository>();

            services.AddScoped(typeof(Domain.Usuario.Services.PerfilUsuarioService));             
            services.AddScoped<Domain.Usuario.Interfaces.Repositories.IPerfilUsuarioRepository, Psicologa.Infra.Data.Repository.Usuario.PerfilUsuarioRepository>();

            services.AddScoped(typeof(Shared.Domain.Cidade.Services.CidadeService));
            services.AddScoped<Shared.Domain.Cidade.Interfaces.Repositories.ICidadeRepository, Shared.Infra.Data.Repository.Cidade.CidadeRepository>();

            services.AddScoped(typeof(Domain.Pessoa.Services.PessoaService));
            services.AddScoped<Domain.Pessoa.Interfaces.Repositories.IPessoaRepository, Infra.Data.Repository.Pessoa.PessoaRepository>();

            services.AddScoped(typeof(Domain.BlogPost.Services.BlogPostService));
            services.AddScoped<Domain.BlogPost.Interfaces.Repositories.IBlogPostRepository, Infra.Data.Repository.BlogPost.BlogPostRepository>();
            
            services.AddScoped(typeof(Domain.Usuario.Services.UsuarioService));
            services.AddScoped<Domain.Usuario.Interfaces.Repositories.IUsuarioRepository, Infra.Data.Repository.Usuario.UsuarioRepository>();

            services.AddScoped(typeof(Domain.LogAplicacao.Services.LogAplicacaoService));
            services.AddScoped<Domain.LogAplicacao.Interfaces.Repositories.ILogAplicacaoRepository, Infra.Data.Repository.LogAplicacao.LogAplicacaoRepository>();
           
            services.AddScoped(typeof(Domain.Configuracao.Services.ConfiguracaoService));
            services.AddScoped<Domain.Configuracao.Interfaces.Repositories.IConfiguracaoRepository, Infra.Data.Repository.Configuracao.ConfiguracaoRepository>();

            services.AddScoped(typeof(Domain.Servico.Services.ServicoService));
            services.AddScoped<Domain.Servico.Interfaces.Repositories.IServicoContatoRepository, Infra.Data.Repository.Servico.ServicoRepository>();

            services.AddScoped(typeof(Domain.Convenio.Services.ConvenioService));
            services.AddScoped<Domain.Convenio.Interfaces.Repositories.IConvenioRepository, Infra.Data.Repository.Convenio.ConvenioRepository>();

            services.AddScoped(typeof(Domain.ServicoContato.Services.ServicoContatoService));
            services.AddScoped<Domain.ServicoContato.Interfaces.Repositories.IServicoContatoRepository, Infra.Data.Repository.ServicoContato.ServicoContatoRepository>();

            services.AddScoped(typeof(Domain.Agendamento.Services.AgendamentoService));
            services.AddScoped<Domain.Agendamento.Interfaces.Repositories.IAgendamentoRepository, Infra.Data.Repository.Agendamento.AgendamentoRepository>();


            #endregion
        }

        public static void RegisterApplicationServices(this IServiceCollection services)
        {
            #region Serviços da Application.
            services.AddScoped(typeof(Application.Usuario.Services.ApplicationPerfilUsuarioService));
            services.AddScoped(typeof(Application.Usuario.Services.ApplicationUsuarioService));
            services.AddScoped(typeof(Application.Pessoa.Services.ApplicationPessoaService));

            services.AddScoped(typeof(Application.Dashboard.Services.ApplicationIndicadorService));

            services.AddScoped(typeof(Application.BlogPost.Services.ApplicationBlogPostService));

            services.AddScoped(typeof(Application.Configuracao.Services.ApplicationConfiguracaoService));
            services.AddScoped(typeof(Application.Servico.Services.ApplicationServicoService));
            services.AddScoped(typeof(Application.Consultar.Services.ApplicationConsultarService));
            services.AddScoped(typeof(Application.Convenio.Services.ApplicationConvenioService));

            services.AddScoped(typeof(Application.ServicoContato.Services.ApplicationServicoContatoService));
            
            services.AddScoped(typeof(Application.Agendamento.Services.ApplicationAgentamentoService));



            #endregion
        }

        public static void RegisterApplicationAPIServices(this IServiceCollection services)
        {
            #region Serviços da Application API.
            #endregion
        }

        public static void RegisterOtherServices(this IServiceCollection services, Application.AppSettings appSettings)
        {
            #region Outros Serviços.
            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", appSettings.MySQLPrincipalConnectionString);
            var connectionFactory = new Shared.Infra.Data.Providers.DBContext(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"), Shared.Infra.Data.Providers.IDBContextFactory.TpProvider.MySQL);
            services.AddSingleton<Shared.Infra.Data.Providers.IDBContextFactory>(connectionFactory);

            Environment.SetEnvironmentVariable("BASE_URL", appSettings.BaseURL);
            #endregion
        }


    }
}
