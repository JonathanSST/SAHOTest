using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace DapperDataObjectLib
{
    public interface IConnectionBuilder
    {
        IDbCommand GetCommand();

        IDbConnection GetConnection();

        IDbDataAdapter GetDataAdapter();
    }
}
