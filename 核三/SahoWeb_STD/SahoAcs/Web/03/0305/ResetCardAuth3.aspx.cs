using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Script.Serialization;

using DapperDataObjectLib;
using SahoAcs.DBModel;



namespace SahoAcs.Web
{
    public partial class ResetCardAuth3 : System.Web.UI.Page
    {
        public List<CardTypeModel> CardTypeList = new List<CardTypeModel>();
        


        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] == null)
            {                
                this.SetCardType();
            }
            else
            {
                if (Request["PageEvent"] == "Reset")
                {
                    this.SetCardAuth();
                }
            }
            
            
            
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("EvoList", "ResetAuth.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
        }



        private void SetCardAuth()
        {
            string CardType = Request["CardType"];
            var CardList = this.od.GetQueryResult("SELECT CardNo AS ScheduleInfo,'CardAuthUpdate' AS ScheduleName,5 AS OrderLv FROM B01_Card WHERE CardType=@CardType AND CardAuthAllow=1 ",new { CardType = CardType });
            //取得送出新增至排程的數據資料
            var count = this.od.Execute("INSERT INTO B00_Schedule (ScheduleInfo,ScheduleName,OrderLv) VALUES (@ScheduleInfo,@ScheduleName,@OrderLv)", CardList);
            //取得待處理的數據資料
            var count2 = this.od.GetQueryResult("SELECT * FROM B00_Schedule INNER JOIN B01_Card ON CardNo=ScheduleInfo AND ExecuteTime IS NULL AND ScheduleName='CardAuthUpdate' AND CardType=@CardType AND CardAuthAllow=1"
                , new { CardType = CardType }).Count();
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true, proc_count=count, ready_count=count2 }));
            Response.End();
        }

        


        private void SetCardType()
        {
            this.CardTypeList = this.od.GetQueryResult<CardTypeModel>(@"SELECT ItemNo AS CardTypeNo, ItemName AS CardTypeName 
                                                    ,(SELECT COUNT(*) FROM B01_Card WHERE CardType=itemNo AND CardAuthAllow=1) CardAmt
                                                    ,(SELECT COUNT(*) FROM B01_Card INNER JOIN B00_Schedule ON CardNo=ScheduleInfo AND CardType=itemNo AND CardAuthAllow=1 AND ScheduleName='CardAuthUpdate' AND ExecuteTime IS NULL) WaitCount
                                                    FROM B00_ItemList WHERE ItemClass = 'CardType' AND ItemNo<>'R' AND ItemInfo1 = 'Default' ORDER BY ItemOrder ").ToList();
        }

    }//end class
}//end namespace