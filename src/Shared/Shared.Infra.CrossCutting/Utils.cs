using Markdig;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Shared.Infra.CrossCutting
{

    public static class Utils
    {
        public static System.Globalization.CultureInfo CultureInfoPadrao()
        {
            return new System.Globalization.CultureInfo("pt-br", false);
        }

        public static string DecimalParaString(decimal valor)
        {
            return valor.ToString("N2", new System.Globalization.CultureInfo("pt-br", false));
        }

        public static string ObterDescricaoEnum(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

        public static bool EnumValido(Enum en)
        {
            Type type = en.GetType();

            return Enum.IsDefined(type, en.ToString());
        }

        public static string ObterDiaSemana (int diaSemana)
        {
            string d = "";
            switch (diaSemana)
            {
                case 1: 
                    d = "Domingo";
                    break;
                case 2:
                    d = "Segunda-feira";
                    break;
                case 3:
                    d = "Terça-feira";
                    break;
                case 4:
                    d = "Quarta-feira";
                    break;
                case 5:
                    d = "Quinta-feira";
                    break;
                case 6:
                    d = "Sexta-feira";
                    break;
                case 7:
                    d = "Sábado";
                    break;

            }

            return d;
        }

        public static string ObterMesNome(int mes, bool abreviado = false)
        {
            string d = "";
            switch (mes)
            {
                case 1:
                    d = !abreviado ? "Janeiro" : "Jan";
                    break;
                case 2:
                    d = !abreviado ? "Fevereiro" : "Fev";
                    break;
                case 3:
                    d = !abreviado ? "Março" : "Mar";
                    break;
                case 4:
                    d = !abreviado ? "Abril" : "Abr";
                    break;
                case 5:
                    d = !abreviado ? "Maio" : "Mai";
                    break;
                case 6:
                    d = !abreviado ? "Junho" : "Jun";
                    break;
                case 7:
                    d = !abreviado ? "Julho" : "Jul";
                    break;
                case 8:
                    d = !abreviado ? "Agosto" : "Ago";
                    break;
                case 9:
                    d = !abreviado ? "Setembro" : "Set";
                    break;
                case 10:
                    d = !abreviado ? "Outubro" : "Out";
                    break;
                case 11:
                    d = !abreviado ? "Novembro" : "Nov";
                    break;
                case 12:
                    d = !abreviado ? "Dezembro" : "Dez";
                    break;


            }

            return d;
        }


        public static string TextoPlanoParaHTML(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            return texto.Replace("\n", "<br />");
        }


        public static string ByteArrayParaHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static string HexStringParaString(string HexString)
        {
            string stringValue = "";
            for (int i = 0; i < HexString.Length / 2; i++)
            {
                string hexChar = HexString.Substring(i * 2, 2);
                int hexValue = Convert.ToInt32(hexChar, 16);
                stringValue += Char.ConvertFromUtf32(hexValue);
            }
            return stringValue;
        }

        public static string RemoverAcentos(string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }

        public static string StringParaBase64(string texto)
        {
            byte[] textoAsBytes = Encoding.ASCII.GetBytes(texto);
            return System.Convert.ToBase64String(textoAsBytes);
        }

        public static string Base64ParaString(string texto)
        {
            byte[] dadosAsBytes = System.Convert.FromBase64String(texto);
            return System.Text.ASCIIEncoding.ASCII.GetString(dadosAsBytes);
        }

        public static string TruncarString(string texto, int tamanho)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            texto = texto.Trim();
            if (texto.Length > tamanho)
            {
                texto = texto.Substring(0, tamanho);
            }

            return texto;
        }

        public static string RemoverCaracteresNaoNumericos(string texto)
        {
            return Regex.Replace(texto, "\\D", String.Empty);
        }


        public static string TextoMarkdownParaHTML(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            var md = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSoftlineBreakAsHardlineBreak().Build();

            texto = Markdown.ToHtml(texto, md);
            return texto;
        }

    }


}
