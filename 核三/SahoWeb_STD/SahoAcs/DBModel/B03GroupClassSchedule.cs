using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class B03GroupClassSchedule : BaseTableEntity
    {
        public Decimal RecordID { get; set; }
        public String GroupName { get; set; }
        public String ClassMonth { get; set; }
        public String ClassNo { get; set; }
        public string ClassNo1 { get; set; }
        public string ClassNo2 { get; set; }
        public DateTime ClassDate { get; set; }
        public String ClassMemo { get; set; }
        public String AdjClassNo { get; set; }
        public string GroupNo { get; set; }
       
    }
}