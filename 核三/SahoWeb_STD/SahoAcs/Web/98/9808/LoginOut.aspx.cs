using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web
{
    public partial class LoginOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
                string sUserID = Session["UserID"].ToString();
                string sUserName = Session["UserName"].ToString();
                oAcsDB.WriteLog(DB_Acs.Logtype.人員登入登出, sUserID, sUserName, "LogOut", "", "", "使用者「" + sUserName + "」登出", "");
                Session.Abandon();
                Session.Clear();
                Response.Redirect("~/Default.aspx", true);
            }
            else
            {
                Session.Abandon();
                Response.Redirect("~/Default.aspx", true);
            }
            
        }
    }
}