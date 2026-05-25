using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infra.CrossCutting
{
    public static class ImagemUtils
    {
        /// <summary>
        /// Converte uma imagem em disco para src base64 inline.
        /// Ex: data:image/png;base64,iVBORw0K...
        /// </summary>
        /// <param name="caminhoCompleto">Caminho físico completo do arquivo. Ex: C:\app\wwwroot\img\logo\logo.png</param>
        public static string ParaBase64Src(string caminhoCompleto)
        {
            if (!File.Exists(caminhoCompleto))
                throw new FileNotFoundException($"Imagem não encontrada: {caminhoCompleto}");

            var bytes = File.ReadAllBytes(caminhoCompleto);
            var base64 = Convert.ToBase64String(bytes);
            var mime = ObterMimeType(caminhoCompleto);

            return $"data:{mime};base64,{base64}";
        }
        /// <summary>
        /// Retorna a tag &lt;img&gt; completa com a imagem em base64,
        /// pronta para ser embutida em HTML de e-mails.
        /// </summary>
        /// <param name="caminhoCompleto">Caminho físico completo do arquivo.</param>
        /// <param name="alt">Texto alternativo da imagem.</param>
        /// <param name="height">Altura em pixels (padrão: 42).</param>
        /// <param name="estiloExtra">Estilos CSS adicionais (opcional).</param>
        public static string ParaImgTagEmail(string caminhoCompleto, string alt = "", int height = 42, string estiloExtra = "")
        {
            var src = ParaBase64Src(caminhoCompleto);
            var estilo = $"display:block;border:0;{estiloExtra}";

            return $"<img src=\"{src}\" alt=\"{alt}\" height=\"{height}\" style=\"{estilo}\" />";
        }

        /// <summary>
        /// Converte um array de bytes já carregado para src base64 inline.
        /// </summary>
        /// <param name="bytes">Bytes da imagem.</param>
        /// <param name="extensao">Extensão do arquivo. Ex: ".png", ".jpg"</param>
        public static string ParaBase64SrcDeBytes(byte[] bytes, string extensao)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentException("O array de bytes está vazio.");

            var base64 = Convert.ToBase64String(bytes);
            var mime = ObterMimeTypePorExtensao(extensao);

            return $"data:{mime};base64,{base64}";
        }

        /// <summary>
        /// Determina o MIME type pelo caminho completo do arquivo.
        /// </summary>
        private static string ObterMimeType(string caminhoCompleto)
        {
            var extensao = Path.GetExtension(caminhoCompleto);
            return ObterMimeTypePorExtensao(extensao);
        }

        /// <summary>
        /// Determina o MIME type pela extensão do arquivo.
        /// </summary>
        private static string ObterMimeTypePorExtensao(string extensao) =>
            extensao?.ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                _ => "image/png"
            };





    }



}
