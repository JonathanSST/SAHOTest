using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using System.Net;
using System.Security.Cryptography;
using System.IO;

namespace SahoAcs
{
    public class DesCryp
    {

        #region 1.宣告變數
        static private string m_DBIV = "D@T@B@S1";
        static private string m_DBKey = "EsAbAtAd";
        #endregion

        public static string EncryptMsg(string sSourceMsg, string sDBIV = "", string sDBKey = "")
        {
            string sEncryptMsg = "";
            byte[] byteIVAY, byteKeyAY, byteSourceAY;
            DESCryptoServiceProvider oDES = new DESCryptoServiceProvider();
            try
            {
                string myKey = m_DBKey;
                string myDBIV = m_DBIV;
                if (sDBIV != "")
                    myDBIV = sDBIV;
                if (sDBKey != "")
                    myKey = sDBKey;
                byteIVAY = Encoding.ASCII.GetBytes(myDBIV);
                byteKeyAY = Encoding.ASCII.GetBytes(myKey);
                oDES = new DESCryptoServiceProvider() { IV = byteIVAY, Key = byteKeyAY };

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, oDES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byteSourceAY = Encoding.UTF8.GetBytes(sSourceMsg);

                        cs.Write(byteSourceAY, 0, byteSourceAY.Length);
                        cs.FlushFinalBlock();

                        sEncryptMsg = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oDES?.Clear(); oDES = null;
                byteIVAY = null; byteKeyAY = null; byteSourceAY = null;
            }

            return sEncryptMsg;
        }

        public static string DecrypMsg(string sEncryptMsg, string sDBIV = "", string sDBKey = "")
        {
            string sDecrypMsg = "";
            byte[] byteIVAY, byteKeyAY, byteEncryptMsg;
            DESCryptoServiceProvider oDES = null;

            try
            {
                string myKey = m_DBKey;
                string myDBIV = m_DBIV;
                if (sDBIV != "")
                    myDBIV = sDBIV;
                if (sDBKey != "")
                    myKey = sDBKey;
                byteIVAY = Encoding.ASCII.GetBytes(myDBIV);
                byteKeyAY = Encoding.ASCII.GetBytes(myKey);
                oDES = new DESCryptoServiceProvider() { IV = byteIVAY, Key = byteKeyAY };

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, oDES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        byteEncryptMsg = Convert.FromBase64String(sEncryptMsg);

                        cs.Write(byteEncryptMsg, 0, byteEncryptMsg.Length);
                        cs.FlushFinalBlock();

                        sDecrypMsg = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                sDecrypMsg = "";
            }
            finally
            {
                oDES?.Clear(); oDES = null;
                byteIVAY = null; byteKeyAY = null; byteEncryptMsg = null;
            }

            return sDecrypMsg;
        }



    }//end class
}//end namespace