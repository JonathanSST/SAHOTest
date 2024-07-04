using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;



namespace SahoTestConsole
{
    public class DB_Acs : Sa.DB.Base
    {        

        //-------------------------------------------------------------------------------------------------------------
        public DB_Acs(string sConnString)
        {
            OpenConnect(sConnString);
        }


        #region SysParameter Procedure

        //-------------------------------------------------------------------------------------------------------------
        public Boolean GetSysParameter(string sParaClass, string sParaNo, string sParaName, string sParaDesc, Boolean blParaDefault, out Boolean blParaValue)
        {
            blParaValue = false;
            Boolean IsOK = false;
            if (!GetSysParameter(sParaClass, sParaNo, out blParaValue))
            {
                blParaValue = blParaDefault;
                IsOK = InsertSysParameter(sParaClass, sParaNo, sParaName, sParaDesc, blParaValue);
            }
            return IsOK;
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean GetSysParameter(string sParaClass, string sParaNo, string sParaName, string sParaDesc, Int32 iParaDefault, out Int32 iParaValue)
        {
            iParaValue = 0;
            Boolean IsOK = false;
            if (!GetSysParameter(sParaClass, sParaNo, out iParaValue))
            {
                iParaValue = iParaDefault;
                IsOK = InsertSysParameter(sParaClass, sParaNo, sParaName, sParaDesc, iParaValue);
            }
            return IsOK;
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean GetSysParameter(string sParaClass, string sParaNo, string sParaName, string sParaDesc, string sParaDefault, out string sParaValue)
        {
            sParaValue = "";
            Boolean IsOK = false;
            if (!GetSysParameter(sParaClass, sParaNo, out sParaValue))
            {
                sParaValue = sParaDefault;
                IsOK = InsertSysParameter(sParaClass, sParaNo, sParaName, sParaDesc, sParaValue);
            }
            return IsOK;
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean GetSysParameter(string sParaClass, string sParaNo, out Boolean blParaValue)
        {
            Boolean IsOK = false;
            Boolean IsNoData = false;

            blParaValue = false;
            List<string> liSqlPara = new List<string>();
            string sSqlCmd = "SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = ? AND ParaNo = ?";
            liSqlPara.Add("S:" + sParaClass);
            liSqlPara.Add("S:" + sParaNo);
            if (CheckDBConnect())
            {
                IsOK = GetBooleanScalar(sSqlCmd, liSqlPara, out blParaValue, out IsNoData);
            }
            return !IsNoData;
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean GetSysParameter(string sParaClass, string sParaNo, out Int32 iParaValue)
        {
            Boolean IsOK = false;
            Boolean IsNoData = false;

            iParaValue = 0;
            List<string> liSqlPara = new List<string>();
            string sSqlCmd = "SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = ? AND ParaNo = ?";
            liSqlPara.Add("S:" + sParaClass);
            liSqlPara.Add("S:" + sParaNo);

            if (CheckDBConnect())
            {
                IsOK = GetIntScalar(sSqlCmd, liSqlPara, out iParaValue, out IsNoData);
            }
            return !IsNoData;
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean GetSysParameter(string sParaClass, string sParaNo, out string sParaValue)
        {
            Boolean IsOK = false;
            Boolean IsNoData = false;

            sParaValue = "";
            List<string> liSqlPara = new List<string>();
            string sSqlCmd = "SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = ? AND ParaNo = ?";
            liSqlPara.Add("S:" + sParaClass);
            liSqlPara.Add("S:" + sParaNo);

            if (CheckDBConnect())
            {
                IsOK = GetStrScalar(sSqlCmd, liSqlPara, out sParaValue, out IsNoData);
            }
            return !IsNoData;
        }


        //-------------------------------------------------------------------------------------------------------------
        public Boolean UpdateSysParameter(string sParaClass, string sParaNo, Boolean blParaValue)
        {
            string sParaValue = "False";
            if (blParaValue) sParaValue = "True";
            return UpdateSysParameter(sParaClass, sParaNo, sParaValue);
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean UpdateSysParameter(string sParaClass, string sParaNo, Int32 iParaValue)
        {
            string sParaValue = iParaValue.ToString();
            return UpdateSysParameter(sParaClass, sParaNo, sParaValue);
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean UpdateSysParameter(string sParaClass, string sParaNo, string sParaValue)
        {
            Boolean IsOK = false;
            List<string> liSqlPara = new List<string>();
            string sSqlCmd = "UPDATE B00_SysParameter SET ParaValue = ? WHERE ParaClass = ? AND ParaNo = ?";
            liSqlPara.Add("S:" + sParaValue);
            liSqlPara.Add("S:" + sParaClass);
            liSqlPara.Add("S:" + sParaNo);

            if (CheckDBConnect())
            {
                Int32 iRet = SqlCommandExecute(sSqlCmd, liSqlPara);
                IsOK = (iRet > 0);
            }
            return IsOK;
        }


        //-------------------------------------------------------------------------------------------------------------
        public Boolean InsertSysParameter(string sParaClass, string sParaNo, string sParaName, string sParaDesc, Boolean blParaValue)
        {
            string sParaValue = "False";
            if (blParaValue) sParaValue = "True";
            return InsertSysParameter(sParaClass, sParaNo, sParaName, "Boolean", sParaDesc, sParaValue);
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean InsertSysParameter(string sParaClass, string sParaNo, string sParaName, string sParaDesc, Int32 iParaValue)
        {
            string sParaValue = iParaValue.ToString();
            return InsertSysParameter(sParaClass, sParaNo, sParaName, "Int32", sParaDesc, sParaValue);
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean InsertSysParameter(string sParaClass, string sParaNo, string sParaName, string sParaDesc, string sParaValue)
        {
            return InsertSysParameter(sParaClass, sParaNo, sParaName, "String", sParaDesc, sParaValue);
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean InsertSysParameter(string sParaClass, string sParaNo, string sParaName, string sParaType, string sParaDesc, string sParaValue)
        {
            Boolean IsOK = false;
            List<string> liSqlPara = new List<string>();
            string sSqlCmd = "INSERT INTO B00_SysParameter (ParaClass, ParaNo, ParaName, ParaValue, ParaType, ParDesc) " +
                             "VALUES (?, ?, ?, ?, ?, ?)";

            if ((sParaName == "<") || (sParaName == "---") || (sParaName == "=")) sParaName = sParaNo;
            if ((sParaDesc == "<") || (sParaDesc == "---") || (sParaDesc == "=")) sParaDesc = sParaName;

            liSqlPara.Add("S:" + sParaClass);
            liSqlPara.Add("S:" + sParaNo);
            liSqlPara.Add("S:" + sParaName);
            liSqlPara.Add("S:" + sParaValue);
            liSqlPara.Add("S:" + sParaType);
            liSqlPara.Add("S:" + sParaDesc);

            if (CheckDBConnect())
            {
                Int32 iRet = SqlCommandExecute(sSqlCmd, liSqlPara);
                IsOK = (iRet > 0);
            }
            return IsOK;
        }

        # endregion


        /// <summary>
        /// 指定TableName，並取回其該Table欄位中文名稱
        /// </summary>
        /// <param name="sTableName">Table名稱</param>
        /// <param name="sLanguage">欄位語系[zhtw:繁中 enus:英文],預設為zhtw</param>
        /// <param name="oHashTable">回傳的HashTable</param>
        /// <returns></returns>
        /// 
        public Boolean GetTableHash(string sTableName, string sLanguage, out Hashtable oHashTable)
        {
            string sql = "";
            Boolean IsOK = false;
            oHashTable = new Hashtable();
            DataTable oTable = new DataTable();
            List<string> liSqlPara = new List<string>();

            sql = @" SELECT * FROM B00_FieldNameList
                    WHERE TableName  = ?";

            liSqlPara.Add("S:" + sTableName);

            IsOK = GetDataTable("TableInfo", sql, liSqlPara, out oTable);

            if (oTable.Rows.Count > 0)
            {
                foreach (DataRow oTr in oTable.Rows)
                {
                    if (string.Compare(sLanguage, "zhtw", true) == 0)
                    {
                        oHashTable.Add(oTr["FieldName"].ToString(), oTr["ChtName"].ToString());
                    }
                    else if (string.Compare(sLanguage, "enus", true) == 0)
                    {
                        oHashTable.Add(oTr["FieldName"].ToString(), oTr["EngName"].ToString());
                    }
                    else
                    {
                        oHashTable.Add(oTr["FieldName"].ToString(), oTr["ChtName"].ToString());
                    }
                }
            }
            return IsOK;
        }       
       
       
        public enum Logtype
        {
            資料查詢 = 0,
            資料新增 = 1,
            資料修改 = 2,
            資料刪除 = 3,
            資料儲存 = 4,
            功能選單切換 = 5,
            人員登入登出 = 10,
            更改使用者密碼 = 11,
            人員權限設定 = 12,
            門禁系統資料維護 = 20,
            門禁設備設定 = 21,
            設備設消碼操作 = 31,
            設備設消碼錯誤 = 32
        }
    }
}

