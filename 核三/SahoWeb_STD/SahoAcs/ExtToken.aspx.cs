using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SSOCOM;
using DapperDataObjectLib;
using SahoAcs.DBClass;

namespace SahoAcs
{
    public partial class ExtToken : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        protected void Page_Load(object sender, EventArgs e)
        {
            string LightID = "";
            if (Request["lightID"] != null)
            {
                LightID = Request["lightID"];
            }
            SSOCOMComponent sso = new SSOCOMComponent();
            int HttpCode = 0;
            string UrlStr = System.Configuration.ConfigurationManager.AppSettings["ChkUrl"];
            string ApName = System.Configuration.ConfigurationManager.AppSettings["ApName"];
            sso.initialize(UrlStr);

            var result = "";
                //sso.validateSession(LightID, ApName, 0);
            var log = DBClass.SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.人員登入登出, "ExtUser", "外部使用者", "ExtToken.aspx");
            log.LogInfo = string.Format("Url:{1}，AP:{2}....sso物件初始化完成，目前的LightID={0} ", LightID, UrlStr, ApName);
            odo.SetSysLogCreate(log);
            //工作階段失效
            //if (result != "")
            //{
            //    Response.Redirect("~/Web/MessagePage/LoginError1.aspx");
            //}
            int min = 0;
            if (System.Configuration.ConfigurationManager.AppSettings["TimeOutMin"] != null)
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TimeOutMin"].ToString(), out min);
            }
            result = sso.getProperty(LightID, 0);
            HttpCode = sso.httpCode;
            log.LogInfo = string.Format("LightID=>{1},HttpCode=>{0},TimeOutMin=>{2} 取得sso lightID result回應:", HttpCode, LightID, min) + result;
            odo.SetSysLogCreate(log);
            string[] UserInfo = result.Split('|');
            string UserNo = "";
            string UserName = "";
            if (UserInfo.Length > 2)
            {
                UserName = UserInfo[1];
                UserNo = UserInfo[0];
            }
            //無效使用者帳號
            Response.Write(string.Format("目前的LightID={0}", LightID));
            Response.Write("<br/>http code:" + HttpCode);
            Response.Write("<br/>" + result);

            var UserList = odo.GetQueryResult("SELECT * FROM B00_SysUser WHERE UserID=@UserID AND GETDATE() BETWEEN UserSTime AND UserETime AND IsOptAllow=1  ", new { UserID = UserNo });
            if (UserList.Count() > 0)
            {
                //導入登入完成頁面
                UserName = Convert.ToString(UserList.First().UserName);
                Session["UserID"] = "";
                Session["UserPW"] = "";
                Session["UserName"] = "";
                Session["ProcNo"] = "";
                Session["PsnID"] = "";
                Session["UserID"] = UserNo;                
                Session["UserPW"] = Convert.ToString(UserList.First().UserPW);
                Session["UserName"] = UserName;
                //Session["LightID"] = LightID;
                log.LogInfo = string.Format("驗證完成 LightID=>{1}...UserId=>{0} 取得sso資料", UserNo, LightID) + result;
                odo.SetSysLogCreate(log);
                //odo.Execute("INSERT INTO B03_ExtTokenInfo (LightID,UserID) VALUES (@LightID,@UserID)", new {LightID, UserID=UserNo});
                Response.Redirect("MainForm.aspx");                     
            }
            else
            {
                if (UserNo != "")
                {
                    log.LogInfo = string.Format("登入失敗 LightID=>{1}...UserId=>{0} 取得sso資料", UserNo, LightID) + result;
                    odo.SetSysLogCreate(log);
                    //導入登入失敗頁面
                    Response.Redirect("~/Web/MessagePage/ExtError.aspx?ErrorCode=2&UserID=" + UserNo + "&UserName" + UserName);
                }

            }

        }
    }//end page class
}//end namespace