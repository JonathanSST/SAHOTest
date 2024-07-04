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
    public partial class MasterCardSetting : System.Web.UI.Page
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
            ClientScript.RegisterClientScriptInclude("MasterCardSetting", "MasterCardSetting.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊Button動作
            popB_Save.Attributes.Add("OnClick", " __doPostBack(popB_Save.id, ''); return false;");
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
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == UpdatePanel1.ClientID && sFormArg == "StarePage")
                {
                    DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                    string sql = "";
                    List<string> liSqlPara = new List<string>();
                    Sa.DB.DBReader dr;

                    #region Process DefaultCardLen
                    sql = @" SELECT CardNoLen FROM B01_EquData WHERE EquID = ? ";
                    liSqlPara.Add("S:" + hideEquID.Value);
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    if (dr.Read())
                    {
                        hideCardLen.Value = dr.DataReader["CardNoLen"].ToString();
                        UpdatePanel_ValueKeep.Update();
                    }
                    #endregion

                    popL_FunctionRemind.Text = @"提醒：卡號需為0~9、A~F組成，長度為" + hideCardLen.Value + @"碼。<br>
                                             　　　密碼需為0~9，長度為4碼。";
                    LoadData();
                }
            }
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string MasterCardstr = "";
            for (int i = 1; i <= 10; i++)
            {
                TextBox CardNo = (TextBox)Page.Form.FindControl("Input_CardNo" + i.ToString());
                TextBox PassWord = (TextBox)Page.Form.FindControl("Input_PassWord" + i.ToString());
                objRet = Check_Input(this.hideCardLen.Value, CardNo.Text, PassWord.Text);
                if (objRet.result && !string.IsNullOrEmpty(CardNo.Text) && !string.IsNullOrEmpty(PassWord.Text))
                {
                    if (!string.IsNullOrEmpty(MasterCardstr)) MasterCardstr += ",";
                    MasterCardstr += CardNo.Text + PassWord.Text;
                }
                else
                    break;
            }
            if (objRet.result)
            {
                Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + MasterCardstr + "';");
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
            Pub.MessageObject objRet = new Pub.MessageObject();
            string[] item;
            string Cardstr, PWDstr;

            if (!string.IsNullOrEmpty(hideParaValue.Value))
            {
                item = hideParaValue.Value.Split(',');
                for (int i = 0; i < item.Length; i++)
                {
                    Cardstr = item[i].Substring(0, int.Parse(hideCardLen.Value));
                    PWDstr = item[i].Substring(int.Parse(hideCardLen.Value), 4);
                    TextBox CardTextBox = form1.FindControl("Input_CardNo" + (i + 1).ToString()) as TextBox;
                    TextBox PWDTextBox = form1.FindControl("Input_PassWord" + (i + 1).ToString()) as TextBox;
                    CardTextBox.Text = Cardstr;
                    PWDTextBox.Text = PWDstr;
                }
            }
        }
        #endregion

        #region Check_Input
        protected Pub.MessageObject Check_Input(string CardLen, string CardNo, string PWD)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string Cardpattern = @"[0-9|a-f|A-F]{" + CardLen + "}";
            string PWDpattern = @"[0-9]{4}";

            if (!string.IsNullOrEmpty(CardNo) && string.IsNullOrEmpty(PWD) || (string.IsNullOrEmpty(CardNo) && !string.IsNullOrEmpty(PWD)))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                objRet.result = false;
                objRet.message += "卡號 與 密碼 需同時輸入";
            }

            if (!string.IsNullOrEmpty(CardNo))
            {
                if (!Regex.IsMatch(CardNo, Cardpattern) || CardNo.Length != int.Parse(CardLen))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                    objRet.result = false;
                    objRet.message += "卡號 格式錯誤。卡號需為0~9、A~F組成，長度為" + CardLen + "碼";
                }
            }

            if (!string.IsNullOrEmpty(PWD))
            {
                if (!Regex.IsMatch(PWD, PWDpattern) || PWD.Length != 4)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                    objRet.result = false;
                    objRet.message += "密碼 格式錯誤。密碼需為0~9，長度為4碼";
                }
            }

            objRet.act = "CheckData";
            return objRet;
        }
        #endregion

        #endregion
    }
}
