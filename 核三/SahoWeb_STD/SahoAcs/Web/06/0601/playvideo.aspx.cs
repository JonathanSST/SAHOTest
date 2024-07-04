using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;


namespace SahoAcs.Web._06._0601
{
    public partial class playvideo : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public string EvoHost = "";
        public string EvoUid = "";
        public string EvoPwd = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            var result = this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN ('EvoUid','EvoPwd','EvoHost') ");
            foreach(var o in result)
            {
                if (Convert.ToString(o.ParaNo) == "EvoHost")
                {
                    this.EvoHost = Convert.ToString(o.ParaValue);
                }
                if (Convert.ToString(o.ParaNo) == "EvoPwd")
                {
                    this.EvoPwd = Convert.ToString(o.ParaValue);
                }
                if (Convert.ToString(o.ParaNo) == "EvoUid")
                {
                    this.EvoUid = Convert.ToString(o.ParaValue);
                }



            }
            
        }//end method page load

    }//end page class
}//end namespace