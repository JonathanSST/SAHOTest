using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MyDataObjectLib;

namespace TempCore
{
    public abstract class BasicCore
    {
        //static string connection = System.Configuration.ConfigurationManager.ConnectionStrings["webconn1"].ConnectionString;
        
        protected SimpleDataObject dbobject = null; //new SimpleDataObject("MsSql", connection);
        
        /// <summary>舊單號</summary>
        protected String StringNoOld = String.Empty;

        protected string InsertNo = string.Empty;

        protected string EditNo = string.Empty;

        protected DataTable DataResult = new DataTable();

        public string Excetion = string.Empty;   //例外資訊
        public bool isSuccess = false;
        public string commandstr = string.Empty;        //command information

        public BasicCore()
        {
            this.dbobject = new SimpleDataObject("", "");
        }

        public BasicCore(string db_type, string connectionstring)
        {
            this.dbobject = new SimpleDataObject(db_type, connectionstring);
        }

        protected abstract void SetInitColumns();

        public abstract DataTable GetBaseData();

        public abstract void SetSaveDataToDB(List<Dictionary<string,string>> ListCols);

        public abstract void SetSaveDetailDataToDB(List<Dictionary<string, string>> ListCols,string _MasterNo);


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
            StrNoShow = paramPreFix +  this.AddZero(StrDig);
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


        protected DataTable GetTableData(List<string> paramTableCol, string TableName,string condition)
        {
            StringBuilder builder = new StringBuilder();
            int colsCnt = 0;
            builder.AppendLine("SELECT ");            
            foreach (string colname in paramTableCol)
            {
                if (colsCnt > 0)
                {
                    builder.Append(",");
                }
                builder.AppendLine(colname);
                colsCnt++;
            }
            builder.AppendLine(" FROM ");
            builder.AppendLine(TableName);
            builder.AppendLine("WHERE 1=1 ");
            builder.AppendLine(condition);
            this.commandstr = builder.ToString();
            return GetQueryData(builder.ToString());
        }


        /// <summary>
        /// 取得資料表
        /// </summary>
        /// <param name="command">CommandString</param>
        /// <returns></returns>
        public DataTable GetQueryData(string command)
        {
            try
            {
                this.DataResult = this.dbobject.GetDataTableBySql(command);
                this.isSuccess = true;
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                this.DataResult = new DataTable();
                this.isSuccess = false;
            }
            return DataResult;
        }


        /// <summary>
        /// 取得資料表
        /// </summary>
        /// <param name="command">CommandString</param>
        /// <param name="parameter">Command Parameter</param>
        /// <returns></returns>
        public DataTable GetQueryData(string command, Dictionary<string, object> parameter)
        {
            try
            {
                this.dbobject.SetParameter(parameter);
                this.DataResult = this.dbobject.GetDataTableBySql(command);
                this.isSuccess = true;
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                this.DataResult = new DataTable();
                this.isSuccess = false;
            }
            return DataResult;
        }


        public DataTable GetQueryData(string command, List<object> liParas)
        {
            try
            {
                this.dbobject.SetParameter(liParas);
                this.DataResult = this.dbobject.GetDataTableBySql(command);
                this.isSuccess = true;
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                this.DataResult = new DataTable();
                this.isSuccess = false;
            }
            return DataResult;
        }

        /// <summary>針對OleDB 的處理方式 可直接丟參數值 不用設定參數</summary>
        /// <param name="paramSqlCmdStr">command string</param>
        /// <param name="liParas">參數集合</param>
        /// <returns></returns>
        public int SetTransactionDb(string paramSqlCmdStr,List<object> liParas)
        {
            int effectrow = 0;
            try
            {
                dbobject.BeginTransaction();
                this.dbobject.SetParameter(liParas);
                effectrow=this.dbobject.ExecuteNonQuery(paramSqlCmdStr);
                this.isSuccess = true;
                dbobject.Commit();
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                this.isSuccess = false;
                dbobject.Rollback();
            }
            return effectrow;
        }

        /// <summary>執行透過具名參數型態的資料交易</summary>
        /// <param name="paramSqlCommandString"></param>
        /// <param name="paramDbParameters"></param>
        /// <returns></returns>
        public int SetTransactionDb(string paramSqlCmdStr, Dictionary<string, object> paramDbParameter)
        {
            int effectrow = 0;
            try
            {
                dbobject.BeginTransaction();
                dbobject.SetParameter(paramDbParameter);
                effectrow=dbobject.ExecuteNonQuery(paramSqlCmdStr);
                dbobject.Commit();
                this.isSuccess = true;
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                this.isSuccess = false;
                dbobject.Rollback();
            }
            return effectrow;
        }


        /// <summary>執行透過具名參數型態的資料交易</summary>
        /// <param name="paramSqlCmdStr"></param>
        /// <param name="paramDbParameters"></param>
        /// <returns></returns>
        public int SetTransactionDb(string paramSqlCmdStr, List<Dictionary<string, object>> paramDbParameters)
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
                this.isSuccess = true;
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                this.isSuccess = false;
                dbobject.Rollback();
            }
            return effectrow;
        }

        public int SetTransactionDbData(List<string> paramSqlCmdStr)
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
                this.isSuccess = true;
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                this.isSuccess = false;
                dbobject.Rollback();
            }
            return effectrow;
        }
        

        public int SetTransactionDb(string paramSqlCmdStr)
        {
            int effectrow = 0;
            try
            {
                dbobject.BeginTransaction();                
                dbobject.ExecuteNonQuery(paramSqlCmdStr);                
                dbobject.Commit();
                this.isSuccess = true;
            }
            catch (Exception e)
            {
                this.Excetion = e.Message;
                dbobject.Rollback();
                this.isSuccess = false;
            }
            return effectrow;
        }


        /// <summary>
        /// 取得新增明細時所關聯的單號
        /// </summary>
        /// <returns></returns>
        public string GetInsertNo()
        {
            return this.InsertNo;
        }


        /// <summary>
        /// 取得修改的commandstring
        /// </summary>
        /// <param name="paramDicTableCol">資料欄位字典</param>
        /// <param name="paramKeyName">主key</param>
        /// <param name="paramTableName">資料表</param>
        /// <returns></returns>
        protected string GetUpdateTableCmd(Dictionary<string, string> paramDicTableCol, string paramKeyName, string paramTableName)
        {
            string cmd = string.Empty;
            cmd += " UPDATE " + paramTableName + " SET ";
            int colcount = 0;
            foreach (KeyValuePair<string, string> key in paramDicTableCol)
            {
                if (colcount > 0)
                {
                    cmd += ",";
                }
                cmd += key.Key + " = '" + key.Value + "' ";
                colcount++;
            }
            cmd += " WHERE " + paramKeyName + "='" + paramDicTableCol[paramKeyName] + "' ";
            return cmd;
        }


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
                cmd += key.Key + " = @" + key.Key ;
                colcount++;
            }
            cmd += " WHERE " + paramKeyName + "=@" +paramKeyName;
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
                cmd += "@"+kv.Key;
                colcount++;
            }
            cmd += ")";
            return cmd;
        }



        /// <summary>取得新增的commandstring</summary>
        /// <param name="paramDicTableCol">資料欄位字典</param>
        /// <param name="paramKeyName">主key</param>
        /// <param name="paramTableName">資料表</param>
        /// <returns></returns>
        protected string GetInsertTableCmd(Dictionary<string, string> paramDicTableCol, string paramKeyName, string paramTableName,string strPreNo)
        {
            string cmd = string.Empty;
            cmd += "INSERT INTO " + paramTableName + " ( ";
            int colcount = 0;
            paramDicTableCol[paramKeyName] = this.GetNo(strPreNo);            
            foreach (KeyValuePair<string, string> key in paramDicTableCol)
            {
                if (colcount > 0)
                {
                    cmd += ",";
                }
                cmd += key.Key;
                colcount++;
            }
            colcount = 0;
            cmd += " ) VALUES ( ";
            foreach (KeyValuePair<string, string> key in paramDicTableCol)
            {
                if (colcount > 0)
                {
                    cmd += ",";
                }
                cmd += "'" + key.Value + "' ";
                colcount++;
            }
            cmd += ")";
            return cmd;
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



    }//end class
}//end namespace
