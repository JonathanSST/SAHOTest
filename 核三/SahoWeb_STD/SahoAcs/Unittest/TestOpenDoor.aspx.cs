using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMS_Communication;
using DapperDataObjectLib;
using System.Configuration;

namespace SahoAcs.Unittest
{
    public partial class TestOpenDoor : System.Web.UI.Page
    {
        string BeType = "0";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["EquNo"] != null)
            {
                SetOpenDoor();
                if (ConfigurationManager.AppSettings["LanguageOption"] != null && new string[] { "1", "0" }.Contains(ConfigurationManager.AppSettings["LanguageOption"].ToString()))
                {
                    BeType = System.Configuration.ConfigurationManager.AppSettings["LanguageOption"].ToString();
                }
            }
        }


        private void SetOpenDoor()
        {
            string[] sIPArray = null;
            SahoWebSocket oSWSocket = null;
            try
            {
                if (!string.IsNullOrEmpty(Request["EquNo"].ToString()))
                {
                    #region 建立與設定SahoWebSocket
                    if (Session["SahoWebSocket"] != null)
                    {
                        if ((!((SahoWebSocket)Session["SahoWebSocket"]).IsWorking) || ((SahoWebSocket)Session["SahoWebSocket"]).IsGameOver)
                        {
                            ((SahoWebSocket)Session["SahoWebSocket"]).Stop();
                            ((SahoWebSocket)Session["SahoWebSocket"]).Start();
                        }
                    }
                    else
                    {
                        #region 取得APP的IP位址
                        string sIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                        if (!string.IsNullOrEmpty(sIPAddress)) { sIPArray = sIPAddress.Split(new char[] { ',' }); } else { sIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; }
                        if (sIPAddress == "::1") { sIPAddress = "127.0.0.1"; }
                        #endregion

                        #region 建立與設定SahoWebSocket物件及基本資料
                        Session["SahoWebSocket"] = new SahoWebSocket();
                        ((SahoWebSocket)Session["SahoWebSocket"]).SetBECommTag(BeType);
                        ((SahoWebSocket)Session["SahoWebSocket"]).UserID = "Saho";
                        ((SahoWebSocket)Session["SahoWebSocket"]).SourceIP = sIPAddress;
                        ((SahoWebSocket)Session["SahoWebSocket"]).SCListenIPPort = System.Web.Configuration.WebConfigurationManager.AppSettings["SCListenIPPort"].ToString();
                        ((SahoWebSocket)Session["SahoWebSocket"]).DbConnectionString = Pub.GetConnectionString(Pub.sConnName);
                        ((SahoWebSocket)Session["SahoWebSocket"]).Start();
                        #endregion
                    }
                    #endregion

                    #region 傳送APP指令字串
                    oSWSocket = (SahoWebSocket)Session["SahoWebSocket"];
                    oSWSocket.ClearCmdResult();
                    //sAppCmdStrArray = sCmdStrList.Split(';');
                    //for (int i = 0; i < sAppCmdStrArray.Length; i++) {
                    oSWSocket.SendAppCmdStr("4@"+Request["EquNo"].ToString()+ "@OpenDoorSet@");
                    #endregion
                }
            }
            finally
            {
                sIPArray = null;
                oSWSocket = null;
            }
        

        }
    }//end class
}//end namespace