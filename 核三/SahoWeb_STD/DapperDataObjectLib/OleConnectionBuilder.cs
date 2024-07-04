using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace DapperDataObjectLib
{
    public class OleConnectionBuilder : BaseConectionBuilder
    {
        public OleConnectionBuilder(string paramConnectionString):base(paramConnectionString)
        {

        }

        public override IDbCommand GetCommand()
        {
            OleDbCommand cmd = new OleDbCommand();            
            return new OleDbCommand();
        }

        public override IDbConnection GetConnection()
        {
            return new OleDbConnection(this.connectionstring);
        }

        public override IDbDataAdapter GetDataAdapter()
        {
            return new OleDbDataAdapter();
        }

    }
}
