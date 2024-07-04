using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows.Forms; 

namespace Sa.DB
{
    public partial class Tools
    {
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 呼叫資料庫連線對話框來產生資料庫連線字串
        /// </summary>
        public static string CreateConnectString()
        {
            // [方案總館]加入參考：[Com][Microsoft ActiveX Data Objects 2.7 Library]
            // [方案總館]加入參考：[Com][Microsoft OLE DB Service Component 1.0 Type Library]
            string sRet = "";
            /*
            MSDASC.DataLinks objDataSourceDlg = new MSDASC.DataLinks();
            ADODB._Connection adoConnection = (ADODB._Connection)objDataSourceDlg.PromptNew();
            if (adoConnection != null) sRet = adoConnection.ConnectionString;
             */
            return sRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 測試資料庫連線字串是否可以連線，若有問題則呼叫資料庫連線對話框來產生資料庫連線字串
        /// </summary>
        /// <param name="sConnString">將要回傳的資料庫連線字串</param>
        /// <param name="sDBName">資料庫描述(名稱)</param>
        /// <returns>連線字串是否正常</returns>
        public static Boolean TestConnString(ref string sConnString, string sDBName)
        {                                          　
            Boolean IsOK = true;

            if (Check.IsEmptyStr(sConnString))
            {
                string sMessage = "資料庫連結字串尚未設定，是否要現在設定 ?";
                if (!Check.IsEmptyStr(sDBName)) sMessage = "【" + sDBName + "】" + sMessage;
                string sCaption = "資料庫連線設定";
                MessageBoxButtons Buttons = MessageBoxButtons.YesNo;
                MessageBoxIcon Icon = MessageBoxIcon.Question;
                DialogResult Result;
                Result = MessageBox.Show(sMessage, sCaption, Buttons, Icon);

                if (Result == DialogResult.Yes)
                {
                    sConnString = CreateConnectString();
                }
            }

            if (!Check.IsEmptyStr(sConnString))
            {
                OleDbConnection oConn = new OleDbConnection(sConnString);
                try
                {
                    oConn.Open();
                    oConn.Close();
                }
                catch
                {
                    string sMessage = "資料庫無法正常連結，是否須要重新設定連結字串 ?";
                    if (!Check.IsEmptyStr(sDBName)) sMessage = "【" + sDBName + "】" + sMessage;
                    string sCaption = "資料庫連結錯誤";
                    MessageBoxButtons Buttons = MessageBoxButtons.YesNo;
                    MessageBoxIcon Icon = MessageBoxIcon.Error;
                    DialogResult Result;
                    Result = MessageBox.Show(sMessage, sCaption, Buttons, Icon);

                    if (Result == DialogResult.Yes)
                    {
                        string sTestString = CreateConnectString();
                        oConn.ConnectionString = sTestString;
                        try
                        {
                            oConn.Open();
                            oConn.Close();
                            sConnString = sTestString;
                        }
                        catch
                        {
                            IsOK = false;
                        }
                    }
                    else IsOK = false;
                }
                oConn.Dispose();
            }
            else IsOK = false;
            return IsOK;
        }


    }
}
