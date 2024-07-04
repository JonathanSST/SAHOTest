using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.DialogBox
{
    public partial class SelectDate : System.Web.UI.Page
    {
        ScriptManager oScriptManager;
        protected void Page_Load(object sender, EventArgs e)
        {
            oScriptManager = ScriptManager1;

            if ((!IsPostBack) && (oScriptManager != null && !oScriptManager.IsInAsyncPostBack))
            {

                DDListY.Items.Clear();
                DDListM.Items.Clear();
                for (Int32 i = 1940; i < 2101; i++) DDListY.Items.Add(Sa.Change.Ntoc(i, 4));
                for (Int32 i = 1; i < 13; i++) DDListM.Items.Add(Sa.Change.Ntoc(i, 2));
            }

            string js = Sa.Web.Fun.ControlToJavaScript(this);//將JavaScript會使用到的物件變數先指定到控制項上
            if ((!IsPostBack) && (oScriptManager!=null && !oScriptManager.IsInAsyncPostBack))
                js += "\nPage_Init();";
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("SelectDate", "SelectDate.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");
            oScriptManager.EnablePageMethods = true;//啟動PageMethod式的Web Service


            #region 註冊元件動作
            DDListY.Attributes["onChange"] = "DateOnChange();";
            DDListM.Attributes["onChange"] = "DateOnChange();";

            ImgPrevious.Attributes["onClick"] = "Call_Previous(); return false;";
            ImgNext.Attributes["onClick"] = "Call_Next(); return false;";
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