using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DB.Fileds
{
    public class SysMenu : Sa.DB.Field
    {
        public StringField MenuNo = new StringField("MenuNo");
        public StringField MenuName = new StringField("MenuName");
        public StringField MenuEName = new StringField("MenuEName");
        public StringField MenuDesc = new StringField("MenuDesc");
        public StringField UpMenuNo = new StringField("UpMenuNo");
        public Int32Field MenuOrder = new Int32Field("MenuOrder");
        public StringField MenuType = new StringField("MenuType");
        public StringField FunTrackType = new StringField("FunTrackType");
        public StringField FunTrack = new StringField("FunTrack");
        public StringField FunParameter = new StringField("FunParameter");
        public StringField FunIcon = new StringField("FunIcon");
        public StringField FunTarget = new StringField("FunTarget");
        public StringField FunAuthSet = new StringField("FunAuthSet");
        public Int32Field Rev01 = new Int32Field("Rev01");
        public StringField Rev02 = new StringField("Rev02");
    }
}