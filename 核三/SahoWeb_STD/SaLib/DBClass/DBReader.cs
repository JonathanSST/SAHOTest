using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;

namespace Sa.DB
{

    public class DBReader 
    {
        public OleDbDataReader DataReader;
        
        //------------------------------------------------------------------------------------------------------------------------
        ~DBReader()
        {
            Free();
        }

        //------------------------------------------------------------------------------------------------------------------------
        public void Free()
        {
            if (DataReader != null)
            {
                if (!DataReader.IsClosed) DataReader.Close();
                DataReader.Dispose();
            }
        }
        
        //------------------------------------------------------------------------------------------------------------------------
        public Boolean HasRows
        {
            get
            {
                return DataReader.HasRows;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 從 DataReader 讀取一筆資料
        /// </summary>
        /// <returns>如果資料則為 true 否則為 false</returns>
        public Boolean Read()
        {
            return DataReader.Read();
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 從 DataReader 讀取指定欄位的值，並以 String 型態傳回
        /// </summary>
        /// <remarks>可避免因欄位值為 Null 而發生錯誤</remarks>
        /// <param name="sFieldName">欄位名稱</param>
        public string ToString(string sFieldName)
        {
            string sRet = "";
            Int32 nIndex = GetFieldNameIndex(sFieldName);
            if (nIndex != -1) sRet = ToString(nIndex);
            return sRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 從 DataReader 讀取指定欄位的值，並以 String 型態傳回
        /// </summary>
        /// <remarks>可避免因欄位值為 Null 而發生錯誤</remarks>
        /// <param name="nIndex">欄位序號</param>
        public string ToString(Int32 nIndex)
        {
            string sRet = "";
            string sTmp;
            if (!DataReader.IsDBNull(nIndex))
            {
                string[] aType = DataReader.GetFieldType(nIndex).ToString().Split(new char[] { '.' });
                sTmp = ":Boolean:Char:String:Decimal:Double:Byte:Int16:Int32:Int64:SByte:Single:UInt16:UInt32:UInt64:";
                if (sTmp.IndexOf(aType[1]) > 0)
                {
                    sRet = DataReader.GetString(nIndex);
                }
                if (aType[1] == "DateTime") sRet = DataReader.GetDateTime(nIndex).ToString("yyyy/MM/dd HH:mm:ss");
            }
            return sRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 從 DataReader 讀取指定欄位的值，並以 Int32 型態傳回
        /// </summary>
        /// <remarks>可避免因欄位值為 Null 而發生錯誤</remarks>
        /// <param name="sFieldName">欄位名稱</param>
        public Int32 ToInt32(string sFieldName)
        {
            Int32 nRet = 0;
            Int32 nIndex = GetFieldNameIndex(sFieldName);
            if (nIndex != -1) nRet = ToInt32(nIndex);
            return nRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 從 DataReader 讀取指定欄位的值，並以 Int32 型態傳回
        /// </summary>
        /// <remarks>可避免因欄位值為 Null 而發生錯誤</remarks>
        /// <param name="nIndex">欄位序號</param>
        public Int32 ToInt32(Int32 nIndex)
        {
            Int32 nRet = 0;
            string sTmp;
            if (!DataReader.IsDBNull(nIndex))
            {
                string[] aType = DataReader.GetFieldType(nIndex).ToString().Split(new char[] { '.' });
                sTmp = ":Boolean:Char:String:";
                if (sTmp.IndexOf(aType[1]) > 0)
                {
                    try
                    {
                        nRet = Convert.ToInt32(DataReader.GetString(nIndex));
                    }
                    catch
                    {
                        nRet = 0;
                    }
                }

                sTmp = ":Decimal:Double:Byte:Int16:Int32:Int64:SByte:Single:UInt16:UInt32:UInt64:";
                if (sTmp.IndexOf(aType[1]) > 0)
                {
                    nRet = Convert.ToInt32(DataReader.GetValue(nIndex));
                }
            }
            return nRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 從 DataReader 讀取指定欄位的值，並以 Boolean 型態傳回
        /// </summary>
        /// <remarks>可避免因欄位值為 Null 而發生錯誤</remarks>
        /// <param name="sFieldName">欄位名稱</param>
        public Boolean ToBoolean(string sFieldName)
        {
            Boolean bRet = false;
            Int32 nIndex = GetFieldNameIndex(sFieldName);
            if (nIndex != -1) bRet = ToBoolean(nIndex);
            return bRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 從 DataReader 讀取指定欄位的值，並以 Boolean 型態傳回
        /// </summary>
        /// <remarks>可避免因欄位值為 Null 而發生錯誤</remarks>
        /// <param name="nIndex">欄位序號</param>
        public Boolean ToBoolean(Int32 nIndex)
        {
            Boolean bRet = false;
            string sTmp;
            if (!DataReader.IsDBNull(nIndex))
            {
                string[] aType = DataReader.GetFieldType(nIndex).ToString().Split(new char[] { '.' });

                if (aType[1] == "Boolean") bRet = DataReader.GetBoolean(nIndex);

                sTmp = ":String:Char:";
                if (sTmp.IndexOf(aType[1]) > 0) bRet = Change.StringToBoolean(DataReader.GetString(nIndex));

                sTmp = ":Decimal:Double:Byte:Int16:Int32:Int64:SByte:Single:UInt16:UInt32:UInt64:";
                if (sTmp.IndexOf(aType[1]) > 0) bRet = Convert.ToInt32(DataReader.GetValue(nIndex)) == 1;           
            
            }
            return bRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 從 DataReader 讀取指定欄位的值，並以 DateTime 型態傳回
        /// </summary>
        /// <remarks>可避免因欄位值為 Null 而發生錯誤</remarks>
        /// <param name="sFieldName">欄位名稱</param>
        public DateTime ToDateTime(string sFieldName)
        {
            DateTime dtRet = DateTime.MinValue;
            Int32 nIndex = GetFieldNameIndex(sFieldName);
            if (nIndex != -1) dtRet = ToDateTime(nIndex);
            return dtRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 從 DataReader 讀取指定欄位的值，並以 DateTime 型態傳回
        /// </summary>
        /// <remarks>可避免因欄位值為 Null 而發生錯誤</remarks>
        /// <param name="nIndex">欄位序號</param>
        public DateTime ToDateTime(Int32 nIndex)
        {
            DateTime dtRet = DateTime.MinValue;
            if (!DataReader.IsDBNull(nIndex))
            {
                string sType = DataReader.GetFieldType(nIndex).ToString();
                switch (sType)
                {
                    case "System.String":
                        try
                        {
                            dtRet = Convert.ToDateTime(DataReader.GetString(nIndex));
                        }
                        catch
                        {
                            dtRet = DateTime.MinValue;
                        }
                        break;
                    case "System.DateTime":
                        dtRet = DataReader.GetDateTime(nIndex);
                        break;
                    default:
                        dtRet = DateTime.MinValue;
                        break;
                }
            }
            return dtRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        private Int32 GetFieldNameIndex(string sFieldName)
        {
            Int32 nIndex = -1;
            try
            {
                nIndex = DataReader.GetOrdinal(sFieldName);
            }
            catch
            {
                nIndex = -1;
            }
            return nIndex;
        }

    }
   
}