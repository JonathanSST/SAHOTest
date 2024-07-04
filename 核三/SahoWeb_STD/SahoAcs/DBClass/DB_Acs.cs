using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


namespace SahoAcs
{
    public class DB_Acs : Sa.DB.Base
    {
        Page __oPage = null;

        //-------------------------------------------------------------------------------------------------------------
        public DB_Acs(string sConnString)
        {
            OpenConnect(sConnString);
        }

        //-------------------------------------------------------------------------------------------------------------
        public DB_Acs(Page oPage, string sConnString)
        {
            __oPage = oPage;
            if (!OpenConnect(sConnString)) __oPage.Response.Redirect("~/Web/MessagePage/DBConnError.aspx");
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
            string sSqlCmd = "INSERT INTO B00_SysParameter (ParaClass, ParaNo, ParaName, ParaValue, ParaType, ParaDesc) " +
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


        /// <summary>
        /// 異動資料前進行備份
        /// </summary>
        /// <param name="Time">備份時間</param>
        /// <param name="ModifyUser">異動者ID</param>
        /// <param name="TableName">Table名稱</param>
        /// <param name="RecordField">備份ID值所歸屬欄位</param>
        /// <param name="RecordID">備份之ID值</param>
        /// <param name="Mode">資料異動類型[True:編輯 False:刪除]</param>
        /// <param name="oAcsDB">傳入目標DB_Acs物件</param>
        #region SaveModifyBackupInfo
        public int SaveModifyBackupInfo(DateTime Time, string ModifyUser, string TableName, string RecordField, string RecordID, bool Mode)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", Fieldsql = "";
            List<string> liSqlPara = new List<string>();
            DataTable FieldTable;
            DataTable DataTable;
            int iRet = 0;
            string FieldInfoStr = "", DataInfoStr = "", ModeStr = "";
            List<string> DataInfoList = new List<string>();

            #region Get Table's FieldInfo
            sql = @" SELECT * FROM V_SysColumns
                     WHERE TABLE_NAME = ? ";
            liSqlPara.Add("S:" + TableName);
            GetDataTable("FieldTable", sql, liSqlPara, out FieldTable);

            foreach (DataRow FieldRow in FieldTable.Rows)
            {
                if (!string.IsNullOrEmpty(FieldInfoStr)) FieldInfoStr += "|;";
                FieldInfoStr += FieldRow["Name"].ToString();

                if (!string.IsNullOrEmpty(Fieldsql)) Fieldsql += ",";
                Fieldsql += FieldRow["Name"].ToString();
            }
            #endregion

            liSqlPara.Clear();

            #region Get Table's DataInfo
            sql = @" SELECT " + Fieldsql + " FROM " + TableName + " WHERE " + RecordField + " = ?";
            liSqlPara.Add("S:" + RecordID);
            GetDataTable("DataTable", sql, liSqlPara, out DataTable);

            foreach (DataRow DataRow in DataTable.Rows)
            {
                DataInfoStr = "";
                foreach (DataRow FieldRow in FieldTable.Rows)
                {
                    if (!string.IsNullOrEmpty(DataInfoStr)) DataInfoStr += "|;";
                    DataInfoStr += DataRow[FieldRow["Name"].ToString()].ToString();
                }
                DataInfoList.Add(DataInfoStr);
            }
            #endregion

            if (Mode) ModeStr = "M";
            else ModeStr = "D";

            liSqlPara.Clear();
            sql = "";

            #region Save ModifyBackup

            for (int i = 0; i < DataInfoList.Count; i++)
            {
                sql += @" INSERT INTO B00_ModifyBackup
                      (BackupTime, BackupUserID, TableName, ModifyMode, RecordID, FieldInfo, DataInfo)
                      VALUES
                      (GETDATE(), ?, ?, ?, ?, ?, ?)";
                //liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + ModifyUser);
                liSqlPara.Add("S:" + TableName);
                liSqlPara.Add("S:" + ModeStr);
                liSqlPara.Add("I:" + RecordID);
                liSqlPara.Add("S:" + FieldInfoStr);
                liSqlPara.Add("S:" + DataInfoList[i]);
            }
            if (DataInfoList.Count > 0)
                iRet = SqlCommandExecute(sql, liSqlPara);
            #endregion
            return iRet;
        }
        #endregion

        /// <summary>
        /// 異動資料前進行備份
        /// </summary>
        /// <param name="Time">備份時間</param>
        /// <param name="ModifyUser">異動者ID</param>
        /// <param name="TableName">Table名稱</param>
        /// <param name="RecordField">備份ID值所歸屬欄位</param>
        /// <param name="RecordID">備份之ID值</param>
        /// <param name="ConditionsField">條件欄位</param>
        /// <param name="ConditionsID">條件欄位資料</param>
        /// <param name="Mode">資料異動類型[True:編輯 False:刪除]</param>
        /// <param name="oAcsDB">傳入目標DB_Acs物件</param>
        #region SaveModifyBackupInfo
        public int SaveModifyBackupInfo(DateTime Time, string ModifyUser, string TableName, string RecordField, string RecordID, string ConditionsField, string ConditionsID, bool Mode)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", Fieldsql = "";
            List<string> liSqlPara = new List<string>();
            DataTable FieldTable;
            Sa.DB.DBReader DataReader;
            int iRet = 0;
            string FieldInfoStr = "", DataInfoStr = "", ModeStr = "";

            #region Get Table's FieldInfo
            sql = @" SELECT * FROM V_SysColumns
                 WHERE TABLE_NAME = ? ";
            liSqlPara.Add("S:" + TableName);
            GetDataTable("FieldTable", sql, liSqlPara, out FieldTable);

            foreach (DataRow FieldRow in FieldTable.Rows)
            {
                if (!string.IsNullOrEmpty(FieldInfoStr)) FieldInfoStr += "|;";
                FieldInfoStr += FieldRow["Name"].ToString();

                if (!string.IsNullOrEmpty(Fieldsql)) Fieldsql += ",";
                Fieldsql += FieldRow["Name"].ToString();
            }
            #endregion

            liSqlPara.Clear();

            #region Get Table's DataInfo
            sql = @" SELECT " + Fieldsql + " FROM " + TableName + " WHERE " + RecordField + " = ? AND " + ConditionsField + " = ? ";
            liSqlPara.Add("S:" + RecordID);
            liSqlPara.Add("S:" + ConditionsID);
            GetDataReader(sql, liSqlPara, out DataReader);


            if (DataReader.Read())
            {
                foreach (DataRow FieldRow in FieldTable.Rows)
                {
                    if (!string.IsNullOrEmpty(DataInfoStr)) DataInfoStr += "|;";
                    DataInfoStr += DataReader.DataReader[FieldRow["Name"].ToString()].ToString();
                }
            }
            #endregion

            if (Mode) ModeStr = "M";
            else ModeStr = "D";

            liSqlPara.Clear();

            #region Save ModifyBackup
            sql = @" INSERT INTO B00_ModifyBackup
                 (BackupTime, BackupUserID, TableName, ModifyMode, RecordID, FieldInfo, DataInfo)
                 VALUES
                 (?, ? , ? , ? , ? , ? , ?)";
            liSqlPara.Add("D:" + Time);
            liSqlPara.Add("S:" + ModifyUser);
            liSqlPara.Add("S:" + TableName);
            liSqlPara.Add("S:" + ModeStr);
            liSqlPara.Add("I:" + RecordID);
            liSqlPara.Add("S:" + FieldInfoStr);
            liSqlPara.Add("S:" + DataInfoStr);
            iRet = SqlCommandExecute(sql, liSqlPara);
            #endregion
            return iRet;
        }
        #endregion

        /// <summary>
        /// Log資料寫入
        /// </summary>
        /// <param name="LogType">Log類型</param>
        /// <param name="UserID">使用者ID</param>
        /// <param name="UserName">使用者姓名</param>
        /// <param name="LogFrom">Log來源</param>
        /// <param name="EquNo">設備No</param>
        /// <param name="EquName">設備名稱</param>
        /// <param name="LogInfo">Log資訊</param>
        /// <param name="LogDesc">Log描述</param>
        /// <returns></returns>
        public Boolean WriteLog(Logtype LogType, string UserID, string UserName, string LogFrom, string EquNo, string EquName, string LogInfo, string LogDesc)
        {
            Boolean IsOK = false;
            string Ip = "", sql = "";
            List<string> liSqlPara = new List<string>();
            Ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            sql = @" INSERT INTO B00_SysLog (LogTime,LogType,UserID,UserName,LogFrom,LogIP,EquNo,EquName,LogInfo,LogDesc)
                     VALUES (GETDATE(), ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            liSqlPara.Add("S:" + LogType);
            liSqlPara.Add("S:" + UserID.Trim());
            liSqlPara.Add("S:" + UserName.Trim());
            liSqlPara.Add("S:" + LogFrom.Trim());
            liSqlPara.Add("S:" + Ip.Trim());
            liSqlPara.Add("S:" + EquNo.Trim());
            liSqlPara.Add("S:" + EquName.Trim());
            liSqlPara.Add("S:" + LogInfo.Trim());
            liSqlPara.Add("S:" + LogDesc.Trim());

            Int32 iRet = SqlCommandExecute(sql, liSqlPara);
            IsOK = (iRet > 0);

            return IsOK;
        }


        public List<string> GetParaPackage(Logtype LogType, string UserID, string UserName, string LogFrom, 
            string EquNo, string EquName, string LogInfo, string LogDesc)
        {            
            string Ip = "", sql = "";
            Ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            List<string> liSqlPara = new List<string>();
            liSqlPara.Add(LogType.ToString());
            liSqlPara.Add(UserID.Trim());
            liSqlPara.Add(UserName.Trim());
            liSqlPara.Add(LogFrom.Trim());
            liSqlPara.Add(Ip.Trim());
            liSqlPara.Add(EquNo.Trim());
            liSqlPara.Add(EquName.Trim());
            liSqlPara.Add(LogInfo.Trim());
            liSqlPara.Add(LogDesc.Trim());
            return liSqlPara;
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
            設備設消碼錯誤 = 32,
            卡片一般權限調整=35,
            卡片權限調整=36,
            預設卡片權限=37,
	        重設卡片權限=38,
            卡號變更=39,
            指令傳送=41
        }
    }
}

