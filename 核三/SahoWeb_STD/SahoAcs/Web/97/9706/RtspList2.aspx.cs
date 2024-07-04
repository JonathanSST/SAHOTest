using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Script.Serialization;

using DapperDataObjectLib;
using SahoAcs.DBModel;



namespace SahoAcs.Web._97._9706
{
    public partial class RtspList2 :SahoAcs.DBClass.BasePage
    {
        public List<RtspEntity> GlobalRtspList = new List<RtspEntity>();

        public string RtspHost = "WS://";
        public int EvoPre = 0;

        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] == null)
            {
                this.SetRtspList();
            }
            
          
            if (Request["PageEvent"] != null && Request["PageEvent"] == "GetResource")
            {
                //this.SetRtspInfo();
            }
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("JsFun", "RtspList.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
        }


        

        private void SetRtspList()
        {            
            this.GlobalRtspList = this.od.GetQueryResult<RtspEntity>("SELECT * FROM B03_RtspInfo").ToList();
            foreach(var o in this.GlobalRtspList)
            {
                o.ChkSource = Guid.NewGuid().ToString();
                o.ChkTarget = Sa.Fun.Encrypt(o.ChkSource);
            }
            Session["RtspList"] = this.GlobalRtspList;
            this.RtspHost = this.od.GetStrScalar("SELECT UserRtspServerUrl FROM B00_SysUser WHERE UserID=@UserID", new { UserID = Sa.Web.Fun.GetSessionStr(this, "UserID") });
        }



    }//end class
}//end namespace