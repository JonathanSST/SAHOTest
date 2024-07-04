using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace DapperDataObjectLib
{
    public abstract class BaseConectionBuilder:IConnectionBuilder
    {
        public string connectionstring;

        public BaseConectionBuilder(string paramConnectionString)
        {
            this.connectionstring = paramConnectionString;
        }

        protected IDbCommand DbCommand;
        protected IDbConnection DbConnection;
        protected IDbDataAdapter DbDataAdapter;

        #region IConnectionBuilder 成員

        public abstract IDbCommand GetCommand();

        public abstract IDbConnection GetConnection();

        public abstract IDbDataAdapter GetDataAdapter();

        #endregion
    }
}
