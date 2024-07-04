using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TempCore
{
    public class ErpUser
    {
        public string USER_NO { set; get; }
        public string USER_SYS_NO { set; get; }
        public string USER_NAME { set; get; }
        public string USER_GROUP { set; get; }
        public System.Data.DataTable USER_MENU { set; get; }
    }
}
