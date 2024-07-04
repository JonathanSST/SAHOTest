using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.DialogBox
{
    public partial class SelectDateTime : System.Web.UI.Page
    {
        ScriptManager oScriptManager;
        protected void Page_Load(object sender, EventArgs e)
        {
            oScriptManager = ScriptManager1;
            
            if ((!IsPostBack) && (!oScriptManager.IsInAsyncPostBack))
            {

                DDListY.Items.Clear();
                DDListM.Items.Clear();
                for (Int32 i = 1940; i < 2101; i++) DDListY.Items.Add(Sa.Change.Ntoc(i, 4));
                for (Int32 i = 1; i < 13; i++) DDListM.Items.Add(Sa.Change.Ntoc(i, 2));
                for (Int32 i = 1; i < 24; i++) DDList1.Items.Add(Sa.Change.Ntoc(i, 2));
                for (Int32 i = 1; i < 60; i++)
                {
                    DDList2.Items.Add(Sa.Change.Ntoc(i, 2));
                    DDList3.Items.Add(Sa.Change.Ntoc(i, 2));
                }
            }

            string js = Sa.Web.Fun.ControlToJavaScript(this);//將JavaScript會使用到的物件變數先指定到控制項上
            if ((!IsPostBack) && (!oScriptManager.IsInAsyncPostBack)) js += "\nPage_Init();";
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("SelectDateTime", "SelectDateTime.js");//加入同一頁面所需的JavaScript檔案            
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");
            oScriptManager.EnablePageMethods = true;//啟動PageMethod式的Web Service

            #region 更改元件Style
            ChangeStyle(LaToDay);
            ChangeStyle(LaFirst);
            ChangeStyle(La15Min);
            ChangeStyle(La30Min);
            ChangeStyle(La45MIn);
            ChangeStyle(LaLast);
            ChangeStyle(LaClean);
            #endregion

            #region 註冊元件動作
            DDListY.Attributes["onChange"] = "DateOnChange();";
            DDListM.Attributes["onChange"] = "DateOnChange();";
            DDList1.Attributes["onChange"] = "ShowSelectDateTime();";
            DDList2.Attributes["onChange"] = "ShowSelectDateTime();";
            DDList3.Attributes["onChange"] = "ShowSelectDateTime();";

            LaToDay.Attributes["onClick"] = "Call_ToDay(); return false;";
            LaFirst.Attributes["onClick"] = "Call_First(); return false;";
            La15Min.Attributes["onClick"] = "Call_15Min(); return false;";
            La30Min.Attributes["onClick"] = "Call_30Min(); return false;";
            La45MIn.Attributes["onClick"] = "Call_45Min(); return false;";
            LaLast.Attributes["onClick"] = "Call_Last(); return false;";
            LaClean.Attributes["onClick"] = "Call_Clean(); return false;";

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