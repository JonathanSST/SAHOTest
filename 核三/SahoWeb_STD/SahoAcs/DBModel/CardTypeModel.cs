using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class CardTypeModel
    {
        public string CardTypeNo { get; set; }
        public string CardTypeName { get; set; }
        public int CardAmt { get; set; }
        public int WaitCount { get; set; }


    }//end model class
}//end namespace