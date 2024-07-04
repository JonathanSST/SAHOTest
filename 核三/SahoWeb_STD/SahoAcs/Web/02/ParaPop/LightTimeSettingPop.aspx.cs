using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class LightTimeSettingPop : Sa.BasePage
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            //oScriptManager.RegisterAsyncPostBackControl(this.popB_Save);

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region LoadProcess
                string js = Sa.Web.Fun.ControlToJavaScript(this);

                js = "<script type='text/javascript'>" + js + "</script>";
                ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
                ClientScript.RegisterClientScriptInclude("LightTimeSettingPop", "/web/02/ParaPop/LightTimeSettingPop.js");//加入同一頁面所需的JavaScript檔案
                ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
                ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
                ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
                ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");
                #endregion

                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                this.hideEquID.Value = Request["EquID"];
                this.hideEquParaID.Value = Request["EquParaID"];
                this.hideParaValue.Value = Request["ParaValue"];
                #endregion
                LoadData();
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == UpdatePanel1.ClientID && sFormArg == "StarePage")
                {
                    //LoadData();
                }
            }
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            string LEDTimestr = "";
            for (int i = 1; i <= 10; i++)
            {
                TextBox LCEText = (TextBox)Page.Form.FindControl("MsgSet" + i.ToString());
                SahoAcs.uc.PickTime PickTimeObj = (SahoAcs.uc.PickTime)Page.FindControl("TimeSet_PickTime" + i.ToString());

                if (!string.IsNullOrEmpty(PickTimeObj.DateValue.Replace(":", "")))
                {
                    if (!string.IsNullOrEmpty(LEDTimestr))
                        LEDTimestr += ",";
                    LEDTimestr += i.ToString() + ":" + PickTimeObj.DateValue.Replace(":", "");
                }
            }
            Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + LEDTimestr + "';");
            Sa.Web.Fun.RunJavaScript(this, "window.close();");
        }
        #endregion

        #endregion

        #region Method

        #region LoadData
        public void LoadData()
        {
            string[] Item;
            string LCDIndex = "", LCDTime = "";
            if (hideParaValue.Value != "")
            {
                string[] Element;
                Item = hideParaValue.Value.Split(',');
                for (int i = 0; i < Item.Length; i++)
                {
                    Element = Item[i].Split(':');
                    LCDIndex = Element[0].ToString();
                    LCDTime = Element[1].Substring(0, 2) + ":" + Element[1].Substring(2, 2);
                    SahoAcs.uc.PickTimePop PickTimeObj = (SahoAcs.uc.PickTimePop)Page.FindControl("TimeSet_PickTime" + LCDIndex.ToString());
                    PickTimeObj.DateValue = LCDTime;
                }
            }
        }
        #endregion

        #endregion
    }
}
