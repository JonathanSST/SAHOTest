using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;


namespace DapperDataObjectLib
{
    public class OracleConnectionBuilder:BaseConectionBuilder
    {
        public OracleConnectionBuilder(string paramConnectionString)
            : base(paramConnectionString)
        {

        }

        public override IDbCommand GetCommand()
        {
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            return cmd;
        }

        public override IDbConnection GetConnection()
        {
            return new OracleConnection(this.connectionstring);
        }

        public override IDbDataAdapter GetDataAdapter()
        {
            return new OracleDataAdapter();
        }

    }//end class
}//end namespace
