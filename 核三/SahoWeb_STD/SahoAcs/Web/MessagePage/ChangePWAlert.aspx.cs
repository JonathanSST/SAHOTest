using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web.MessagePage
{
    public partial class ChangePWAlert : System.Web.UI.Page
    {
        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            ToolkitScriptManager1.EnablePageMethods = true;

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ChangePWAlert", "ChangePWAlert.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊主頁Button動作
            btGoChangePW.Attributes["onClick"] = "DivMode(); return false;";
            btChangePW.Attributes["onClick"] = "ChangePWExcute(); return false;";
            #endregion

            #endregion

            if (!IsPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion
            }
        }
        #endregion

        #endregion

        #region Method

        #region ChangePW
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ChangePW(string UserID, string OldPWD, string NewPWD, string CheckPWD)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", PWCtrl = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB(UserID, OldPWD, NewPWD, CheckPWD, out PWCtrl);
            #region 儲存密碼變更
            if (objRet.result)
            {

                #region Process String - Updata Role
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
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            objRet.act = "ChangePW";
            return objRet;
        }
        #endregion

        #region Check_Input_DB
        protected static Pub.MessageObject Check_Input_DB(string UserID, string OldPWD, string NewPWD, string CheckPWD, out string PWCtrl)
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
                objRet.message += "舊密碼 必須輸入";
            }
            else if (OldPWD.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "舊密碼 字數超過上限";
            }

            if (string.IsNullOrEmpty(NewPWD.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "新密碼 必須輸入";
            }
            else if (NewPWD.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "新密碼 字數超過上限";
            }

            if (string.IsNullOrEmpty(CheckPWD.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "確認密碼 必須輸入";
            }
            #endregion

            #region DB

            sql = @" SELECT UserPW, UserPWCtrl FROM B00_SysUser WHERE UserID = ? AND UserPW = ?  ";
            liSqlPara.Add("S:" + UserID.Trim());
            liSqlPara.Add("S:" + OldPWD.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                if (string.Compare(NewPWD, CheckPWD) != 0)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "密碼輸入二次前後不一致";
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
                        objRet.message += "新密碼不同與前三次密碼相同";
                        break;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "舊密碼不符";
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