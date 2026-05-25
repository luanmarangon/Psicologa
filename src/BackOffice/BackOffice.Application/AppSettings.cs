namespace Psicologa.Application
{
    public class AppSettings : IAppSettings
    {
       public string Nome { get; set; }
        public string BaseURL { get; set; }
        public string BaseURLPrefix { get; set; }
        public string MySQLPrincipalConnectionString { get; set; }
        public int CookieAuthDaysLife { get; set; }
        public string Version { get; set; }
        public string VersionData { get; set; }
        public IAppSettings.TpEmailPadrao Email { get; set; }
        public IAppSettings.TpMaterialEducativo MaterialEducativo { get; set; }
        public IAppSettings.TpEGestor EGestor { get; set; }
        public string ContentRootPath { get; set; }
        public string SuperOraculoURLAPI { get; set; }
        public string[] OrigensReconhecidas { get; set; } = new string[0];

        public AppSettings()
        {
            Email = new IAppSettings.TpEmailPadrao();
        }

    }

    public interface IAppSettings
    {
        public class TpEmailPadrao
        {
            public string Email { get; set; }
            public string Senha { get; set; }
            public string Remetente { get; set; }
            public string SMTP { get; set; }
            public int Porta { get; set; }

        }
        public struct TpMaterialEducativo
        {
            public string URL { get; set; }
            public string Chave { get; set; }
        }

        public struct TpEGestor
        {
            public string URL { get; set; }
            public string Usuario { get; set; }
            public string Chave { get; set; }
        }

        string Nome { get; set; }
        string BaseURL { get; set; }
        string BaseURLPrefix { get; set; }
        string MySQLPrincipalConnectionString { get; set; }
        int CookieAuthDaysLife { get; set; }
        string Version { get; set; }
        string VersionData { get; set; }
        string ContentRootPath { get; set; }
        TpEmailPadrao Email { get; set; }
        public TpMaterialEducativo MaterialEducativo { get; set; }
        TpEGestor EGestor { get; set; }


        public string SuperOraculoURLAPI { get; set; }
        public string[] OrigensReconhecidas { get; set; }


    }
}
