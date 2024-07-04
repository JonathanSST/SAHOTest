using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs
{
    public partial class EquGroupEdit : Sa.BasePage
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<EquGroupModel> groups = new List<EquGroupModel>();

        public List<EquGroupData> groupdata = new List<EquGroupData>();

        public List<EquGroupData> nonusedata = new List<EquGroupData>();

        public List<CardRuleData> RuleResult = new List<CardRuleData>();

        public int grpid = 0;

        public string EquName = "";

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.groups = this.odo.GetQueryResult<EquGroupModel>("SELECT * FROM B01_EquGroup").ToList();

            if (Request["InEquGroup"] != null && Request["PageEvent"] == "Query")
            {
                this.SetQuery();
                string js = Sa.Web.Fun.ControlToJavaScript(this);                
                js = "<script src='EquGroupEdit.js'></script>";
                js+= "<script src=\"../../../uc/QueryTool.js\" Type=\"text/javascript\"></script>\n";
                ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);                
            }
            if (Request["InEquGroup"] != null && Request["PageEvent"] == "Save")
            {
                this.SetSave();
                this.SetQuery();
            }
        }

        void SetSave()
        {
            //要進行儲存的EquID
            string[] EquIds = Request.Form.GetValues("CHK_COL_1");
            string[] EquGrpIDs = Request.Form.GetValues("EquGrpID");
            //取得Insert Data
            var insert_group = this.odo.GetQueryResult<EquGroupData>(@"SELECT *,@EquGrpID AS EquGrpID FROM B01_EquData 
                WHERE EquID NOT IN (SELECT EquID FROM B01_EquGroupData WHERE EquGrpID=@EquGrpID) AND EquID IN @EquID"
                , new { EquID = EquIds, EquGrpID = Request["InputEquGrp"] });
            foreach (var o in insert_group)
            {
                var rule = Request[string.Format("CardRule_{0}", o.EquID)].ToString();
                if (rule == "" || rule == "null")
                    rule = "0";
                o.CardRule = rule;
                o.CardExtData = Request[string.Format("CardExtData_{0}", o.EquID)].ToString();
                o.CreateTime = DateTime.Now;
                o.CreateUserID = "Saho";
                if (o.CardExtData == "" && o.EquClass == "Elevator")
                {
                    o.CardExtData = "000000000000";
                }
            }
            this.odo.Execute(@"INSERT INTO B01_EquGroupData (EquGrpID,EquID,CardRule,CreateUserID,CreateTime,CardExtData) 
                VALUES (@EquGrpID,@EquID,@CardRule,@CreateUserID,@CreateTime,@CardExtData) ", insert_group);
            //取得要修改的 Update Data
            var update_group = this.odo.GetQueryResult<EquGroupData>(@"SELECT A.*,B.EquClass 
                FROM B01_EquGroupData A INNER JOIN B01_EquData B ON A.EquID=B.EquID  WHERE EquGrpID = @EquGrpID AND A.EquID IN @EquID",
                new { EquID = EquIds, EquGrpID = Request["InputEquGrp"] });
            foreach (var o in update_group)
            {
                var rule = Request[string.Format("CardRule_{0}", o.EquID)].ToString();
                if (rule == "" || rule == "null")
                    rule = "0";
                o.CardRule = rule;
                o.CardExtData = Request[string.Format("CardExtData_{0}", o.EquID)].ToString();
                o.UpdateTime = DateTime.Now;
                o.UpdateUserID = "Saho";
                if (o.CardExtData == "" && o.EquClass == "Elevator")
                {
                    o.CardExtData = "000000000000";
                }
            }
            this.odo.Execute(@"UPDATE B01_EquGroupData SET CardRule=@CardRule,UpdateTime=@UpdateTime,UpdateUserID=@UpdateUserID,CardExtData=@CardExtData 
                                            WHERE EquID=@EquID AND EquGrpID=@EquGrpID ", update_group);
            //刪除EquID            
            if (Request["CHK_COL_1"] != null)
            {
                this.odo.Execute("DELETE B01_EquGroupData WHERE EquID NOT IN @EquID AND EquGrpID=@EquGrpID "
                , new { EquID = EquIds, EquGrpID = Request["InputEquGrp"] });
            }
            else
            {
                this.odo.Execute("DELETE B01_EquGroupData WHERE EquGrpID=@EquGrpID ",new { EquGrpID = Request["InputEquGrp"] });
            }
        }

        void SetQuery()
        {
            var grouplist = this.odo.GetQueryResult("SELECT * FROM B01_EquGroup WHERE EquGrpNo=@EquGrpNo", new { EquGrpNo = Request["InEquGroup"] });
          
            if (grouplist.Count() > 0)
            {
                grpid = Convert.ToInt32(grouplist.First().EquGrpID);
                this.EquName = Convert.ToString(grouplist.First().EquGrpName);
            }
            this.groupdata = this.odo.GetQueryResult<EquGroupData>(@"SELECT B.EquNo, B.EquName, B.EquClass, A.* FROM B01_EquGroupData A 
                INNER JOIN B01_EquData B ON A.EquID = B.EquID WHERE EquGrpID=@EquGrpID", new { EquGrpID = grpid }).ToList();
            this.nonusedata = this.odo.GetQueryResult<EquGroupData>(@"SELECT * FROM B01_EquData 
                WHERE EquID NOT IN (SELECT EquID FROM B01_EquGroupData WHERE EquGrpID=@EquGrpID)", new { EquGrpID = grpid }).ToList();
            this.groupdata = this.groupdata.Union(this.nonusedata).OrderBy(i => i.EquNo).OrderByDescending(i => i.EquGrpID).ToList();
            string ruledef_str = @"SELECT RuleID,RuleNo,RuleName,EquModel FROM B01_CardRuleDef";
            List<CardRuleData> rules = odo.GetQueryResult<CardRuleData>(ruledef_str).ToList();
            List<CardRuleData> rule_paras = odo.GetQueryResult<CardRuleData>(@"SELECT D.EquID,ParaValue,E.EquModel FROM 
	            B01_EquParaData D 
	            INNER JOIN B01_EquParaDef E  ON D.EquParaID=E.EquParaID
                WHERE ParaName = 'CardRule' AND ParaValue<>'' ").ToList();
            foreach (var s in rule_paras)
            {
                foreach (var para in s.ParaValue.Split(','))
                {
                    if (para.Split(':').Length >= 2)
                    {
                        string para_id = para.Split(':')[0];
                        int rule_id = int.Parse(para.Split(':')[1]);
                        this.RuleResult.Add(new CardRuleData()
                        {
                            EquID = s.EquID,
                            EquModel = s.EquModel,
                            RuleName = rules.Where(i => i.RuleID == rule_id).Count() > 0 ? rules.Where(i => i.RuleID == rule_id).FirstOrDefault().RuleName : "",
                            RuleNo = rules.Where(i => i.RuleID == rule_id).Count() > 0 ? rules.Where(i => i.RuleID == rule_id).FirstOrDefault().RuleNo : "",
                            RuleID = int.Parse(para_id),
                            ParaValue = para_id.ToString()
                        });
                    }
                }//end 針對讀卡規則清單進行設定 
            }//end 取得各個設備的讀卡規則參數設定
        }


    }//end page class
}//end namespace