using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DapperDataObjectLib
{
    public class SqlConnectionBuilder:BaseConectionBuilder
    {
        public SqlConnectionBuilder(string paramConnectionString):base(paramConnectionString)
        {

        }

        public override IDbCommand GetCommand()
        {            
            return new SqlCommand();
        }

        public override IDbConnection GetConnection()
        {
            return new SqlConnection(this.connectionstring);
        }

        public override IDbDataAdapter GetDataAdapter()
        {
            return new SqlDataAdapter();
        }

    }
}
