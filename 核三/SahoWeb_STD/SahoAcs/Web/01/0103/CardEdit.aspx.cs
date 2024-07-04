using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;

namespace SahoAcs.Web._01._0103
{
    public partial class CardEdit : Sa.BasePage
    {

        public List<ItemList> CardTypes = new List<ItemList>();
        public List<dynamic> CardAuth = new List<dynamic>();
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        static GlobalMsg gms = new GlobalMsg();
        public CardEntity CardObj = new CardEntity();
        public int CardLen = 0;
        public string ErrMsg = "";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.CardSTime.DateValue = DateTime.Now.ToString(CommFormat.DateTimeF);
            //設為預設的到期日
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='DefaultEndDateTime' "))
            {
                this.CardETime.DateValue = Convert.ToString(o.ParaValue) + " 23:59:59";
            }            

            dynamic auth1 = new System.Dynamic.ExpandoObject();
            auth1.Name = "有效";
            auth1.CardAuthAllow = 1;
            this.CardAuth.Add(auth1);
            auth1 = new System.Dynamic.ExpandoObject();
            auth1.Name = "無效";
            auth1.CardAuthAllow = 0;
            this.CardAuth.Add(auth1);

            if (Request["DoAction"] != null)
            {
                if (Request["DoAction"] == "add")
                {
                    this.SetNewUI();
                }
                if(Request["DoAction"]=="update")
                {
                    this.SetEdit();
                }
                if (Request["DoAction"] == "delete")
                {
                    this.SetDelete();
                }
                if (Request["DoAction"] == "Save")
                {
                    this.SetSaveCard();
                }



            }

        }


        private void SetDelete()
        {
            //Request.UrlReferrer;
            foreach(var o in this.odo.GetQueryResult<CardEntity>("SELECT * FROM B01_Card WHERE CardID=@CardID",new { CardID = Request["CardID"] }))
            {
                this.odo.Execute("UPDATE B01_CardAuth SET OpMode='Del',OpStatus='',ErrCnt=0 WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B01_CardEquAdj WHERE CardID=@CardID", o);
                this.odo.Execute("DELETE B01_CardEquGroup WHERE CardID=@CardID", o);
                this.odo.Execute("DELETE B01_CardExt WHERE CardID=@CardID", o);
                this.odo.Execute("DELETE B01_Card WHERE CardID=@CardID", o);
                this.odo.Execute("DELETE B02_EyeData WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FaceData WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FPData WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FaceData2 WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FaceImageData WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FaceImageData2 WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FVPData WHERE CardNo=@CardNo", o);
                var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料刪除, Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this,"UserName"), "0103");
                log.LogInfo = string.Format("Delete Card CardNo='{0}'", o.CardNo);
                log.LogDesc = "Delete Card ";
                this.odo.SetSysLogCreate(log);
                //處理預設權限相關
                this.odo.SetOrgEquData(this.CardObj.CardNo, this.CardObj.PsnID, Session["UserID"].ToString(), gms);
            }
            
        }

        private void SetEdit()
        {
            //取得目前的卡號及卡別資訊    
            this.CardObj = this.odo.GetQueryResult<CardEntity>("SELECT * FROM B01_Card WHERE CardID=@CardID", new { CardID = Request["CardID"] }).First();
            this.CardTypes = this.odo.GetQueryResult<ItemList>("SELECT * FROM B00_ItemList WHERE ItemClass='CardType' AND ItemNo=@ItemNo ORDER BY ItemNo ",new {ItemNo=this.CardObj.CardType}).ToList();
            //this.CardObj.CardETime = "";
            //this.CardObj.CardSTime = "";
            this.CardSTime.DateValue = this.CardObj.CardSTime.ToString(CommFormat.DateTimeF);
            this.CardETime.DateValue = this.CardObj.CardETime.Value.ToString(CommFormat.DateTimeF);
            this.CardLen = this.CardObj.CardNo.Length;
        }

        private void SetNewUI()
        {
            var CardDef= DongleVaries.GetCardType(); 
            this.CardTypes = this.odo.GetQueryResult<ItemList>(@"SELECT * FROM B00_ItemList WHERE ItemClass='CardType' AND ItemNo NOT IN ('R','TEMP') AND ItemNo IN @CardDef
                AND ItemNo NOT IN (SELECT CardType FROM B01_Card WHERE PsnID=@PsnID)ORDER BY ItemNo ", new { ItemNo = this.CardObj.CardType,PsnID=Request["PsnID"], CardDef = CardDef.Select(i=>i.ItemNo)}).ToList();
            
            CardTypes = CardTypes.Where(i => CardDef.Select(c => c.ItemNo).Contains(i.ItemNo)).ToList();   //取得目前可用的卡片版本
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            var MgnList = this.odo.GetQueryResult(@"SELECT A.*,MgaNo from B00_SysUserMgns A
                                                                                    INNER JOIN B00_ManageArea B ON A.MgaID=B.MgaID WHERE UserID=@UserID", new {  UserID }).ToList();
            //若管理區沒有全區
            if (MgnList.Where(i => Convert.ToString(i.MgaNo).Equals("M999")).Count() == 0)
            {                
                this.CardTypes = this.CardTypes.Where(i => i.ItemNo.Equals("T")).ToList();
            }
           
            this.CardObj.CardAuthAllow = 1;
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='CardLen' "))
            {
                this.CardLen = int.Parse(Convert.ToString(o.ParaValue));
            }
        }



        private void SetCardData()
        {
            if (Request["CardID"] == "0")
            {
                this.CardObj.CardNo = Request["popCardNo"];
            }
            else
            {
                this.CardObj.CardNo = Request["NowCardNo"];
            }
            string PreStr = "pop";
            this.CardObj.CardPW = Request[PreStr + "CardPW"];
            this.CardObj.CardSTime = DateTime.Parse(Request[Request.Form.AllKeys.Where(i => i.Contains("CardSTime")).First()]);
            this.CardObj.CardETime = DateTime.Parse(Request[Request.Form.AllKeys.Where(i => i.Contains("CardETime")).First()]);
            this.CardObj.CardAuthAllow = int.Parse(Request["popCardAuthAllow"]);
            this.CardObj.CardVer = Request[PreStr + "CardVer"];
            this.CardObj.CardType = Request[PreStr + "CardType"];
            this.CardObj.PsnID = int.Parse(Request["PsnID"]);
            this.CardObj.CardSerialNo = Request[PreStr + "CardSerialNo"];
            this.CardObj.CardNum = Request[PreStr + "CardNum"];
            this.CardObj.CreateTime = DateTime.Now;
            this.CardObj.UpdateTime = DateTime.Now;
            this.CardObj.CreateUserID = Session["UserID"].ToString();
            this.CardObj.UpdateUserID = Session["UserID"].ToString();
            this.CardObj.CardDesc = Request["popCardDesc"];
            this.CardObj.CardID = int.Parse(Request["CardID"]);
        }

        private void SetSaveCard()
        {
            this.SetCardData();
            if (Request["CardID"] == "0")
            {
                if(this.odo.GetQueryResult("SELECT CardID FROM B01_Card WHERE CardNo = @CardNo OR (PsnID=@PsnID AND CardType=@CardType)",this.CardObj).Count() > 0)
                {
                    this.ErrMsg = "卡號或卡別重複定義";
                }
                else
                {
                    this.odo.Execute(@"INSERT INTO B01_Card(CardNo, CardVer, CardPW, CardSerialNo, CardNum,
                        CardType, PsnID, CardAuthAllow, CardSTime, CardETime, CardDesc, CreateUserID,
                        UpdateUserID, UpdateTime) VALUES (@CardNo, @CardVer, @CardPW, @CardSerialNo, @CardNum,
                        @CardType, @PsnID, @CardAuthAllow, @CardSTime, @CardETime, @CardDesc, @CreateUserID,
                        @UpdateUserID, @UpdateTime)",this.CardObj);
                    this.ErrMsg = this.odo.DbExceptionMessage;
                    var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料新增, Sa.Web.Fun.GetSessionStr(this, "UserID"), "", "0103");
                    log.LogInfo = string.Format("ADD Card CardNo='{0}'", this.CardObj.CardNo);
                    log.LogDesc = "INSERT Card ";
                    this.odo.SetSysLogCreate(log);
                    //處理預設權限相關
                    this.odo.SetOrgEquData(this.CardObj.CardNo,this.CardObj.PsnID,Session["UserID"].ToString(),gms);
                }
            }
            else
            {
                this.odo.Execute(@"UPDATE B01_Card SET CardNo = @CardNo, CardVer = @CardVer, CardPW = @CardPW, CardSerialNo = @CardSerialNo,
                        CardNum = @CardNum, CardType = @CardType, CardAuthAllow = @CardAuthAllow, CardSTime = @CardSTime, CardETime = @CardETime,
                        CardDesc = @CardDesc, UpdateUserID = @UpdateUserID, UpdateTime = @UpdateTime WHERE CardID = @CardID;",this.CardObj);
                this.ErrMsg = this.odo.DbExceptionMessage;
                //處理預設權限相關
                var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料修改, Sa.Web.Fun.GetSessionStr(this, "UserID"), "", "0103");
                log.LogInfo = string.Format("Updata Card CardNo='{0}'", this.CardObj.CardNo);
                log.LogDesc = "Update Card ";
                this.odo.SetSysLogCreate(log);
                this.odo.SetOrgEquData(this.CardObj.CardNo, this.CardObj.PsnID, Session["UserID"].ToString(), gms);
            }
            //處理權限重整相關

        }

    }//end page class
}//end namespace