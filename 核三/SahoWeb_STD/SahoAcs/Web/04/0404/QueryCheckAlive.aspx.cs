using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SMS_Communication;
using SahoAcs.DBModel;
using System.Configuration;


namespace SahoAcs.Web._04._0404
{
    public partial class QueryCheckAlive : System.Web.UI.Page
    {
        public OrmDataObject od_saho = new OrmDataObject("MsSql", string.Format(Pub.db_connection_template,
          Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));

        List<EquDataModel> EquDatas = new List<EquDataModel>();

        protected void Page_Load(object sender, EventArgs e)
        {
            string js = @"<script src='/scripts/vue.min.js'></script>";
            js += "<script src='QueryCheckAlive.js?+"+Pub.GetNowTime+"'></script>"; 
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);                        
            if (Request["QueryConn"] != null)
            {
                if (Request["QueryConn"].Equals("Send"))
                {
                    if (Session["SahoWebSocket"] != null)
                    {
                        //((SahoWebSocket)Session["SahoWebSocket"]).ClearCmdResult();
                    }
                    //string equs = Request["EquList[]"];
                    //this.QueryEquConn();
                }
                else if(Request["QueryConn"].Equals("Get"))
                {
                    this.GetResult();
                }
                else
                {
                    this.GetDefault();
                }
            }
        }//end page_load

        private void QueryEquConn()
        {
            if (Request["EquList[]"] != null && Request["EquList[]"].ToString().Split(',').Length > 0)
            {
                this.EquDatas = this.od_saho.GetQueryResult<EquDataModel>("SELECT * FROM V_McrInfo WHERE EquID IN @ids",
              new { ids = Request["EquList[]"].ToString().Split(',') }).ToList();
                string sendConnStr = "";
                foreach (var s in this.EquDatas)
                {
                    if (!string.IsNullOrEmpty(sendConnStr))
                    {
                        sendConnStr += ";";
                    }
                    sendConnStr += string.Format("{0}@{1}@{2}@", s.EquID, s.EquNo, "CheckAlive");
                }
                this.SendAppCmdStrList(sendConnStr);
            }
            //this.GetResult();
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { equ_lists = this.EquDatas.OrderByDescending(i => i.EquDesc).ToList(),message="OK" }));
            Response.End();

        }


        private void SendAppCmdStrList(string sCmdStrList)
        {
            this.EquDatas = this.od_saho.GetQueryResult<EquDataModel>("SELECT * FROM V_McrInfo").ToList();
            string[] sIPArray = null, sAppCmdStrArray = null;
            SahoWebSocket oSWSocket = null;
            try
            {
                if (!string.IsNullOrEmpty(sCmdStrList))
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
                        ((SahoWebSocket)Session["SahoWebSocket"]).UserID = Session["UserID"].ToString();
                        ((SahoWebSocket)Session["SahoWebSocket"]).SourceIP = sIPAddress;
                        ((SahoWebSocket)Session["SahoWebSocket"]).SCListenIPPort = System.Web.Configuration.WebConfigurationManager.AppSettings["SCListenIPPort"].ToString();
                        ((SahoWebSocket)Session["SahoWebSocket"]).DbConnectionString = Pub.GetConnectionString(Pub.sConnName);
                        ((SahoWebSocket)Session["SahoWebSocket"]).Start();
                        #endregion
                    }
                    #endregion

                    #region 傳送APP指令字串
                    oSWSocket = (SahoWebSocket)Session["SahoWebSocket"];
                    string BeType = "0";
                    if (ConfigurationManager.AppSettings["LanguageOption"] != null && new string[] { "1", "0" }.Contains(ConfigurationManager.AppSettings["LanguageOption"]))
                    {
                        BeType = System.Configuration.ConfigurationManager.AppSettings["LanguageOption"];
                    }
                    oSWSocket.SetBECommTag(BeType);
                    oSWSocket.ClearCmdResult();
                    sAppCmdStrArray = sCmdStrList.Split(';');
                    for (int i = 0; i < sAppCmdStrArray.Length; i++)
                    {
                        oSWSocket.SendAppCmdStr(sAppCmdStrArray[i]);
                    }
                    #endregion
                }
            }
            finally
            {
                sIPArray = null;
                oSWSocket = null;
                sAppCmdStrArray = null;
            }
        }

        private void GetDefault()
        {
            try
            {
                this.EquDatas = this.od_saho.GetQueryResult<EquDataModel>(@"SELECT A.*,B.EquAliveState AS EquDesc,EquVarStateTime AS EquResult FROM 
	                B01_EquData A
	                INNER JOIN V_MEAliveState B ON A.EquID=B.EquID").ToList();
            }
            catch(Exception ex)
            {
            
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { equ_lists = this.EquDatas}));
            Response.End();
        }


        private void GetResult()
        {
            string message = "";
            List<B00Sysdeviceoplog> oplogs = new List<DBModel.B00Sysdeviceoplog>();
            try
            {
                this.EquDatas = this.od_saho.GetQueryResult<EquDataModel>("SELECT * FROM V_McrInfo WHERE 1=1 ").ToList();
                if (Request["EquList[]"] != null && Request["EquList[]"].ToString().Split(',').Length > 0)
                {                    
                    var eququery = this.od_saho.GetQueryResult<EquDataModel>("SELECT * FROM V_McrInfo WHERE EquID IN @ids",
                        new { ids = Request["EquList[]"].ToString().Split(',') }).ToList();
                    message = "Query...";
                    oplogs = this.od_saho.GetQueryResult<B00Sysdeviceoplog>(@"SELECT TOP 100 * FROM B00_SysDeviceOPLog 
                                                                                                    WHERE DOPActive='CheckAlive' AND EquNo IN @nos
                                                                                                    order by RecordID DESC ",
                                                                                                    new {nos= eququery.Select(i=>i.EquNo),UserID=Session["UserID"].ToString()}).ToList();
                    
                    foreach (var s in this.EquDatas)
                    {
                        if (eququery.Select(i=>i.EquID).Contains(s.EquID)&& oplogs.Where(i =>Convert.ToString(i.EquNo) == s.EquNo).Count() > 0)
                        {
                            if (Convert.ToString(oplogs.Where(i => i.EquNo == s.EquNo).First().DOPState) == "0")
                            {
                                s.EquDesc = "失敗";
                            }
                            else
                            {
                                s.EquDesc = "成功";
                            }
                            //s.EquDesc = Convert.ToString(oplogs.Where(i => Convert.ToString(i.EquNo) == s.EquNo).First().DOPState);
                            s.EquResult = Convert.ToString(oplogs.Where(i => Convert.ToString(i.EquNo) == s.EquNo).First().ResultMsg);                            
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                message += ex.Message;
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { equ_lists = this.EquDatas, message = message}));
            Response.End();
        }


        public class EquDataModel
        {
            public int EquID { get; set; }
            public string EquName { get; set; }
            public string EquEName { get; set; }
            public string EquClass { get; set; }
            public string EquModel { get; set; }
            public string Floor { get; set; }
            public string Building { get; set; }
            public string EquNo { get; set; }
            public string EquDesc { get; set; }
            public string EquResult { get; set; }
            public int IsAndTrt { get; set; }
        }



    }//end class
}//end namespace