using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Epicoin.Library.Tools
{
    public static class Rsa
    {
        public static string[] GenKey(int nbByte)
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(nbByte);
            RSAParameters privKey = csp.ExportParameters(true);
            RSAParameters pubKey = csp.ExportParameters(false);
            string priKeyString = RsaToString(privKey);
            string pubKeyString = RsaToString((pubKey));
            return new[] {priKeyString, pubKeyString};
        }

        public static string RsaToString(RSAParameters key)
        {
            StringWriter sw = new System.IO.StringWriter();
            XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, key);
            string keyString = sw.ToString();
            return keyString;
        }

        public static RSAParameters StringToRsa(string key)
        {
            StringReader sr = new System.IO.StringReader(key);
            XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            RSAParameters rsaKey = (RSAParameters) xs.Deserialize(sr);
            return rsaKey;
        }

        public static string Encrypt(string pubKey, string text)
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(StringToRsa(pubKey));
            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(text);
            var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
            var cypherText = Convert.ToBase64String(bytesCypherText);
            return cypherText;
        }

        public static string Decrypt(string privKey, string cryptText)
        {
            byte[] bytesCypherText = Convert.FromBase64String(cryptText);
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(StringToRsa(privKey));
            byte[] bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
            string plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
            return plainTextData;
        }
    }
}