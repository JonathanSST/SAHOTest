using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SahoAcs.DBClass
{


    public class RSAEnc
    {
        private static string _privateKey;
        private static string _publicKey;
        private static UnicodeEncoding _encoder = new UnicodeEncoding();
        private static RSACryptoServiceProvider rsa;

        public static string Encrypt(string data)
        {
            /*
            var _cspParameters = new CspParameters();
            _cspParameters.KeyContainerName = "RsaCspParameters_Key";
            _cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
            _cspParameters.ProviderName = "Microsoft Strong Cryptographic Provider";
            _cspParameters.ProviderType = 1;                        
            rsa = new RSACryptoServiceProvider(1024,_cspParameters);
            */
            _publicKey = "<RSAKeyValue><Modulus>0A3qDTi4bYcFV9buFTW1akASDY+2hjfbFrclb6NTmVBoG+Fw0ZlTQUb8rIqoibs7dwLpHKCB1quI4eZlNYSBATXxa7BqTznXwP032IKTzaadHsyvKS4KAyvxHmNSbLCGqxnUCUPJnI8nL/T6TSOtNdvHsmVNzOKXQ73ki8z9dlk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_publicKey);
            Encoding enc = Encoding.UTF8;
            var originalData = enc.GetBytes(data);
            var encryptData = Encryptor(originalData);
            var base64 = Convert.ToBase64String(encryptData);
            return base64;
        }


        public static byte[] Encryptor(byte[] OriginalData)
        {
            if (OriginalData == null || OriginalData.Length <= 0)
            {
                throw new NotSupportedException();
            }
            if (rsa == null)
            {
                throw new ArgumentNullException();
            }            

            int bufferSize = (rsa.KeySize / 8) - 11;
            byte[] buffer = new byte[bufferSize];

            //分段加密
            using (MemoryStream input = new MemoryStream(OriginalData))
            using (MemoryStream ouput = new MemoryStream())
            {
                while (true)
                {
                    int readLine = input.Read(buffer, 0, bufferSize);
                    if (readLine <= 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[readLine];
                    Array.Copy(buffer, 0, temp, 0, readLine);
                    byte[] encrypt = rsa.Encrypt(temp, false);
                    ouput.Write(encrypt, 0, encrypt.Length);
                }
                return ouput.ToArray();
            }
        }

    }//end class
}//end namesapce