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
    public partial class LightInfoSetting : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.popB_Save);

            string js = Sa.Web.Fun.ControlToJavaScript(this);

            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("LightInfoSetting", "LightInfoSetting.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                

                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == UpdatePanel1.ClientID && sFormArg == "StarePage")
                {
                    LoadData();
                }
            }
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string LEDTextstr = "";
            for (int i = 1; i <= 10; i++)
            {
                TextBox LCEText = (TextBox)Page.Form.FindControl("MsgSet" + i.ToString());
                SahoAcs.uc.PickTime PickTimeObj = (SahoAcs.uc.PickTime)Page.FindControl("TimeSet_PickTime" + i.ToString());

                objRet = Check_Input(LCEText.Text);
                if (objRet.result && !string.IsNullOrEmpty(LCEText.Text))
                {
                    if (!string.IsNullOrEmpty(LEDTextstr))
                        LEDTextstr += ",";
                    LEDTextstr += i.ToString() + ":" + LCEText.Text;
                }
            }
            if (objRet.result)
            {
                Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + LEDTextstr + "';");
                Sa.Web.Fun.RunJavaScript(this, "window.close();");
            }
            else
                Sa.Web.Fun.RunJavaScript(this, "alert('" + objRet.message + "')");
        }
        #endregion

        #endregion

        #region Method

        #region LoadData
        public void LoadData()
        {
            string[] Item;
            string LCDIndex = "", LCDText = "";
            if (hideParaValue.Value != "")
            {
                string[] Element;
                Item = hideParaValue.Value.Split(',');
                for (int i = 0; i < Item.Length; i++)
                {
                    Element = Item[i].Split(':');
                    LCDIndex = Element[0].ToString();
                    LCDText = Element[1].ToString();
                    TextBox LCETextBox = (TextBox)Page.Form.FindControl("MsgSet" + LCDIndex.ToString());
                    LCETextBox.Text = LCDText;
                }
            }
        }
        #endregion

        #region Check_Input
        protected Pub.MessageObject Check_Input(string LCEText)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();

            if (LCEText.Length > 4)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "LCD字數超出長度";
            }

            objRet.act = "CheckData";
            return objRet;
        }
        #endregion

        #endregion
    }
}
