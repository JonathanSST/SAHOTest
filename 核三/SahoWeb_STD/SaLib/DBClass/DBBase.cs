using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace Sa.DB
{
    public class Base : IDisposable
    {
        private string __sConnString = "";
        private Boolean __IsConnected = false;
        private string __sLastErrorMessage = "";
        private string __sLine = "----------------------------------------------------------------------------------------------------";

        private OleDbConnection __Conn = null;
        private OleDbConnection __ConnT = null;
        private OleDbCommand __Cmd = null;
        private OleDbTransaction __Trans = null;

        #region IDisposable 成員

        void IDisposable.Dispose()
        {
            CloseConnect();
            __Conn.Dispose();
            throw new NotImplementedException();
        }

        #endregion

        //------------------------------------------------------------------------------------------------------------------------
        public string GetLastErrorMessage
        {
            get { return __sLastErrorMessage; }
        }

        //-------------------------------------------------------------------------------------------------------------
        public Boolean Connected
        {
            get { return __IsConnected; }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 開啟資料庫連線
        /// </summary>
        /// <param name="sConnString">資料庫連線字串</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean OpenConnect(string sConnString)
        {
            if (__Conn == null)
            {
                __sConnString = sConnString;
                __IsConnected = CreateConnect(ref __Conn);

                if (__IsConnected)
                {
                    if (__Cmd == null) __Cmd = new OleDbCommand();
                    __Cmd.Connection = __Conn;
                }
            }
            return __IsConnected;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 關閉資料庫連線
        /// </summary>
        public void CloseConnect()
        {
            if (__Cmd != null) __Cmd.Dispose();
            if (__Conn != null)
            {
                __Conn.ResetState();
                if (__Conn.State == ConnectionState.Open)
                {
                    __Conn.Close();
                    __Conn.Dispose();
                }
            }

            if (__ConnT != null)
            {
                if (__Trans != null) __Trans.Dispose();

                __ConnT.ResetState();
                if (__ConnT.State == ConnectionState.Open)
                {
                    __ConnT.Close();
                    __ConnT.Dispose();
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 開啟資料庫交易模式
        /// </summary>
        public Boolean BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.RepeatableRead);
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 開啟資料庫交易模式
        /// <param name="emIsolationLevel">指定要鎖定的交易行為</param>
        /// </summary>
        public Boolean BeginTransaction(IsolationLevel emIsolationLevel)
        {
            Boolean IsOK = false;
            if (__ConnT == null) CreateConnect(ref __ConnT);
            if (__ConnT == null) return false;

            __ConnT.ResetState();
            if (__ConnT.State == ConnectionState.Open)
            {
                if (__Trans == null)
                {
                    __Trans = __ConnT.BeginTransaction(emIsolationLevel);
                    __Cmd.Connection = __ConnT;
                    __Cmd.Transaction = __Trans;
                    IsOK = true;
                }
            }
            return IsOK;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 資料庫交易復原
        /// </summary>
        public Boolean Rollback()
        {
            Boolean IsOK = false;
            if (__ConnT != null)
            {
                __ConnT.ResetState();
                if (__ConnT.State == ConnectionState.Open)
                {
                    if (__Trans != null)
                    {
                        __Trans.Rollback();
                        IsOK = true;
                    }
                }
                if (__Trans != null)
                {
                    __Cmd.Transaction = null;
                    __Cmd.Connection = __Conn;
                    __Trans.Dispose();
                    __Trans = null;
                }
            }
            return IsOK;
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 資料庫交易認可
        /// </summary>
        public Boolean Commit()
        {
            Boolean IsOK = false;

            if (__ConnT != null)
            {
                __ConnT.ResetState();
                if (__ConnT.State == ConnectionState.Open)
                {
                    if (__Trans != null)
                    {
                        __Trans.Commit();
                        IsOK = true;
                    }
                }
                if (__Trans != null)
                {
                    __Cmd.Transaction = null;
                    __Cmd.Connection = __Conn;
                    __Trans.Dispose();
                    __Trans = null;
                }
            }
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 建立 Connect 物件，依指定的連線字串建立資料庫連線，並將資料庫連結結果回傳
        /// </summary>
        private Boolean CreateConnect(ref OleDbConnection oConn)
        {
            oConn = new OleDbConnection(__sConnString);
            if (!DoConnection(ref oConn))
            {
                oConn.Dispose();
                oConn = null;
            }
            return (oConn != null);
        }

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 檢查資料庫是否還在連線中，如連線中斷則重新再連線。
        /// </summary>
        public Boolean CheckDBConnect()
        {
            __IsConnected = false;
            if (__Conn != null)
            {
                __Conn.ResetState();
                if (__Conn.State != ConnectionState.Open)
                {
                    __Conn.ConnectionString = __sConnString;
                    __IsConnected = DoConnection(ref __Conn);
                }
                else
                {
                    DateTime dtTime;
                    __IsConnected = GetServerTime(out dtTime);
                    if (!__IsConnected)
                    {
                        __Conn.ConnectionString = __sConnString;
                        __IsConnected = DoConnection(ref __Conn);
                    }
                }
            }

            if (__IsConnected && __ConnT != null)
            {
                __ConnT.ResetState();
                if (__ConnT.State != ConnectionState.Open)
                {
                    if (__Trans != null)
                    {
                        __Trans.Dispose();
                        __Trans = null;
                    }
                    __ConnT.ConnectionString = __sConnString;
                    __IsConnected = DoConnection(ref __ConnT);
                }
            }
            return __IsConnected;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 進行 Connect 資料庫連線，並將結果回傳
        /// </summary>
        /// <param name="oConn">要連線的資料庫物件</param>
        private Boolean DoConnection(ref OleDbConnection oConn)
        {
            __sLastErrorMessage = "";
            Boolean IsOK = false;
            try
            {
                oConn.Open();
                IsOK = true;
            }
            catch (OleDbException e)
            {
                __sLastErrorMessage = "資料庫連結發生錯誤，無法連上資料庫。";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog(__sLastErrorMessage);
                SaveOleDbExceptionLog(e);
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "資料庫連結發生錯誤，無法連上資料庫。";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog(__sLastErrorMessage);
                Fun.SaveExceptionLog(e);
            }
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 向資料庫查詢伺服器目前時間
        /// </summary>
        /// <param name="dtRetValue">回傳結果值(DateTime)</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetServerTime(out DateTime dtRetValue)
        {
            dtRetValue = DateTime.MinValue;
            __Conn.ResetState();
            Boolean IsOK = (__Conn.State == ConnectionState.Open);
            if (IsOK)
            {
                string sProvider = __Conn.Provider.ToUpper();
                string sSql = "";
                if (sProvider.IndexOf("JET.OLEDB") >= 0)
                    sSql = "SELECT NOW() AS FTime";
                if (sProvider.IndexOf("SQLOLEDB") >= 0 || sProvider.IndexOf("SQLNCLI") >= 0)
                    sSql = "SELECT GETDATE() AS FTime";
                if (sProvider.IndexOf("ORA") >= 0)
                    sSql = "SELECT SYSDATE AS FTime FROM DUAL";                
                if (sSql != "")
                    dtRetValue = GetDateTimeScalar(sSql);
            }
            IsOK = (dtRetValue != DateTime.MinValue);
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 用來執行 Insert、Update 及 Delete 命令的方法
        /// </summary>
        /// <param name="sCommandText">命令字串</param>
        /// <returns>傳回受影響的資料筆數，如果傳回 -1 則表示指令執行失敗</returns>
        public Int32 SqlCommandExecute(string sCommandText)
        {
            return SqlCommonExecute(CommandType.Text, sCommandText, new List<string>());
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 用來執行 Insert、Update 及 Delete 命令的方法
        /// </summary>
        /// <param name="sCommandText">命令字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <returns>傳回受影響的資料筆數，如果傳回 -1 則表示指令執行失敗</returns>
        public Int32 SqlCommandExecute(string sCommandText, List<string> liParameters)
        {
            return SqlCommonExecute(CommandType.Text, sCommandText, liParameters);
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 用來執行資料庫預存程序的方法
        /// </summary>
        /// <param name="sCommandText">要執行的預存程序名稱</param>
        /// <returns>傳回受影響的資料筆數，如果傳回 -1 則表示指令執行失敗</returns>
        public Int32 SqlProcedureExecute(string sCommandText)
        {
            return SqlCommonExecute(CommandType.StoredProcedure, sCommandText, new List<string>());
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 用來執行資料庫預存程序的方法
        /// </summary>
        /// <param name="sCommandText">要執行的預存程序名稱</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <returns>傳回受影響的資料筆數，如果傳回 -1 則表示指令執行失敗</returns>
        public Int32 SqlProcedureExecute(string sCommandText, List<string> liParameters)
        {
            return SqlCommonExecute(CommandType.StoredProcedure, sCommandText, liParameters);
        }

        //------------------------------------------------------------------------------------------------------------------------
        private Int32 SqlCommonExecute(CommandType enCmdType, string sCommandText, List<string> liParameters)
        {
            __sLastErrorMessage = "";
            Int32 nRet = -1;

            __Cmd.CommandText = sCommandText;
            __Cmd.CommandType = enCmdType;
            CommandSetParameters(__Cmd, liParameters);
            try
            {
                nRet = __Cmd.ExecuteNonQuery();
            }
            catch (OleDbException e)
            {
                __sLastErrorMessage = "執行 SQL 指令時發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog(__sLastErrorMessage);
                SaveExceptionLog(sCommandText, liParameters, e);
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "一般程式類型錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog(__sLastErrorMessage);
                SaveExceptionLog(sCommandText, liParameters, e);
            }
            return nRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 產生 Command 物件，但不執行
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <returns>回傳 Command 物件</returns>
        private OleDbCommand GetCommand(string sCommandText)
        {
            return GetCommand(sCommandText, new List<string>());
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 產生 Command 物件，但不執行
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <returns>回傳 Command 物件</returns>
        private OleDbCommand GetCommand(string sCommandText, List<string> liParameters)
        {
            OleDbCommand oCmd = new OleDbCommand(sCommandText, __Conn);
            oCmd.CommandType = CommandType.Text;
            CommandSetParameters(oCmd, liParameters);
            return oCmd;
        }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 產生 Command 物件，但不執行
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <returns>回傳 Command 物件</returns>
        private OleDbCommand GetCommand(string sCommandText, List<string> liParameters, bool TransFlag)
        {
            __Cmd.Connection = __ConnT;
            __Cmd.CommandText = sCommandText;
            __Cmd.CommandType = CommandType.Text;
            CommandSetParameters(__Cmd, liParameters);            
            OleDbCommand oCmd = __Cmd;
            return oCmd;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 產生執行預存程序的 Command 物件，但不執行
        /// </summary>
        /// <param name="sCommandText">要執行的預存程序名稱</param>
        /// <returns>回傳 Command 物件</returns>
        private OleDbCommand GetStoredProcCommand(string sCommandText)
        {
            return GetStoredProcCommand(sCommandText, new List<string>());
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 產生執行預存程序的 Command 物件，但不執行
        /// </summary>
        /// <param name="sCommandText">要執行的預存程序名稱</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <returns>回傳 Command 物件</returns>
        private OleDbCommand GetStoredProcCommand(string sCommandText, List<string> liParameters)
        {
            OleDbCommand oCmd = new OleDbCommand(sCommandText, __Conn);
            oCmd.CommandType = CommandType.StoredProcedure;
            CommandSetParameters(oCmd, liParameters);
            return oCmd;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(String)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <returns>回傳結果值(String)</returns>
        public string GetStrScalar(string sCommandText)
        {
            return GetStrScalar(sCommandText, new List<string>());
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(String)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <returns>回傳結果值(String)</returns>
        public string GetStrScalar(string sCommandText, List<string> liParameters)
        {
            string sValue;
            Boolean IsNoData;
            GetStrScalar(sCommandText, liParameters, out sValue, out IsNoData);
            if (IsNoData) sValue = null;
            return sValue;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(String)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="sValue">回傳結果值(String)</param>
        /// <param name="IsNoData">如果查無資料則回傳 True</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetStrScalar(string sCommandText, out string sValue, out Boolean IsNoData)
        {
            return GetStrScalar(sCommandText, new List<string>(), out sValue, out IsNoData);
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(String)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <param name="sValue">回傳結果值(String)</param>
        /// <param name="IsNoData">如果查無資料則回傳 True</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetStrScalar(string sCommandText, List<string> liParameters, out string sValue, out Boolean IsNoData)
        {
            __sLastErrorMessage = "";

            Boolean IsOK = false;
            sValue = "";
            IsNoData = true;

            OleDbCommand oCmd = GetCommand(sCommandText, liParameters);
            try
            {
                Object oObj = oCmd.ExecuteScalar();
                if (oObj != null)
                {
                    IsNoData = false;
                    string[] aType = oObj.GetType().ToString().Split(new char[] { '.' });
                    string sTmp = ":Boolean:Char:String:Decimal:Double:Byte:Int16:Int32:Int64:SByte:Single:UInt16:UInt32:UInt64:";
                    if (sTmp.IndexOf(aType[1]) > 0) sValue = oObj.ToString();
                    if (aType[1] == "DateTime")
                    {
                        DateTime dtTmp = (DateTime)oObj;
                        sValue = dtTmp.ToString("yyyy/MM/dd hh:mm:ss");
                    }
                }
                IsOK = true;
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "從資料庫讀取單一欄位字串資料時，發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog("執行 GetStrScalar 指令時發生錯誤");
                SaveExceptionLog(sCommandText, liParameters, e);
            }
            oCmd.Dispose();
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(整數型態)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <returns>回傳結果值(Int32)</returns>
        public Int32 GetIntScalar(string sCommandText)
        {
            return GetIntScalar(sCommandText, new List<string>());
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(Int32)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <returns>回傳結果值(Int32)</returns>
        public Int32 GetIntScalar(string sCommandText, List<string> liParameters)
        {
            Int32 iValue;
            Boolean IsNoData;
            GetIntScalar(sCommandText, liParameters, out iValue, out IsNoData);
            if (IsNoData) iValue = -9999;
            return iValue;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(整數型態)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="iValue">回傳結果值(Int32)</param>
        /// <param name="IsNoData">如果查無資料則回傳 True</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetIntScalar(string sCommandText, out Int32 iValue, out Boolean IsNoData)
        {
            return GetIntScalar(sCommandText, new List<string>(), out iValue, out IsNoData);
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(整數型態)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <param name="iValue">回傳結果值(Int32)</param>
        /// <param name="IsNoData">如果查無資料則回傳 True</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetIntScalar(string sCommandText, List<string> liParameters, out Int32 iValue, out Boolean IsNoData)
        {
            __sLastErrorMessage = "";
            Boolean IsOK = false;
            iValue = 0;
            IsNoData = true;

            OleDbCommand oCmd = GetCommand(sCommandText, liParameters);
            try
            {
                Object oObj = oCmd.ExecuteScalar();
                if (oObj != null)
                {
                    IsNoData = false;
                    string[] aType = oObj.GetType().ToString().Split(new char[] { '.' });
                    string sTmp = ":String:Char:";
                    if (sTmp.IndexOf(aType[1]) > 0)
                    {
                        try
                        {
                            iValue = Convert.ToInt32(oObj.ToString());
                        }
                        catch
                        {
                            iValue = 0;
                        }
                    }

                    sTmp = ":Decimal:Double:Byte:Int16:Int32:Int64:SByte:Single:UInt16:UInt32:UInt64:";
                    if (sTmp.IndexOf(aType[1]) > 0)
                    {
                        iValue = Convert.ToInt32(oObj);
                    }
                }
                IsOK = true;
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "從資料庫讀取單一欄位整數資料時，發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog("執行 GetIntScalar 指令時發生錯誤");
                SaveExceptionLog(sCommandText, liParameters, e);
            }
            oCmd.Dispose();
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(Boolean)
        /// </summary>
        /// <param name="sCommandText">SQL查詢語法字串</param>
        /// <returns>回傳結果值(Boolean)</returns>
        public Boolean GetBooleanScalar(string sCommandText)
        {
            return GetBooleanScalar(sCommandText, new List<string>());
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(Boolean)
        /// </summary>
        /// <param name="sCommandText">SQL查詢語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <returns>回傳結果值(Boolean)</returns>
        public Boolean GetBooleanScalar(string sCommandText, List<string> liParameters)
        {
            Boolean blValue;
            Boolean IsNoData;
            GetBooleanScalar(sCommandText, liParameters, out blValue, out IsNoData);
            if (IsNoData) blValue = false;
            return blValue;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(Boolean)
        /// </summary>
        /// <param name="sCommandText">SQL查詢語法字串</param>
        /// <param name="blValue">回傳結果值(邏輯型態)</param>
        /// <param name="IsNoData">如果查無資料則回傳 True</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetBooleanScalar(string sCommandText, out Boolean blValue, out Boolean IsNoData)
        {
            return GetBooleanScalar(sCommandText, new List<string>(), out blValue, out IsNoData);
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(Boolean)
        /// </summary>
        /// <param name="sCommandText">SQL查詢語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <param name="blValue">回傳結果值(Boolean)</param>
        /// <param name="IsNoData">如果查無資料則回傳 True</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetBooleanScalar(string sCommandText, List<string> liParameters, out Boolean blValue, out Boolean IsNoData)
        {
            __sLastErrorMessage = "";
            Boolean IsOK = false;
            IsNoData = true;
            blValue = false;
            string sTmp;

            OleDbCommand oCmd = GetCommand(sCommandText, liParameters);
            try
            {
                Object oObj = oCmd.ExecuteScalar();
                if (oObj != null)
                {
                    IsNoData = false;
                    string[] aType = oObj.GetType().ToString().Split(new char[] { '.' });
                    if (aType[1] == "Boolean") blValue = (Boolean)oObj;

                    sTmp = ":Char:String:";
                    if (sTmp.IndexOf(aType[1]) > 0) blValue = Change.StringToBoolean(oObj.ToString());

                    sTmp = ":Decimal:Double:Byte:Int16:Int32:Int64:SByte:Single:UInt16:UInt32:UInt64:";
                    if (sTmp.IndexOf(aType[1]) > 0) blValue = (Convert.ToInt32(oObj) == 1);
                }
                IsOK = true;
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "從資料庫讀取單一欄位邏輯資料時，發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog("執行 GetBooleanScalar 指令時發生錯誤");
                SaveExceptionLog(sCommandText, liParameters, e);
            }
            oCmd.Dispose();
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(DateTime)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <returns>回傳結果值(DateTime)</returns>
        public DateTime GetDateTimeScalar(string sCommandText)
        {
            return GetDateTimeScalar(sCommandText, new List<string>());
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(DateTime)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <returns>回傳結果值(DateTime)</returns>
        public DateTime GetDateTimeScalar(string sCommandText, List<string> liParameters)
        {
            DateTime dtValue = DateTime.MinValue;
            Boolean IsNoData;
            GetDateTimeScalar(sCommandText, liParameters, out dtValue, out IsNoData);
            if (IsNoData) dtValue = DateTime.MinValue;
            return dtValue;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(DateTime)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="dtValue">回傳結果值(DateTime)</param>
        /// <param name="IsNoData">如果查無資料則回傳 True</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetDateTimeScalar(string sCommandText, out DateTime dtValue, out Boolean IsNoData)
        {
            return GetDateTimeScalar(sCommandText, new List<string>(), out dtValue, out IsNoData);
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回單一結果值(DateTime)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <param name="dtValue">回傳結果值(DateTime)</param>
        /// <param name="IsNoData">如果查無資料則回傳 True</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetDateTimeScalar(string sCommandText, List<string> liParameters, out DateTime dtValue, out Boolean IsNoData)
        {
            __sLastErrorMessage = "";
            Boolean IsOK = false;
            dtValue = DateTime.MinValue;
            IsNoData = true;

            OleDbCommand oCmd = GetCommand(sCommandText, liParameters);
            try
            {
                Object oObj = oCmd.ExecuteScalar();
                if (oObj != null)
                {
                    IsNoData = false;
                    string sType = oObj.GetType().ToString();
                    switch (sType)
                    {
                        case "System.String":
                            try
                            {
                                dtValue = Convert.ToDateTime(oObj.ToString());
                            }
                            catch
                            {
                                dtValue = DateTime.MinValue;
                            }
                            break;
                        case "System.DateTime":
                            dtValue = (DateTime)oObj;
                            break;
                    }
                }
                IsOK = true;
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "從資料庫讀取單一欄位日期時間資料時，發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog("執行 GetDateTimeScalar 指令時發生錯誤");
                SaveExceptionLog(sCommandText, liParameters, e);
            }
            oCmd.Dispose();
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回其資料集(QReader)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="oReader">回傳的資料集(QReader)</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetDataReader(string sCommandText, out DBReader oReader)
        {
            return GetDataReader(sCommandText, new List<string>(), out oReader);
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回其資料集(QReader)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <param name="oReader">回傳的資料集(QReader)</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetDataReader(string sCommandText, List<string> liParameters, out DBReader oReader)
        {
            __sLastErrorMessage = "";
            Boolean IsOK = false;

            oReader = new DBReader();
            OleDbCommand oCmd = GetCommand(sCommandText, liParameters);
            try
            {
                oReader.DataReader = oCmd.ExecuteReader();
                IsOK = true;
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "從資料庫讀取資料時，發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog("執行 GetDataReader 指令時發生錯誤");
                SaveExceptionLog(sCommandText, liParameters, e);
            }
            oCmd.Dispose();
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回其資料表(DataTable)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="oTable">回傳的資料表(DataTable)</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetDataTable(string sTableName, string sCommandText, out DataTable oTable)
        {
            return GetDataTable(sTableName, sCommandText, new List<string>(), out oTable);
        }

        public Boolean GetDataTable(string sTableName, string sCommandText, List<object> liParameters, out DataTable oTable)
        {
            __sLastErrorMessage = "";
            Boolean IsOK = false;
            OleDbCommand oCmd = new OleDbCommand(sCommandText, this.__Conn);

            oCmd.CommandTimeout = 120;

            oCmd.CommandType = CommandType.Text;
            foreach (var o in liParameters)
            {
                var p = oCmd.CreateParameter();
                p.Value = o;
                oCmd.Parameters.Add(p);
            }
            OleDbDataAdapter oDap = new OleDbDataAdapter(oCmd);
            oTable = new DataTable(sTableName);
            try
            {
                oDap.Fill(oTable);
                IsOK = true;
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "從資料庫讀取資料時，發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog("執行 GetDataTable 指令時發生錯誤");
                //SaveExceptionLog(sCommandText, liParameters, e);
                oTable.Dispose();
            }
            oDap.Dispose();
            oCmd.Dispose();
            return IsOK;
        }


        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回其資料表(DataTable)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <param name="oTable">回傳的資料表(DataTable)</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetDataTable(string sTableName, string sCommandText, List<string> liParameters, out DataTable oTable)
        {
            __sLastErrorMessage = "";
            Boolean IsOK = false;
            OleDbCommand oCmd = GetCommand(sCommandText, liParameters);
      
            oCmd.CommandTimeout = 120;
                       
            OleDbDataAdapter oDap = new OleDbDataAdapter(oCmd);
            oTable = new DataTable(sTableName);
            try
            {
                oDap.Fill(oTable);
                IsOK = true;
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "從資料庫讀取資料時，發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog("執行 GetDataTable 指令時發生錯誤");
                SaveExceptionLog(sCommandText, liParameters, e);
                oTable.Dispose();
            }
            oDap.Dispose();
            oCmd.Dispose();
            return IsOK;
        }

        public Boolean GetDataTable(string sTableName, string sCommandText, List<string> liParameters, string strTimeOut, out DataTable oTable)
        {
            __sLastErrorMessage = "";
            Boolean IsOK = false;
            OleDbCommand oCmd = GetCommand(sCommandText, liParameters);

            if (strTimeOut == "")
            {
                oCmd.CommandTimeout = 360;
            }
            else
            {
                oCmd.CommandTimeout = 0;
            }

            OleDbDataAdapter oDap = new OleDbDataAdapter(oCmd);
            oTable = new DataTable(sTableName);
            try
            {
                oDap.Fill(oTable);
                IsOK = true;
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "從資料庫讀取資料時，發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog("執行 GetDataTable 指令時發生錯誤");
                SaveExceptionLog(sCommandText, liParameters, e);
                oTable.Dispose();
            }
            oDap.Dispose();
            oCmd.Dispose();
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回其資料表(DataTable)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="oTable">回傳的資料表(DataTable)</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetDataTable(string sTableName, string sCommandText, bool TransFlag, out DataTable oTable)
        {
            return GetDataTable(sTableName, sCommandText, new List<string>(), TransFlag, out oTable);
        }


        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 透過SQL語法向資料庫查詢，並取回其資料表(DataTable)
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        /// <param name="oTable">回傳的資料表(DataTable)</param>
        /// <returns>回傳執行結果(true = 成功; false = 失敗)</returns>
        public Boolean GetDataTable(string sTableName, string sCommandText, List<string> liParameters, bool TransFlag, out DataTable oTable)
        {
            __sLastErrorMessage = "";
            Boolean IsOK = false;
            OleDbCommand oCmd = GetCommand(sCommandText, liParameters, TransFlag);

            oCmd.CommandTimeout = 120;

            OleDbDataAdapter oDap = new OleDbDataAdapter(oCmd);
            oTable = new DataTable(sTableName);
            try
            {
                oDap.Fill(oTable);
                IsOK = true;
            }
            catch (Exception e)
            {
                __sLastErrorMessage = "從資料庫讀取資料時，發生錯誤";
                Fun.EventLog(__sLine, SaveTimeMarkMode.NoSpace);
                Fun.EventLog("執行 GetDataTable 指令時發生錯誤");
                SaveExceptionLog(sCommandText, liParameters, e);
                oTable.Dispose();
            }
            oDap.Dispose();
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 將格式化的參數列字串，分別置入 OleDbCommand 中
        /// </summary>
        /// <param name="oCmd">OleDbCommand</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        public void CommandSetParameters(OleDbCommand oCmd, List<string> liParameters)
        {
            oCmd.Parameters.Clear();
            if (liParameters.Count > 0)
            {
                Int32 i;
                string sType, sValue;
                DateTime dtValue;

                for (i = 0; i <liParameters.Count; i++)
                {
                    sValue = liParameters[i];
                    sType = Sa.Fun.GetStrL(ref sValue, ":", true);

                    if (sType != "")
                    {
                        if (sType == "D" || sType == "DATETIME")
                        {
                            try
                            {
                                dtValue = Convert.ToDateTime(sValue);
                            }
                            catch
                            {
                                dtValue = DateTime.MinValue;
                            }
                            oCmd.Parameters.AddWithValue("", dtValue);
                        }

                        if (sType.Substring(0, 1) == "I")
                        {
                            if (!Sa.Check.IsInt(sValue)) sValue = "0";
                            if (sType == "I" || sType == "INT32")
                                oCmd.Parameters.Add(new OleDbParameter() {Value=Convert.ToInt32(sValue),OleDbType=OleDbType.Integer,ParameterName=""}); //oCmd.Parameters.AddWithValue("", Convert.ToInt32(sValue));
                            if (sType == "INT16")
                                oCmd.Parameters.Add(new OleDbParameter() { Value = Convert.ToInt16(sValue), OleDbType = OleDbType.SmallInt, ParameterName = "" });//oCmd.Parameters.AddWithValue("", Convert.ToInt16(sValue));
                            if (sType == "INT64")
                                oCmd.Parameters.Add(new OleDbParameter() { Value = Convert.ToInt64(sValue), OleDbType = OleDbType.BigInt, ParameterName = "" });//oCmd.Parameters.AddWithValue("", Convert.ToInt64(sValue));
                        }

                        if (sType == "S" || sType == "String") oCmd.Parameters.AddWithValue("", sValue);
                        if (sType == "A" || sType == "String") oCmd.Parameters.Add(new OleDbParameter() { Value = sValue, OleDbType = OleDbType.VarChar, ParameterName = "" });
                    }
                    else oCmd.Parameters.AddWithValue("", null);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 將操作資料庫時所發生的錯誤訊息及指令參數記錄下來
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        private void SaveOleDbExceptionLog(string sCommandText, List<string> liParameters, OleDbException e)
        {
            string sPara = "";
            if (liParameters.Count > 0)
            {
                for (int i = 0; i < liParameters.Count; i++) sPara = sPara + "|" + liParameters[i];
            }
            Fun.EventLog("CommandText : " + sCommandText, SaveTimeMarkMode.UseSpaceMark);
            Fun.EventLog(" Parameters : " + sPara, SaveTimeMarkMode.UseSpaceMark);
            Fun.EventLog("", SaveTimeMarkMode.NoSpace);
            SaveOleDbExceptionLog(e);
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 將操作資料庫時所發生的所有錯誤訊息記錄下來
        /// </summary>
        private void SaveOleDbExceptionLog(OleDbException e)
        {
            string sErrorLog = "";
            for (int i = 0; i < e.Errors.Count; i++)
            {
                sErrorLog =
                    "        Index : " + Change.Ntoc(i, 3) + "\r\n" +
                    "      Message : " + e.Errors[i].Message + "\r\n" +
                    "  NativeError : " + e.Errors[i].NativeError + "\r\n" +
                    "       Source : " + e.Errors[i].Source + "\r\n" +
                    "     SQLState : " + e.Errors[i].SQLState;
                Fun.EventLog(sErrorLog, SaveTimeMarkMode.NoSpace);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 發生錯誤時候訊息將錯誤訊息及指令參數記錄下來
        /// </summary>
        /// <param name="sCommandText">SQL語法字串</param>
        /// <param name="liParameters">格式化的參數字串清單</param>
        private void SaveExceptionLog(string sCommandText, List<string> liParameters, Exception e)
        {
            string sPara = "";
            if (liParameters.Count > 0)
            {
                for (int i = 0; i < liParameters.Count; i++)
                    sPara = sPara + "|" + liParameters[i];
            }
            Fun.EventLog("CommandText : " + sCommandText, SaveTimeMarkMode.UseSpaceMark);
            Fun.EventLog(" Parameters : " + sPara, SaveTimeMarkMode.UseSpaceMark);
            Fun.EventLog("", SaveTimeMarkMode.NoSpace);
            Fun.SaveExceptionLog(e);
        }

        /// <summary>
        /// 回應查詢語法資料集合總筆數
        /// </summary>
        /// <param name="strSQLCom">查詢語法</param>
        /// <param name="liParameters">條件參數</param>
        /// <returns></returns>
        public int DataCount(string strSQLCom, List<string> liParameters)
        {
            string strQuery = " SELECT COUNT(*) AS DataCount FROM (" + strSQLCom + ") AS FilterData";
            DataTable dtDataCount = new DataTable();
            GetDataTable("DataCount", strQuery, liParameters, out dtDataCount);

            if (dtDataCount.Rows.Count == 0)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(dtDataCount.Rows[0].ItemArray[0].ToString());
            }
        }

        public int DataCount(string strSQLCom, List<object> liParameters)
        {
            string strQuery = " SELECT COUNT(*) AS DataCount FROM (" + strSQLCom + ") AS FilterData";
            DataTable dtDataCount = new DataTable();
            GetDataTable("DataCount", strQuery, liParameters, out dtDataCount);

            if (dtDataCount.Rows.Count == 0)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(dtDataCount.Rows[0].ItemArray[0].ToString());
            }
        }


        /// <summary>
        /// 回應查詢語法資料集合分頁
        /// </summary>
        /// <param name="strSQLCom">查詢語法</param>
        /// <param name="liParameters">條件參數</param>
        /// <param name="strPK">主鍵值</param>
        /// <param name="intPageIndx">分頁索引</param>
        /// <param name="intPageSize">分頁SIZE</param>
        /// <param name="intDataCount">資料總筆數</param>
        /// <returns></returns>
        public DataTable PageData(string strSQLCom, List<string> liParameters, string strPK, int intPageIndx, int intPageSize)
        {
            DataTable dtPageData = new DataTable();
            string strQuery = "";

            if (intPageIndx == 1)
            {
                strQuery = " SELECT TOP " + intPageSize.ToString() + " * FROM (" + strSQLCom +
                    " ) AS Main WHERE Main." + strPK + " >= (SELECT MIN(" + strPK +
                    ") FROM (SELECT TOP " + ((intPageIndx) * intPageSize).ToString() +
                    " " + strPK + " FROM (" + strSQLCom + " ) AS TopData) AS FilterData) ";
            }
            else
            {
                strQuery = " SELECT TOP " + intPageSize.ToString() + " * FROM (" + strSQLCom +
                    " ) AS Main WHERE Main." + strPK + " > (SELECT MAX(" + strPK +
                    ") FROM (SELECT TOP " + ((intPageIndx - 1) * intPageSize).ToString() +
                    " " + strPK + " FROM (" + strSQLCom + " ) AS TopData) AS FilterData) ";
            }

            for (int i = 0, j = liParameters.Count; i < j; i++)
            {
                liParameters.Add(liParameters[i].ToString());
            }

            GetDataTable("PageData", strQuery, liParameters, out dtPageData);

            return dtPageData;
        }

        /// <summary>
        /// 回應查詢語法資料集合分頁(Between)
        /// </summary>
        /// <param name="strSQLCom">查詢語法</param>
        /// <param name="liParameters">條件參數</param>
        /// <param name="intPageIndx">分頁索引</param>
        /// <param name="intPageSize">分頁筆數</param>
        /// <returns></returns>
        public DataTable PageData(string strSQLCom, List<string> liParameters, int intPageIndx, int intPageSize)
        {
            DataTable dtPageData = new DataTable();
            string strQuery = "SELECT * FROM (" + strSQLCom + ") AS Q WHERE Q.NewIDNum BETWEEN " +
                (int)(intPageSize * intPageIndx + 1) + " AND " + (int)(intPageSize * (intPageIndx + 1));

            GetDataTable("PageData", strQuery, liParameters, out dtPageData);
            int row_count = dtPageData.Rows.Count;
            return dtPageData;
        }
    }
}
