using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MyDataObjectLib;



namespace TempCore
{
    public abstract class JanchenCore
    {
        #region 全域變數區

        static string connection = System.Configuration.ConfigurationManager.ConnectionStrings["webconn"].ConnectionString;

        protected SimpleDataObject dbobject = new SimpleDataObject("MsSql", connection);

        /// <summary>舊單號</summary>
        protected String StringNoOld = String.Empty;
        protected string InsertNo = string.Empty;
        protected string EditNo = string.Empty;
        protected DataTable DataResult = new DataTable();

        /// <summary>主檔資料表欄位Collection</summary>
        public List<string> MasterColumns = new List<string>();

        /// <summary>明細檔資料表欄位Collection</summary>
        public List<string> DetailColumns = new List<string>();

        /// <summary>主檔資料表</summary>
        public string MasterTable = "TB_GOODS";

        /// <summary>明細資料表</summary>
        public string DetailTable = "TB_ITEM_DETAIL";

        /// <summary>主檔PreFix</summary>
        public  string StrMasterPre = "GD";
        
        /// <summary>明細檔PreFix</summary>
        public string StrDetailPre = "ID";

        /// <summary>主檔No</summary>
        public string StrMasterCol = "GD_NO";

        /// <summary>明細檔no</summary>
        public string StrDetailCol = "ID_NO";

        /// <summary>主檔NO_SHOW名稱</summary>
        public string StrMasterNoShow = "GD_NO_SHOW";

        /// <summary>
        /// 明細檔join主檔欄位名稱
        /// </summary>
        public string StrDetailRefMasterNo = "ID_ITEM_NO";     


        public string Excetion = string.Empty;   //例外資訊

        #endregion

        #region 建構式

        public JanchenCore()
        {
        }

        #endregion

        #region 抽象類別方法

        protected abstract void SetInitColumns();

        #endregion


        #region 新增、修改與刪除資料


        /// <summary>可供覆寫的具名參數的資料交易設定</summary>
        /// <param name="_paramCols"></param>
        public virtual void SetDataToDB(List<Dictionary<string, object>> _paramCols)
        {
            string updatestr = string.Empty;
            string insertstr = string.Empty;
            List<Dictionary<string, object>> insertCols = _paramCols.Where(i => i[this.StrMasterCol].ToString().Equals("")).ToList();
            List<Dictionary<string, object>> updateCols = _paramCols.Where(i => !i[this.StrMasterCol].ToString().Equals("")).ToList();
            foreach (Dictionary<string, object> colDic in insertCols)
            {
                //colDic["GD_DATE"] = DateTime.Now;
                insertstr = this.GetInsertTableCmd(colDic, this.MasterTable, this.StrMasterCol, this.StrMasterPre);
                this.InsertNo = colDic[this.StrMasterCol].ToString();
            }
            foreach (Dictionary<string, object> colDic in updateCols)
            {
                updatestr = this.GetUpdateTableCmd(colDic, this.StrMasterCol, this.MasterTable);
                this.InsertNo = colDic[this.StrMasterCol].ToString();
            }
            if (insertstr.Length > 0)
            {
                this.SetTransactionDb(insertstr, insertCols);
            }
            if (updatestr.Length > 0)
            {
                this.SetTransactionDb(updatestr, updateCols);
            }
        }//end method


        /// <summary>透過具名參數的明細資料交易設定</summary>
        /// <param name="_paramCols"></param>
        public void SetDataToDetailDB(List<Dictionary<string, object>> _paramCols, string _InsertNo)
        {
            string updatestr = string.Empty;
            string insertstr = string.Empty;
            List<Dictionary<string, object>> insertCols = _paramCols.Where(i => i[this.StrDetailCol].ToString().Equals("")).ToList();
            List<Dictionary<string, object>> updateCols = _paramCols.Where(i => !i[this.StrDetailCol].ToString().Equals("")).ToList();
            foreach (Dictionary<string, object> colDic in insertCols)
            {
                //設定FQ_NO_SHOW                        
                colDic[this.StrDetailRefMasterNo] = _InsertNo;     //設定欄位的Detail資料
                insertstr = this.GetInsertTableCmd(colDic, this.DetailTable, this.StrDetailCol, this.StrDetailPre);
            }
            foreach (Dictionary<string, object> colDic in updateCols)
            {
                updatestr = this.GetUpdateTableCmd(colDic, this.StrDetailCol, this.DetailTable);
            }

            if (insertstr.Length > 0)
            {
                this.SetTransactionDb(insertstr, insertCols);
            }
            if (updatestr.Length > 0)
            {
                this.SetTransactionDb(updatestr, updateCols);
            }
        }//end method

        /// <summary>刪除品名明細資料</summary>
        /// <param name="paramListDetailNo"></param>
        public virtual void SetDelDetailData(List<string> paramListDetailNo)
        {
            List<string> listDelCmdStr = new List<string>();
            foreach (string s in paramListDetailNo)
            {
                listDelCmdStr.Add("DELETE " + this.MasterTable + " WHERE " + this.StrDetailCol + "='" + s + "' ");
            }
            if (listDelCmdStr.Count > 0)
            {
                this.SetTransactionDbData(listDelCmdStr);
            }
        }

        #endregion



        #region 資料交易設定


        /// <summary>執行透過具名參數型態的資料交易</summary>
        /// <param name="paramSqlCommandString"></param>
        /// <param name="paramDbParameters"></param>
        /// <returns></returns>
        protected int SetTransactionDb(string paramSqlCmdStr, List<Dictionary<string, object>> paramDbParameters)
        {
            int effectrow = 0;
            try
            {
                dbobject.BeginTransaction();
                foreach (Dictionary<string, object> paramcols in paramDbParameters)
                {
                    dbobject.SetParameter(paramcols);
                    dbobject.ExecuteNonQuery(paramSqlCmdStr);
                }
                dbobject.Commit();
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                dbobject.Rollback();
            }
            return effectrow;
        }

        protected int SetTransactionDbData(List<string> paramSqlCmdStr)
        {
            int effectrow = 0;
            try
            {
                dbobject.BeginTransaction();
                foreach (string strsql in paramSqlCmdStr)
                {
                    dbobject.ExecuteNonQuery(strsql);
                }
                dbobject.Commit();
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                dbobject.Rollback();
            }
            return effectrow;
        }

        public int SetTransactionDbData(string paramSqlCmdStr)
        {
            int effectrow = 0;
            try
            {
                dbobject.BeginTransaction();
                dbobject.ExecuteNonQuery(paramSqlCmdStr);
                dbobject.Commit();
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                dbobject.Rollback();
            }
            return effectrow;
        }

        #endregion


        #region 資料字串組合設定
        /// <summary>取得修改的commandstring</summary>
        /// <param name="paramDicTableCol">資料欄位參數</param>
        /// <param name="paramKeyName">主key</param>
        /// <param name="paramTableName">資料表</param>
        /// <returns></returns>
        protected string GetUpdateTableCmd(Dictionary<string, object> paramDicTableCol, string paramKeyName, string paramTableName)
        {
            string cmd = string.Empty;
            cmd += " UPDATE " + paramTableName + " SET ";
            int colcount = 0;
            foreach (KeyValuePair<string, object> key in paramDicTableCol)
            {
                if (colcount > 0)
                {
                    cmd += ",";
                }
                cmd += key.Key + " = @" + key.Key;
                colcount++;
            }
            cmd += " WHERE " + paramKeyName + "=@" + paramKeyName;
            return cmd;
        }



        /// <summary>get of sqlcommandstring of insert data</summary>
        /// <param name="_paramParamCols">sql parameterCols</param>
        /// <param name="paramTableName">TableName</param>
        /// <param name="paramKeyName">Key</param>
        /// <param name="strPreNo">Valuee</param>
        /// <returns></returns>
        protected string GetInsertTableCmd(Dictionary<string, object> _paramParamCols, string paramTableName, string paramKeyName, string strPreNo)
        {
            string cmd = string.Empty;
            cmd += "INSERT INTO " + paramTableName + " ( ";
            int colcount = 0;
            _paramParamCols[paramKeyName] = this.GetNo(strPreNo);
            foreach (KeyValuePair<string, object> kv in _paramParamCols)
            {
                if (colcount > 0)
                {
                    cmd += ",";
                }
                cmd += kv.Key;
                colcount++;
            }
            colcount = 0;
            cmd += " ) VALUES ( ";
            foreach (KeyValuePair<string, object> kv in _paramParamCols)
            {
                if (colcount > 0)
                {
                    cmd += ",";
                }
                cmd += "@" + kv.Key;
                colcount++;
            }
            cmd += ")";
            return cmd;
        }


        #endregion


        #region 資料查詢設定

        public DataTable GetQueryData(string command)
        {
            try
            {
                this.DataResult = this.dbobject.GetDataTableBySql(command);
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                this.DataResult = new DataTable();
            }
            return DataResult;
        }

        #endregion



        #region 單據資料編號設定

        /// <summary>取得編號</summary>
        /// <param name="paramFix">前置修飾</param>
        /// <returns>編號</returns>
        protected String GetNo(String paramFix)
        {
            String StrNoNew = String.Empty;
            StrNoNew = paramFix + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            while (StrNoNew == this.StringNoOld)
            {
                StrNoNew = paramFix + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            }
            this.StringNoOld = StrNoNew;
            return StrNoNew;
        }


        /// <summary>依單據日期取得NO_SHOW欄位</summary>
        /// <param name="paramPreFix"></param>
        /// <param name="paramColumnName"></param>
        /// <param name="paramTableName"></param>
        /// <param name="paramDate"></param>
        /// <returns></returns>
        public String GetNoShow(String paramPreFix, String paramColumnName, String paramTableName)
        {
            String start = (paramPreFix.Length + 1).ToString();
            String SqlCommandString = " SELECT ";
            String StrNoShow = String.Empty;
            String StrDig = String.Empty;
            SqlCommandString += "  ISNULL(MAX(RIGHT(" + paramColumnName + ",3)+1),'001') ";
            SqlCommandString += " FROM " + paramTableName + " ";
            SqlCommandString += " WHERE ";
            SqlCommandString += " SUBSTRING(" + paramColumnName + "," + start + ",0) = '' ";

            System.Data.DataTable dt = this.GetQueryData(SqlCommandString);
            //PublicUtility.GetSimpleQueryDataResult(SqlCommandString);
            if (dt.Rows.Count > 0)
            {
                StrDig = dt.Rows[0][0].ToString();
            }
            StrNoShow = paramPreFix + this.AddZero(StrDig);
            return StrNoShow;
        }


        /// <summary>依單據日期取得NO_SHOW欄位</summary>
        /// <param name="paramPreFix"></param>
        /// <param name="paramColumnName"></param>
        /// <param name="paramTableName"></param>
        /// <param name="paramDate"></param>
        /// <returns></returns>
        public String GetNoShow(String paramPreFix, String paramColumnName, String paramTableName, String paramDate)
        {
            String start = (paramPreFix.Length + 1).ToString();
            String SqlCommandString = " SELECT ";
            String StrNoShow = String.Empty;
            String StrDig = String.Empty;
            SqlCommandString += "  ISNULL(MAX(RIGHT(" + paramColumnName + ",3)+1),'001') ";
            SqlCommandString += " FROM " + paramTableName + " ";
            SqlCommandString += " WHERE ";
            SqlCommandString += " SUBSTRING(" + paramColumnName + "," + start + ",6) = '" + paramDate.Substring(2, 6) + "'";

            System.Data.DataTable dt = this.GetQueryData(SqlCommandString);
            //PublicUtility.GetSimpleQueryDataResult(SqlCommandString);
            if (dt.Rows.Count > 0)
            {
                StrDig = dt.Rows[0][0].ToString();
            }
            StrNoShow = paramPreFix + paramDate.Substring(2, 6) + this.AddZero(StrDig);
            return StrNoShow;
        }



        /// <summary>進行補0的處理</summary>
        /// <param name="paramDig">來源資料</param>
        /// <returns>處理完成</returns>
        private String AddZero(String paramDig)
        {
            while (paramDig.Length < 3)
            {
                paramDig = "0" + paramDig;
            }
            return paramDig;
        }


        /// <summary>
        /// 取得新增明細時所關聯的單號
        /// </summary>
        /// <returns></returns>
        public string GetInsertNo()
        {
            return this.InsertNo;
        }



        #endregion

    }//end class
}//end namespace
