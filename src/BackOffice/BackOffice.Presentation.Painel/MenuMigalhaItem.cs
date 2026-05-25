namespace Psicologa.Presentation.Painel
{
    public class MenuMigalhaItem
    {
        public string Nome { get; set; }
        public string URL { get; set; }

        public MenuMigalhaItem(string nome, string url)
        {
            Nome = nome;
            URL = url;
        }
    }
}
