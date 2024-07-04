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



namespace SahoAcs.Web._97._9706
{
    public partial class ClassSchedule : SahoAcs.DBClass.BasePage
    {
        public List<B03Empclassschedule> ClassList = new List<B03Empclassschedule>();
        public List<B03Empclassschedule> ClassLast2 = new List<B03Empclassschedule>();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<string> ClassNoList = new List<string>();
        public List<string> ClassAllList = new List<string>();
        public string NowMonth = DateTime.Now.ToString("yyyy/MM");
        protected override void Page_Load(object sender, EventArgs e)
        {
            this.SetInit();
            if (Request["PageEvent"] == null)
            {
                
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryData")
            {
                //查詢當月份班表，若無紀錄則帶出空白
                this.SetClassList();
            }
            if (Request["PageEvent"]!=null && Request["PageEvent"] == "CreateClass")
            {
                this.SetCreateSchedule();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "JoinClass")
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
            ClientScript.RegisterClientScriptInclude("JsFun", "ClassSchedule.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }


        void SetInit()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            this.PersonList = this.od.GetQueryResult<PersonEntity>(@"
                                                        select A.* from B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE Text5='ABCR' AND D.UserID=@UserID ORDER BY PsnNo", new { UserID }).OrderBy(i=>i.PsnName).ToList();
            this.ClassNoList.Add("A");
            this.ClassNoList.Add("B");
            this.ClassNoList.Add("C");
            this.ClassNoList.Add("R");
            this.ClassAllList.Add("A");
            this.ClassAllList.Add("A");
            this.ClassAllList.Add("R");
            this.ClassAllList.Add("B");
            this.ClassAllList.Add("B");
            this.ClassAllList.Add("C");
            this.ClassAllList.Add("C");
            this.ClassAllList.Add("R");
        }

        private void SetClassList()
        {
            string PsnNo = "", ClassMonth = "";
            if (Request.Form["PsnNo"] != null)
            {
                PsnNo = Request.Form["PsnNo"];
            }
            if (Request.Form["ClassMonth"] != null)
            {
                ClassMonth = Request.Form["ClassMonth"];
            }            
            this.ClassList = this.od.GetQueryResult<B03Empclassschedule>("SELECT * FROM B03_EmpClassSchedule WHERE PsnNo=@PsnNo AND ClassMonth=@ClassMonth ORDER BY ClassDate",new { PsnNo, ClassMonth }).ToList();
            //取得前一個月的排班
            this.ClassLast2 = this.od.GetQueryResult<B03Empclassschedule>("SELECT TOP 2 * FROM B03_EmpClassSchedule WHERE PsnNo=@PsnNo AND ClassMonth<@ClassMonth ORDER BY ClassDate DESC",
                new { PsnNo,ClassMonth }).OrderBy(i=>i.ClassDate).ToList();
            DateTime DateS = DateTime.Now, DateE = DateTime.Now, DateChk = DateTime.Now;
            if (DateTime.TryParse(ClassMonth + "/26", out DateChk))
            {
                DateS = DateChk;
            }
            DateS = DateS.AddMonths(-1);
            if (DateTime.TryParse(ClassMonth + "/25", out DateChk))
            {
                DateE = DateChk;
            }
            if (ClassLast2.Count == 0)
            {
                for(int i = 0; i < 2; i++)
                {
                    this.ClassLast2.Add(new B03Empclassschedule()
                    {
                        ClassNo = "A",
                        ClassDate = DateS.AddDays(-(2-i)),
                        ClassMonth=DateS.ToString("yyyy/MM"),
                        PsnNo = PsnNo
                    });
                }
            }
            if (this.ClassList.Count == 0)
            {
                while (DateS <= DateE)
                {
                    var WorkInfo = new B03Empclassschedule();
                    WorkInfo.ClassDate = DateS;
                    WorkInfo.ClassMonth = ClassMonth;
                    WorkInfo.PsnNo = PsnNo;
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
            string RuleClassNo = "AARBBCCR", PsnNo = "", ClassMonth = DateTime.Now.ToString("yyyy/MM");
            //取得後兩筆班別的資料
            string LastClassNo = "";
            if (Request.Form["ClassNoDef"] != null)
            {
                LastClassNo = Request.Form["ClassNoDef"].Replace(",","");
            }
            int now_index = 0;           
            if (LastClassNo.Equals("RA"))
            {
                now_index = 1;
            }
            if (RuleClassNo.Contains(LastClassNo))
            {
                now_index = RuleClassNo.IndexOf(LastClassNo);
                now_index += LastClassNo.Length;
            }            
            if (Request.Form["PsnNo"] != null)
            {
                PsnNo = Request.Form["PsnNo"];
            }
            if (Request.Form["ClassMonth"] != null)
                ClassMonth = Request.Form["ClassMonth"];
            if(this.od.GetQueryResult("SELECT * FROM B03_EmpClassSchedule WHERE PsnNo=@PsnNo AND ClassMonth>@ClassMonth",new {PsnNo, ClassMonth }).Count()>0)
            {
                Message = $"因系統現有班表月份大於{ClassMonth}，因此無法建立{ClassMonth}班表資料";
                result = false;
            }
            else
            {
                this.od.Execute("DELETE B03_EmpClassSchedule WHERE PsnNo=@PsnNo AND ClassMonth=@ClassMonth", new { PsnNo, ClassMonth });
                DateTime DateS = DateTime.Now, DateE = DateTime.Now, DateChk = DateTime.Now;
                if (DateTime.TryParse(ClassMonth + "/26", out DateChk))
                {
                    DateS = DateChk;
                }
                DateS = DateS.AddMonths(-1);
                if (DateTime.TryParse(ClassMonth + "/25", out DateChk))
                {
                    DateE = DateChk;
                }
                this.ClassList = new List<B03Empclassschedule>();
                while (DateS <= DateE)
                {
                    if (now_index == 8)
                    {
                        now_index = 0;
                    }
                    var WorkInfo = new B03Empclassschedule();
                    WorkInfo.ClassDate = DateS;
                    WorkInfo.ClassMonth = ClassMonth;
                    WorkInfo.PsnNo = PsnNo;
                    WorkInfo.ClassNo = ClassAllList[now_index];                    
                    WorkInfo.CreateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
                    WorkInfo.UpdateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
                    this.od.Execute(@"INSERT INTO B03_EmpClassSchedule (PsnNo,ClassMonth,ClassNo,ClassDate,AdjClassNo,
                                                    CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                                                    VALUES (@PsnNo,@ClassMonth,@ClassNo,@ClassDate,@ClassNo,@UpdateUserID,GETDATE(),@UpdateUserID,GETDATE())", WorkInfo);
                    DateS = DateS.AddDays(1);
                    now_index++;
                }
            }
           
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = Message, success=result }));
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
                    this.od.Execute("UPDATE B03_EmpClassSchedule SET AdjClassNo=@adjno WHERE RecordID=@rec", new { rec, adjno });
                    index++;
                }
            }
          
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = Message, success = result }));
            Response.End();
        }

    }//end class
}//end namespace