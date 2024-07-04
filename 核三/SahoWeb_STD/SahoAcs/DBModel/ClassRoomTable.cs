using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class ClassRoomTable
    {
        public string PsnNo { get; set; }
        public string InOutDate { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }

        public string CardTime { get; set; }
        public string CardDate { get; set; }
        
        public string PsnName { get; set; }

        public string CardNo { get; set; }

        public string DepID { get; set; }
        public string DepName { get; set; }
        public string Company { get; set; }
        public string EquNo { get; set; }
        public string EquName { get; set; }




    }
}