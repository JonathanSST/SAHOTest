using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs.Web
{
    public partial class CopyRule : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());

        public List<EquAdj> EquModelList = new List<EquAdj>();

        public string RuleName = "";

        public string RuleNo = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["DoAction"] == "SetDef")
            {
                var RuleList = this.odo.GetQueryResult("SELECT * FROM B01_CardRuleDef WHERE RuleNo=@RuleNo", new { RuleNo = Request["rule_no"] });
                foreach (var o in RuleList)
                {
                    string UpdateCardRuleDef = @"UPDATE B01_EquParaDef 
                            SET DefaultValue='0:'+Convert(varchar,@RuleID) WHERE EquModel=@EquModel AND ParaName='CardRule'";
                    this.odo.Execute(UpdateCardRuleDef, new { RuleID = o.RuleID, EquModel = Request["equ_model"] });
                }
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "OK", success = true }));
                Response.End();
            }

            if (Request["DoAction"] == "Query")
            {
                var rule = this.odo.GetQueryResult("SELECT * FROM B01_CardRuleDef WHERE RuleNo=@NO ", new {NO=Request["rule_id"] });
                this.EquModelList = this.odo.GetQueryResult<EquAdj>(@" SELECT A.ItemName AS EquName,A.ItemNo AS EquModel 
                    FROM B00_ItemList A INNER JOIN B01_EquParaDef B ON A.ItemNo=B.EquModel AND B.ParaName='CardRule'
                     WHERE ItemClass = 'EquModel'").ToList();
                if (rule.Count() > 0)
                {
                    RuleName =Convert.ToString(rule.First().RuleName);
                    RuleNo =Convert.ToString(rule.First().RuleNo);
                }
            }

            if (Request["DoAction"] == "CheckData")
            {
                this.RuleNo = Request["rule_no"];
                this.RuleName = Request["rule_name"];
                string EquModel = Request["equ_model"];

                var counts = this.odo.GetQueryResult("SELECT * FROM B01_CardRuleDef WHERE RuleNo=@NO", new { NO = this.RuleNo }).Count();

                Response.Clear();

                if (counts > 0)
                {
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "Error", success = false }));
                }
                else
                {                    
                    var source_data = this.odo.GetQueryResult("SELECT * FROM B01_CardRuleDef WHERE RuleNo=@NO", new {NO=Request["rule_id"]});
                    if (source_data.Count() > 0)
                    {
                        string RuleInfo = Convert.ToString(source_data.First().RuleInfo);
                        this.odo.Execute(@"INSERT INTO 
                                                             B01_CardRuleDef (EquModel,RuleNo,RuleName,RuleInfo,CreateUserID,CreateTime) 
                                                             VALUES (@EquModel,@RuleNo,@RuleName,@RuleInfo,@CreateUserID,GETSYSDATE())", 
                                                            new {EquModel=EquModel,RuleNo=this.RuleNo,RuleName=this.RuleName,RuleInfo=RuleInfo,CreateUserID=Request["user_id"]});
                    }
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "OK", success = true }));
                }               
                Response.End();
            }

        }
    }//end class
}//end namespace