using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;


namespace DapperDataObjectLib
{
    public class OrmDataObject:IDataObject,IDisposable
    {
        IDbConnection connection;
        IDbCommand command;
        IDbDataAdapter dataadapter;


        /// <summary>Controls whether this SimpleDataObject is transactional.</summary>       
        private IDbTransaction dbTransaction;

        private bool isDisposed = false;

        private DynamicParameters dynamic_param=new DynamicParameters ();

        public string DbExceptionMessage = string.Empty;         //例外資訊
        public bool isSuccess = false;        

        public OrmDataObject()
        {

        }



        public OrmDataObject(string paramKind, String paramConnectionString)
        {
            IConnectionBuilder ConnectionBuilder = ConnectionBuilderFactory.CreateConnectionBuilder(paramKind, paramConnectionString);
            this.SetDataObject(ConnectionBuilder);
        }

        /// <summary>完成本地資料庫物件的設定</summary>
        /// <param name="_builder"></param>
        private void SetDataObject(IConnectionBuilder _builder)
        {
            try
            {
                this.connection = _builder.GetConnection();
                this.command = _builder.GetCommand();
                this.dataadapter = _builder.GetDataAdapter();
            }
            catch (Exception ex)
            {
                this.DbExceptionMessage = ex.GetBaseException().Message;
            }            
        }

        public bool CheckConnection()
        {
            try
            {
                if (this.connection != null)
                {
                    this.connection.Open();                    
                    this.connection.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.DbExceptionMessage = ex.GetBaseException().Message;
                return false;
            }                        
        }


        #region IDataObject 成員
      
        /// <summary>取得交易數量</summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            try
            {
                this.command.CommandType = CommandType.Text;
                this.command.CommandText = sql;
                return this.command.ExecuteNonQuery();
            }
            catch (System.Data.Common.DbException ex)
            {
                this.DbExceptionMessage = ex.GetBaseException().Message;
                return 0;
            }
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

        public int Execute(string cmd, object param = null)
        {
            int result = 0;
            try
            {
                this.isSuccess = true;
                result = this.connection.Execute(cmd, param,this.dbTransaction);                
            }
            catch(Exception ex)
            {
                this.DbExceptionMessage = ex.GetBaseException().Message;
                this.isSuccess = false;
                WriteLogExtension.SetLogs(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                WriteLogExtension.SetLogs(ex.GetBaseException().Message, cmd);
            }
            return result;
        }

        public int ExecuteDynamic(string cmd)
        {
            int result = 0;
            try
            {
                this.isSuccess = true;
                result = this.connection.Execute(cmd, this.dynamic_param);
            }
            catch (Exception ex)
            {
                this.DbExceptionMessage = ex.GetBaseException().Message;
                this.isSuccess = false;
                WriteLogExtension.SetLogs(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                WriteLogExtension.SetLogs(ex.GetBaseException().Message, cmd);
                //return 0;
            }
            return result;
        }

        public void AddAnsiDapper(string _param_name, string _param_value)
        {
            this.dynamic_param.Add(_param_name, _param_value, DbType.AnsiString);
        }

        public void AddStringDapper(string _param_name,string _param_value)
        {
            this.dynamic_param.Add(_param_name, _param_value,DbType.String);
        }

        public void AddDateTimeDapper(string _param_name, DateTime _param_value)
        {
            this.dynamic_param.Add(_param_name, _param_value, DbType.DateTime);
        }

        public void AddIntegerDapper(string _name,long _value)
        {
            this.dynamic_param.Add(_name, _value,DbType.Int64);
        }

        public void AddDoubleDapper(string _name, double _value)
        {
            this.dynamic_param.Add(_name, _value, DbType.Double);
        }

        public void AddParaDapper(string _name,object _value)
        {
            this.dynamic_param.Add(_name, _value);
        }

        public IEnumerable<dynamic> GetQueryResult(string cmd, object param = null)
        {
            lock (this)
            {
                if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }
                try
                {
                    return this.connection.Query(cmd, param);
                }
                catch (Exception ex)
                {
                    this.DbExceptionMessage = ex.GetBaseException().Message;
                    WriteLogExtension.SetLogs(this.DbExceptionMessage, cmd);
                    return new List<dynamic>();
                }
                finally
                {
                    if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                    {
                        this.connection.Close();
                    }
                }
            }
        }        

        public IEnumerable<dynamic> GetQueryResult(string cmd)
        {
            lock (this)
            {
                if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }
                try
                {
                    //DynamicParameters dynamic_params=new  DynamicParameters();                    
                    //foreach (var s in this.DynamicParams)
                    //{
                    //    dynamic_params.Add(s.Key, s.Value);
                    //}
                    //this.DynamicParams.Clear();
                    return this.connection.Query(cmd,this.dynamic_param);
                }
                catch (Exception ex)
                {
                    this.DbExceptionMessage = ex.GetBaseException().Message;                    
                    WriteLogExtension.SetLogs(ex.GetBaseException().Message, cmd);
                    return new List<dynamic>();
                }
                finally
                {
                    if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                    {
                        this.connection.Close();
                    }
                }
            }
        }


        public IEnumerable<T> GetQueryResult<T>(string cmd,object param=null)
        {
            lock (this)
            {
                if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }                
                try
                {
                    string typename = param.GetType().ToString();
                    return this.connection.Query<T>(cmd, param);
                }
                catch (Exception ex)
                {
                    this.DbExceptionMessage = ex.GetBaseException().Message;                    
                    WriteLogExtension.SetLogs(ex.GetBaseException().Message, cmd);
                    return new List<T>();
                }
                finally
                {                
                    if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                    {
                        this.connection.Close();
                    }
                }
            }
        }


        public IEnumerable<T> GetQueryResult<T>(string cmd)
        {
            lock (this)
            {
                if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }
                try
                {
                    //DynamicParameters dynamic_params = new DynamicParameters();
                    //foreach (var s in this.DynamicParams)
                    //{
                    //    dynamic_params.Add(s.Key, s.Value);
                    //}
                    //this.DynamicParams.Clear();
                    return this.connection.Query<T>(cmd, this.dynamic_param);
                }
                catch (Exception ex)
                {
                    this.DbExceptionMessage = ex.GetBaseException().Message;
                    WriteLogExtension.SetLogs(ex.GetBaseException().Message, cmd);
                    return new List<T>();
                }
                finally
                {
                    if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                    {
                        this.connection.Close();
                    }
                }
            }
        }


        public int GetIntScalar(string cmd, object param = null)
        {
            lock (this)
            {
                if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }
                try
                {
                    return this.connection.ExecuteScalar<int>(cmd, param);
                }
                catch (Exception ex)
                {
                    this.DbExceptionMessage = ex.GetBaseException().Message;
                    this.SetLogs(cmd);
                    return 0;
                }
                finally
                {
                    if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                    {
                        this.connection.Close();
                    }
                }
            }
        }

        public string GetStrScalar(string cmd, object param = null)
        {
            lock (this)
            {
                if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }
                try
                {
                    return this.connection.ExecuteScalar<string>(cmd, param);
                }
                catch (Exception ex)
                {
                    this.DbExceptionMessage = ex.GetBaseException().Message;
                    this.SetLogs(cmd);
                    return "";
                }
                finally
                {
                    if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                    {
                        this.connection.Close();
                    }
                }
            }
        }

        public DateTime GetDateTimeScalar(string cmd, object param = null)
        {
            lock (this)
            {
                if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }
                try
                {
                    return this.connection.ExecuteScalar<DateTime>(cmd, param);
                }
                catch (Exception ex)
                {
                    this.DbExceptionMessage = ex.GetBaseException().Message;
                    this.SetLogs(cmd);
                    return DateTime.Now;
                }
                finally
                {
                    if (this.dbTransaction == null && this.connection.State == ConnectionState.Open)
                    {
                        this.connection.Close();
                    }
                }
            }
        }

        public static DataTable ToDataTable<T>(List<dynamic> items)
        {

            DataTable dtDataTable = new DataTable();
            if (items.Count == 0)
                return dtDataTable;

            ((IEnumerable)items[0]).Cast<dynamic>().Select(p => p.Key).ToList().ForEach(col => { dtDataTable.Columns.Add(col); });

            ((IEnumerable)items).Cast<dynamic>().ToList().
                ForEach(data =>
                {
                    DataRow dr = dtDataTable.NewRow();
                    ((IEnumerable)data).Cast<dynamic>().ToList().ForEach(Col => { dr[Col.Key] = Col.Value; });
                    dtDataTable.Rows.Add(dr);
                });
            return dtDataTable;
        }

        public static DataTable IEnumerableToTable<T>(IEnumerable<T> paramlist)
        {
            DataTable dt = new DataTable();
            bool schemaIsBuild = false;
            System.Reflection.PropertyInfo[] props = null;            
            if (!schemaIsBuild)
            {
                props = typeof(T).GetProperties();
                    //item.GetType().GetProperties();
                foreach (var p in props)
                {
                    Type propType = p.PropertyType;
                    if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propType = Nullable.GetUnderlyingType(propType);
                    }
                    dt.Columns.Add(new DataColumn(p.Name, propType));
                }
                schemaIsBuild = true;
            }            
            foreach (object item in paramlist)
            {                
                var row = dt.NewRow();
                foreach (var pi in props)
                {
                    if (pi.GetValue(item, null) == null)
                    {
                        row[pi.Name] = DBNull.Value;
                    }
                    else
                    {
                        row[pi.Name] = pi.GetValue(item, null);
                    }
                }
                dt.Rows.Add(row);
            }
            dt.TableName = "InputTable";
            return dt;
        }


        #endregion


        #region 具名參數設定

        public void SetParameter(List<object> liParas)
        {
            this.command.Parameters.Clear();
            foreach (var o in liParas)
            {
                var p = this.command.CreateParameter();
                p.Value = o;
                this.command.Parameters.Add(p);
            }
        }

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



        public void SetParameterClear()
        {
            this.command.Parameters.Clear();
        }

        private void AddParameter(IDbDataParameter para)
        {
            this.command.Parameters.Add(para);
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


    }//end class
}//end namespace
