using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using Sa.DB;
using Sa.Web;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;

namespace SahoAcs.Web._01._0102
{
    public partial class OrgGroupEdit :Sa.BasePage
    {


        public DataTable DataResult1 = new DataTable();
        public DataTable DataResult2 = new DataTable();

        string sql = "";

        public List<OrgEquGroup> in_group = new List<OrgEquGroup>();
        public List<OrgEquGroup> out_group = new List<OrgEquGroup>();
        public List<EquAdj> in_adj = new List<EquAdj>();
        public List<EquAdj> out_adj = new List<EquAdj>();
        public List<CardRuleData> RuleResult = new List<CardRuleData>();
        string LogIp = "";
        public SysLogEntity syslog_dto = new SysLogEntity();
        public OrmDataObject odo;
        string card_no = "";
        string orgstruc_no = "";
        
        List<string> err_list = new List<string>();
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Request["DoAction"] != null && Request["DoAction"].ToString() == "Query")
            {
                this.syslog_dto.UserID = Session["UserId"].ToString();
                SetQuery();
            }
            else if (Request["DoAction"] != null && Request["DoAction"].ToString() == "Save")
            {
                this.odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
                this.LogIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];            
                this.syslog_dto.LogType = DB_Acs.Logtype.門禁系統資料維護.ToString();
                this.syslog_dto.UserID = Session["UserId"].ToString();
                this.syslog_dto.UserName = Session["UserName"].ToString();
                this.syslog_dto.LogIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                this.syslog_dto.LogFrom = "0102";
                //SetSave();
                
                
                try
                {
                    this.SetGroupWork();
                    this.SetAdjWork();
                    if (odo.DbExceptionMessage.Length > 0)
                    {
                        this.err_list.Add(odo.DbExceptionMessage);
                    }
                }
                catch (Exception ex)
                {
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                   new { message = "OK",error_msg=ex.Message }));
                    Response.End();
                }
                if (this.err_list.Count==0)
                {
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "OK" }));
                    Response.End();
                }
                else
                {
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "Error",ErrList=this.err_list}));
                    Response.End();
                }
                
            }
            else
            {

            }
        }//end page_load

        
        private void SetGroupWork()
        {            
            List<OrgStrucInfo> infos = this.odo.GetQueryResult<OrgStrucInfo>("SELECT * FROM B01_OrgStruc WHERE OrgStrucID=@OrgStrucID"
                , new { OrgStrucID = Request["OrgStrucID"] }).ToList();
            this.orgstruc_no = infos.FirstOrDefault().OrgStrucNo;
            
            //取得要加入的設備群組ID
            string[] in_grp = Request.Form.GetValues("InCardEquGroup");
            //取得要加入的設備
            List<string> in_equ = Request["EquIDAll"].Split(',').ToList();
            if (in_equ == null)
            {
                in_equ = new List<string>();
            }
            else if (in_grp == null)
            {
                in_grp = new string[] { };
            }
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //var card_no = oAcsDB.GetStrScalar("SELECT CardNo FROM B01_Card WHERE CardID=? ", new List<string>() { "S:" + Request["CardID"] });            
            string cmdstr = @"SELECT EG.*,OEG.OrgStrucID FROM 
	                                        B01_EquGroup EG 
	                                        INNER JOIN B01_OrgEquGroup OEG ON EG.EquGrpID=OEG.EquGrpID AND OrgStrucID=@OrgStrucID 
                                            AND EG.EquGrpID IN (SELECT EquGrpID FROM B01_MgnEquGroup A INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID WHERE UserID=@UserID) ";                       
            List<OrgEquGroup> groups = new List<OrgEquGroup>();
            groups = this.odo.GetQueryResult<OrgEquGroup>(cmdstr,new {OrgStrucID=Request["OrgStrucID"], UserID=this.syslog_dto.UserID }).ToList();
            //取得新加入的群組編號清單        
            var add_lists = in_grp.Where(i => !groups.Select(j => j.EquGrpID.ToString()).Contains(i)).ToList();
            List<OrgEquGroup> insert_group = new List<OrgEquGroup>();
            foreach (var s in add_lists)
            {
                insert_group.Add(new OrgEquGroup() { OrgStrucID = Convert.ToInt32(Request["OrgStrucID"]), EquGrpID = Convert.ToInt32(s), CreateUserID = Session["UserId"].ToString() });
            }
            this.odo.Execute("INSERT INTO B01_OrgEquGroup (OrgStrucID,EquGrpID,CreateUserID) VALUES (@OrgStrucID,@EquGrpID,@CreateUserID)",insert_group);
            if (!this.odo.isSuccess)
                this.err_list.Add(this.odo.DbExceptionMessage);
            //建立log
            List<EquGroupLog> logs = new List<EquGroupLog>();
            insert_group.ForEach(delegate (OrgEquGroup o)
            {
                //logs.Add(new OrgEquGroup() {OrgStrucID=card_no,EquGrpID=o.EquGrpID,EquGrpNo=o.EquGrpNo,Action="ADD"});
            });           
            //取得要移除的群組
            var del_groups = groups.Where(i => !in_grp.Contains(i.EquGrpID.ToString())).ToList();
            del_groups.ForEach(delegate (OrgEquGroup o)
            {
                //logs.Add(new EquGroupLog() { CardNo = card_no, EquGrpID = o.EquGrpID, EquGrpNo = o.EquGrpNo, Action = "Remove" });
                o.OrgStrucID = Convert.ToInt32(Request["OrgStrucID"]);
            });
            this.odo.Execute("DELETE B01_OrgEquGroup WHERE OrgStrucID=@OrgStrucID AND EquGrpID=@EquGrpID", del_groups);
            if (!this.odo.isSuccess)
                this.err_list.Add(this.odo.DbExceptionMessage);
            //this.odo.SetSysLogCreate(this.syslog_dto, logs);
        }

        private void SetAdjWork()
        {
            List<OrgStrucInfo> infos = this.odo.GetQueryResult<OrgStrucInfo>("SELECT * FROM B01_OrgStruc WHERE OrgStrucID=@OrgStrucID"
                , new { OrgStrucID = Request["OrgStrucID"] }).ToList();
            this.orgstruc_no = infos.FirstOrDefault().OrgStrucNo;
            
            //取得要加入的設備
            List<string> in_equ = Request["EquIDAll"].Split(',').ToList();
            if (in_equ == null)
            {
                in_equ = new List<string>();
            }
            var cmdstr = @"SELECT OEJ.* FROM 
	                                        B01_OrgEquAdj OEJ 
	                                        INNER JOIN B01_EquData ED ON OEJ.EquID=ED.EquID AND OrgStrucID=@OrgStrucID AND ED.EquID IN  (SELECT EquID FROM B01_MgnEquGroup A 
                                                    INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID 
                                                    INNER JOIN B01_EquGroupData C ON A.EquGrpID=C.EquGrpID
                                                    WHERE UserID=@userid)";
            List<EquAdj> equadjs = this.odo.GetQueryResult<EquAdj>(cmdstr, new { OrgStrucID = Request["OrgStrucID"], userid=this.syslog_dto.UserID}).ToList();
            var update_equs = in_equ.Where(i => equadjs.Select(j => j.EquID.ToString()).Contains(i)).ToList();
            var insert_equs = in_equ.Where(i => !equadjs.Select(j => j.EquID.ToString()).Contains(i)).ToList();
            List<EquAdj> all_equdata = this.odo.GetQueryResult<EquAdj>("SELECT * FROM B01_EquData").ToList();
            List<EquAdj> modify_equ = new List<EquAdj>();
            List<EquAdjLog> logs = new List<EquAdjLog>();
            //List<EquAdj> delete_equ = new List<EquAdj>();
            foreach (var s in update_equs)
            {
                modify_equ.Add(new EquAdj()
                {
                    OrgStrucID = Convert.ToInt32(Request["OrgStrucID"]),
                    EquID = Convert.ToInt32(s),
                    OpMode = (Request["OpMode_" + s].ToString().Equals("1") ? "+" : "-"),
                    CardExtData = Request["CardExtData_" + s].ToUpper().Trim() != "" ? Request["CardExtData_" + s].ToUpper() : "",
                    CardRule = Request["CardRule_" + s].ToString(),
                    UpdateUserID = Session["UserId"].ToString(),
                    EquNo=all_equdata.Where(i=>i.EquID==Convert.ToInt32(s)).FirstOrDefault().EquNo,
                    EquName = all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquName
                });
                if (all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquClass == "Elevator" && modify_equ.LastOrDefault().CardExtData == "")
                {
                    modify_equ.LastOrDefault().CardExtData = "000000000000";
                }
            }
            odo.Execute(@"UPDATE B01_OrgEquAdj SET OpMode=@OpMode,CardExtData=@CardExtData,CardRule=@CardRule
                                            WHERE EquID=@EquID AND OrgStrucID=@OrgStrucID", modify_equ);
            if (!this.odo.isSuccess)
                this.err_list.Add(this.odo.DbExceptionMessage);
            //logs.AddRange(modify_equ.GetEquAdjLogsPackage("Update", card_no));

            modify_equ.Clear();
            foreach (var s in insert_equs)
            {
                if (s != "")
                {
                    modify_equ.Add(new EquAdj()
                    {
                        OrgStrucID = Convert.ToInt32(Request["OrgStrucID"]),
                        EquID = Convert.ToInt32(s),
                        OpMode = (Request["OpMode_" + s].ToString().Equals("1") ? "+" : "-"),
                        CardExtData = Request["CardExtData_" + s].ToUpper().Trim() != "" ? Request["CardExtData_" + s].ToUpper() : "",
                        CardRule = Request["CardRule_" + s].ToString(),
                        CreateUserID = Session["UserId"].ToString(),
                        EquNo = all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquNo,
                        EquName = all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquName
                    });
                    if (all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquClass == "Elevator" && modify_equ.LastOrDefault().CardExtData == "")
                    {
                        modify_equ.LastOrDefault().CardExtData = "000000000000";
                    }
                }
            }
            this.odo.Execute(@"INSERT B01_OrgEquAdj (OrgStrucID,EquID,OpMode,CardExtData,CardRule,CreateUserID) 
                                                                    VALUES (@OrgStrucID,@EquID,@OpMode,@CardExtData,@CardRule,@CreateUserID)", modify_equ);
            if (!this.odo.isSuccess)
                this.err_list.Add(this.odo.DbExceptionMessage);
            //logs.AddRange(modify_equ.GetEquAdjLogsPackage("Add", card_no));

            //Delete EquAdj            
            var del_equs = equadjs.Where(i => !in_equ.Contains(i.EquID.ToString())).ToList();
            modify_equ.Clear();
            foreach (var s in del_equs)
            {
                modify_equ.Add(new EquAdj()
                {
                    OrgStrucID = Convert.ToInt32(Request["OrgStrucID"]),
                    EquID = Convert.ToInt32(s.EquID),EquNo=s.EquNo,EquName=s.EquName
                });
            }
            odo.Execute(@"DELETE B01_OrgEquAdj WHERE OrgStrucID=@OrgStrucID AND EquID=@EquID", modify_equ);
            if (!this.odo.isSuccess)
                this.err_list.Add(this.odo.DbExceptionMessage);
            //logs.AddRange(modify_equ.GetEquAdjLogsPackage("Remove", card_no));
            //this.odo.SetSysLogCreate(this.syslog_dto, logs);             
        }

        private void SetQuery()
        {
            this.odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            var OrgStrucID = Request["OrgStrucID"].ToString();
            var cmd_str1 = @"SELECT 
	                                        EG.* 
	                                        ,OEG.OrgStrucID
                                        FROM 
	                                        B01_EquGroup EG 
	                                        LEFT JOIN B01_OrgEquGroup OEG ON EG.EquGrpID=OEG.EquGrpID AND OrgStrucID=@OrgStrucID 
                                            WHERE OEG.OrgStrucID IS NULL AND EG.OwnerList like @OwnerList
                                             AND EG.EquGrpID IN (SELECT EquGrpID FROM B01_MgnEquGroup A INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID WHERE UserID=@UserID)";

            var cmd_str2 = @"SELECT 
	                                        EG.* 
	                                        ,OEG.OrgStrucID
                                        FROM 
	                                        B01_EquGroup EG 
	                                        LEFT JOIN B01_OrgEquGroup OEG ON EG.EquGrpID=OEG.EquGrpID AND OrgStrucID=@OrgStrucID WHERE OEG.OrgStrucID IS NOT NULL 
                                            AND EG.EquGrpID IN (SELECT EquGrpID FROM B01_MgnEquGroup A INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID WHERE UserID=@UserID) ";
            //取得未加入的群組清單 by Sam 2022/8/8
            out_group = this.odo.GetQueryResult<OrgEquGroup>(cmd_str1, new
            {
                OrgStrucID = OrgStrucID,
                UserID = this.syslog_dto.UserID
                                            ,
                OwnerList = string.Format("%\\{0}\\%", this.syslog_dto.UserID)
            }).ToList();

            in_group = this.odo.GetQueryResult<OrgEquGroup>(cmd_str2, new { OrgStrucID = OrgStrucID, UserID = this.syslog_dto.UserID }).ToList();
            var cmd_str3 = @"SELECT * FROM B01_EquData WHERE EquID IN  (SELECT EquID FROM B01_MgnEquGroup A 
                                                    INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID 
                                                    INNER JOIN B01_EquGroupData C ON A.EquGrpID=C.EquGrpID
                                                    WHERE UserID=@userid)";
            this.in_adj = this.odo.GetQueryResult<EquAdj>(cmd_str3, new { userid = this.syslog_dto.UserID }).ToList();
            var temp_orgequgroup = this.odo.GetQueryResult<EquAdj>(@"SELECT EGD.* FROM B01_EquData ED 
                            INNER JOIN B01_EquGroupData EGD ON ED.EquID = EGD.EquID
                            INNER JOIN B01_OrgEquGroup OEG ON OEG.EquGrpID = EGD.EquGrpID
                            WHERE OrgStrucID = @OrgStrucID", new { OrgStrucID = OrgStrucID }).ToList();
            var temp_orgequadj = this.odo.GetQueryResult<EquAdj>(@"SELECT * FROM B01_OrgEquAdj WHERE OrgStrucID=@OrgStrucID", new { OrgStrucID = OrgStrucID }).ToList();
            foreach (var o in this.in_adj)
            {
                if (temp_orgequadj.Where(i => i.EquID == o.EquID).Count() > 0)
                {
                    o.OpMode = temp_orgequadj.Where(i => i.EquID == o.EquID).First().OpMode;
                    o.CardRule = temp_orgequadj.Where(i => i.EquID == o.EquID).First().CardRule;
                    o.CardExtData = temp_orgequadj.Where(i => i.EquID == o.EquID).First().CardExtData;
                    if (o.OpMode == "-")
                    {
                        o.OrgStrucID = 0;
                    }
                    else
                    {
                        o.OrgStrucID = int.Parse(OrgStrucID);
                    }
                }
                else if (temp_orgequgroup.Where(i => i.EquID == o.EquID).Count() > 0)
                {
                    o.OpMode = "";
                    if (temp_orgequgroup.Where(i => i.EquID == o.EquID).Count() == 1)
                    {//資料剛好等於1，才做賦值
                        o.CardRule = temp_orgequgroup.Where(i => i.EquID == o.EquID).First().CardRule;
                        o.CardExtData = temp_orgequgroup.Where(i => i.EquID == o.EquID).First().CardExtData;
                    }
                    o.OrgStrucID = int.Parse(OrgStrucID);
                }
                else
                {
                    o.OrgStrucID = 0;
                }
            }
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
                }
            }
        }

    }//end class
}//end namespace