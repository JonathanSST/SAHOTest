using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class FloorAccess
    {
        public int EquID { get; set; }
        public string FloorName { get; set; }
        public string IoIndex { get; set; }
        public string BindValue { get; set; }
        public bool OpenVal { get; set; }
    }
}