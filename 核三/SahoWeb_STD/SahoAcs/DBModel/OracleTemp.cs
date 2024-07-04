using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class OracleTemp
    {
        public string IPLTLIC { get; set; }
        public string IDNO { get; set; }
        public string IPLTTM { get; set; }
        public string OPLTTM { get; set; }
        public string NM { get; set; }
        public string DPID { get; set; }
        public string DPNM { get; set; }
        public string VNDID { get; set; }
        public string VNDNM { get; set; }
        public string VHNO { get; set; }
        public string RecDesc { get; set; }
        public string IPLTTM2 { get; set; }
        public DateTime? InTime2nd { get; set; }
        public DateTime? OutTime2nd { get; set; }
        public int InId { get; set; }
        public int OutId { get; set; }
    }
}