using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class FunMenu
    {
        public string MenuNo { get; set; }
        public string MenuName { get; set; }
        public string MenuEName { get; set; }
        public string MenuDesc { get; set; }
        public string UpMenuNo { get; set; }
        public int MenuOrder { get; set; }
        public string MenuType { get; set; }
        public string FunTrackType { get; set; }
        public string FunTrack { get; set;}
        public string FunParameter { get; set; }
        public string FunIcon { get; set; }
        public string FunTarget { get; set; }
        public string FunAuthSet { get; set; }
        public string FunAuthDef { get; set; }
        public string OpMode { get; set; }
    }
}