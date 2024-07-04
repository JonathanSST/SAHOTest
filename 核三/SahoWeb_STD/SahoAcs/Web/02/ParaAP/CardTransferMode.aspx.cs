using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class CardTransferMode : System.Web.UI.Page
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
            ClientScript.RegisterClientScriptInclude("CardTransferMode", "CardTransferMode.js");//加入同一頁面所需的JavaScript檔案
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
                    sql = @" SELECT CardNoLen,EquClass,EquModel FROM B01_EquData WHERE EquID = ? ";
                    liSqlPara.Add("S:" + hideEquID.Value);
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    if (dr.Read())
                    {
                        hideCardLen.Value = dr.DataReader["CardNoLen"].ToString();
                        this.hideEquClass.Value = dr.DataReader["EquClass"].ToString();
                        this.hideEquModel.Value = dr.DataReader["EquModel"].ToString();
                        UpdatePanel_ValueKeep.Update();
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(hideParaValue.Value))
                    {
                        this.Input_TransferMode.SelectedValue = hideParaValue.Value.ToString().Substring(0, 2);
                        UpdatePanel1.Update();
                    }
                }
            }
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            string CardTransferModeStr = "";
            if (this.hideEquModel.Value.Equals("SCM320.Elev"))
            {
                CardTransferModeStr = this.Input_TransferMode.SelectedValue + "01" + Convert.ToString(int.Parse(hideCardLen.Value.Trim()), 16).ToUpper().PadLeft(2, '0');
            }
            else if (this.hideEquModel.Value.Equals("SCM320"))
            {
                CardTransferModeStr = this.Input_TransferMode.SelectedValue + "00" + Convert.ToString(int.Parse(hideCardLen.Value.Trim()), 16).ToUpper().PadLeft(2, '0');
            }
            else
            {
                CardTransferModeStr = this.Input_TransferMode.SelectedValue + "02" + Convert.ToString(int.Parse(hideCardLen.Value.Trim()), 16).ToUpper().PadLeft(2, '0');
            }                
            Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + CardTransferModeStr + "';");
            Sa.Web.Fun.RunJavaScript(this, "window.close();");
        }
        #endregion

        #endregion
    }
}
