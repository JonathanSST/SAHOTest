using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace SahoAcs
{
    public partial class MingLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = Pub.sTitel + " - 登入";
            Session["UserID"] = "";
            Session["UserPW"] = "";
            Session["UserName"] = "";
            Session["ProcNo"] = "";
            Session["PsnID"] = "";      //因應人員登入需求需增加PsnID的Session Add By Eric 20140930
            //this.txtUid.Focus();
            if (Request["PageEvent"] != null)
            {
                DoLogin();
            }
        }

        private void DoLogin()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            string sFromIP = HttpContext.Current.Request.UserHostAddress;
            string sUserID = "", sUserName = "", sPsnID = "";
            DateTime dtBegin = DateTime.Now.AddDays(-1);
            DateTime dtEnd = DateTime.Now.AddDays(-1);
            DateTime dtPWTime = DateTime.MinValue;
            Boolean IsOptAllow = false;
            string sUserPW = "";
            string sSqlCmd = "";
            string ResultMsg = "";
            string RedirectPage = "";
            List<string> liSqlPara = new List<string>();
            
            if (Request["txtUid"].Trim().Equals("") || Request["txtPwd"].Trim().Equals(""))
            {
                ResultMsg = this.GetGlobalResourceObject("Resource", "lblLoginChk").ToString();                
            }
            else
            {
                ResultMsg = "";
            }        
            sSqlCmd = @"SELECT 
	                                    B01_Person.PsnID, U.UserID, U.UserName, U.UserSTime, U.UserETime, U.PWChgTime, U.IsOptAllow,U.UserPW
                                    FROM 
	                                    B01_Person, 
	                                    (SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser WHERE UserID = 'User') AS U
                                    WHERE 
	                                    B01_Person.PsnAccount = ? AND B01_Person.PsnPW = ?
                                    UNION
                                    SELECT 0,UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser 
                                    WHERE (UserID = ?) AND (UserPW = ?)";
            sUserID = Request["txtUid"];
            sUserPW = Request["txtPwd"];
             
            liSqlPara.Add("S:" + sUserID);
            liSqlPara.Add("S:" + sUserPW);
            liSqlPara.Add("S:" + sUserID);
            liSqlPara.Add("S:" + sUserPW);

           
            Sa.DB.DBReader oReader = null;
            if (oAcsDB.GetDataReader(sSqlCmd, liSqlPara, out oReader))
            {
                if (oReader.HasRows)
                {
                    oReader.Read();
                    sUserID = oReader.ToString("UserID");
                    sUserPW = oReader.ToString("UserPW");
                    sUserName = oReader.ToString("UserName");
                    dtBegin = oReader.ToDateTime("UserSTime");
                    dtEnd = oReader.ToDateTime("UserETime");
                    dtPWTime = oReader.ToDateTime("PWChgTime");
                    IsOptAllow = oReader.ToBoolean("IsOptAllow");
                    //因應人員登入需求需增加PsnID的Session Add By Eric 20140930
                    if (oReader.ToInt32("PsnID") != 0)
                    {
                        sPsnID = oReader.ToInt32("PsnID").ToString();
                    }
                }
            }

            if (Sa.Check.IsEmptyStr(sUserID) || (sUserID.Equals("User") && Sa.Check.IsEmptyStr(sPsnID)))
            {
                //Response.Redirect("~/Web/MessagePage/LoginError1.aspx");
                RedirectPage = "/Web/MessagePage/LoginError1.aspx";
            }
            else
            {
                if (!IsOptAllow)
                {
                    //Response.Redirect("~/Web/MessagePage/LoginError2.aspx");
                    RedirectPage = "/Web/MessagePage/LoginError2.aspx";
                }
                else if (sPsnID.Equals("") && sUserID.Equals("User"))
                {
                    //Response.Redirect("~/Web/MessagePage/LoginError2.aspx");
                    RedirectPage = "/Web/MessagePage/LoginError2.aspx";
                }
                else
                {
                    DateTime dtNow = DateTime.Now;

                    if (dtEnd == DateTime.MinValue)
                    {
                        dtEnd = DateTime.MaxValue;
                    }

                    if (dtBegin > dtNow || dtEnd < dtNow)
                    {
                        //Response.Redirect("~/Web/MessagePage/LoginError3.aspx");
                        RedirectPage = "/Web/MessagePage/LoginError3.aspx";
                    }
                    else
                    {
                        Session["UserID"] = sUserID;
                        //Session["UserPW"] = rdoPsn.Checked == true ? "User" : txtPwd.Text;
                        Session["UserPW"] = sUserPW;
                        Session["UserName"] = sUserName;
                        Session["MenuNo"] = "";
                        Session["MenuName"] = "";
                        Session["FunAuthSet"] = "";
                        Session["PsnID"] = sPsnID;//因應人員登入需求需增加PsnID的Session Add By Eric 20140930

                        if (dtPWTime == DateTime.MinValue)
                        {
                            //Response.Redirect("~/Web/MessagePage/ChangePWAlert.aspx");
                            RedirectPage = "/Web/MessagePage/ChangePWAlert.aspx";
                        }
                        else
                        {
                            oAcsDB.WriteLog(DB_Acs.Logtype.人員登入登出, sUserID, sUserName, "Login", "", "", "使用者「" + sUserName + "」登入", "");
                            //Response.Redirect("~/MainForm.aspx");
                            RedirectPage ="/MainForm.aspx";
                        }
                    }
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
             new { message = "OK", redirect_page=RedirectPage,result_message=ResultMsg }));
            Response.End();
        }


        protected void btnLogin_Click(object sender,EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            string sFromIP = HttpContext.Current.Request.UserHostAddress;
            string sUserID = "", sUserName = "", sPsnID = "";
            DateTime dtBegin = DateTime.Now.AddDays(-1);
            DateTime dtEnd = DateTime.Now.AddDays(-1);
            DateTime dtPWTime = DateTime.MinValue;
            Boolean IsOptAllow = false;
            string sUserPW = "";
            string sSqlCmd = "";
            List<string> liSqlPara = new List<string>();
            if (this.txtUid.Text.Trim().Equals("") || this.txtPwd.Text.Trim().Equals(""))
            {
                //ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowAlert", "alert('帳號或密碼不得為空白!!')");
                this.lblMessage.Text = this.GetGlobalResourceObject("Resource", "lblLoginChk").ToString();
                return;
            }
            else
            {
                this.lblMessage.Text = "";
            }
            //因應人員登入需求查詢人員資料驗證 Add By Eric 20140930
            /*
            if(rdoPsn.Checked == true)
            {
                sSqlCmd = @"SELECT B01_Person.PsnID, U.UserID, U.UserName, U.UserSTime, U.UserETime, U.PWChgTime, U.IsOptAllow
                    FROM B01_Person, (SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow
                    FROM B00_SysUser WHERE UserID = 'User') AS U
                    WHERE B01_Person.PsnAccount = ? AND B01_Person.PsnPW = ?";
                liSqlPara.Add("S:" + txtUid.Text);
                liSqlPara.Add("S:" + txtPwd.Text);
            }
            else
            {
                sSqlCmd = "SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow FROM B00_SysUser WHERE (UserID = ?) AND (UserPW = ?)";
                liSqlPara.Add("S:" + txtUid.Text);
                liSqlPara.Add("S:" + txtPwd.Text);
            }
            */
            // Update By Sam 20160516 可同時使用系統使用者或人員的登入
            sSqlCmd = @"SELECT 
	                                    B01_Person.PsnID, U.UserID, U.UserName, U.UserSTime, U.UserETime, U.PWChgTime, U.IsOptAllow,U.UserPW
                                    FROM 
	                                    B01_Person, 
	                                    (SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser WHERE UserID = 'User') AS U
                                    WHERE 
	                                    B01_Person.PsnAccount = ? AND B01_Person.PsnPW = ?
                                    UNION
                                    SELECT 0,UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser 
                                    WHERE (UserID = ?) AND (UserPW = ?)";
            liSqlPara.Add("S:" + txtUid.Text);
            liSqlPara.Add("S:" + txtPwd.Text);
            liSqlPara.Add("S:" + txtUid.Text);
            liSqlPara.Add("S:" + txtPwd.Text);

            Sa.DB.DBReader oReader = null;
            if (oAcsDB.GetDataReader(sSqlCmd, liSqlPara, out oReader))
            {
                if (oReader.HasRows)
                {
                    oReader.Read();
                    sUserID = oReader.ToString("UserID");
                    sUserPW = oReader.ToString("UserPW");
                    sUserName = oReader.ToString("UserName");
                    dtBegin = oReader.ToDateTime("UserSTime");
                    dtEnd = oReader.ToDateTime("UserETime");
                    dtPWTime = oReader.ToDateTime("PWChgTime");
                    IsOptAllow = oReader.ToBoolean("IsOptAllow");
                    //因應人員登入需求需增加PsnID的Session Add By Eric 20140930
                    if (oReader.ToInt32("PsnID") != 0)
                    {
                        sPsnID = oReader.ToInt32("PsnID").ToString();
                    }
                }
            }

            if (Sa.Check.IsEmptyStr(sUserID) || (sUserID.Equals("User") && Sa.Check.IsEmptyStr(sPsnID)))
            {
                Response.Redirect("~/Web/MessagePage/LoginError1.aspx");
            }
            else
            {
                if (!IsOptAllow)
                {
                    Response.Redirect("~/Web/MessagePage/LoginError2.aspx");
                }
                else if (sPsnID.Equals("") && sUserID.Equals("User"))
                {
                    Response.Redirect("~/Web/MessagePage/LoginError2.aspx");
                }
                else
                {
                    DateTime dtNow = DateTime.Now;

                    if (dtEnd == DateTime.MinValue)
                    {
                        dtEnd = DateTime.MaxValue;
                    }

                    if (dtBegin > dtNow || dtEnd < dtNow)
                    {
                        Response.Redirect("~/Web/MessagePage/LoginError3.aspx");
                    }
                    else
                    {
                        Session["UserID"] = sUserID;
                        //Session["UserPW"] = rdoPsn.Checked == true ? "User" : txtPwd.Text;
                        Session["UserPW"] = sUserPW;
                        Session["UserName"] = sUserName;
                        Session["MenuNo"] = "";
                        Session["MenuName"] = "";
                        Session["FunAuthSet"] = "";
                        Session["PsnID"] = sPsnID;//因應人員登入需求需增加PsnID的Session Add By Eric 20140930

                        if (dtPWTime == DateTime.MinValue)
                        {
                            Response.Redirect("~/Web/MessagePage/ChangePWAlert.aspx");
                        }
                        else
                        {
                            oAcsDB.WriteLog(DB_Acs.Logtype.人員登入登出, sUserID, sUserName, "Login", "", "", "使用者「" + sUserName + "」登入", "");
                            Response.Redirect("~/MainForm.aspx");
                        }
                    }
                }
            }
        }

    }//end class
}//end namespace