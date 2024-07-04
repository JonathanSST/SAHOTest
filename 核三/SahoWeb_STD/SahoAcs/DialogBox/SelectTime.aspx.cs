using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.DialogBox
{
    public partial class SelectTime : System.Web.UI.Page
    {
        ScriptManager oScriptManager;
        protected void Page_Load(object sender, EventArgs e)
        {
            oScriptManager = ScriptManager1;

            string js = Sa.Web.Fun.ControlToJavaScript(this);                                       // 將 JavaScript 會使用到的物件變數先指定到控制項上
            if ((!IsPostBack) && (!oScriptManager.IsInAsyncPostBack)) js += "\nPage_Init();";
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("SelectTime", "SelectTime.js");//加入同一頁面所需的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");
            oScriptManager.EnablePageMethods = true;// 啟動 PageMethod 式的 Web Service

            #region 更改元件Style
            ChangeStyle(LaFirst);
            ChangeStyle(La00Min);
            ChangeStyle(La15Min);
            ChangeStyle(La30Min);
            ChangeStyle(La45MIn);
            ChangeStyle(LaLast);
            ChangeStyle(LaClean);
            #endregion

            #region 註冊元件動作

            LaFirst.Attributes["onClick"] = "Call_First(); return false;";
            La00Min.Attributes["onClick"] = "Call_00Min(); return false;";
            La15Min.Attributes["onClick"] = "Call_15Min(); return false;";
            La30Min.Attributes["onClick"] = "Call_30Min(); return false;";
            La45MIn.Attributes["onClick"] = "Call_45Min(); return false;";
            LaLast.Attributes["onClick"] = "Call_Last(); return false;";
            LaClean.Attributes["onClick"] = "Call_Clean(); return false;";

            BSubmit.Attributes["onClick"] = "Call_Yes(); return false;";
            BCancel.Attributes["onClick"] = "Call_Cancel(); return false;";
            #endregion
        }

        protected void ChangeStyle(Label item)
        {
            item.Attributes.Add("OnMouseMove", "this.style.color='#1E75BB';");
            item.Attributes.Add("OnMouseOut", "this.style.color='#000000';");
            item.Style.Add("cursor", "pointer;");
        }
    }
}