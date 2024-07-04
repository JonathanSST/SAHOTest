using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace DapperDataObjectLib
{
    public class DbWorkStation:IDisposable
    {
        MyDbContext context = new MyDbContext();

        private DataTable DtResult;

        public DataTable GetQueryDb(string _commandstring)
        {
            
            this.DtResult = new DataTable();

            return DtResult;
        }




        #region IDisposable 成員

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
