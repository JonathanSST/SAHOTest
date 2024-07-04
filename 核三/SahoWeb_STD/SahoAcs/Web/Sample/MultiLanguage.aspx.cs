using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web.Sample
{
    public partial class MultiLanguage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
           
            Response.Cookies.Add(new HttpCookie("Language", "zh-tw"));
            Response.Redirect("MultiLanguage.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Cookies.Add(new HttpCookie("Language", "en-us"));
            Response.Redirect("MultiLanguage.aspx");
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Response.Cookies.Add(new HttpCookie("Language", "zh-cn"));
            Response.Redirect("MultiLanguage.aspx");
        }

    }
}