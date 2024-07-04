using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;

namespace SahoAcs
{
    public partial class VmsVideo : System.Web.UI.Page
    {
        public int VmsNext = 10;
        public int VmsPre = 10;
        public OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            //ClientScript.RegisterClientScriptInclude("VmsVideo", "VmsVideo.js");//加入同一頁面所需的JavaScript檔案        
            var paralist = this.od.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN ('VmsPre','VmsNext')");    
            foreach(var pkv in paralist)
            {
                if (Convert.ToString(pkv.ParaNo) == "VmsPre")
                {
                    this.VmsPre = Convert.ToInt32(pkv.ParaValue);
                }
                if (Convert.ToString(pkv.ParaNo) == "VmsNext")
                {
                    this.VmsNext = Convert.ToInt32(pkv.ParaValue);
                }

            }
        }
    }
}