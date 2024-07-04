using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class AliveState
    {
        public string DOPType { get; set; }
        public string DOPActive { get; set; }
        public string DOPState { get; set; }
        public string EquNo { get; set; }
        public string IPAddress { get; set; }
        public string ResultMsg { get; set; }
        public DateTime CreateTime { get; set; }
    }
}