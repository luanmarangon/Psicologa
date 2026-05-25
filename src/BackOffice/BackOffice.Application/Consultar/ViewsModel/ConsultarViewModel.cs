using System;

namespace Psicologa.Application.BlogPost.ViewsModel
{
    public class ConsultarViewModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Url { get; set; }
        public string ImagemCapa { get; set; }
        public string Resumo { get; set; }
        public DateTime? DataPublicacao { get; set; }
    }
}
