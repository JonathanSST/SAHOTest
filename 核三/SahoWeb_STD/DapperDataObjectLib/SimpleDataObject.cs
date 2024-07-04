using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace DapperDataObjectLib
{
    public class SimpleDataObject:IDataObject,IDisposable
    {
        IDbConnection connection;
        IDbCommand command;
        IDbDataAdapter dataadapter;

        /// <summary>Controls whether this SimpleDataObject is transactional.</summary>       
        private IDbTransaction dbTransaction;

        private bool isDisposed = false;

        /// <summary>
        /// 設定資料庫基本物件
        /// </summary>
        public SimpleDataObject()
        {
            IConnectionBuilder builder = ConnectionBuilderFactory.CreateConnectionBuilder();
            this.SetDataObject(builder);
        }


        /// <summary>
        /// 設定資料庫基本物件
        /// </summary>
        /// <param name="paramXmlPath"></param>
        public SimpleDataObject(String paramXmlPath)
        {
            IConnectionBuilder ConnectionBuilder = ConnectionBuilderFactory.CreateConnectionBuilder(paramXmlPath);
            this.SetDataObject(ConnectionBuilder);
        }

        /// <summary>
        /// 設定資料庫基本物件
        /// </summary>
        /// <param name="paramKind"></param>
        /// <param name="paramConnectionString"></param>
        public SimpleDataObject(String paramKind, String paramConnectionString)
        {
            IConnectionBuilder ConnectionBuilder = ConnectionBuilderFactory.CreateConnectionBuilder(paramKind, paramConnectionString);
            this.SetDataObject(ConnectionBuilder);
        }

        /// <summary>完成本地資料庫物件的設定</summary>
        /// <param name="_builder"></param>
        private void SetDataObject(IConnectionBuilder _builder)
        {
            this.connection = _builder.GetConnection();
            this.command = _builder.GetCommand();
            this.dataadapter = _builder.GetDataAdapter();
        }


        #region IDataObject 成員

        /// <summary>設定交易處理的具名參數集合</summary>
        /// <param name="paramCols"></param>
        public void SetParameter(Dictionary<string, object> paramCols)
        {
            //設定參數前先清除目前存在的具名參數
            this.command.Parameters.Clear();     
            //透過資料參數集合設定具名參數
            foreach (KeyValuePair<string, object> kv in paramCols)
            {
                var p = this.command.CreateParameter();
                p.ParameterName = kv.Key;
                p.Value = kv.Value;
                //p.DbType = DbType.String;               
                this.command.Parameters.Add(p);                                
            }
        }

        /// <summary>取得交易數量</summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            this.command.CommandType = CommandType.Text;
            this.command.CommandText = sql; 
            return this.command.ExecuteNonQuery();
        }

        /// <summary>取得查詢資料表</summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataTableBySql(string sql)
        {
            lock (this)
            {
                if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }

                this.command.Connection = this.connection;
                this.command.CommandType = CommandType.Text;
                this.command.CommandText = sql;                
                this.dataadapter.SelectCommand = command;

                DataSet dataSet = new DataSet();

                try
                {
                    this.dataadapter.Fill(dataSet);
                    return dataSet.Tables[0];
                }
                finally
                {
                    dataSet.Dispose();
                    if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                    {
                        this.connection.Close();
                    }
                }
            }
        }


        /// <summary>取得查詢資料表</summary>
        /// <param name="sql"></param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecord"></param>
        /// <param name="srcTable"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataTableBySql(string sql, int startRecord, int maxRecord, string srcTable)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 開啟資料新刪修的交易機制
        /// </summary>
        public void BeginTransaction()
        {
            if (this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
            }
            this.dbTransaction = this.connection.BeginTransaction();            
            this.command.Connection = this.connection;
            this.command.Transaction = this.dbTransaction;
        }

        /// <summary>送出交易請求確認</summary>
        public void Commit()
        {
            this.dbTransaction.Commit();

            if (this.connection.State == ConnectionState.Open)
            {
                this.connection.Close();
            }
            //reset Transaction
            this.dbTransaction = null;
        }

        public void Rollback()
        {
            this.dbTransaction.Rollback();

            if (this.connection.State == ConnectionState.Open)
            {
                this.connection.Close();
            }

            //reset Transaction
            this.dbTransaction = null;
        }

        /// <summary>
        /// 取得DBConnection物件
        /// </summary>
        public System.Data.IDbConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        #endregion


        #region IDisposable 成員

        public void Dispose()
        {
            if (!isDisposed)
            {
                if (this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }
                this.connection.Dispose();
            }
            isDisposed = true;
            // tell the GC not to finalize
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
