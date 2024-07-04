using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAnnotationMapper;
using MyDataObjectLib;
using System.Data;


namespace TempCore
{
    public class OrgCore:BasicCore
    {

        protected override void SetInitColumns()
        {
            throw new NotImplementedException();
        }

        public override DataTable GetBaseData()
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
