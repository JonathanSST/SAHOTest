using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class RtspEntity
    {
        public int ResourceID { get; set; }
        public string VideoName { get; set; }
        public string RtspVideo { get; set; }
        public string RtspMemo { get; set; }
        public string ChkSource { get; set; }
        public string ChkTarget { get; set; }

    }
}