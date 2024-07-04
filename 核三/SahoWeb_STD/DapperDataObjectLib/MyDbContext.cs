using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;


namespace DapperDataObjectLib
{
    public class MyDbContext:IDisposable
    {
        public IDbConnection DbConn;
        public IDbTransaction trans;
        private bool isDisposed = false;

        public MyDbContext()
        {
            this.DbConn = GetMyDbConn();            
        }

        private static DbConnection GetMyDbConn()
        {
            return new SqlConnection();            
        }

        
        


        /// <summary>送出資料庫交易的請求 並關閉資料庫連線</summary>
        public void Commit()
        {
            this.trans.Commit();

            if (this.DbConn.State == ConnectionState.Open)
            {
                this.DbConn.Close();
            }            
            this.trans = null;
        }


        #region IDisposable 成員

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
