using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class SysParaData
    {
        public int RecordID { get; set; }
        public string ParaNo { get; set; }
        public string ParaClass { get; set; }
        public string ParaName { get; set; }
        public string ParaValue { get; set; }
        public string ParaType { get; set; }
        public string ParaDesc { get; set; }
        public string CreateUserID { get; set; }
        public string UpdateUserID { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}