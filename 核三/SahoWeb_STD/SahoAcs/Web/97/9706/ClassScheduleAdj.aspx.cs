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
using SahoAcs.DBClass;


namespace SahoAcs.Web._97._9706
{
    public partial class ClassScheduleAdj : SahoAcs.DBClass.BasePage
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
            if (Request["PageEvent"] != null && Request["PageEvent"] == "MergeClass")
            {
                this.SetMergeSchedule();
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
            ClientScript.RegisterClientScriptInclude("JsFun", "ClassScheduleAdj.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }


        void SetInit()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            this.PersonList = this.od.GetQueryResult<PersonEntity>(@"SELECT A.* FROM B01_Person A
                                                                                                    INNER JOIN OrgStrucAllData('Group') G ON A.OrgStrucID=G.OrgStrucID AND OrgNo<>'' 
                                                                                                    INNER JOIN B01_MgnOrgStrucs MO ON G.OrgStrucID=MO.OrgStrucID  
                                                                                                    INNER JOIN B00_SysUserMgns M ON MO.MgaID=M.MgaID AND UserID=@UserID
                                                                                                    ORDER BY PsnNo", new { UserID }).OrderBy(i=>i.PsnName).ToList();
            this.ClassNoList.Add("1");
            this.ClassNoList.Add("2");
            this.ClassNoList.Add("3");
            this.ClassNoList.Add("0");
            this.ClassAllList.Add("1");
            this.ClassAllList.Add("1");
            this.ClassAllList.Add("0");
            this.ClassAllList.Add("2");
            this.ClassAllList.Add("2");
            this.ClassAllList.Add("3");
            this.ClassAllList.Add("3");
            this.ClassAllList.Add("0");
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
            string ClassMonth = this.GetFormEqlValue("ClassMonth");
            string GroupNo = "";
            string PsnNo = this.GetFormEqlValue("PsnNo");
            var ResultData = this.od.GetQueryResult<PersonEntity>(@" SELECT A.*,G.OrgNo AS OrgNoList FROM B01_Person A
                                                INNER JOIN OrgStrucAllData('Group') G ON A.OrgStrucID=G.OrgStrucID AND OrgNo<>'' WHERE PsnNo=@PsnNo ",new { PsnNo = PsnNo });
            foreach (var psn in ResultData)
            {
                GroupNo = psn.OrgNoList;
            }
            this.od.Execute("DELETE B03_EmpClassSchedule WHERE PsnNo=@PsnNo", new { PsnNo = PsnNo });       //刪除人員的現有排班
                                                                                                            //加該組的人員般別資料附加至 B03_EmpClassSchedule   update by Sam 2021/11/22
            this.od.Execute(@"INSERT INTO B03_EmpClassSchedule (PsnNo,ClassNo,ClassMonth,ClassDate,AdjClassNo,CreateUserID,UpdateUserID,CreateTime,UpdateTime)
                            SELECT P.PsnNo,G.ClassNo,ClassMonth,ClassDate,AdjClassNo,@UserID,@UserID,GETDATE(),GETDATE() from B03_GroupClassSchedule G
                            INNER JOIN OrgStrucAllData('Group') O ON G.GroupNo=O.OrgNo
                            INNER JOIN B01_Person P ON P.OrgStrucID=O.OrgStrucID
                            WHERE GroupNo=@GroupNo AND ClassMonth>=@ClassMonth AND PsnNo=@PsnNo", new {PsnNo=PsnNo, GroupNo=GroupNo, ClassMonth=ClassMonth, UserID=Sa.Web.Fun.GetSessionStr(this,"UserID")});

            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = Message, success=result }));
            Response.End();
        }


        private void SetMergeSchedule()
        {
            string Message = "";
            var result = true;
            string ClassMonth = this.GetFormEqlValue("ClassMonth");
            string GroupNo = "";
            string PsnNo = this.GetFormEqlValue("PsnNo");
            var ResultData = this.od.GetQueryResult<PersonEntity>(@" SELECT A.*,G.OrgNo AS OrgNoList FROM B01_Person A
                                                INNER JOIN OrgStrucAllData('Group') G ON A.OrgStrucID=G.OrgStrucID AND OrgNo<>'' WHERE PsnNo=@PsnNo ", new { PsnNo = PsnNo });
            foreach (var psn in ResultData)
            {
                GroupNo = psn.OrgNoList;
            }
            this.od.Execute("DELETE B03_EmpClassSchedule WHERE PsnNo=@PsnNo AND ClassMonth>=@ClassMonth", new { PsnNo = PsnNo, ClassMonth = ClassMonth });       //刪除人員次月後的排班
                                                                                                            //加該組的人員般別資料附加至 B03_EmpClassSchedule   update by Sam 2021/11/22
            this.od.Execute(@"INSERT INTO B03_EmpClassSchedule (PsnNo,ClassNo,ClassMonth,ClassDate,AdjClassNo,CreateUserID,UpdateUserID,CreateTime,UpdateTime)
                            SELECT P.PsnNo,G.ClassNo,ClassMonth,ClassDate,AdjClassNo,@UserID,@UserID,GETDATE(),GETDATE() from B03_GroupClassSchedule G
                            INNER JOIN OrgStrucAllData('Group') O ON G.GroupNo=O.OrgNo
                            INNER JOIN B01_Person P ON P.OrgStrucID=O.OrgStrucID
                            WHERE GroupNo=@GroupNo AND ClassMonth>=@ClassMonth AND PsnNo=@PsnNo", new { PsnNo = PsnNo, GroupNo = GroupNo, ClassMonth = ClassMonth, UserID = Sa.Web.Fun.GetSessionStr(this, "UserID") });

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