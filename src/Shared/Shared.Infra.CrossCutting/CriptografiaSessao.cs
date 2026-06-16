using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infra.CrossCutting
{
    public static class CriptografiaSessao
    {
        private static string _chaveSecreta = "psicologia";
        private static byte[] GerarChave(string usuarioId, string usuarioNome)
        {
            string chaveBase = $"{usuarioId}|{usuarioNome}|{_chaveSecreta}";

            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(chaveBase));
            }
        }

        public static string Criptografar(string texto, string usuarioId, string usuarioNome)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            byte[] chave = GerarChave(usuarioId, usuarioNome);

            using (Aes aes = Aes.Create())
            {
                aes.Key = chave;
                aes.GenerateIV();

                using MemoryStream ms = new();

                // Salva IV no início
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new(
                    ms,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write))
                using (StreamWriter sw = new(cs))
                {
                    sw.Write(texto);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string Descriptografar(string textoCriptografado, string usuarioId, string usuarioNome)
        {
            if (string.IsNullOrWhiteSpace(textoCriptografado))
                return string.Empty;

            byte[] dados = Convert.FromBase64String(textoCriptografado);
            byte[] chave = GerarChave(usuarioId, usuarioNome);

            using Aes aes = Aes.Create();

            byte[] iv = new byte[aes.BlockSize / 8];

            Array.Copy(dados, 0, iv, 0, iv.Length);

            aes.Key = chave;
            aes.IV = iv;

            using MemoryStream ms = new(dados, iv.Length, dados.Length - iv.Length);

            using CryptoStream cs = new(
                ms,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read);

            using StreamReader sr = new(cs);

            return sr.ReadToEnd();
        }
    }
}
