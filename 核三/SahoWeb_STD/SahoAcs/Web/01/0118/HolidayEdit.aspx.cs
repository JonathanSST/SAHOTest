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
using SahoAcs;
using SahoAcs.DBClass;

namespace SahoAcs.Web._01._0118
{
    public partial class HolidayEdit : BasePage
    {

        public List<ItemList> CardTypes = new List<ItemList>();
        public List<dynamic> CardAuth = new List<dynamic>();
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        static GlobalMsg gms = new GlobalMsg();
        public B03PsnHoliday HolidayEntity = new B03PsnHoliday() { LicType = "58", LicNo = "0000", Hours="0", Daily="1", Minutes="0" };
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<dynamic> VacList = new List<dynamic>();
        public int CardLen = 0;
        public string ErrMsg = "";
        public string PsnNo = "", PsnName = "";


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.MainStartTime.DateValue = DateTime.Now.ToString("yyyy/MM/dd 09:00:00");
            this.MainEndTime.DateValue = DateTime.Now.ToString("yyyy/MM/dd 16:00:00");
            this.VacList = this.odo.GetQueryResult("SELECT * FROM B00_VacationData ORDER BY VNo").ToList();
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            this.PsnNo = "";
            
            if (string.IsNullOrEmpty(this.PsnNo))
            {
                
            }
            this.HolidayEntity.PsnNo = PsnNo;            
            this.PsnName = Sa.Web.Fun.GetSessionStr(this, "UserName");
            this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"SELECT A.* FROM B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID", new { UserID }).OrderBy(i => i.PsnName).ToList();            
            /*
            dynamic auth1 = new System.Dynamic.ExpandoObject();
            auth1.Name = "有效";
            auth1.CardAuthAllow = 1;
            this.CardAuth.Add(auth1);
            auth1 = new System.Dynamic.ExpandoObject();
            auth1.Name = "無效";
            auth1.CardAuthAllow = 0;
            this.CardAuth.Add(auth1);
            */

            if (Request["DoAction"] != null)
            {
                if(Request["DoAction"]=="Edit")
                {
                    this.SetEdit();
                }
                if (Request["DoAction"] == "Delete")
                {
                    this.SetDelete();
                }
                if (Request["DoAction"] == "Save")
                {
                    this.SetSaveData();
                }



            }

        }


        private void SetDelete()
        {
            //Request.UrlReferrer;
            this.odo.Execute("DELETE B03_PsnHoliday WHERE RecordID=@RecordID", new { RecordID = Request["RecordID"] });
            string ErrMsg = this.odo.DbExceptionMessage;
            Pub.MessageObject sRet = new Pub.MessageObject()
            {
                result = this.odo.isSuccess,
                act = "Delete",
                message = ErrMsg
            };           
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }

        private void SetEdit()
        {
            //取得目前的卡號及卡別資訊    
            if (Request.Form["RecordID"] != null && Request.Form["RecordID"] == "0")
            {
                this.HolidayEntity.RecordID = 0;
            }
            if(Request.Form["RecordID"] != null && Request.Form["RecordID"] != "0")
            {
                this.HolidayEntity = this.odo.GetQueryResult<B03PsnHoliday>(@"SELECT H.*,P.PsnName FROM B03_PsnHoliday H INNER JOIN B01_Person P ON H.PsnNo=P.PsnNo WHERE RecordID=@RecordID", new {RecordID=Request.Form["RecordID"]}).First();
                this.MainStartTime.DateValue = this.HolidayEntity.StartTime.ToString(CommFormat.DateTimeF);
                this.MainEndTime.DateValue = this.HolidayEntity.EndTime.ToString(CommFormat.DateTimeF);
            }
        }




        /// <summary>
        /// 儲存休假資訊
        /// </summary>
        private void SetSaveData()
        {
            Pub.MessageObject sRet = new Pub.MessageObject() { result = false };
            List<string> ColInfo = new List<string>();
            string CmdStr = "", ErrMsg = "";
            ColInfo.Add("RecordID");
            ColInfo.Add("PsnNo");
            ColInfo.Add("LicType");
            ColInfo.Add("HoliNo");
            ColInfo.Add("OrgNo");
            ColInfo.Add("Daily");
            ColInfo.Add("Hours");
            ColInfo.Add("Minutes");
            ColInfo.Add("StartTime");
            ColInfo.Add("EndTime");
            try
            {
                var para = this.GetMasterPackage(ColInfo);
                this.HolidayEntity = this.DictionaryToObject<B03PsnHoliday>(para);
                string sqlcmd = @"SELECT Org.* FROM(
                                    SELECT * FROM OrgStrucAllData('Unit')
                                    WHERE OrgNo<>''
                                    UNION
                                    SELECT * FROM OrgStrucAllData('Department')
                                    WHERE OrgNo<>''
                                    ) AS Org INNER JOIN B01_Person AS P ON Org.OrgStrucID=P.OrgStrucID WHERE PsnNo=@PsnNo";
                foreach (var o in this.odo.GetQueryResult<OrgStrucInfo>(sqlcmd, new { PsnNo=this.GetFormEqlValue("PsnNo")}))
                {
                    this.HolidayEntity.OrgNo = o.OrgNo;
                }
                //this.HolidayEntity.OrgNo
                if (this.odo.GetQueryResult("SELECT * FROM B03_PsnHoliday WHERE RecordID<>@RecordID AND PsnNo=@PsnNo AND ((@StartTime BETWEEN StartTime AND EndTime) OR (@EndTime BETWEEN StartTime AND EndTime))", this.HolidayEntity).Count() > 0)
                {
                    sRet.result = false;
                    ErrMsg = " 休假起訖時間重複!!，請進行修改";
                }
                else
                {
                    if (this.HolidayEntity.RecordID == 0)
                    {
                        CmdStr = @"INSERT INTO B03_PsnHoliday (PsnNo,LicType,licNo,HoliNo,OrgNo,Daily,Hours,Minutes,StartTime,EndTime,CreateUserID,CreateTime,UpdateUserID,UpdateTime,KeyIn) 
                                                        VALUES (@PsnNo,@LicType,@LicNo,@HoliNo,@OrgNo,@Daily,@Hours,@Minutes,@StartTime,@EndTime,@CreateUserID,GETDATE(),@UpdateUserID,GETDATE(),@CreateUserID)";
                    }
                    else
                    {
                        CmdStr = "UPDATE B03_PsnHoliday SET HoliNo=@HoliNo,OrgNo=@OrgNo,Daily=@Daily,Hours=@Hours,Minutes=@Minutes,StartTime=@StartTime,EndTime=@EndTime,UpdateTime=GETDATE(),UpdateUserID=@UpdateUserID WHERE RecordID=@RecordID";
                    }
                    int daily, hour=0, min = 0;
                    TimeSpan span = new TimeSpan(this.HolidayEntity.EndTime.Ticks - this.HolidayEntity.StartTime.Ticks);
                    DateTime ds = this.HolidayEntity.StartTime;
                    DateTime de = this.HolidayEntity.EndTime;
                    daily = new TimeSpan(de.Ticks - ds.Ticks).Days;
                    if (daily >= 1)
                    {
                        if (ds.Hour <= 9 && de.Hour >= 15)
                        {
                            daily++;
                        }
                        else if(ds.Hour<=9 && de.Hour<=12)
                        {
                            hour = 4;
                        }
                        else if (de.Hour <= 16 && ds.Hour >= 12)
                        {
                            hour = 4;
                        }
                    }
                    else
                    {
                        if (ds.Hour <= 9 && de.Hour >= 15)
                        {
                            daily++;
                        }
                        else if (ds.Hour <= 9 && de.Hour <= 12)
                        {
                            hour = 4;
                        }
                        else if (de.Hour <= 16 && ds.Hour >= 12)
                        {
                            hour = 4;
                        }
                    }
                    this.HolidayEntity.Daily = this.GetCheckNumber(this.HolidayEntity.Daily);//daily.ToString();
                    this.HolidayEntity.Hours = this.GetCheckNumber(this.HolidayEntity.Hours);  //hour.ToString();
                    this.HolidayEntity.LicNo = "0000";
                    this.HolidayEntity.LicType = "58";
                    this.HolidayEntity.CreateUserID = Sa.Web.Fun.GetSessionStr(this,"UserID");
                    this.HolidayEntity.UpdateUserID = Sa.Web.Fun.GetSessionStr(this,"UserID");
                    this.odo.Execute(CmdStr, this.HolidayEntity);
                    ErrMsg = this.odo.DbExceptionMessage;
                    sRet.result = this.odo.isSuccess;
                }              
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                sRet.result = false;
            }
            sRet.act = "Save";
            sRet.message = ErrMsg;
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }

    }//end page class
}//end namespace