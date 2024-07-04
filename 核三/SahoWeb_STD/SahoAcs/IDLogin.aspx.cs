using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Web.Security.AntiXss;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.IO;
using DapperDataObjectLib;
using SahoAcs.DBClass;



namespace SahoAcs
{
    public partial class IDLogin : System.Web.UI.Page
    {
        public string Version = "";

        //-------------------------------------------------------------------------------------------------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.Cookies.Add(new HttpCookie("SessID", ""));
            }
            if(DongleVaries.GetDateAlive())
            {

            }
            SetVersion();
            this.Title = Pub.sTitel + " - 登入";            
            Session["UserID"] = "";
            Session["UserPW"] = "";
            Session["UserName"] = "";
            Session["ProcNo"] = "";
            Session["PsnID"] = "";      //因應人員登入需求需增加PsnID的Session Add By Eric 20140930
            Session["TimeOffset"] = "";
            txtUid.Focus();

            if (Request.Form["DoAction"] != null && Request["DoAction"].Equals("Login"))
            {
                this.SetProcLogin();
            }
            
        }

        void SetVersion()
        {
            var oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));            
            oAcsDB.GetSysParameter("SysVersion", Request.Cookies["i18n"].Value.ToString(), "", "", "", out this.Version);
            if (this.Version != "")
            {
                
            }            
        }
      

        //-------------------------------------------------------------------------------------------------------------------------------------------
        private void SetProcLogin()
        {
            if (Request.Url.Authority != (Request.UrlReferrer != null ? Request.UrlReferrer.Authority : ""))
            {
                Response.Redirect("~/Web/MessagePage/LoginError1.aspx");
            }
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            string sFromIP = HttpContext.Current.Request.UserHostAddress;
            string sUserID = "", sUserName = "", sPsnID = "", sIDNum = "";
            DateTime dtBegin = DateTime.Now.AddDays(-1);
            DateTime dtEnd = DateTime.Now.AddDays(-1);
            DateTime dtPWTime = DateTime.Now.AddMonths(-3);
            Boolean IsOptAllow = false;
            string sUserPW = "";
            string sSqlCmd="";
            int ErrCnt = 0;
            List<string> liSqlPara = new List<string>();
            this.txtPwd.Text = Request.Form["txtPwd"];
            this.txtUid.Text = Request.Form["txtUid"];
            //if (!this.txtUid.Text.Trim().Equals("") && this.txtPwd.Text.Trim().Equals(""))
            //{
            //    DataTable dt = new DataTable();
            //    //判斷是否使用身份證號直接登入
            //    liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtUid.Text, true));
            //    oAcsDB.GetDataTable("PersonID", "SELECT * FROM B01_Person WHERE IDNum=? ", liSqlPara, out dt);
            //    if (dt.Rows.Count > 0)
            //    {
            //        Session["IDNum"]=AntiXssEncoder.HtmlEncode(txtUid.Text, true);
            //        Response.Redirect("/Web/06/0602/0602_1.aspx");
            //    }
            //}
            if (this.txtUid.Text.Trim().Equals("") || this.txtPwd.Text.Trim().Equals(""))
            {
                //ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowAlert", "alert('帳號或密碼不得為空白!!')");
                this.lblMessage.Text = this.GetGlobalResourceObject("Resource","lblLoginChk").ToString();
                return;
            }
            else
            {
                this.lblMessage.Text = "";
            }           
            // Update By Sam 20211013 更新密碼有效期限的判斷
            sSqlCmd = @"SELECT 
	                                    IDNum, B01_Person.PsnID, U.UserID, U.UserName, U.UserSTime, U.UserETime, U.IsOptAllow, PsnPW AS UserPW, CASE ISDATE(Text2) WHEN 1 THEN CONVERT(DATETIME,Text2) ELSE DATEADD(MONTH,-3,GETDATE()) END AS PWChgTime, ISNULL(ErrCnt,0) AS ErrCnt
                                    FROM 
	                                    B01_Person, 
	                                    (SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser WHERE UserID = 'User') AS U
                                    WHERE 
	                                    B01_Person.PsnAccount = ?
                                    UNION
                                    SELECT '', 0,UserID, UserName, UserSTime, UserETime, IsOptAllow, UserPW, ISNULL(PWChgTime,DATEADD(MONTH,-3,GETDATE())) AS PWChgTime, ISNULL(ErrCnt,0) AS ErrCnt FROM B00_SysUser 
                                    WHERE (UserID = ?)";
            liSqlPara.Clear();
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtUid.Text,true));
            //liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtPwd.Text,true));
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtUid.Text,true));
            //liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(this.txtPwd.Text,true));

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
                    sIDNum = oReader.ToString("IDNum");
                    ErrCnt = oReader.ToInt32("ErrCnt");
                    //因應人員登入需求需增加PsnID的Session Add By Eric 20140930
                    if (oReader.ToInt32("PsnID")!=0)
                    {
                        sPsnID = oReader.ToInt32("PsnID").ToString();
                    }
                    //登入資料錯誤的相關設定==>密碼過期、密碼錯誤


                }
            }

            if (Sa.Check.IsEmptyStr(sUserID) || (sUserID.Equals("User") && Sa.Check.IsEmptyStr(sPsnID)))
            {
                Response.Redirect("~/Web/MessagePage/LoginError1.aspx");
            }
            else
            {
                if (ErrCnt >= 3)
                {
                    Response.Redirect("~/Web/MessagePage/LoginErrCnt3.aspx");
                }
                if (!sUserPW.Equals(this.txtPwd.Text))
                {                    
                    liSqlPara.Clear();
                    if (!sPsnID.Equals(""))
                    {
                        liSqlPara.Add("I:" + sPsnID);
                        oAcsDB.SqlCommandExecute("UPDATE B01_Person SET ErrCnt=ISNULL(ErrCnt,0)+1 WHERE PsnID=?", liSqlPara);
                    }
                    else
                    {
                        liSqlPara.Add("S:" + sUserID);
                        oAcsDB.SqlCommandExecute("UPDATE B00_SysUser SET ErrCnt=ISNULL(ErrCnt,0)+1 WHERE UserID=?", liSqlPara);
                    }
                    Response.Redirect("~/Web/MessagePage/LoginError1.aspx");
                }
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
                        Session["IDNum"] = sIDNum;
                        //Session["TimeOffset"] = this.HiddenTimeOffset.Value;                        
                        if (dtPWTime < DateTime.Now.AddMonths(-3))
                        {
                            Response.Redirect("~/Web/MessagePage/ChangePWAlert2.aspx");  //這裡用新版的密碼異動頁面進行修正
                        }
                        else
                        {
                            oAcsDB.WriteLog(DB_Acs.Logtype.人員登入登出, sUserID, sUserName, "Login", "", "", "使用者「" + sUserName + "」登入", "");
                            liSqlPara.Clear();
                            if (!sPsnID.Equals(""))
                            {
                                liSqlPara.Add("I:" + sPsnID);
                                oAcsDB.SqlCommandExecute("UPDATE B01_Person SET ErrCnt=0 WHERE PsnID=?", liSqlPara);
                            }
                            else
                            {
                                liSqlPara.Add("S:" + sUserID);
                                oAcsDB.SqlCommandExecute("UPDATE B00_SysUser SET ErrCnt=0 WHERE UserID=?", liSqlPara);
                            }
                            if (!sIDNum.Equals(""))
                            {
                                Response.Redirect("/Web/06/0602/0602_1.aspx");
                            }
                            Response.Redirect("~/MainForm.aspx");
                        }
                    }
                }
            }
        }

        protected void hlkTW_Click(object sender, EventArgs e)
        {
            //增加一個Cookie
            Response.Cookies.Add(new HttpCookie("i18n", "zh-TW"));
            //重新導向頁面
            Response.Redirect("Default.aspx");
        }

        protected void hlkUS_Click(object sender, EventArgs e)
        {
            //增加一個Cookie
            Response.Cookies.Add(new HttpCookie("i18n", "en-US"));
            //重新導向頁面
            Response.Redirect("Default.aspx");
        }





    }//end page class

}//end namespace