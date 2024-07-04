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

namespace SahoAcs.Web._01._0103
{
    public partial class CardGroupEdit : Sa.BasePage
    {
        public class EquGroup
        {
            public string GroupName { get; set; }
            public int EquGrpID { get; set; }
            public string EquGrpNo { get; set; }
            public int? CardID { get; set; }
            public string CreateUserID { get; set; }
            public string UpdateUserID { get; set; }
        }
        

        public class CardInfo
        {
            public int CardID { get; set; }
            public string CardNo { get; set; }
            public DateTime CardSTime { get; set; }
            public DateTime CardETime { get; set; }
        }

        public class CardRuleData
        {
            public int RuleID { get; set; }
            public string RuleNo { get; set; }
            public string RuleName { get; set; }
            public string EquModel { get; set; }
            public int EquID { get; set; }
            public int RecordID { get; set; }
            public string ParaValue { get; set; }
        }


        public DataTable DataResult1 = new DataTable();
        public DataTable DataResult2 = new DataTable();

        string sql = "";

        public List<EquGroup> in_group = new List<EquGroup>();
        public List<EquGroup> out_group = new List<EquGroup>();
        public List<EquAdj> in_adj = new List<EquAdj>();
        public List<EquAdj> out_adj = new List<EquAdj>();
        public List<CardRuleData> RuleResult = new List<CardRuleData>();
        string LogIp = "";
        public SysLogEntity syslog_dto = new SysLogEntity();
        public OrmDataObject odo;
        string user_id = "";
        string card_no = "";
        List<string> err_list = new List<string>();
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            this.user_id = Session["UserId"].ToString();
            if (Request["DoAction"] != null && Request["DoAction"].ToString() == "Query")
            {                
                SetQuery();
            }
            else if (Request["DoAction"] != null && Request["DoAction"].ToString() == "Setting")
            {                
                this.SetVerifiMode();
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                  new { message = "OK" }));
                Response.End();
            }
            else if (Request["DoAction"] != null && Request["DoAction"].ToString() == "GetAuth")
            {
                //this.SetVerifiMode();
                Response.Clear();
                Response.Write(this.GetCardAuthStatus());
                Response.End();
            }
            else if (Request["DoAction"] != null && Request["DoAction"].ToString() == "Save")
            {
                this.odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
                this.LogIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                this.syslog_dto.LogType = DB_Acs.Logtype.卡片權限調整.ToString();
                this.syslog_dto.UserID = Session["UserId"].ToString();
                this.syslog_dto.UserName = Session["UserName"].ToString();
                this.syslog_dto.LogIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                this.syslog_dto.LogFrom = "0103";
                this.syslog_dto.LogDesc = "人員基本資料，設備權限調整";
                //SetSave();
                Response.Clear();
               
                try
                {
                    this.SetGroupWork();
                    this.SetAdjWork();
                    odo.Execute(@"EXEC CardAuth_Update @sCardNo = @CardNo,@sUserID = @UserID,
                                                        @sFromProc = @UserID,@sFromIP = '127.0.0.1',@sOpDesc = '一般權限重整'", new { CardNo = this.card_no, UserID = Session["UserId"].ToString() });
                    odo.SetSysLogCreateByEquAuth(this.syslog_dto, Convert.ToInt32(Request["CardID"]), this.card_no);
                    if (odo.DbExceptionMessage.Length > 0)
                    {
                        this.err_list.Add(odo.DbExceptionMessage);
                    }
                    //設定驗證模式  20190426 設定靜電測試的預設驗證模式
                    this.SetVerifiData();
                }
                catch (Exception ex)
                {
                    this.err_list.Add(ex.GetBaseException().Message);
                }
                if (this.err_list.Count == 0)
                {
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "OK" }));
                }
                else
                {
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "Error", ErrList = this.err_list }));
                }
                Response.End();
            }
            else
            {

            }
        }//end page_load


        private void SetVerifiData()
        {
            //取得要加入的設備
            List<string> in_equ = Request["EquIDAll"].Split(',').ToList();
            foreach (var equid in in_equ)
            {
                if (Request.Form["VerifiMode_"+equid]!=null && Request.Form["VerifiMode_" + equid] != "0")
                {
                    this.odo.Execute(@"UPDATE B01_CardAuth SET OpMode = 'Reset', OpStatus = '', ErrCnt = 0, VerifiMode =@VerifiMode, UpdateTime = GETDATE(), UpdateUserID =@UserID
                                        WHERE CardNo =@CardNo AND EquID =@EquID AND OpMode <> 'Del'", new { CardNo = this.card_no, EquID = equid, VerifiMode = Request["VerifiMode_" + equid], UserID = Session["UserID"] });
                }
            }
        }


        public void SetVerifiMode()
        {
            List<CardInfo> infos = this.odo.GetQueryResult<CardInfo>("SELECT * FROM B01_Card WHERE CardID=@CardID"
                , new { CardID = Request["CardID"] }).ToList();
            this.card_no = infos.FirstOrDefault().CardNo;
            string[] equs = Request.Form.GetValues("EquID");
            foreach(var equ in equs)
            {
                if (Request["VerifiMode_" + equ] != null)
                {

                    this.odo.Execute(@"UPDATE B01_CardAuth SET OpMode = 'Reset', OpStatus = '', ErrCnt = 0, VerifiMode =@VerifiMode, UpdateTime = GETDATE(), UpdateUserID =@UserID
                                        WHERE CardNo =@CardNo AND EquID =@EquID AND OpMode <> 'Del'"
                    ,new {
                        CardNo=this.card_no, UserID=Session["UserID"].ToString(), EquID=equ, VerifiMode=String.IsNullOrEmpty(Request["VerifiMode_"+equ].Trim())?"0": Request["VerifiMode_" + equ].Trim()
                    });
                }
            }
        }


        private void SetGroupWork()
        {            
            List<CardInfo> infos = this.odo.GetQueryResult<CardInfo>("SELECT * FROM B01_Card WHERE CardID=@CardID"
                , new { CardID = Request["CardID"] }).ToList();
            this.card_no = infos.FirstOrDefault().CardNo;
            //取得要加入的設備群組ID
            string[] in_grp = Request.Form.GetValues("InCardEquGroup");
            if (in_grp == null)
            {
                in_grp = new string[] { };
            }
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //var card_no = oAcsDB.GetStrScalar("SELECT CardNo FROM B01_Card WHERE CardID=? ", new List<string>() { "S:" + Request["CardID"] });            
            string cmdstr = @"SELECT EG.*,CEG.CardID FROM 
	                                        B01_EquGroup EG 
	                                        INNER JOIN B01_CardEquGroup CEG ON EG.EquGrpID=CEG.EquGrpID AND CardID=@CardID 
                                            WHERE EG.EquGrpID IN (SELECT EquGrpID FROM B01_MgnEquGroup A INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID WHERE UserID=@UserID)";
            var AllEquGroup = this.odo.GetQueryResult<EquGroup>("SELECT * FROM B01_EquGroup");
            List<EquGroup> groups = new List<EquGroup>();
            groups = this.odo.GetQueryResult<EquGroup>(cmdstr,new {CardID=Request["CardID"], UserID=user_id}).ToList();
            //取得新加入的群組編號清單        
            var add_lists = in_grp.Where(i => !groups.Select(j => j.EquGrpID.ToString()).Contains(i)).ToList();
            List<EquGroup> insert_group = new List<EquGroup>();
            foreach (var s in add_lists)
            {
                insert_group.Add(new EquGroup() { CardID = Convert.ToInt32(Request["CardID"]), EquGrpID = Convert.ToInt32(s), CreateUserID = Session["UserId"].ToString() });
                this.odo.SetSysLogCreateByNo(Convert.ToInt32(Request["CardID"]), this.card_no, AllEquGroup.Where(g=>g.EquGrpID==Convert.ToInt32(s)).First().EquGrpNo,"Add EquGroup ", "0103");
            }
            if (insert_group.Count > 0)
            {
                this.odo.Execute("INSERT INTO B01_CardEquGroup (CardID,EquGrpID,CreateUserID) VALUES (@CardID,@EquGrpID,@CreateUserID)", insert_group);
            }                
            if (!this.odo.isSuccess)
            {
                this.err_list.Add(this.odo.DbExceptionMessage);
            }                
            //取得要移除的群組
            var del_groups = groups.Where(i => !in_grp.Contains(i.EquGrpID.ToString())).ToList();
            foreach(var o in del_groups)
            {
                this.odo.SetSysLogCreateByNo(o.CardID, this.card_no, o.EquGrpNo, " Delete EquGruop ", "0103");
            }
            this.odo.Execute(@"DELETE B01_CardEquGroup WHERE CardID=@CardID AND EquGrpID=@EquGrpID ", del_groups);
            if (!this.odo.isSuccess)
                this.err_list.Add(this.odo.DbExceptionMessage);            
        }


        private void SetAdjWork()
        {
            List<CardInfo> infos = this.odo.GetQueryResult<CardInfo>("SELECT * FROM B01_Card WHERE CardID=@CardID"
                , new { CardID = Request["CardID"] }).ToList();
            this.card_no = infos.FirstOrDefault().CardNo;
            
            //取得要加入的設備
            List<string> in_equ = Request["EquIDAll"].Split(',').ToList();
            if (in_equ == null)
            {
                in_equ = new List<string>();
            }
            var cmdstr = @"SELECT CEJ.*,EquNo,EquName FROM 
	                                        B01_CardEquAdj CEJ 
	                                        INNER JOIN B01_EquData ED ON CEJ.EquID=ED.EquID AND CardID=@CardID ";
            List<EquAdj> equadjs = this.odo.GetQueryResult<EquAdj>(cmdstr, new { CardID = Request["CardID"] }).ToList();
            var update_equs = in_equ.Where(i => equadjs.Select(j => j.EquID.ToString()).Contains(i)).ToList();
            var insert_equs = in_equ.Where(i => !equadjs.Select(j => j.EquID.ToString()).Contains(i)).ToList();
            List<EquAdj> all_equdata = this.odo.GetQueryResult<EquAdj>("SELECT * FROM B01_EquData").ToList();       //所有設備資料
            List<EquAdj> modify_equ = new List<EquAdj>();
            List<EquAdjLog> logs = new List<EquAdjLog>();
            //List<EquAdj> delete_equ = new List<EquAdj>();
            foreach (var s in update_equs)
            {
                modify_equ.Add(new EquAdj()
                {
                    CardID = Convert.ToInt32(Request["CardID"]),
                    EquID = Convert.ToInt32(s),
                    OpMode = (Request["OpMode_" + s].ToString().Equals("1") ? "+" : "-"),
                    CardExtData = Request["CardExtData_" + s].ToUpper().Trim() != ""  ? Request["CardExtData_" + s].ToUpper() : "",                                        
                    CardRule = Request["CardRule_" + s].ToString(),
                    UpdateUserID = Session["UserId"].ToString(),EquClass= all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquClass,
                    EquNo =all_equdata.Where(i=>i.EquID==Convert.ToInt32(s)).FirstOrDefault().EquNo,
                    EquName = all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquName
                });
                if(all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquClass == "Elevator"&&modify_equ.LastOrDefault().CardExtData=="")
                {
                    modify_equ.LastOrDefault().CardExtData = "000000000000";
                }
            }
            odo.Execute(@"UPDATE B01_CardEquAdj SET OpMode=@OpMode,CardExtData=@CardExtData,CardRule=@CardRule
                                            WHERE EquID=@EquID AND CardID=@CardID", modify_equ);
            modify_equ.ForEach(i =>
            {
                odo.SetSysLogCreateByNo(i.CardID, this.card_no, i.EquNo, "Update CardEquAdj OpMode" + i.OpMode, "0103");
            });
            if (!this.odo.isSuccess)
                this.err_list.Add(this.odo.DbExceptionMessage);
            logs.AddRange(modify_equ.GetEquAdjLogsPackage("Update", card_no));
            modify_equ.Clear();
            foreach (var s in insert_equs)
            {
                if (s != "")
                {
                    modify_equ.Add(new EquAdj()
                    {
                        CardID = Convert.ToInt32(Request["CardID"]),
                        EquID = Convert.ToInt32(s),
                        OpMode = (Request["OpMode_" + s].ToString().Equals("1") ? "+" : "-"),
                        CardExtData = Request["CardExtData_" + s].ToUpper().Trim() != "" ? Request["CardExtData_" + s].ToUpper() : "",
                        CardRule = Request["CardRule_" + s].ToString(),
                        CreateUserID = Session["UserId"].ToString(),
                        EquClass = all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquClass,
                        EquNo = all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquNo,
                        EquName = all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquName
                    });
                    if (all_equdata.Where(i => i.EquID == Convert.ToInt32(s)).FirstOrDefault().EquClass == "Elevator" && modify_equ.LastOrDefault().CardExtData == "")
                    {
                        modify_equ.LastOrDefault().CardExtData = "000000000000";
                    }
                }
            }
            this.odo.Execute(@"INSERT B01_CardEquAdj (CardID,EquID,OpMode,CardExtData,CardRule,CreateUserID) 
                                                                    VALUES (@CardID,@EquID,@OpMode,@CardExtData,@CardRule,@CreateUserID)", modify_equ);
            modify_equ.ForEach(i =>
            {
                odo.SetSysLogCreateByNo(i.CardID, this.card_no, i.EquNo, "Add CardEquAdj OpMode" + i.OpMode, "0103");
            });
            if (!this.odo.isSuccess)
                this.err_list.Add(this.odo.DbExceptionMessage);
            logs.AddRange(modify_equ.GetEquAdjLogsPackage("Add", card_no));

            //增加管理區內的設備
            var cmd_str4 = @"SELECT ED.* FROM 
	                                                B01_EquData ED 	                                                
                                                    WHERE ED.EquID IN
                                                    (SELECT EquID FROM B01_MgnEquGroup A 
                                                    INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID 
                                                    INNER JOIN B01_EquGroupData C ON A.EquGrpID=C.EquGrpID
                                                    WHERE UserID=@userid) ";
            var mngequ = odo.GetQueryResult<EquAdj>(cmd_str4, new { userid = this.user_id }).ToList();
            //Delete EquAdj            
            var del_equs = equadjs.Where(i => !in_equ.Contains(i.EquID.ToString()) && mngequ.Select(mga=>mga.EquID).Contains(i.EquID)).ToList();
            modify_equ.Clear();
            foreach (var s in del_equs)
            {
                modify_equ.Add(new EquAdj()
                {
                    CardID = Convert.ToInt32(Request["CardID"]),
                    EquID = Convert.ToInt32(s.EquID),EquNo=s.EquNo,EquName=s.EquName
                });
            }
            odo.Execute(@"DELETE B01_CardEquAdj WHERE CardID=@CardID AND EquID=@EquID", modify_equ);
            modify_equ.ForEach(i =>
            {
                odo.SetSysLogCreateByNo(i.CardID, this.card_no, i.EquNo, "Delete CardEquAdj ", "0103");
            });
            if (!this.odo.isSuccess)
                this.err_list.Add(this.odo.DbExceptionMessage);
            logs.AddRange(modify_equ.GetEquAdjLogsPackage("Remove", card_no));
            //this.odo.SetSysLogCreate(this.syslog_dto, logs);                                     
        }


        private void SetQuery()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<CardInfo> infos = this.odo.GetQueryResult<CardInfo>("SELECT * FROM B01_Card WHERE CardID=@CardID"
                , new { CardID = Request["card_id"] }).ToList();
            if (infos.Count > 0)
            {
                this.card_no = infos.First().CardNo;
                this.popLabel_OrgList.Text = "「" + this.card_no + "」" + this.popLabel_OrgList.Text;
                this.Label2.Text = "「" + this.card_no + "」" + this.Label2.Text;
            }
            var card_id = Request["card_id"].ToString();
            var cmd_str1 = @"SELECT 
	                                        EG.* 
	                                        ,CEG.CardID
                                        FROM 
	                                        B01_EquGroup EG 
	                                        LEFT JOIN B01_CardEquGroup CEG ON EG.EquGrpID=CEG.EquGrpID AND CardID=?                                             
                                            WHERE CEG.CardID IS NULL AND 
                                            EG.EquGrpID IN (SELECT EquGrpID FROM B01_MgnEquGroup A INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID WHERE UserID=?) ";
            var cmd_str2 = @"SELECT 
	                                        EG.* 
	                                        ,CEG.CardID
                                        FROM 
	                                        B01_EquGroup EG 
	                                        LEFT JOIN B01_CardEquGroup CEG ON EG.EquGrpID=CEG.EquGrpID AND CardID=? WHERE CEG.CardID IS NOT NULL 
                                            AND EG.EquGrpID IN (SELECT EquGrpID FROM B01_MgnEquGroup A INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID WHERE UserID=?) ";            
            oAcsDB.GetDataTable("OutGroup", cmd_str1, new List<string>() { "S:" + card_id,"S:"+this.user_id }, out this.DataResult1);
            oAcsDB.GetDataTable("InGroup", cmd_str2, new List<string>() { "S:" + card_id,"S:"+this.user_id}, out this.DataResult2);
            foreach (DataRow r in this.DataResult1.Rows)
            {
                out_group.Add(new EquGroup()
                {
                    EquGrpID = Convert.ToInt32(r["EquGrpID"]),
                    EquGrpNo = Convert.ToString(r["EquGrpNo"]),
                    GroupName = Convert.ToString(r["EquGrpName"]),                    
                });
            }
            foreach (DataRow r in this.DataResult2.Rows)
            {
                in_group.Add(new EquGroup()
                {
                    EquGrpID = Convert.ToInt32(r["EquGrpID"]),
                    EquGrpNo = Convert.ToString(r["EquGrpNo"]),
                    GroupName = Convert.ToString(r["EquGrpName"])
                });
            }
            var cmd_str3 = @"SELECT 
	                                                ED.* 
	                                                ,CEJ.CardID
                                                    ,'' AS OpMode,CardExtData,BI.ItemInfo3
                                                FROM 
	                                                B01_EquData ED 
                                                    INNER JOIN B00_ItemList BI ON ED.EquModel=BI.ItemNo AND ItemClass='EquModel'
	                                                LEFT JOIN B01_CardEquAdj CEJ ON ED.EquID=CEJ.EquID AND CardID=? WHERE CEJ.CardID IS NULL  ";
            var cmd_str4 = @"SELECT 
	                                                ED.* 
	                                                ,CEJ.CardID,ISNULL(CA.CardRule,CEJ.CardRule) AS CardRule,ISNULL(CA.VerifiMode,0) AS VerifiMode
                                                    ,ISNULL(CEJ.OpMode,'') AS OpMode,ISNULL(CA.CardExtData,CEJ.CardExtData) AS CardExtData,ISNULL(CardNo,'') AS CardNo,BI.ItemInfo3
                                                FROM 
	                                                B01_EquData ED 
                                                    INNER JOIN B00_ItemList BI ON ED.EquModel=BI.ItemNo AND ItemClass='EquModel' AND ED.EquClass=BI.ItemInfo1
	                                                LEFT JOIN B01_CardEquAdj CEJ ON ED.EquID=CEJ.EquID AND CardID=@CardID 
													LEFT JOIN (SELECT EquID,CardNo,CardExtData,CardRule,VerifiMode FROM B01_CardAuth WHERE 
                                                    CardNo=(SELECT TOP 1 CardNo FROM B01_Card WHERE CardID=@CardID) AND OpMode<>'Del') CA ON ED.EquID=CA.EquID
                                                    WHERE ED.EquID IN
                                                    (SELECT EquID FROM B01_MgnEquGroup A 
                                                    INNER JOIN B00_SysUserMgns B ON A.MgaID=B.MgaID 
                                                    INNER JOIN B01_EquGroupData C ON A.EquGrpID=C.EquGrpID
                                                    WHERE UserID=@userid)
                                                ORDER BY CardNo DESC,EquNo ";
            //oAcsDB.GetDataTable("OutGroup", cmd_str3, new List<string>() { "S:" + card_id }, out this.DataResult1);
            //oAcsDB.GetDataTable("InGroup", cmd_str4, new List<string>() { "S:" + card_id, "S:" + card_id }, out this.DataResult2);
            OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            in_adj = odo.GetQueryResult<EquAdj>(cmd_str4,new { CardID = card_id,userid=this.user_id }).ToList();           
            string ruledef_str = @"SELECT RuleID,RuleNo,RuleName,EquModel FROM B01_CardRuleDef";
            List<CardRuleData> rules = odo.GetQueryResult<CardRuleData>(ruledef_str).ToList();
            List<CardRuleData> rule_paras = odo.GetQueryResult<CardRuleData>(@"SELECT D.EquID,ParaValue,E.EquModel FROM 
	            B01_EquParaData D 
	            INNER JOIN B01_EquParaDef E  ON D.EquParaID=E.EquParaID
                WHERE ParaName = 'CardRule' AND ParaValue<>'' ").ToList();            
            foreach(var s in rule_paras)
            {
                foreach(var para in s.ParaValue.Split(','))
                {
                    if (para.Split(':').Length >= 2)
                    {
                        string para_id = para.Split(':')[0];
                        int rule_id= int.Parse(para.Split(':')[1]);
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
            /*
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { in_group = in_group, out_group = out_group, in_adj = in_adj, out_adj = out_adj }));
            Response.End();
             */
        }


        private string GetCardAuthStatus()
        {
            var result = this.odo.GetQueryResult(@"SELECT * FROM B01_CardAuth WHERE CardNo=(SELECT TOP 1 CardNo FROM B01_Card WHERE CardID=@CardID) 
            AND OpMode<>'Del' AND EquID=@EquID AND OpStatus='Setted'",new {CardID=Request["CardID"],EquID=Request["EquID"]});
            string ResultMsg = "尚未完成設碼!!";
            foreach(var o in result)
            {
                ResultMsg = string.Format("設備於{0:yyyy/MM/dd HH:mm:ss}已完成設碼", o.CompleteTime);
            }
            return ResultMsg;
        }


    }//end class
}//end namespace