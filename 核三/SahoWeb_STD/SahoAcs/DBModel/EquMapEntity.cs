using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class EquMapEntity
    {
        public string EquNo { get; set; }
        public string MapObjID { get; set; }
        public string EquName { get; set; }
        public int PointX { get; set; }
        public int PointY { get; set; }
        public int IsOpen { get; set; }
       
        public int AliveState { get; set; }
        public string PicID { get; set; }
    }//end model class
}//end namespace