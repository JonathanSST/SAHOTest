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
    public partial class SpecialCodeSetting : System.Web.UI.Page
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
            ClientScript.RegisterClientScriptInclude("SpecialCodeSetting", "SpecialCodeSetting.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊Button動作

            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B01_EquParaDef", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["RoleNo"].ToString();
            //Label_Name.Text = TableInfo["RoleName"].ToString();
            //Label_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_No.Text = TableInfo["RoleNo"].ToString();
            //popLabel_Name.Text = TableInfo["RoleName"].ToString();
            //popLabel_EName.Text = TableInfo["RoleEName"].ToString();
            //popLabel_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_Desc.Text = TableInfo["RoleDesc"].ToString();
            //popLabel_Remark.Text = TableInfo["Remark"].ToString();
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                popL_FunctionRemind.Text = @"提醒：密碼需為0~9，長度為4碼。";
              
            }
            else
            {
                CreateSpecialCodeTable();
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
            string SpecialCodeStr = "", EmptyStr = "";

            for (int i = 1; i <= 16; i++)
            {
                TextBox TBox = (TextBox)Page.Form.FindControl("Input_SpecialCode" + i.ToString());

                objRet = Check_Input(TBox.Text.ToString());
                if (objRet.result)
                {
                    if (string.IsNullOrEmpty(TBox.Text.ToString().Trim()))
                        EmptyStr += "FFFF";
                    else
                        SpecialCodeStr += TBox.Text.ToString();
                }
                else
                    break;
            }

            if (objRet.result)
            {
                SpecialCodeStr = SpecialCodeStr + EmptyStr;
                Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + SpecialCodeStr + "';");
                Sa.Web.Fun.RunJavaScript(this, "window.close();");
            }
            else
                Sa.Web.Fun.RunJavaScript(this, "alert('" + objRet.message + "')");
        }
        #endregion

        #endregion

        #region Method

        #region CreateSpecialCodeTable
        public void CreateSpecialCodeTable()
        {
            TableRow tr;
            TableCell td;
            tr = new TableRow();

            for (int i = 1; i <= 16; i++)
            {
                if (i % 4 == 1)
                    tr = new TableRow();
                td = new TableCell();
                td.Text = i.ToString() + ".";
                tr.Controls.Add(td);
                td.HorizontalAlign = HorizontalAlign.Right;

                td = new TableCell();
                TextBox TBox = new TextBox();
                TBox.Width = 40;
                TBox.MaxLength = 4;
                TBox.ID = "Input_SpecialCode" + i.ToString();
                td.Controls.Add(TBox);
                tr.Controls.Add(td);
                if (i % 4 == 1)
                    SpecialCodeTable.Controls.Add(tr);
            }
            SpecialCodeTable.Attributes.Add("cellpadding", "3");
        }
        #endregion

        #region LoadData
        public void LoadData()
        {
            string SpecialCode = hideParaValue.Value;

            if (!string.IsNullOrEmpty(SpecialCode))
            {
                for (int i = 1; i <= 16; i++)
                {
                    TextBox TBox = (TextBox)SpecialCodeTable.FindControl("Input_SpecialCode" + i.ToString());
                    if (string.Compare(SpecialCode.Substring(0, 4), "FFFF") != 0)
                    {
                        TBox.Text = SpecialCode.Substring(0, 4);
                        SpecialCode = SpecialCode.Substring(4);
                    }
                    else if (string.Compare(SpecialCode.Substring(0, 4), "FFFF") == 0)
                        break;
                }
            }
        }
        #endregion

        #region Check_Input
        protected Pub.MessageObject Check_Input(string SpecialCode)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string SpecialCodepattern = @"[0-9]{4}";

            if (!string.IsNullOrEmpty(SpecialCode))
            {
                if (!Regex.IsMatch(SpecialCode, SpecialCodepattern) || SpecialCode.Length != 4)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                    objRet.result = false;
                    objRet.message += "密碼 格式錯誤。卡號需為0~9組成，長度為4碼";
                }
            }

            objRet.act = "CheckData";
            return objRet;
        }
        #endregion

        #endregion
    }
}
