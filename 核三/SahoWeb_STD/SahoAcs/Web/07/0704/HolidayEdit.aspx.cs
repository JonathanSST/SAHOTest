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
namespace SahoAcs.Web._07._0704
{
    public partial class HolidayEdit : BasePage
    {
        public List<ItemList> CardTypes = new List<ItemList>();
        public List<dynamic> CardAuth = new List<dynamic>();
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        static GlobalMsg gms = new GlobalMsg();
        public B03PsnHoliday HolidayEntity = new B03PsnHoliday() { LicType = "58", LicNo = "0000", Hours = "0", Daily = "1", Minutes = "0" };
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<dynamic> VacList = new List<dynamic>();
        public int CardLen = 0;
        public string ErrMsg = "";
        public string PsnNo = "", PsnName = "";
        string _Name = string.Empty;
        string _No = string.Empty;


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string WorkStartTime = "", WorkEndTime = "";
            string Para_Sql = " SELECT * FROM B00_SysParameter WHERE ParaClass = @ParaClass";
            List<SysParaData> ListParaData = this.odo.GetQueryResult<SysParaData>(Para_Sql, new { ParaClass = "LeaveDay" }).ToList();
            if (ListParaData.Count != 0)
            {
                WorkStartTime = ListParaData.Where(x => x.ParaNo == "WorkStartTime").FirstOrDefault().ParaValue.ToString();
                WorkEndTime = ListParaData.Where(x => x.ParaNo == "WorkEndTime").FirstOrDefault().ParaValue.ToString();
            }

            if (!string.IsNullOrEmpty(WorkStartTime))
            {
                this.MainStartTime.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " " + WorkStartTime;
            }
            else
            {
                this.MainStartTime.DateValue = DateTime.Now.ToString("yyyy/MM/dd 08:30:00");
            }
            if (!string.IsNullOrEmpty(WorkEndTime))
            {
                this.MainEndTime.DateValue = DateTime.Now.ToString("yyyy/MM/dd" + " " + WorkEndTime);
            }
            else
            {
                this.MainEndTime.DateValue = DateTime.Now.ToString("yyyy/MM/dd 17:30:00");
            }
            this.VacList = this.odo.GetQueryResult("SELECT * FROM B00_VacationData ORDER BY VNo").ToList();
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            this.PsnNo = "";

            this.HolidayEntity.PsnNo = PsnNo;
            this.PsnName = Sa.Web.Fun.GetSessionStr(this, "UserName");
            this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"SELECT A.* FROM B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID", new { UserID }).OrderBy(i => i.PsnName).ToList();

            if (UserID.Equals("User"))
            {
                string PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
                List<string> liSqlPara = new List<string>();
                Sa.DB.DBReader oReader = null;
                string sSqlCmd = @"SELECT B01_Person.PsnNo,B01_Person.PsnID, U.UserID, PsnName AS UserName, U.UserSTime, U.UserETime, U.PWChgTime, U.IsOptAllow
                    FROM B01_Person, (SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow
                    FROM B00_SysUser WHERE UserID = 'User') AS U
                    WHERE B01_Person.PsnID=?";
                liSqlPara.Add("S:" + PsnID);
                if (oAcsDB.GetDataReader(sSqlCmd, liSqlPara, out oReader))
                {
                    if (oReader.HasRows)
                    {
                        oReader.Read();
                        this.HolidayEntity.PsnName = oReader.ToString("UserName");
                        this.HolidayEntity.PsnNo = oReader.ToString("PsnNo");
                    }
                    oReader.Free();
                }
            }

            if (Request["DoAction"] != null)
            {
                if (Request["DoAction"] == "Edit")
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
            if (Request.Form["RecordID"] != null && Request.Form["RecordID"] != "0")
            {
                this.HolidayEntity = this.odo.GetQueryResult<B03PsnHoliday>(@"SELECT H.*,P.PsnName FROM B03_PsnHoliday H INNER JOIN B01_Person P ON H.PsnNo=P.PsnNo WHERE RecordID=@RecordID", new { RecordID = Request.Form["RecordID"] }).First();
                this.MainStartTime.DateValue = this.HolidayEntity.StartTime.ToString(CommFormat.DateTimeF);
                this.MainEndTime.DateValue = this.HolidayEntity.EndTime.ToString(CommFormat.DateTimeF);

            }
            
        }

        DataTable GetWorkDayData()
        {
            DataTable dtWorkDate = new DataTable();
            DataRow Datarow;
            dtWorkDate.Columns.Add("WorkDate", typeof(string));
            dtWorkDate.Columns.Add("WorkDay", typeof(string));
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = @" SELECT * FROM B00_SysParameter WHERE ParaNo='Workday' ";
            Sa.DB.DBReader dr;
            oAcsDB.GetDataReader(sql, out dr);
           
            if (dr.Read())
            {
                #region ProcessParaValue
                string ParaValueStr = dr.DataReader["ParaValue"].ToString();
                while (ParaValueStr.Length > 0)
                {
                    Datarow = dtWorkDate.NewRow();
                    string _WorkDate = DateTime.Now.Year + "/" + ParaValueStr.Substring(0, 2).ToString() + "/" + int.Parse(ParaValueStr.Substring(2, 2)).ToString();
                    Datarow["WorkDate"] = _WorkDate;
                    Datarow["WorkDay"] = ParaValueStr.Substring(4, 2).ToString();
                    dtWorkDate.Rows.Add(Datarow);
                    ParaValueStr = ParaValueStr.Substring(6);
                }
                #endregion
            }
            return dtWorkDate;
        }

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
                foreach (var o in this.odo.GetQueryResult<OrgStrucInfo>(sqlcmd, new { PsnNo = this.GetFormEqlValue("PsnNo") }))
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

                    #region mark
                    //int daily, hour = 0, min = 0;
                    //TimeSpan span = new TimeSpan(this.HolidayEntity.EndTime.Ticks - this.HolidayEntity.StartTime.Ticks);
                    //DateTime ds = this.HolidayEntity.StartTime;
                    //DateTime de = this.HolidayEntity.EndTime;
                    //daily = new TimeSpan(de.Ticks - ds.Ticks).Days;
                    //if (daily >= 1)
                    //{
                    //    if (ds.Hour <= 9 && de.Hour >= 15)
                    //    {
                    //        daily++;
                    //    }
                    //    else if (ds.Hour <= 9 && de.Hour <= 12)
                    //    {
                    //        hour = 4;
                    //    }
                    //    else if (de.Hour <= 16 && ds.Hour >= 12)
                    //    {
                    //        hour = 4;
                    //    }
                    //}
                    //else
                    //{
                    //    if (ds.Hour <= 9 && de.Hour >= 15)
                    //    {
                    //        daily++;
                    //    }
                    //    else if (ds.Hour <= 9 && de.Hour <= 12)
                    //    {
                    //        hour = 4;
                    //    }
                    //    else if (de.Hour <= 16 && ds.Hour >= 12)
                    //    {
                    //        hour = 4;
                    //    }
                    //}
                    #endregion

                    string WorkStartTime = "", WorkEndTime = "", BreakStartTime = "", BreakEndTime = "";
                    string Para_Sql = " SELECT * FROM B00_SysParameter WHERE ParaClass = @ParaClass";
                    List<SysParaData> ListParaData = this.odo.GetQueryResult<SysParaData>(Para_Sql, new { ParaClass = "LeaveDay" }).ToList();
                    if (ListParaData.Count != 0)
                    {
                        WorkStartTime = ListParaData.Where(x => x.ParaNo == "WorkStartTime").FirstOrDefault().ParaValue.ToString();
                        WorkEndTime = ListParaData.Where(x => x.ParaNo == "WorkEndTime").FirstOrDefault().ParaValue.ToString();
                        BreakStartTime = ListParaData.Where(x => x.ParaNo == "BreakStartTime").FirstOrDefault().ParaValue.ToString();
                        BreakEndTime = ListParaData.Where(x => x.ParaNo == "BreakEndTime").FirstOrDefault().ParaValue.ToString();
                    }

                    string strDate = DateTime.Now.ToString("yyyy/MM/dd");
                    string strDateStart = this.HolidayEntity.StartTime.ToString("yyyy/MM/dd"); //使用者填入的日期
                    string strDateEnd = this.HolidayEntity.EndTime.ToString("yyyy/MM/dd");  //使用者填入的日期
                    string strTimeStart = this.HolidayEntity.StartTime.ToString("HH:mm:00"); //使用者選擇的時間
                    string strTimeEnd = this.HolidayEntity.EndTime.ToString("HH:mm:00");  //使用者選擇的時間

                    //上下班時間設定、午休時間設定
                    TimeSpan WorkStart = new TimeSpan(DateTime.Parse(strDate + " " + WorkStartTime).Ticks); //上班時間
                    TimeSpan WorkEnd = new TimeSpan(DateTime.Parse(strDate + " " + WorkEndTime).Ticks);     //下班時間
                    TimeSpan BreakStart = new TimeSpan(DateTime.Parse(strDate + " " + BreakStartTime).Ticks);   //午休開始
                    TimeSpan BreakEnd = new TimeSpan(DateTime.Parse(strDate + " " + BreakEndTime).Ticks);     //午休結束

                    if (string.IsNullOrEmpty(WorkStartTime))
                    {
                        WorkStart = new TimeSpan(DateTime.Parse(strDate + " 08:30:00").Ticks);
                    }
                    if (string.IsNullOrEmpty(WorkEndTime))
                    {
                        WorkEnd = new TimeSpan(DateTime.Parse(strDate + " 17:30:00").Ticks);
                    }
                    if (string.IsNullOrEmpty(BreakStartTime))
                    {
                        BreakStart = new TimeSpan(DateTime.Parse(strDate + " 12:30:00").Ticks);
                    }
                    if (string.IsNullOrEmpty(BreakEndTime))
                    {
                        BreakEnd = new TimeSpan(DateTime.Parse(strDate + " 13:30:00").Ticks);
                    }
                    TimeSpan t1 = new TimeSpan(DateTime.Parse(strDateStart + " " + strTimeStart).Ticks);
                    TimeSpan t2 = new TimeSpan(DateTime.Parse(strDateEnd + " " + strTimeEnd).Ticks);
                    double ts = t2.Subtract(t1).TotalHours;

                    TimeSpan date1 = new TimeSpan(DateTime.Parse(strDateStart).Ticks);
                    TimeSpan date2 = new TimeSpan(DateTime.Parse(strDateEnd).Ticks);
                    double dates = date2.Subtract(date1).TotalDays; //跨越天數
                    double DayOffHours = 0;        //請假時數
                    double NonWorkDayHours = 0;    //非工作時數

                    for (int i = 0; i <= dates; i++)
                    {
                        TimeSpan DayOffStart = new TimeSpan(DateTime.Parse(strDate + " " + strTimeStart).Ticks);  //請假開始
                        Boolean MorningStart = BreakStart.Subtract(DayOffStart).TotalHours >= 0 ? true : false;            //上午開始
                        Boolean AfternoonStart = BreakEnd.Subtract(DayOffStart).TotalHours <= 0 ? true : false;    //下午開始
                        TimeSpan DayOffEnd = new TimeSpan(DateTime.Parse(strDate + " " + strTimeEnd).Ticks); //請假結束
                       Boolean MorningEnd = BreakStart.Subtract(DayOffEnd).TotalHours >= 0 ? true : false;   //上午結束
                        Boolean AfternoonEnd = BreakEnd.Subtract(DayOffEnd).TotalHours <= 0 ? true : false;   //下午結束

                        if (i == 0 && i == dates) //同一天
                        {
                            //請上午,請下午
                            if (MorningStart && MorningEnd || AfternoonStart && AfternoonEnd)
                            {
                                DayOffHours += DayOffEnd.Subtract(DayOffStart).TotalHours;
                            }
                            else //請整天,跨越午休
                            {
                                if (MorningStart) DayOffHours += BreakStart.Subtract(DayOffStart).TotalHours;
                                if (AfternoonEnd) DayOffHours += DayOffEnd.Subtract(BreakEnd).TotalHours;
                            }
                        }
                        else if (i == 0 && i < dates) //第一天
                        {
                            if (MorningStart) DayOffHours += BreakStart.Subtract(DayOffStart).TotalHours + 4.5;
                            if (AfternoonStart) DayOffHours += WorkEnd.Subtract(DayOffStart).TotalHours;
                        }
                        else if (i != 0 && i == dates) //最後一天
                        {
                            if (MorningEnd) DayOffHours += DayOffEnd.Subtract(WorkStart).TotalHours;
                            else DayOffHours += 3.5;
                            if (AfternoonEnd) DayOffHours += DayOffEnd.Subtract(BreakEnd).TotalHours;
                        }
                        else
                        {
                            DayOffHours += 8;
                        }
                    }


                    if (dates > 0)
                    {
                        DataTable getWorkData = GetWorkDayData();
                        for (int i = 0; i <= dates; i++)
                        {
                            DateTime Day = DateTime.Parse(strDateStart).AddDays(i);
                            if (Day.DayOfWeek.Equals(DayOfWeek.Saturday) || Day.DayOfWeek.Equals(DayOfWeek.Sunday))
                            {
                                DataRow[] rows = getWorkData.Select("WorkDate = '" + Day.ToString("yyyy/MM/dd") + "'");
                                if (rows.Length ==0)
                                {
                                    NonWorkDayHours += 8;
                                }
                            }
                        }
                    }
                    else
                    {
                        DataTable getWorkData = GetWorkDayData();
                        DateTime Day = DateTime.Parse(strDateStart);
                        if (Day.DayOfWeek.Equals(DayOfWeek.Saturday) || Day.DayOfWeek.Equals(DayOfWeek.Sunday))
                        {
                            DataRow[] rows = getWorkData.Select("WorkDate = '" + Day.ToString("yyyy/MM/dd") + "'");
                            if (rows.Length == 0)
                            {
                                NonWorkDayHours += 8;
                            }
                        }
                    }
                    DayOffHours = DayOffHours - NonWorkDayHours;

                    string TotalDayOffHours = DayOffHours.ToString(); //總時數
                    string TotalDayOffDays = (DayOffHours / 8).ToString();          //總天數

                    if (double.Parse(TotalDayOffDays) < 1)
                    {
                        this.HolidayEntity.Daily = "0";
                        this.HolidayEntity.Hours = TotalDayOffHours;
                    }
                    else
                    {
                        int TotalDays = (int)(Math.Floor(DayOffHours) / 8);
                        int TotalHours = (int)(Math.Floor(DayOffHours) % 8);
                        this.HolidayEntity.Daily = TotalDays.ToString();
                        this.HolidayEntity.Hours = TotalHours.ToString();
                    }

                    if (DayOffHours <= 0)
                    {
                        ErrMsg = "該天為非上班日，故無需請假!!";
                        sRet.result = false;
                    }
                    else
                    {
                        //this.HolidayEntity.Daily = this.GetCheckNumber(this.HolidayEntity.Daily);//daily.ToString();
                        //this.HolidayEntity.Hours = this.GetCheckNumber(this.HolidayEntity.Hours);  //hour.ToString();
                        this.HolidayEntity.LicNo = "0000";
                        this.HolidayEntity.LicType = "58";
                        this.HolidayEntity.CreateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
                        this.HolidayEntity.UpdateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
                        this.odo.Execute(CmdStr, this.HolidayEntity);
                        ErrMsg = this.odo.DbExceptionMessage;
                        sRet.result = this.odo.isSuccess;
                    }
                   
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

    }
}