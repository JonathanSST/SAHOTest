using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._98._9830
{
    public partial class DateSelect2 : System.Web.UI.Page
    {
        public string MyMonth = DateTime.Now.ToString("MM");
        public string MyYear = DateTime.Now.ToString("yyyy");
        public DateTime DateS = DateTime.Now;
        public DateTime DateE = DateTime.Now;
        public string QueryDate = DateTime.Now.ToString("yyyy-MM-dd");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["date"] != null && !string.IsNullOrEmpty(Request["date"]))
            {
                this.QueryDate = Request["date"];
            }
            else
            {
                //this.QueryDate
            }
            this.DateS = DateTime.Parse(MyYear + "/" + MyMonth + "/01");
            this.DateE = this.DateS.AddMonths(1).AddDays(-1);
            var js = "<script type='text/javascript'>Page_Init()</script>";
            ClientScript.RegisterClientScriptInclude("DateSelect", "DateSelect2.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
        }
    }
}