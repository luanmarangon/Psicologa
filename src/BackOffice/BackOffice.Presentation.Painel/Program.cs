using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Psicologa.Application;
using Psicologa.Infra.CrossCutting.IOC;
using Psicologa.Presentation.Painel;
using Serilog;
using System.Text.Json;

var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

string pathAppsettings = "appsettings.json";

if (enviroment == "Development")
{
    pathAppsettings = "appsettings.Development.json";
}

var config = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: false)
.Build();


#region Serilog
string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
Directory.CreateDirectory(logFolder);
Log.Logger = new LoggerConfiguration()
   .ReadFrom.Configuration(config)
   .Enrich.FromLogContext()
   .WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
   /*.WriteTo.File(new CompactJsonFormatter(), (Path.Combine(logFolder, @".json")),
        retainedFileCountLimit: 1,
        rollingInterval: RollingInterval.Day)*/
   .WriteTo.File((Path.Combine(logFolder, @".log")),
        retainedFileCountLimit: 1,
        rollingInterval: RollingInterval.Day)
   .CreateLogger();
Log.Information("Logger funcionado...");
Log.Information("ASPNETCORE_ENVIRONMENT: " + enviroment);
#endregion

try
{
    Log.Information("Starting...");


    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();
    builder.WebHost.UseUrls(config.GetValue<string>("KestrelUrl"));


    #region Serviço de configuração da aplicação.
    AppSettings _appSettings = new AppSettings();
    var cfcoAppSettings = new ConfigureFromConfigurationOptions<IAppSettings>(builder.Configuration.GetSection("AppSettings"));
    cfcoAppSettings.Configure(_appSettings);
    builder.Services.AddSingleton<IAppSettings>(_appSettings);
    #endregion


    #region Serviço para Cookie Authorization

    //Configurando como o token será validado.
    builder.Services.AddAuthentication(authOptions =>
    {
        authOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        authOptions.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        authOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    }).AddCookie(options =>
    {
        options.Cookie.Name = "CookieAuthPsicologa";
        options.AccessDeniedPath = new PathString("/Login/Index");
        options.LoginPath = new PathString("/Login/Index/");
        options.ExpireTimeSpan = TimeSpan.FromDays(_appSettings.CookieAuthDaysLife);
    });

    // Ativa o uso do token como forma de autorizar o acesso a recursos deste projeto.
    builder.Services.AddAuthorization(auth =>
    {
        auth.AddPolicy("CookieAuth", new AuthorizationPolicyBuilder()
          .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
          .RequireAuthenticatedUser().Build());
    });

    #endregion


    #region Serviço para obter o usuário autenticado
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddScoped<UsuarioAutenticado>();
    #endregion

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddScoped<RequisicaoAtual>();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddScoped<Configuracao>();



    #region Serviço para obter o usuário anônimo
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddScoped<OrigemReconhecida>();
    #endregion


    //builder.Services.AddHostedService<LimpezaPaginasHostedService>();
    //builder.Services.AddHostedService<ChatIAHostedService>();

    builder.Services.RegisterDomainServices();
    builder.Services.RegisterApplicationServices();
    builder.Services.RegisterOtherServices(_appSettings);

    builder.Services.AddControllersWithViews()
       .AddJsonOptions(options =>
       {
           options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
           options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

       });

    //habilitar ver as atualizações da View do Browser, sem recompilar o projeto.
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();


    //Acertar problemas de rotas de arquivos estáticos envolvendo o NGINX.
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    // Evita que a chave da aplicação seja renovada a cada nova publicação. Isso foi necessário para manter a cookie de autenticação válida.
    string keysFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keys");
    builder.Services.AddDataProtection()
          .SetApplicationName($"Psicologa")
          .PersistKeysToFileSystem(new DirectoryInfo(keysFolder));

    var app = builder.Build();
    _appSettings.ContentRootPath = app.Environment.ContentRootPath;

    string routePrefix = "";

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        //Acertar problemas de rotas de arquivos estáticos envolvendo o NGINX.
        routePrefix = _appSettings.BaseURLPrefix;
        app.UseForwardedHeaders();
        app.UsePathBase(routePrefix);
        app.Use((context, next) =>
        {
            context.Request.PathBase = new PathString(routePrefix);
            return next();
        });
    }
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        endpoints.MapControllerRoute(
           name: "Administrativo",
           pattern: "{area:exists}/{controller=Login}/{action=Index}/{id?}");
    });

    app.Run();


}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


