using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Script.Serialization;
using SahoAcs.DBClass;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.Web._01._0119
{
    public partial class GroupClassSchedule : SahoAcs.DBClass.BasePage
    {
        public List<B03GroupClassSchedule> ClassList = new List<B03GroupClassSchedule>();
        public List<B03GroupClassSchedule> ClassLast2 = new List<B03GroupClassSchedule>();
        public List<OrgDataEntity> OrgDataList = new List<OrgDataEntity>();
        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<string> ClassNoList = new List<string>();
        public List<string> ClassAllList = new List<string>();
        public string NowMonth = DateTime.Now.ToString("yyyy/MM");

        protected override void Page_Load(object sender, EventArgs e)
        {
            this.SetInit();
            if (Request["PageEvent"] != null && Request["PageEvent"] == "ChangeGroup")
            {
                this.ChangeGroup();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryData")
            {
                //查詢當月份班表，若無紀錄則帶出空白
                this.SetClassList();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "CreateClass")
            {
                this.SetCreateSchedule();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "UpdateSchedule")
            {
                this.SetUpdateSchedule();
            }
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("JsFun", "GroupClassSchedule.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        void SetInit()
        {
            this.OrgDataList = this.od.GetQueryResult<OrgDataEntity>(@"Select OrgNo,OrgName From B01_OrgData WHERE OrgClass = 'Group'  ORDER BY OrgNo").ToList();
            this.ClassNoList.Add("0");
            this.ClassNoList.Add("1");
            this.ClassNoList.Add("2");
            this.ClassNoList.Add("3");
            this.ClassNoList.Add("E");
            this.ClassNoList.Add("F");
            this.ClassNoList.Add("G");
            this.ClassAllList.Add("1");
            this.ClassAllList.Add("1");
            this.ClassAllList.Add("0");
            this.ClassAllList.Add("2");
            this.ClassAllList.Add("2");
            this.ClassAllList.Add("3");
            this.ClassAllList.Add("3");
            this.ClassAllList.Add("0");
        }

        private void ChangeGroup()
        {
            string GroupName = "";
            string ClassMonth = "";
            string StartMonth = string.Empty, EndMonth = string.Empty;
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (!this.GetFormEndValue("GroupName").Equals(""))
            {
                GroupName = this.GetFormEndValue("GroupName");
                string sql = @"
                            Select  Top 1
                            ClassMonth From B03_GroupClassSchedule 
                            Where GroupNo = @GroupNo
                            Order by ClassMonth DESC ";
                ClassMonth = this.od.GetStrScalar(sql, new { GroupNo = GroupName });
                DateTime DateMonth = DateTime.Now;
                DateTime DateChk = DateTime.Now;
                if (!string.IsNullOrEmpty(ClassMonth))
                {
                    if (DateTime.TryParse(ClassMonth + "/01", out DateChk))
                    {
                        DateMonth = DateChk;
                    }
                    StartMonth = DateMonth.AddMonths(1).ToString("yyyy/MM");
                    EndMonth = DateMonth.AddMonths(2).ToString("yyyy/MM");                    
                }
                else
                {
                    StartMonth = DateTime.Now.ToString("yyyy/MM");
                    EndMonth = DateTime.Now.ToString("yyyy/MM");
                }
            }
            data.Add("startmonth", StartMonth);
            data.Add("endmonth", EndMonth);
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data));
            Response.End();
        }

        private void SetClassList()
        {
            //string Message = "";
            //var result = true;
            string GroupName = "";
            string ClassMonth = "";
            string EndMonth = "";

            if (Request.Form["GroupName"] != null)
            {
                GroupName = Request.Form["GroupName"];
            }
            if (Request.Form["ClassMonth"] != null)
            {
                ClassMonth = Request.Form["ClassMonth"];
            }
            if (Request.Form["EndMonth"] != null)
            {
                EndMonth = Request.Form["EndMonth"];
            }
            this.ClassList = this.od.GetQueryResult<B03GroupClassSchedule>(
                "SELECT * FROM B03_GroupClassSchedule WHERE GroupNo=@GroupName AND ClassMonth=@ClassMonth ORDER BY ClassDate", 
                new { GroupName, ClassMonth }).ToList();

            //取得前一個月的排班
            this.ClassLast2 = this.od.GetQueryResult<B03GroupClassSchedule>(
                "SELECT TOP 2 * FROM B03_GroupClassSchedule WHERE GroupNo=@GroupName AND ClassMonth<@ClassMonth " +
                "ORDER BY ClassDate DESC ",
                new { GroupName, ClassMonth }).OrderBy(i => i.ClassDate).ToList();

            DateTime DateS = DateTime.Now;
            DateTime DateE = DateTime.Now;
            DateTime DateChk = DateTime.Now;

            if (DateTime.TryParse(ClassMonth + "/01", out DateChk))
            {
                DateS = DateChk;
            }

            DateTime EndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(EndMonth))
            {
                EndDate = Convert.ToDateTime(EndMonth + "/01");
            }
            else
            {
                EndDate = Convert.ToDateTime(ClassMonth + "/01");
            }
            
            if (DateTime.TryParse(EndDate.AddMonths(1).AddDays(-1).ToString(), out DateChk))
            {
                DateE = DateChk;
            }
            if (ClassLast2.Count == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    this.ClassLast2.Add(new B03GroupClassSchedule()
                    {
                        ClassNo = "1",
                        ClassDate = DateE,
                        ClassMonth = DateS.ToString("yyyy/MM"),
                        GroupName = GroupName,
                        GroupNo=GroupName
                    });
                }
            }
            if (this.ClassList.Count == 0)
            {
                while (DateS <= DateE)
                {
                    var WorkInfo = new B03GroupClassSchedule();
                    WorkInfo.ClassDate = DateS;
                    WorkInfo.ClassMonth = ClassMonth;
                    WorkInfo.GroupName = GroupName;
                    WorkInfo.GroupNo = GroupName;
                    WorkInfo.ClassNo = "";
                    WorkInfo.AdjClassNo = "";
                    this.ClassList.Add(WorkInfo);
                    DateS = DateS.AddDays(1);
                }
            }

        }

        private void SetCreateSchedule()
        {
            string Message = "";
            var result = true;
            string GroupName = string.Empty;
            string ClassMonth = string.Empty;
            string EndMonth = string.Empty;
            
            int now_index = 0;
           
            if (Request.Form["GroupName"] != null)
            {
                GroupName = Request.Form["GroupName"];
            }
            if (Request.Form["ClassMonth"] != null)
            {
                ClassMonth = Request.Form["ClassMonth"];
            }
            if (Request.Form["EndMonth"] != null)
            {
                EndMonth = Request.Form["EndMonth"];
            }

            DateTime DateS = DateTime.Now, DateE = DateTime.Now, DateChk = DateTime.Now, EndDate = DateTime.Now, StartDate = DateTime.Now;
            if(DateTime.TryParse(this.GetFormEqlValue("ClassLast")+"/01", out DateChk))
            {
                StartDate = DateChk;
                StartDate = StartDate.AddMonths(1);
            }           
            if (DateTime.TryParse(ClassMonth + "/01", out DateChk))
            {
                DateS = DateChk;
            }
            if (!string.IsNullOrEmpty(EndMonth))
            {
                EndDate = Convert.ToDateTime(EndMonth + "/01");
            }
            else
            {
                EndDate = Convert.ToDateTime(ClassMonth + "/01");
            }            
            //if (DateTime.TryParse(DateS.AddYears(1).AddMonths(1).AddDays(-1).ToString(), out DateChk))
            if (DateTime.TryParse(EndDate.AddMonths(1).AddDays(-1).ToString(), out DateChk))
            {
                DateE = DateChk;
            }
            if (DateS > StartDate)
            {
                DateS = StartDate;
            }
            List<B03GroupClassSchedule> DbData = new List<B03GroupClassSchedule>();
            DbData = this.od.GetQueryResult<B03GroupClassSchedule>(@"SELECT * FROM B03_GroupClassSchedule WHERE GroupNo=@GroupName 
                AND ClassDate Between Convert(varchar(10),@DateS,111) AND Convert(varchar(10),@DateE,111)  ",
                new
                {
                    GroupName,
                    DateS,
                    DateE
                }).ToList();

            if (DbData.Count() != 0)
            {
                Message = DbData[0].ClassMonth.ToString() + " 月份已產生報表!!";
                result = false;
            }
            else
            {
                DataTable DataResult = OrmDataObject.IEnumerableToTable(DbData);
                this.ClassList = new List<B03GroupClassSchedule>();
                string RuleClassNo = "11022330", LastClassNo = "";
                if(!this.GetFormEndValue("ClassNoDef").Equals(""))
                {
                    LastClassNo = Request.Form["ClassNoDef"].Replace(",", "");
                }
                if (LastClassNo.Equals("01"))
                {
                    now_index = 1;
                }
                if (RuleClassNo.Contains(LastClassNo))
                {
                    now_index = RuleClassNo.IndexOf(LastClassNo);
                    now_index += LastClassNo.Length;
                }
                while (DateS <= DateE)
                {
                    if (now_index == 8)
                    {
                        now_index = 0;
                    }
                    string _ClassNo = string.Empty;
                    string Days = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateS.DayOfWeek);                                                       
                    var WorkInfo = new B03GroupClassSchedule();
                    WorkInfo.ClassDate = DateS;
                    WorkInfo.GroupName = GroupName;
                    WorkInfo.GroupNo = GroupName;
                    WorkInfo.ClassMonth = DateS.ToString("yyyy/MM");
                    WorkInfo.GroupName = GroupName;
                    //WorkInfo.ClassNo = ClassAllList[now_index];
                    WorkInfo.ClassNo = this.ClassAllList[now_index];
                    WorkInfo.CreateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
                    WorkInfo.UpdateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");                    
                    DataRow[] rows = DataResult.Select("classdate = '" + DateS + "' And GroupNo='" + GroupName + "'");
                    if (rows.Length == 0)
                    {
                        this.od.Execute(@"INSERT INTO B03_GroupClassSchedule (GroupNo,GroupName,ClassMonth,ClassNo,ClassDate,AdjClassNo,
                                                    CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                                                    VALUES (@GroupName,@GroupName,@ClassMonth,@ClassNo,@ClassDate,@ClassNo,@UpdateUserID,GETDATE(),@UpdateUserID,GETDATE())", WorkInfo);
                        //清除該組的人員般別資料       update by Sam 2021/11/22
                        this.od.Execute(@"DELETE B03_EmpClassSchedule WHERE PsnNo IN (SELECT PsnNo FROM B01_Person P INNER JOIN OrgStrucAllData('Group') G ON P.OrgStrucID=G.OrgStrucID AND OrgNo=@GroupNo) AND ClassDate=@ClassDate", WorkInfo);
                        //加該組的人員般別資料附加至 B03_EmpClassSchedule   update by Sam 2021/11/22
                        this.od.Execute(@"INSERT INTO B03_EmpClassSchedule (PsnNo,ClassNo,ClassMonth,ClassDate,AdjClassNo,CreateUserID,UpdateUserID,CreateTime,UpdateTime)
                            SELECT P.PsnNo,G.ClassNo,ClassMonth,ClassDate,AdjClassNo,G.CreateUserID,G.UpdateUserID,GETDATE(),GETDATE() from B03_GroupClassSchedule G
                            INNER JOIN OrgStrucAllData('Group') O ON G.GroupNo=O.OrgNo
                            INNER JOIN B01_Person P ON P.OrgStrucID=O.OrgStrucID
                            WHERE GroupNo=@GroupNo AND ClassDate=@ClassDate", WorkInfo);
                    }
                    DateS = DateS.AddDays(1);
                    now_index++;
                }
            }

            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = Message, success = result }));
            Response.End();
        }


        /// <summary>
        /// 修改當月排程、調班
        /// </summary>
        private void SetUpdateSchedule()
        {
            string Message = "";
            var result = true;
            string[] RecordID = Request.Form.GetValues("RecordID");
            string[] AdjClassNo = Request.Form.GetValues("AdjClassNo");
            if (RecordID.Length != AdjClassNo.Length)
            {
                Message = "資料格式有誤，無法進行異動";
                result = true;
            }
            else
            {
                int index = 0;
                foreach (var rec in RecordID)
                {
                    var adjno = AdjClassNo[index];
                    var UpdateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
                    this.od.Execute("UPDATE B03_GroupClassSchedule SET ClassNo=@adjno,AdjClassNo=@adjno,UpdateUserID=@UpdateUserID,UpdateTime=Getdate() " +
                        "WHERE RecordID=@rec ", new { rec, adjno, UpdateUserID });
                    index++;
                }
            }

            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = Message, success = result }));
            Response.End();
        }
    }
}