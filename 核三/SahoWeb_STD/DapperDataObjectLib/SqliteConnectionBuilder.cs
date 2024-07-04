using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;


namespace DapperDataObjectLib
{
    public class SqliteConnectionBuilder : BaseConectionBuilder
    {
        public SqliteConnectionBuilder(string paramConnectionString)
            : base(paramConnectionString)
        {

        }

        public override IDbCommand GetCommand()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            return cmd;
        }

        public override IDbConnection GetConnection()
        {
            return new SQLiteConnection(this.connectionstring);
        }

        public override IDbDataAdapter GetDataAdapter()
        {
            return new SQLiteDataAdapter();
        }
    }
}
