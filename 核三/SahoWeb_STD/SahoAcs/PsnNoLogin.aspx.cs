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
    public partial class PsnNoLogin : System.Web.UI.Page
    {
        public string Version = "";
        public string PsnName = "";
        //-------------------------------------------------------------------------------------------------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            if(DongleVaries.GetDateAlive())
            {

            }

            SetVersion();
            this.Title = Pub.sTitel + " - 登入";
            this.radio.Visible = false;
            Session["UserID"] = "";
            Session["UserPW"] = "";
            Session["UserName"] = "";
            Session["ProcNo"] = "";
            Session["PsnID"] = "";      //因應人員登入需求需增加PsnID的Session Add By Eric 20140930
            Session["TimeOffset"] = "";
            txtUid.Focus();

            //Session["UserAccount"] = "Saho";
            //Session["PassWD"] = "1234";
            //Response.Redirect("~/MainForm.aspx");

            //HttpBrowserCapabilities hbc = Request.Browser;            
            //Response.Write(hbc.Browser.ToString() + "<br/>"); //取得瀏覽器名稱
            //Response.Write(hbc.Version.ToString() + "<br/>"); //取得瀏覽器版本號
            //Response.Write(hbc.Platform.ToString() + "<br/>");     //取得作業系統名稱
            
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
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            string sFromIP = HttpContext.Current.Request.UserHostAddress;
            string sUserID = "", sUserName = "", sPsnID = "";
            DateTime dtBegin = DateTime.Now.AddDays(-1);
            DateTime dtEnd = DateTime.Now.AddDays(-1);
            DateTime dtPWTime = DateTime.MinValue;
            Boolean IsOptAllow = false;
            string sUserPW = "";
            string sSqlCmd="";
            List<string> liSqlPara = new List<string>();
            if (!this.txtUid.Text.Trim().Equals("") && this.txtPwd.Text.Trim().Equals(""))
            {
                DataTable dt = new DataTable();
                //判斷是否使用身份證號直接登入
                liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtUid.Text, true));
                oAcsDB.GetDataTable("PersonID", "SELECT * FROM B01_Person WHERE PsnNo=? ", liSqlPara, out dt);
                if (dt.Rows.Count > 0)
                {
                    Session["IDNum"]=AntiXssEncoder.HtmlEncode(txtUid.Text, true);
                    Response.Redirect("/Web/06/0650/QueryPersonMobileLog.aspx");
                }
            }
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
	                                    B01_Person.PsnID, U.UserID, PsnName AS UserName, U.UserSTime, U.UserETime, U.PWChgTime, U.IsOptAllow,U.UserPW
                                    FROM 
	                                    B01_Person, 
	                                    (SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser WHERE UserID = 'User') AS U
                                    WHERE 
	                                    B01_Person.PsnNo = ? AND B01_Person.PsnPW = ?
                                    UNION
                                    SELECT 0,UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser 
                                    WHERE (UserID = ?) AND (UserPW = ?)";
            liSqlPara.Clear();
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtUid.Text,true));
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtPwd.Text,true));
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtUid.Text,true));
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(this.txtPwd.Text,true));

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
                    if (oReader.ToInt32("PsnID")!=0)
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
                        Session["TimeOffset"] = this.HiddenTimeOffset.Value;
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