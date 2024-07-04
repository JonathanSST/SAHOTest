using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web.Sample
{
    public partial class ModifyBackup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Literal1.Text = Pub.ModifyBackupInfo("B00_SysUserMenus", "0");
        }
    }
}