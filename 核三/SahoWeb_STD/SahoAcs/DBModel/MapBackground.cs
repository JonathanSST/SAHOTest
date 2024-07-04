using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class MapBackground
    {
        public int PicID { get; set; }
        public string PicName { get; set; }

        public string PicDesc { get; set; }

        public int PicAngle { get; set; }
        public string PicType { get; set; }
        public int IsOpen { get; set; }
    }
}