using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TempCore;

namespace TempCore.manage
{
    public class PersonSyncCore:BasicCore
    {
        public PersonSyncCore()
            : base()
        {
            
        }

        public PersonSyncCore(string db_type, string connectionstring)
            : base(db_type, connectionstring)
        {            
        }


        protected override void SetInitColumns()
        {
            throw new NotImplementedException();
        }

        public override System.Data.DataTable GetBaseData()
        {
            throw new NotImplementedException();
        }

        public override void SetSaveDataToDB(List<Dictionary<string, string>> ListCols)
        {
            throw new NotImplementedException();
        }

        public override void SetSaveDetailDataToDB(List<Dictionary<string, string>> ListCols, string _MasterNo)
        {
            throw new NotImplementedException();
        }
    }
}
