using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web.Sample
{
    public partial class SelectTime : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");               // 加入頁面共用的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");               // 加入頁面共用的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");           // 加入頁面共用的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");     // 加入搭配 GridView 顯示光棒用的 JavaScript 檔案
            #endregion
            SetTime.DateValue = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            SetTime.SetReadOnly();
            SetTime.SetWidth(500);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Write(StartTime.DateValue.ToString() + "<br>");
            Response.Write(EndTime.DateValue.ToString() + "<br>");
        }
    }
}