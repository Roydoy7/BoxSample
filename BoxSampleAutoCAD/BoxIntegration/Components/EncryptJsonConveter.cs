using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BoxSampleAutoCAD.BoxIntegration.Components
{
    public class EncryptJsonConveter<T> : JsonConverter<T> where T : class
    {
        private readonly string mKey;

        public EncryptJsonConveter(string key)
        {
            this.mKey = key;
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return default;
            var obj = JObject.Load(reader);
            var result = Activator.CreateInstance(objectType) as T;

            foreach (var p in objectType.GetProperties())
            {
                if (p.CanRead && p.CanWrite && !p.GetIndexParameters().Any())
                {
                    var token = obj[p.Name];
                    try
                    {
                        var val = token != null ? token.ToObject(p.PropertyType, serializer) : null;
                        var decryptedVal = string.Empty;
                        if (val != null)
                        {
                            var valStr = val.ToString();
                            decryptedVal = Decrypt(valStr, mKey);
                        }

                        p.SetValue(result, decryptedVal);
                    }
                    catch
                    {
                        p.SetValue(result, default);
                    }
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, T value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var obj = new JObject();
            foreach (var p in value.GetType().GetProperties())
            {
                if (p.CanRead && p.CanWrite && !p.GetIndexParameters().Any())
                {
                    var val = p.GetValue(value).ToString();
                    var encryptedVal = string.Empty;
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        encryptedVal = Encrypt(val, mKey);
                    }

                    obj.Add(p.Name, JToken.FromObject(encryptedVal));
                }
            }
            obj.WriteTo(writer);
        }

        //Borrowed from https://www.c-sharpcorner.com/UploadFile/f8fa6c/data-encryption-and-decryption-in-C-Sharp/
        private string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
