using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using System.Data;
using SahoAcs.DBModel;

namespace SahoAcs
{
    public partial class MoexLogin : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        DBModel.City city = new City();
        List<City> list_data = new List<City>();
        public DataTable DataParaList = new DataTable();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["Login"] != null)
            {
                DoLogin();
            }


            this.DataParaList = this.odo.GetDataTableBySql("SELECT * FROM B00_SysParameter WHERE ParaClass='LinkPage' ");
        }



        private void DoLogin()
        {
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

            sSqlCmd = @"SELECT 
	                                    B01_Person.PsnID, U.UserID, U.UserName, U.UserSTime, U.UserETime, U.PWChgTime, U.IsOptAllow,U.UserPW
                                    FROM 
	                                    B01_Person, 
	                                    (SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser WHERE UserID = 'User') AS U
                                    WHERE 
	                                    B01_Person.PsnAccount = @Account AND B01_Person.PsnPW = @Password
                                    UNION
                                    SELECT 0,UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow, UserPW FROM B00_SysUser 
                                    WHERE (UserID = @Account) AND (UserPW = @Password)";
            sUserID = Request["account"];
            sUserPW = Request["password"];
            var result = this.odo.GetQueryResult(sSqlCmd, new { Account = sUserID, Password = sUserPW });
            foreach(var o in result)
            {
                sUserID = Convert.ToString(o.UserID);
                sUserPW = Convert.ToString(o.UserPW);
                sUserName = Convert.ToString(o.UserName);
                dtBegin = Convert.ToDateTime(o.UserSTime);
                dtEnd = Convert.ToDateTime(o.UserETime);
                dtPWTime = Convert.ToDateTime(o.PWChgTime);
                IsOptAllow = Convert.ToBoolean(o.IsOptAllow);
                //因應人員登入需求需增加PsnID的Session Add By Eric 20140930
                if (Convert.ToInt32(o.PsnID) != 0)
                {
                    sPsnID = Convert.ToInt32(o.PsnID).ToString();
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
                            //odo.WriteLog(DB_Acs.Logtype.人員登入登出, sUserID, sUserName, "Login", "", "", "使用者「" + sUserName + "」登入", "");
                            //Response.Redirect("~/MainForm.aspx");
                            RedirectPage = "/MainForm.aspx";
                        }
                    }
                }
            }
            Response.Redirect(RedirectPage);


        }


    }//end page class 
}//end namespace