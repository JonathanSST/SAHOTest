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
using SahoAcs.DBClass;


namespace SahoAcs
{
    public partial class Default_ : System.Web.UI.Page
    {
        public string Version = "";


        //-------------------------------------------------------------------------------------------------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Response.Cookies.Add(new HttpCookie("SessID", ""));
                string token = CreateToken();
                SaveTokenToClient(token);
                SaveTokenToServer(token);
            }
            else
            {
                if (Request.Url.Authority != (Request.UrlReferrer != null ? Request.UrlReferrer.Authority : ""))
                {
                    Response.Redirect("/Web/MessagePage/LoginSorry.aspx");
                }               
            }
            if (Request.QueryString.Count > 0)
            {
                Response.Redirect("/Web/MessagePage/LoginSorry.aspx");
            }
            this.SetVersion();
            CheckLoginPage();
            this.Title = Pub.sTitel + " - 登入";
            this.radio.Visible = false;
            //Session["UserID"] = "";
            //Session["UserPW"] = "";
            //Session["UserName"] = "";
            //Session["ProcNo"] = "";
            //Session["PsnID"] = "";      //因應人員登入需求需增加PsnID的Session Add By Eric 20140930
            //Session["TimeOffset"] = "";
            txtUid.Focus();
            
        }

        void SetVersion()
        {
            this.Version = this.GetSysVersion();
            if (this.Version != "")
            {

            }
        }


        private void CheckLoginPage()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            string sSqlCmd = @"SELECT * FROM B00_SysParameter WHERE ParaNo=? ";
            liSqlPara.Add("S:LoginPage");
            Sa.DB.DBReader oReader = null;
            if (oAcsDB.GetDataReader(sSqlCmd, liSqlPara, out oReader))
            {
                if (oReader.HasRows)
                {
                    oReader.Read();
                    Response.Redirect(oReader.ToString("ParaValue"));
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            string sFromIP = HttpContext.Current.Request.UserHostAddress;
            string sUserID = "", sUserName = "", sPsnID = "", sPsnNo = "";
            DateTime dtBegin = DateTime.Now.AddDays(-1);
            DateTime dtEnd = DateTime.Now.AddDays(-1);
            DateTime dtPWTime = DateTime.MinValue;
            Boolean IsOptAllow = false;
            string sUserPW = "";
            string sSqlCmd="";
            List<string> liSqlPara = new List<string>();
            if (string.IsNullOrEmpty(this.ClientToken.Value.Trim()) || !this.VeriftyToken(AntiXssEncoder.HtmlEncode(this.ClientToken.Value,true)))
            {
                this.lblMessage.Text = "網頁授權碼遺失";
                return;
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
            // Update By Sam 20160516 可同時使用系統使用者或人員的登入
            sSqlCmd = @"SELECT 
	                                    PsnNo,B01_Person.PsnID, U.UserID, U.UserName, U.UserSTime, U.UserETime, U.PWChgTime, U.IsOptAllow,U.UserPW
                                    FROM 
	                                    B01_Person, 
	                                    (SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser WHERE UserID = 'User') AS U
                                    WHERE 
	                                    B01_Person.PsnAccount = ? AND B01_Person.PsnPW = ?
                                    UNION
                                    SELECT '',0,UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser 
                                    WHERE (UserID = ?) AND (UserPW = ?)";
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtUid.Text,true));
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtPwd.Text,true));
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtUid.Text,true));
            liSqlPara.Add("S:" + AntiXssEncoder.HtmlEncode(txtPwd.Text,true));

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
                    if (oReader.ToString("PsnNo") != "")
                    {
                        sPsnNo = oReader.ToString("PsnNo");
                    }
                }
                else
                {
                    oAcsDB.WriteLog(DB_Acs.Logtype.人員登入登出, this.txtUid.Text, this.txtUid.Text, "Login", "", "", "使用者「" + this.txtUid.Text + "」嘗試登入失敗", "");
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
                        //Response.Cookies.Add(new HttpCookie("SessID", ""));
                        Session["UserID"] = sUserID;
                        //Session["UserPW"] = rdoPsn.Checked == true ? "User" : txtPwd.Text;
                        Session["UserPW"] = sUserPW;
                        Session["UserName"] = sUserName;
                        Session["MenuNo"] = "";
                        Session["MenuName"] = "";
                        Session["FunAuthSet"] = "";
                        Session["PsnID"] = sPsnID;              //因應人員登入需求需增加PsnID的Session Add By Eric 20140930
                        Session["PsnNo"] = sPsnNo;
                        Session["TimeOffset"] = this.HiddenTimeOffset.Value;
                        if (dtPWTime == DateTime.MinValue)
                        {
                            Response.Redirect("~/Web/MessagePage/ChangePWAlert.aspx");
                        }
                        else
                        {
                            oAcsDB.WriteLog(DB_Acs.Logtype.人員登入登出, Sa.Web.Fun.GetSessionStr(this.Page, "UserID") , Sa.Web.Fun.GetSessionStr(this.Page, "UserName") , "Login", "", "", "使用者「" + sUserName + "」登入", "");
                            Response.Redirect("~/MainForm.aspx");
                        }
                    }
                }
            }
        }
        #region 資安設定
        private string CreateToken()
        {
            string tokenKey = this.Session.SessionID + DateTime.Now.Ticks.ToString();
            System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                    new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] b = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(tokenKey));
            return BitConverter.ToString(b).Replace("-", string.Empty);
        }

        private void SaveTokenToClient(string pToken)
        {
            //hfToken.Value = pToken;
            this.ClientToken.Value = pToken;
        }

        private void SaveTokenToServer(string pToken)
        {
            Session["Token"] = pToken;
        }

        private bool VeriftyToken(string clientToken)
        {
            if (string.IsNullOrEmpty(clientToken) || Session["Token"]==null)
                return false;

            string serverToken = Session["Token"].ToString();
            if (clientToken.Equals(serverToken))
                return true;
            else
                return false;
        }

        #endregion

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
    }
}