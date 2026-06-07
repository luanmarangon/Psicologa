using Microsoft.AspNetCore.Http;
using System;

namespace Psicologa.Application.Prontuario.ViewsModel
{
    public class ProntuarioAnexoViewModel
    {
        public int Id { get; set; }
        public int ProntuarioId { get; set; }
        public int TipoAnexo { get; set; }
        public string Nome { get; set; }
        public string NomeArquivo { get; set; }
        public string MimeType { get; set; }
        public long TamanhoArquivo { get; set; }
        public byte[] Arquivo { get; set; }
        public string Observacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class ProntuarioAnexoConsultaViewModel
    {
        public int Id { get; set; }
        public int ProntuarioId { get; set; }
        public int TipoAnexo { get; set; }
        public string TipoAnexoDescricao { get; set; }
        public string Nome { get; set; }
        public string NomeArquivo { get; set; }
        public string MimeType { get; set; }
        public string TipoArquivo { get; set; }
        public long TamanhoArquivo { get; set; }
        public byte[] Arquivo { get; set; }
        public string Observacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

}