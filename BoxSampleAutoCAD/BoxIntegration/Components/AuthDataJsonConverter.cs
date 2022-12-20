using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BoxSampleAutoCAD.BoxIntegration.DataModels;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BoxSampleAutoCAD.BoxIntegration.Components
{
    /// <summary>
    /// Encrypts token.
    /// </summary>
    public class AuthDataJsonConverter : JsonConverter<AuthData>
    {
        private readonly string mKey;

        public AuthDataJsonConverter(string key)
        {
            this.mKey = key;
        }

        public override AuthData ReadJson(JsonReader reader, Type objectType, AuthData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return default;
            var obj = JObject.Load(reader);
            var result = new AuthData();

            foreach (var p in objectType.GetProperties())
            {
                if (p.CanRead && p.CanWrite && !p.GetIndexParameters().Any())
                {
                    var token = obj[p.Name];
                    var val = token != null ? token.ToObject(p.PropertyType, serializer) : null;
                    if (p.Name.EndsWith("Token"))
                    {
                        try
                        {
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
                    else
                    {
                        if (val != null)
                        {
                            p.SetValue(result, val);
                        }
                    }
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, AuthData value, JsonSerializer serializer)
        {
            var obj = new JObject();
            foreach (var p in value.GetType().GetProperties())
            {
                if (p.CanRead && p.CanWrite && !p.GetIndexParameters().Any())
                {
                    var val = p.GetValue(value)?.ToString();
                    var encryptedVal = string.Empty;
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        if (p.Name.EndsWith("Token"))
                        {
                            encryptedVal = Encrypt(val, mKey);
                            obj.Add(p.Name, JToken.FromObject(encryptedVal));
                        }
                        else
                            obj.Add(p.Name, JToken.FromObject(val));
                    }

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
