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
    public partial class ClassDefine : SahoAcs.DBClass.BasePage
    {
        public List<B03Empclassschedule> ClassList = new List<B03Empclassschedule>();
        public List<B03Empclassschedule> ClassLast2 = new List<B03Empclassschedule>();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<string> ClassNoList = new List<string>();
        public List<string> ClassAllList = new List<string>();

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
            ClientScript.RegisterClientScriptInclude("JsFun", "ClassDefine.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
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
            this.PersonList = this.od.GetQueryResult<PersonEntity>(@"
                                                        SELECT A.* FROM B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE Text5='ABCR' AND D.UserID=@UserID", new { UserID }).OrderBy(i => i.PsnNo).ToList();
            var ClassData = this.od.GetQueryResult<B03Empclassschedule>("SELECT DISTINCT PsnNo FROM B03_EmpClassSchedule");
            foreach (var psn in this.PersonList)
            {
                if (ClassData.Where(i => i.PsnNo.Equals(psn.PsnNo)).Count() > 0)
                {
                    continue;
                }
                this.ClassList.Add(new B03Empclassschedule()
                {
                    PsnNo = psn.PsnNo,
                    PsnName = psn.PsnName,
                    ClassNo1 = "A",
                    ClassNo2 = "A"
                });
                if (this.ClassList.Count > 50)
                {
                    break;  //一次進行50筆資料作業
                }
            }
            this.ClassNoList.Add("A");
            this.ClassNoList.Add("B");
            this.ClassNoList.Add("C");
            this.ClassNoList.Add("R");
            //判斷班表的順序
            this.ClassAllList.Add("A");
            this.ClassAllList.Add("A");
            this.ClassAllList.Add("R");
            this.ClassAllList.Add("B");
            this.ClassAllList.Add("B");
            this.ClassAllList.Add("C");
            this.ClassAllList.Add("C");
            this.ClassAllList.Add("R");
        }


        private void SetCreateDefine()
        {
            string Message = "";
            var result = true;
            string RuleClassNo = "AARBBCCR", ClassMonth = DateTime.Now.ToString("yyyy/MM");
            List<string> PsnQuee = Request.Form.GetValues("PsnNo").ToList();
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
            if (LastClassNo.Equals("RA"))
            {
                now_index = 1;
            }
            if (RuleClassNo.Contains(LastClassNo))
            {
                now_index = RuleClassNo.IndexOf(LastClassNo);
                now_index += LastClassNo.Length;
            }
            if (ClassQuee1.Count ==PsnQuee.Count && ClassQuee2.Count ==PsnQuee.Count)
            {
                foreach (var psn in PsnQuee)
                {
                    int count_int = PsnQuee.IndexOf(psn);
                    LastClassNo = ClassQuee2[count_int] + ClassQuee1[count_int];
                    if (LastClassNo.Equals("RA") || RuleClassNo.Contains(LastClassNo))
                    {                        
                        B03Empclassschedule classdata= new B03Empclassschedule();
                        classdata.CreateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
                        classdata.ClassDate = DateTime.Parse(ClassMonth + "/24");
                        classdata.ClassMonth = ClassMonth;
                        classdata.ClassNo = ClassQuee2[count_int];                        
                        classdata.PsnNo = psn;
                        this.od.Execute(@"INSERT INTO B03_EmpClassSchedule (PsnNo,ClassMonth,ClassNo,ClassDate,AdjClassNo,
                                                    CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                                                    VALUES (@PsnNo,@ClassMonth,@ClassNo,@ClassDate,@ClassNo,@UpdateUserID,GETDATE(),@UpdateUserID,GETDATE())", classdata);
                        classdata.ClassNo = ClassQuee1[count_int];                        
                        classdata.ClassDate = DateTime.Parse(ClassMonth + "/25");
                        this.od.Execute(@"INSERT INTO B03_EmpClassSchedule (PsnNo,ClassMonth,ClassNo,ClassDate,AdjClassNo,
                                                    CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                                                    VALUES (@PsnNo,@ClassMonth,@ClassNo,@ClassDate,@ClassNo,@UpdateUserID,GETDATE(),@UpdateUserID,GETDATE())", classdata);
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