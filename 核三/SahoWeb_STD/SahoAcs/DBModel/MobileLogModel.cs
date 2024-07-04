using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class MobileLogModel
    {
        public Int64 RecordID { get; set; }
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public DateTime CardTime { get; set; }
        public string Note { get; set; }
        public string OpType { get; set; }
        public string OpName { get; set; }
        public double OpLatitude { get; set; }
        public double OpLongitude { get; set; }
        public string CardPic { get; set; }
        public string CardPic2 { get; set; }
    }
}