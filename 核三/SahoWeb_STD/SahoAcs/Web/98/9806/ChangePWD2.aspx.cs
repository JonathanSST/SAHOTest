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
using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs
{
    public partial class ChangePWD2 : System.Web.UI.Page
    {
        #region Main Description
        //AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        static string msgOldMatch, msgCover, msgConfirmMatch,msgMaxOld,msgMaxNew,msgReqOld,msgReqNew,msgReqConfirm = "";

        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            msgOldMatch = "舊密碼不符";
            msgCover = "新密碼不可與前三次密碼相同";
            msgConfirmMatch = "密碼輸入二次前後不一致";
            msgMaxNew = "新密碼 超過長度上限";
            msgMaxOld = "舊密碼 超過長度上限";
            msgReqConfirm = "確認密碼 必須輸入";
            msgReqNew = "新密碼 必須輸入";
            msgReqOld = "舊密碼 必須輸入";

            #region LoadProcess

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ChangePWD", "ChangePWD2.js");//加入同一頁面所需的JavaScript檔案

            #endregion
            if (!IsPostBack)
            {
                //if(Request[])
                if (Request["DoAction"] != null && Request["DoAction"] == "Update")
                {
                    this.SetSaveChange();
                }
            }
        }
        #endregion

        #endregion

        #region Method


        void SetSaveChange()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string UpdateCmdStr = "";
            DateTime Time = DateTime.Now.AddMonths(3);
            string UserID = Request["hideUserID"];
            string PsnID = Request["hidePsnID"];
            string OldPwd = Request["OldPwd"];
            string NewPwd = Request["NewPwd"];
            string PWCtrl = "";
            if (UserID.Equals("User") && !PsnID.Equals(""))
            {
                objRet.result = true;

            }
            else
            {
                objRet = Check_Input_DB(UserID, PsnID, OldPwd, NewPwd, out PWCtrl);
            }
            if (objRet.result)
            {
                if (UserID.Equals("User") && !PsnID.Equals(""))
                {
                    UpdateCmdStr = @"UPDATE B01_Person SET PsnPW = @Pwd, UpdateUserID = @UserID, UpdateTime = @NowTime, Text2=CONVERT(VARCHAR(20),DATEADD(MONTH,3,GETDATE()),111)
                         WHERE PsnID = @PsnID ";
                }
                else
                {
                    UpdateCmdStr = @"UPDATE B00_SysUser 
                         SET UserPW = @Pwd, UserPWCtrl = @PWCtrl, PWChgTime=GETDATE(), DueDate=@NowTime, 
                         UpdateUserID = @UserID, UpdateTime = @NowTime
                         WHERE UserID = @UserID";
                }
                this.odo.Execute(UpdateCmdStr, new { PsnID = PsnID,UserID=UserID,NowTime=Time,Pwd=NewPwd, PWCtrl=PWCtrl});
                objRet.result = this.odo.isSuccess;
                objRet.message = this.odo.DbExceptionMessage;
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }//end method

        

        #region Check_Input_DB
        protected Pub.MessageObject Check_Input_DB(string UserID, string PsnID, string OldPWD, string NewPWD, out string PwdCtrl)
        {
            //DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            //List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;
            string[] TempArray;
            List<string> PWArray = new List<string>();
            PwdCtrl = "";
            #region DB
            if (UserID.Equals("User") && !PsnID.Equals(""))
            {
                sql = @" SELECT PsnPW AS 'UserPW', '' AS 'UserPWCtrl' FROM B01_Person WHERE PsnID = @PsnID AND PsnPW = @Pwd  ";                
            }
            else
            {
                sql = @" SELECT UserPW, UserPWCtrl FROM B00_SysUser WHERE UserID = @UserID AND UserPW = @Pwd  ";
            }

            var result = this.odo.GetQueryResult(sql, new {UserID=UserID,PsnID=PsnID,Pwd=OldPWD});
            if (result.Count() > 0)
            {
                TempArray = Convert.ToString(result.First().UserPWCtrl).Split(',');
                if (TempArray != null && TempArray.Count() > 0)
                {
                    PWArray = TempArray.ToList();
                }
                if (PWArray.Count >= 3)
                {
                    PWArray.RemoveAt(0);
                }
                PWArray.Add(Convert.ToString(result.First().UserPW));
                PwdCtrl = Convert.ToString(result.First().PwdCtrl); 
                if (PWArray.Where(i => i.Contains(NewPWD)).Count()>0)
                {
                    objRet.result = false;
                    objRet.message += msgCover;
                }
                PwdCtrl = string.Join(",", PWArray.Where(i=>i!=""));
            }
            
            #endregion

            return objRet;
        }
        #endregion

        #endregion
    }
}