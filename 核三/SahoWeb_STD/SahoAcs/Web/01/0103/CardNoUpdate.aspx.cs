﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.Web._01._0103
{
    public partial class CardNoUpdate : Sa.BasePage
    {
        OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());
        public string card_id="";
        public string OldCardNo = "";
        public string NewCardNo = "";
        public string CardName = "";
        public int CardLength = 10;
        public string ErrorMessage = "";
        string SysLogCmd = @"INSERT INTO B00_SysLog (LogTime,LogType,UserID,UserName,EquNo,EquName,LogFrom,LogInfo,LogIP,LogDesc) 
                                                            VALUES (GETDATE(),@LogType,@UserID,@UserName,@EquNo,@EquName,@LogFrom,@LogInfo,@LogIP,@LogDesc) ";

        public CardEntity cardentity = new CardEntity();
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Save")
            {
                Response.Clear();
                this.SetChangeCard();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                   new { message = "OK",ErrorMessage=this.ErrorMessage }));
                Response.End();
            }
            else
            {
                this.QueryDefault();
            }
        }//end page_load

        private void SetChangeCard()
        {
            var result=this.odo.GetQueryResult("SELECT * FROM B01_Card WHERE CardNo=@NewCardNo", new { NewCardNo = Request["NewCardNo"] });
            
            if (result.Count() == 0)
            {
                var PsnCardData = this.odo.GetQueryResult("SELECT * FROM V_PsnCard WHERE CardID=@CardID",new {CardID=Request["CardID"]});
                SysLogEntity syslog_dto = new SysLogEntity();
                string PsnName = "", PsnNo = "";
                foreach(var o in PsnCardData)
                {
                    syslog_dto.LogType = DB_Acs.Logtype.卡片權限調整.ToString();
                    syslog_dto.UserID = Session["UserId"].ToString();
                    syslog_dto.UserName = Session["UserName"].ToString();
                    syslog_dto.LogIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    syslog_dto.LogFrom = "0103";                    
                    syslog_dto.LogDesc = "人員基本資料，卡號變更";
                    PsnName = Convert.ToString(o.PsnName);
                    PsnNo = Convert.ToString(o.PsnNo);
                }               
                this.odo.Execute("UPDATE B01_Card SET CardAuthAllow=0 WHERE CardID=@CardID", new { CardID = Request["CardID"] });
                odo.Execute(@"EXEC CardAuth_Update @sCardNo = @CardNo,@sUserID = @UserID,
                                                        @sFromProc = @UserID,@sFromIP = '127.0.0.1',@sOpDesc = '一般權限重整'", new { CardNo = Request["CardNo"], UserID = Session["UserId"].ToString() });
                syslog_dto.LogInfo = string.Format("人員卡片變更(人員:{0}_{1}_{2}_停用門禁權限",PsnNo,PsnName,Request["CardNo"]);
                this.odo.Execute(this.SysLogCmd, syslog_dto);
                this.odo.Execute(@"UPDATE B01_Card SET 
                                    UpdateTime=GETDATE(),UpdateUserID=@UserID,CardAuthAllow=1,CardNo=@CardNo WHERE CardID=@CardID", 
                                    new {CardNo=Request["NewCardNo"], CardID = Request["CardID"],UserID=Session["UserID"] });
                this.odo.Execute(@"UPDATE B02_FPData SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                                    new { NewCardNo = Request["NewCardNo"],CardNo=Request["CardNo"] });
                this.odo.Execute(@"UPDATE B02_FaceData SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                    new { NewCardNo = Request["NewCardNo"], CardNo = Request["CardNo"] });
                this.odo.Execute(@"UPDATE B02_FaceData2 SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                   new { NewCardNo = Request["NewCardNo"], CardNo = Request["CardNo"] });
                this.odo.Execute(@"UPDATE B02_FVPData SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                    new { NewCardNo = Request["NewCardNo"], CardNo = Request["CardNo"] });
                this.odo.Execute(@"UPDATE B02_FaceImageData SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                    new { NewCardNo = Request["NewCardNo"], CardNo = Request["CardNo"] });
                this.odo.Execute(@"UPDATE B02_FaceImageData2 SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                    new { NewCardNo = Request["NewCardNo"], CardNo = Request["CardNo"] });
                //20210412 update
                this.odo.Execute(@"UPDATE B02_FaceData3 SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                    new { NewCardNo = Request["NewCardNo"], CardNo = Request["CardNo"] });
                this.odo.Execute(@"UPDATE B02_FPData_F2 SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                    new { NewCardNo = Request["NewCardNo"], CardNo = Request["CardNo"] });
                this.odo.Execute(@"UPDATE B02_FaceImageData3 SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                    new { NewCardNo = Request["NewCardNo"], CardNo = Request["CardNo"] });
                //20210535 update
                this.odo.Execute(@"UPDATE B02_HandData SET 
                                    CardNo=@NewCardNo WHERE CardNo=@CardNo",
                    new { NewCardNo = Request["NewCardNo"], CardNo = Request["CardNo"] });


                this.odo.Execute(@"EXEC CardAuth_Update @sCardNo = @CardNo,@sUserID = @UserID,
                                                        @sFromProc = @UserID,@sFromIP = '127.0.0.1',@sOpDesc = '一般權限重整'", new { CardNo = Request["NewCardNo"], UserID = Session["UserId"].ToString() });
                syslog_dto.LogInfo = string.Format("人員卡片變更(人員:{0}_{1}_{2}_啟用門禁權限", PsnNo, PsnName, Request["NewCardNo"]);
                this.odo.Execute(this.SysLogCmd, syslog_dto);
                syslog_dto.LogType = DB_Acs.Logtype.卡號變更.ToString();
                syslog_dto.LogInfo = string.Format("PsnNo={0},PsnName={1},CardNo={2},NewCardNo={3}",PsnNo,PsnName,Request["CardNo"],Request["NewCardNo"]);
                this.odo.Execute(this.SysLogCmd, syslog_dto);
            }
            else
            {
                this.ErrorMessage = "卡號重複使用!!";
            }
        }


        private void QueryDefault()
        {
            card_id = Request["card_id"];
            var card_info = this.odo.GetQueryResult<CardEntity>(@"SELECT 
	                                                                                                                            C.*,ItemName AS CardTypeName
                                                                                                                            FROM 
	                                                                                                                            B01_Card AS C
	                                                                                                                            INNER JOIN B00_ItemList ON CardType=ItemNo AND ItemClass='CardType' WHERE CardID=@CardID AND CardAuthAllow=1 ",
                                                                                                                            new { CardID = card_id }).ToList();
            foreach (var o in card_info)
            {
                this.cardentity = o;
            }
            var resultpara = this.odo.GetQueryResult(@"SELECT TOP 1 
                CASE ISNUMERIC(ItemInfo2) WHEN 0 THEN '8' ELSE ItemInfo2 END  AS ItemInfo2
                , ItemNo FROM B00_ItemList WHERE ItemClass = 'CardType' AND ItemNo=@CardType",this.cardentity);
            foreach (var data in resultpara)
            {
                this.CardLength = Convert.ToInt32(data.ItemInfo2);
            }
        }

    }//end class
}//end namespace