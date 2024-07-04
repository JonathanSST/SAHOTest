using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using DapperDataObjectLib;
using SMS_Communication;


namespace SahoAcs.Web._04._0406
{
    public partial class QueryCtrlEquLogCount : System.Web.UI.Page
    {
        public OrmDataObject od_saho = new OrmDataObject("MsSql", string.Format(Pub.db_connection_template,
          Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));

        List<EquDataModel> EquDatas = new List<EquDataModel>();

        protected void Page_Load(object sender, EventArgs e)
        {
            string js = @"<script src='/scripts/vue.min.js'></script>
                        <script src='QueryCtrlEquLogCount.js'></script>"; 
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);                        
            if (Request["QueryConn"] != null)
            {
                if (Request["QueryConn"].Equals("Send"))
                {
                    if (Session["SahoWebSocket"] != null)
                    {
                        ((SahoWebSocket)Session["SahoWebSocket"]).ClearCmdResult();
                    }
                    this.QueryEquConn();
                }
                else
                {
                    this.GetResult();
                }
            }
        }//end page_load

        private void QueryEquConn()
        {
            this.EquDatas = this.od_saho.GetQueryResult<EquDataModel>("SELECT * FROM B01_EquData").ToList();
            string sendConnStr = "";
            foreach (var s in this.EquDatas)
            {
                if (!string.IsNullOrEmpty(sendConnStr))
                {
                    sendConnStr += ";";
                }
                sendConnStr += string.Format("{0}@{1}@{2}@", s.EquID, s.EquNo, "CardLogCountGetSP");
            }
            this.SendAppCmdStrList(sendConnStr);
            this.GetResult();
        }


        private void SendAppCmdStrList(string sCmdStrList)
        {
            this.EquDatas = this.od_saho.GetQueryResult<EquDataModel>("SELECT * FROM B01_EquData").ToList();
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

        private void GetResult()
        {
            string sEquID = "", sEquNo = "";
            SahoWebSocket oSWSocket = null;
            try
            {
                oSWSocket = (SahoWebSocket)Session["SahoWebSocket"];
                this.EquDatas = this.od_saho.GetQueryResult<EquDataModel>("SELECT * FROM B01_EquData").ToList();
                
                foreach (var s in this.EquDatas)
                {                    
                    sEquID = s.EquID.ToString();
                    sEquNo = s.EquNo.ToString();                    

                    #region 更新GridView控制項的指令執行狀態和結果

                    if ((oSWSocket != null) && oSWSocket.EquNoRecordIDHashtable.ContainsKey(sEquNo))
                    {
                        this.EquDatas.Where(i=>i.EquNo==sEquNo).First().EquDesc = 
                            ((SahoWebSocketCmdResult)oSWSocket.EquNoRecordIDHashtable[sEquNo]).CmdStateDesc;
                        this.EquDatas.Where(i => i.EquNo == sEquNo).First().EquResult =
                            ((SahoWebSocketCmdResult)oSWSocket.EquNoRecordIDHashtable[sEquNo]).ResultMsg;                        
                    }
                    #endregion                 
                }                 
            }
            catch
            {
                oSWSocket = null;
            }
            this.EquDatas.ForEach(i =>
            {
                if (i.EquDesc == null)
                {
                    i.EquDesc = "";
                }
            });
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { equ_lists = this.EquDatas.OrderByDescending(i => i.EquDesc).ToList() }));
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