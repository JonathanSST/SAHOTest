using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class VideoAccessModel
    {
        public DateTime CardTime { get; set; }
        public string EquNo { get; set; }
        public string EquName { get; set; }
        public string CamNo { get; set; }
        public string CardNo { get; set; }
        public string EquDir { get; set; }

        public string PsnName { get; set; }

    }//end entity class
}//end namespace