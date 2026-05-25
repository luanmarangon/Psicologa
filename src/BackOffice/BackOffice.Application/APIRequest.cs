using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Psicologa.Application
{
    public class APIRequest
    {
        public static string CriptoChave = "super-oraculo-client";

        public enum TpMethod
        {
            POST,
            GET,
            DELETE,
            PUT
        }

        public string Request(string url, string partialEndPoint, TpMethod method, object data)
        {
            ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Uri baseUri = new Uri(url);
            baseUri = new Uri(baseUri, partialEndPoint);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(baseUri.AbsoluteUri);
            webRequest.Method = method.ToString();
            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json";
            webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            webRequest.Headers.Add("Authorization", Token());

            if (method == TpMethod.POST || method == TpMethod.PUT)
            {
                if (data != null)
                {
                    using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        var serializeOptions = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        };

                        string json = System.Text.Json.JsonSerializer.Serialize(data, serializeOptions);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }
                }
            }

            string jsonResp = "";

            try
            {
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        jsonResp = rd.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //AppLogger.ErrorContext(ex, $"Erro na comunicação com a API - EndPoint: {baseUri.AbsoluteUri}  - Token usado: {Token()}", partialEndPoint, data, true);

                if (ex.GetType() == typeof(WebException))
                {
                    var r = ((WebException)ex).Response;
                    if (r != null)
                    {
                        using (StreamReader rd = new StreamReader(r.GetResponseStream()))
                        {
                            jsonResp = rd.ReadToEnd();
                            jsonResp = Regex.Unescape(jsonResp);
                        }
                    }
                }
            }

            return jsonResp;
        }

        public string RequestEGestor(string url, string token, string usuario,  string partialEndPoint, TpMethod method, object data)
        {
            ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Uri baseUri = new Uri(url);
            baseUri = new Uri(baseUri, partialEndPoint);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(baseUri.AbsoluteUri);
            webRequest.Method = method.ToString();
            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json";
            webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            webRequest.Headers.Add("Authorization", "Bearer " + token);
            webRequest.Headers.Add("Usuario", usuario);

            if (method == TpMethod.POST || method == TpMethod.PUT)
            {
                if (data != null)
                {
                    using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        var serializeOptions = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        };

                        string json = System.Text.Json.JsonSerializer.Serialize(data, serializeOptions);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }
                }
            }

            string jsonResp = "";

            try
            {
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        jsonResp = rd.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //AppLogger.ErrorContext(ex, $"Erro na comunicação com a API - EndPoint: {baseUri.AbsoluteUri}  - Token usado: {Token()}", partialEndPoint, data, true);

                if (ex.GetType() == typeof(WebException))
                {
                    var r = ((WebException)ex).Response;
                    if (r != null)
                    {
                        using (StreamReader rd = new StreamReader(r.GetResponseStream()))
                        {
                            jsonResp = rd.ReadToEnd();
                            jsonResp = Regex.Unescape(jsonResp);
                        }
                    }
                }
            }

            return jsonResp;
        }

        private string Token()
        {
            var token = new
            {
                validade = DateTime.Now.AddHours(4)
            };

            string valor = System.Text.Json.JsonSerializer.Serialize(token);

            return APIRequest.Criptografar(valor);
        }

        public static string Descriptografar(string valor)
        {
            string texto = "";
            byte[] data = Convert.FromBase64String(valor);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(CriptoChave));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    texto = UTF8Encoding.UTF8.GetString(results);
                }
            }

            return texto;
        }

        public static string Criptografar(string valor)
        {

            byte[] data = UTF8Encoding.UTF8.GetBytes(valor);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(CriptoChave));
                var teste = Convert.ToBase64String(keys, 0, keys.Length);
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }
    }
}
