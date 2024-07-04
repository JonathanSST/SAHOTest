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
    public partial class GroupClassDefine : SahoAcs.DBClass.BasePage
    {
        public List<B03GroupClassSchedule> ClassList = new List<B03GroupClassSchedule>();
        public List<B03GroupClassSchedule> ClassLast2 = new List<B03GroupClassSchedule>();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgList = new List<OrgDataEntity>();
        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<string> ClassNoList = new List<string>();
        public List<string> ClassAllList = new List<string>();
        public string Date2 = "";
        public string Date1 = "";
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
            if (Request["PageEvent"]!=null && Request["PageEvent"] == "Save")
            {
                this.SetCreateDefine();
            }

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("JsFun", "GroupClassDefine.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", Request.ApplicationPath.TrimEnd('/')+"/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", Request.ApplicationPath.TrimEnd('/')+"/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }


        void SetInit()
        {
            
        }

        private void SetClassList()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            this.OrgList = this.od.GetQueryResult<OrgDataEntity>("SELECT * FROM B01_OrgData WHERE OrgClass='Group' ").ToList();
            var ClassData = this.od.GetQueryResult<B03GroupClassSchedule>("SELECT DISTINCT GroupNo FROM B03_GroupClassSchedule");
            Date2 = DateTime.Parse(this.GetFormEndValue("ClassMonth") + "/01").AddMonths(1).AddDays(-2).ToString("yyyy/MM/dd");
            Date1 = DateTime.Parse(this.GetFormEndValue("ClassMonth") + "/01").AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
            foreach (var org in this.OrgList)
            {
                if (ClassData.Where(i =>i.GroupNo.Equals(org.OrgNo)).Count() > 0)
                {
                    continue;
                }
                this.ClassList.Add(new B03GroupClassSchedule()
                {
                    GroupNo = org.OrgNo,
                    GroupName = org.OrgName,
                    ClassNo1 = "1",
                    ClassNo2 = "1"
                });
                if (this.ClassList.Count > 50)
                {
                    break;  //一次進行50筆資料作業
                }
            }
            this.ClassNoList.Add("1");
            this.ClassNoList.Add("2");
            this.ClassNoList.Add("3");
            this.ClassNoList.Add("0");
            //判斷班表的順序
            this.ClassAllList.Add("1");
            this.ClassAllList.Add("1");
            this.ClassAllList.Add("0");
            this.ClassAllList.Add("2");
            this.ClassAllList.Add("2");
            this.ClassAllList.Add("3");
            this.ClassAllList.Add("3");
            this.ClassAllList.Add("0");
        }


        private void SetCreateDefine()
        {
            string Message = "";
            var result = true;
            string RuleClassNo = "11022330", ClassMonth = DateTime.Now.ToString("yyyy/MM");
            List<string> OrgQuee = Request.Form.GetValues("GroupNo").ToList();
            List<string> ClassQuee1 = Request.Form.GetValues("ClassNo1").ToList();
            List<string> ClassQuee2 = Request.Form.GetValues("ClassNo2").ToList();
            if (Request.Form["ClassMonth"] != null)
                ClassMonth = Request.Form["ClassMonth"];

            //取得後兩筆班別的資料
            string LastClassNo = "";
            if (Request.Form["ClassNoDef"] != null)
            {
                LastClassNo = Request.Form["ClassNoDef"].Replace(",","");
            }
            int now_index = 0;           
            if (LastClassNo.Equals("01"))
            {
                now_index = 1;
            }
            if (RuleClassNo.Contains(LastClassNo))
            {
                now_index = RuleClassNo.IndexOf(LastClassNo);
                now_index += LastClassNo.Length;
            }
            if (ClassQuee1.Count ==OrgQuee.Count && ClassQuee2.Count ==OrgQuee.Count)
            {
                foreach (var org in OrgQuee)
                {
                    int count_int = OrgQuee.IndexOf(org);
                    LastClassNo = ClassQuee2[count_int] + ClassQuee1[count_int];
                    if (LastClassNo.Equals("01") || RuleClassNo.Contains(LastClassNo))
                    {                        
                        B03GroupClassSchedule classdata= new B03GroupClassSchedule();
                        classdata.CreateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
                        classdata.ClassDate = DateTime.Parse(ClassMonth + "/01").AddMonths(1).AddDays(-2);
                        classdata.ClassMonth = ClassMonth;
                        classdata.ClassNo = ClassQuee2[count_int];
                        classdata.GroupNo = org;
                        string mainCmdStr = @"INSERT INTO b03_GroupClassSchedule (GroupNo,GroupName,ClassMonth,ClassNo,ClassDate,AdjClassNo,
                                                    CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                                                    VALUES (@GroupNo,@GroupNo,@ClassMonth,@ClassNo,@ClassDate,@ClassNo,@UpdateUserID,GETDATE(),@UpdateUserID,GETDATE())";
                        this.od.Execute(mainCmdStr, classdata);
                        classdata.ClassNo = ClassQuee1[count_int];                        
                        classdata.ClassDate = DateTime.Parse(ClassMonth + "/01").AddMonths(1).AddDays(-1);
                        this.od.Execute(mainCmdStr, classdata);
                    }                    
                }
            }
          
           
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "完成初始化排班", success=result }));
            Response.End();
        }


    }//end class
}//end namespace