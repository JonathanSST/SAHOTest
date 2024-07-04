using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class ChangePWD : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;

        static string msgOldMatch, msgCover, msgConfirmMatch,msgMaxOld,msgMaxNew,msgReqOld,msgReqNew,msgReqConfirm = "";

        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            msgOldMatch = this.GetLocalResourceObject("msgOldMatch").ToString();
            msgCover = this.GetLocalResourceObject("msgCover").ToString();
            msgConfirmMatch = this.GetLocalResourceObject("msgConfirmMatch").ToString();
            msgMaxNew = this.GetLocalResourceObject("msgMaxNew").ToString();
            msgMaxOld = this.GetLocalResourceObject("msgMaxOld").ToString();
            msgReqConfirm = this.GetLocalResourceObject("msgReqConfirm").ToString();
            msgReqNew = this.GetLocalResourceObject("msgReqNew").ToString();
            msgReqOld = this.GetLocalResourceObject("msgReqOld").ToString();

            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.SaveButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ChangePWD", "ChangePWD.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            SaveButton.Attributes["onClick"] = "SaveExcute();return false;";
            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B00_SysRole", "zhtw", out TableInfo);
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
                hidePsnID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                #endregion
            }
        }
        #endregion

        #endregion

        #region Method

        #region SaveChange
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SaveChange(string UserID, string PsnID, string OldPWD, string NewPWD, string CheckPWD)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", PWCtrl = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            if (UserID.Equals("User") && !PsnID.Equals(""))
            {
                objRet.result = true;
            }
            else
            {
                objRet = Check_Input_DB(UserID, PsnID, OldPWD, NewPWD, CheckPWD, out PWCtrl);
            }
			
            #region 儲存密碼變更
            if (objRet.result)
            {
                #region Process String - Updata Role
                if (UserID.Equals("User") && !PsnID.Equals(""))
                {
                    sql = @" UPDATE B01_Person SET PsnPW = ?, UpdateUserID = ?, UpdateTime = ?
                         WHERE PsnID = ? ";
                    liSqlPara.Add("S:" + NewPWD.Trim());
                    liSqlPara.Add("S:" + PsnID.Trim());
                    liSqlPara.Add("D:" + Time);
                    liSqlPara.Add("S:" + PsnID.Trim());
                }
                else
                {
                    sql = @" UPDATE B00_SysUser 
                         SET UserPW = ?, UserPWCtrl = ?, PWChgTime = ?, 
                         UpdateUserID = ?, UpdateTime = ?
                         WHERE UserID = ? ";
                    liSqlPara.Add("S:" + NewPWD.Trim());
                    liSqlPara.Add("S:" + PWCtrl.Trim());
                    liSqlPara.Add("D:" + Time);
                    liSqlPara.Add("S:" + UserID.Trim());
                    liSqlPara.Add("D:" + Time);
                    liSqlPara.Add("S:" + UserID.Trim());
                }
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            objRet.act = "SaveChange";
            return objRet;
        }
        #endregion

        #region Check_Input_DB
        protected static Pub.MessageObject Check_Input_DB(string UserID, string PsnID, string OldPWD, string NewPWD, string CheckPWD, out string PWCtrl)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;
            string[] TempArray;
            List<string> PWArray = new List<string>();
            
            #region Input
            if (string.IsNullOrEmpty(OldPWD.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += msgReqOld;
            }
            else if (OldPWD.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += msgMaxOld;
            }

            if (string.IsNullOrEmpty(NewPWD.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += msgReqNew;
            }
            else if (NewPWD.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += msgMaxNew;
            }

            if (string.IsNullOrEmpty(CheckPWD.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += msgReqConfirm;
            }
            #endregion

            #region DB
            if (UserID.Equals("User") && !PsnID.Equals(""))
            {
                sql = @" SELECT PsnPW AS 'UserPW', '' AS 'UserPWCtrl' FROM B01_Person WHERE PsnID = ? AND PsnPW = ?  ";
                liSqlPara.Add("S:" + PsnID.Trim());
            }
            else
            {
                sql = @" SELECT UserPW, UserPWCtrl FROM B00_SysUser WHERE UserID = ? AND UserPW = ?  ";
                liSqlPara.Add("S:" + UserID.Trim());
            }

            liSqlPara.Add("S:" + OldPWD.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                if (string.Compare(NewPWD, CheckPWD) != 0)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += msgConfirmMatch;
                }


                TempArray = dr.DataReader["UserPWCtrl"].ToString().Split(',');
                for (int i = 0; i < TempArray.Length; i++)
                {
                    PWArray.Add(TempArray[i].ToString());
                }
                PWArray.Add(dr.DataReader["UserPW"].ToString());

                for (int i = 0; i < PWArray.Count; i++)
                {
                    if (string.Compare(PWArray[i], NewPWD, true) == 0)
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += msgCover;
                        break;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += msgOldMatch;
            }
            #endregion

            if (objRet.result)
            {
                PWCtrl = "";
                if (PWArray.Count >= 3)
                    PWArray.Remove(PWArray[0]);
                for (int i = 0; i < PWArray.Count; i++)
                {
                    if (!string.IsNullOrEmpty(PWCtrl)) PWCtrl += ",";
                    PWCtrl += PWArray[i];
                }
            }
            else
                PWCtrl = null;

            return objRet;
        }
        #endregion

        #endregion
    }
}