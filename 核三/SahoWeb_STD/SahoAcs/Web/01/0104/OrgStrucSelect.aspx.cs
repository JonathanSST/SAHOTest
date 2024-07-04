using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._01._0104
{
    public partial class OrgStrucSelect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string js = "<script src='OrgStrucSelect.js'></script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
        }
    }
}