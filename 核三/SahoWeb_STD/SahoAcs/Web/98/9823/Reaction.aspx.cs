using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;


namespace SahoAcs
{
    public partial class Reaction : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public string DbVer = "";
        public string DbUpdVer = "";
        public string DbChgTime = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='DbCrtVer'"))
            {
                this.DbVer = Convert.ToString(o.ParaValue);
            }
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='DbUpdVer'"))
            {
                this.DbUpdVer = Convert.ToString(o.ParaValue);
            }
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='DbChgTime'"))
            {
                this.DbChgTime = Convert.ToString(o.ParaValue);
            }
        }//end method
    }//end page class
}//end namespace