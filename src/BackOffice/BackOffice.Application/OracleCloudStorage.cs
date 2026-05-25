using Oci.Common;
using Oci.Common.Auth;
using Oci.ObjectstorageService;
using Oci.ObjectstorageService.Requests;
using System;
using System.IO;
using System.Text;

namespace Psicologa.Application
{
    public class OracleCloudStorage
    {
        public enum TpBucket
        {
            BancoProdutoImagens = 1,
            SuperMeuDeliveryMarcasClientes = 2
        }

        private string _namespaceName = "grcd2uh6cblo";
        private string _bucket = "portal-academy";

        private string _bucketURLPublico = "";

        private ObjectStorageClient _osClient;


        public OracleCloudStorage()
        {
            _bucketURLPublico = $@"https://objectstorage.sa-saopaulo-1.oraclecloud.com/n/{_namespaceName}/b/{_bucket}/o/";

            var provider = new ConfigFileAuthenticationDetailsProvider(@".oci/config", "DEFAULT");
            _osClient = new ObjectStorageClient(provider, new ClientConfiguration());
        }

        public bool Upload(byte[] arquivo, string objectName, string folder = "", int cacheMinutos = 0)
        {
            try
            {
                var putObjectRequest = new PutObjectRequest()
                {
                    BucketName = _bucket,
                    NamespaceName = _namespaceName,
                    ObjectName = objectName,
                    PutObjectBody = new MemoryStream(arquivo)
                };

                if (!string.IsNullOrEmpty(folder))
                {
                    putObjectRequest.ObjectName = folder + "/" + objectName;
                }

                if (cacheMinutos > 0)
                    putObjectRequest.CacheControl = "public, max-age=" + cacheMinutos * 60;

                var r = _osClient.PutObject(putObjectRequest).Result;

                return !string.IsNullOrEmpty(r.ETag);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public byte[] Get(string objectName, string folder = "")
        {
            try
            {
                var getObjectRequest = new GetObjectRequest()
                {
                    BucketName = _bucket,
                    NamespaceName = _namespaceName,
                    ObjectName = objectName
                };


                if (!string.IsNullOrEmpty(folder))
                {
                    getObjectRequest.ObjectName = folder + "/" + objectName;
                }

                var r = _osClient.GetObject(getObjectRequest).Result;

                byte[] obj = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    r.InputStream.CopyTo(ms);
                    obj = ms.ToArray();
                }

                return obj;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public bool Delete(string objectName, string folder = "")
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest()
                {
                    BucketName = _bucket,
                    NamespaceName = _namespaceName,
                    ObjectName = objectName,
                };

                if (!string.IsNullOrEmpty(folder))
                {
                    deleteObjectRequest.ObjectName = folder + "/" + objectName;
                }

                var r = _osClient.DeleteObject(deleteObjectRequest).Result;

                return string.IsNullOrEmpty(r.VersionId);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool DeleteFolder(string folder)
        {
            try
            {

                var listObjectsRequest = new ListObjectsRequest
                {
                    BucketName = _bucket,
                    NamespaceName = _namespaceName,
                    Prefix = folder + "/"
                };

                var objectsResponse = _osClient.ListObjects(listObjectsRequest).Result;

                foreach (var obj in objectsResponse.ListObjects.Objects)
                {
                    var deleteObjectRequest = new DeleteObjectRequest
                    {
                        BucketName = _bucket,
                        NamespaceName = _namespaceName,
                        ObjectName = obj.Name
                    };

                    _osClient.DeleteObject(deleteObjectRequest).Wait();
                }

                return true;



            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public string GetObjectPublicURI(string objectName, string folder = "")
        {
            if (!string.IsNullOrEmpty(folder))
            {
                objectName = folder + "/" + objectName;
            }

            return _bucketURLPublico + objectName;
        }

        public static string GetHashObjectName(string name)
        {
            string entrada = name + "bUcK3t$";

            try
            {
                using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(entrada);
                    byte[] hashBytes = sha.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }


                    return sb.ToString().ToLower();
                }
            }
            catch
            {
            }

            return string.Empty;
        }

    }
}
