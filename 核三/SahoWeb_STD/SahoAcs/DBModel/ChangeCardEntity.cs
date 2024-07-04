using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class ChangeCardEntity:SysLogEntity
    {
        public string CardNo { get; set; }

        public string NewCardNo { get; set; }

        public string PsnNo { get; set; }

        public string PsnName { get; set; }


    }//end entity class
}//end namespace