using System;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Infra.CrossCutting
{
    public static class Criptografia
    {
        /// <summary>
        /// Criptografa um parâmetro.
        /// </summary>
        /// <param name="valor">valor a ser criptografado</param>      
        /// <param name="usarChaveSecreta">OPCIONAL: False indica que não usuará uma chave secreta na criptografia. Por padrão, o método gera chaves dinâmicas para aumentar a segurança.</param> 
        /// <returns>Valor criptografado.</returns>
        public static string Criptografar(string valor, bool usarChaveSecreta = false)
        {
            if (string.IsNullOrEmpty(valor) || valor == "0")
                return "0";

            int tamanhoChaveSecreta = 10;

            Random r = new Random();
            string chaveSecreta = "";
            string caracteres = "0123456789abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVXYZ";

            if (usarChaveSecreta)
            {
                for (int i = 0; i < tamanhoChaveSecreta; i++)
                {
                    int num = r.Next(0, 1000000);
                    chaveSecreta += caracteres[((i + 1) * num) % caracteres.Length];
                }
            }
            else chaveSecreta = "".PadLeft(tamanhoChaveSecreta, '0');

            string aux = valor;

            byte[] bytes = System.Text.ASCIIEncoding.UTF8.GetBytes(aux);
            string base64String = System.Convert.ToBase64String(bytes);
            string base64StringInvertida = "";

            for (int i = base64String.Length - 1; i >= 0; i--)
            {
                base64StringInvertida += base64String[i];
            }


            return chaveSecreta + base64StringInvertida;
        }


        public static string Descriptografar(string valor)
        {
            if (string.IsNullOrEmpty(valor) || valor == "0")
                return "0";

            try
            {
                int tamanhoChaveSecreta = 10;
                valor = valor.Substring(tamanhoChaveSecreta);
                string base64String = "";
                for (int i = valor.Length - 1; i >= 0; i--)
                {
                    base64String += valor[i];
                }
                byte[] bytes = System.Convert.FromBase64String(base64String);

                return System.Text.ASCIIEncoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #region Criptografia TripleDES
        public static string DescriptografarTripleDES(string conteudo, string chave)
        {
            byte[] data = Convert.FromBase64String(conteudo);
            using (MD5 md5 = MD5.Create())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(chave));

                using (TripleDES tripDes = TripleDES.Create())
                {
                    tripDes.Key = keys;
                    tripDes.Mode = CipherMode.ECB;
                    tripDes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return UTF8Encoding.UTF8.GetString(results);
                }
            }
        }

        public static string CriptografarTripleDES(string conteudo, string chave)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(conteudo);
            using (MD5 md5 = MD5.Create())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(chave));
                var teste = Convert.ToBase64String(keys, 0, keys.Length);
                using (TripleDES tripDes = TripleDES.Create())
                {
                    tripDes.Key = keys;
                    tripDes.Mode = CipherMode.ECB;
                    tripDes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }

        }

        #endregion


        public static string CriptografarMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }


}
