using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;


namespace SahoAcs.Web
{
    public partial class PersonDateChange : System.Web.UI.Page
    {

        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        
        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {            
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nDefault('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("PersonDateChange", "PersonDateChange.js?"+Pub.GetNowTime);                                // 加入同一頁面所需的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");               // 加入頁面共用的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");               // 加入頁面共用的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");           // 加入頁面共用的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");     // 加入搭配 GridView 顯示光棒用的 JavaScript 檔案

            Input_Time.SetWidth(180);
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {            
            #region 註冊主頁Button動作            
            
            #endregion

            #region 註冊pop1頁Button動作
            //ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            //popB_OK1.Attributes["onClick"] = "LoadPsnDataList(); return false;";
            //popB_Cancel1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            //popB_Enter1.Attributes["onClick"] = "DataEnterRemove('Add'); return false;";
            //popB_Remove1.Attributes["onClick"] = "DataEnterRemove('Del'); return false;";
            //popB_Query.Attributes["onClick"] = "QueryPsnData(); return false;";
            #endregion

            Input_Time.SetWidth(180);
        }
        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                hUserId.Value = Session["UserID"].ToString();
                LoadProcess();
                RegisterObj();
                if (!IsPostBack)
                {
                    if(Request["DoAction"]!=null && Request["DoAction"] == "Query")
                    {
                        this.SetQueryPsnData();
                    }
                    if (Request["DoAction"] != null && Request["DoAction"] == "Exec")
                    {
                        this.SetExecProcData();
                    }
                }
                
            }
            else
            {
                Response.Redirect("~/Web/MessagePage/LoginSorry.aspx");
            }
        }

        #endregion

     

        #endregion

        #region JavaScript及aspx共用方法

        #region 載入人員資料

        public void SetQueryPsnData()
        {
            //判斷起始時間/結束時間
            //string chkPopType = Request["poptype"].ToString();
            //日期
            //string chkPsnTime = Request["popsettime"].ToString();
            string chkSqlTime = "";
            string chkDDLType = "PsnETime";
            chkDDLType = Request["ddlType"];
            chkSqlTime = " AND " + chkDDLType + " >= @PsnTimeS ";
            chkSqlTime += " AND " + chkDDLType + " <= @PsnTimeE ";
            string sql = @" SELECT DISTINCT(OrgStrucID),OrgStrucNo 
                FROM  (SELECT B00_SysUserMgns.UserID, B00_SysUserMgns.MgaID, B01_MgnOrgStrucs.OrgStrucID, B01_OrgStruc.OrgStrucNo, B01_OrgStruc.OrgIDList 
             FROM B00_SysUserMgns 
                INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID 
              INNER JOIN B01_OrgStruc ON B01_MgnOrgStrucs.OrgStrucID = B01_OrgStruc.OrgStrucID) a WHERE a.UserID = @UserID ";
            var OrgDataList = this.odo.GetQueryResult(sql,new {UserID=Request["UserID"]}).ToList();
            sql = @" SELECT DISTINCT TOP 100 (B01_Person.PsnID), B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName, B01_Person.PsnType,  
                            B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount, B01_Person.PsnPW,  
                            B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource, B01_Person.Remark,      
                            B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID, B01_Person.UpdateTime, B01_Person.Rev01,
                            B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo, OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList  
                            FROM B01_Person INNER JOIN 
                            OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID LEFT OUTER JOIN 
                            B01_Card ON B01_Person.PsnID = B01_Card.PsnID WHERE B01_Person.OrgStrucID IN @OrgStrucID  ";
            sql += chkSqlTime;
            string wherestr = @" AND  ( PsnNo LIKE @KeyName OR PsnName LIKE @KeyName OR PsnEName LIKE @KeyName OR CardNo LIKE @KeyName OR OrgNameList LIKE @KeyName OR OrgNoList LIKE @KeyName )";
            if (Request["KeyName"] != null && Request["KeyName"] != "")
            {
                sql += wherestr;
            }
            var PsnDataList = this.odo.GetQueryResult<PersonEntity>(sql, 
                new {OrgStrucID=OrgDataList.Select(i=>Convert.ToString(i.OrgStrucID)), KeyName="%"+Request["KeyName"]+"%",
                    PsnTimeS = Request["PsnTimeS"], PsnTimeE=Request["PsnTimeE"] }).ToList();
            string MsgStr = GetGlobalResourceObject("Resource", "NonData").ToString();
            if (PsnDataList.Count > 0)
                MsgStr = "";
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                result = PsnDataList.OrderBy(i=>i.PsnNo).ToList(), message=MsgStr
            }));
            Response.End();
        }
        
        #endregion


       


        #region 執行期限變更

        void SetExecProcData()
        {
            //get psn data
            var PsnData = this.odo.GetQueryResult<PersonEntity>("SELECT * FROM B01_Person WHERE PsnID IN @PsnList", new { PsnList = Request["PsnList"].Split(',') });
            string msg = "(無異動)", sMsg="";
            List<int> PsnChange = new List<int>();   
            if (PsnData.Count() == 0)
            {
                msg = "(無異動)";
            }

            foreach(var o in PsnData)
            {
                if (Request["ddlType"] == "PsnSTime")
                {
                    if (DateTime.Parse(Request["PsnDate"]) != Convert.ToDateTime(o.PsnSTime))
                    {
                        sMsg += o.PsnName + " - [人員啟用時間異動] " + DateTime.Parse(o.PsnSTime).ToString("yyyy/MM/dd HH:mm:ss") + " -> " + Request["PsnDate"] + "|";
                        PsnChange.Add(o.PsnID);                                    
                    }
                    else
                    {
                        sMsg += o.PsnName + " - [人員啟用時間異動] " + DateTime.Parse(o.PsnSTime).ToString("yyyy/MM/dd HH:mm:ss") + " -> " + Request["PsnDate"] + msg + "|";
                    }
                }
                else
                {
                    if (DateTime.Parse(Request["PsnDate"]) != Convert.ToDateTime(o.PsnETime))
                    {
                        sMsg += o.PsnName + " - [人員停用時間異動] " + DateTime.Parse(o.PsnSTime).ToString("yyyy/MM/dd HH:mm:ss") + " -> " + Request["PsnDate"] + "|";
                        PsnChange.Add(o.PsnID);
                    }
                    else
                    {
                        sMsg += o.PsnName + " - [人員停用時間異動] " + DateTime.Parse(o.PsnSTime).ToString("yyyy/MM/dd HH:mm:ss") + " -> " + Request["PsnDate"] + msg + "|";
                    }
                }                
            }
            if (PsnChange.Count > 0)
            {
                string ddlTypeName = Request["ddlType"];
                string UpdateCardAuth = " EXEC CardAuth_Update @sCardNo = @CardNo,@sUserID = @UserID,@sFromProc = 'PersonDateChange',@sFromIP = @IPInfo,@sOpDesc = '執行人員期限變更' ; ";
                string IpInfo = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //修改人員的起迄時間
                this.odo.Execute("UPDATE B01_Person SET "+ddlTypeName+"=@PsnDate,UpdateTime=GETDATE(),UpdateUserID=@UserID WHERE PsnID IN @PsnIDs ", 
                    new {ddlType=Request["ddlType"], PsnDate=Request["PsnDate"], PsnIDs=PsnChange,UserID=Sa.Web.Fun.GetSessionStr(this,"UserID") });
                this.odo.Execute("UPDATE B01_Card SET "+ddlTypeName.Replace("Psn","Card")+ "=@PsnDate,UpdateTime=GETDATE(),UpdateUserID=@UserID WHERE PsnID IN @PsnIDs ", 
                    new { ddlType = Request["ddlType"].ToString().Replace("Psn","Card"), PsnDate = Request["PsnDate"], PsnIDs = PsnChange,UserID=Sa.Web.Fun.GetSessionStr(this,"UserID") });
                var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料修改, Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this,"UserName"), "0105");
                log.LogInfo = "";
                log.LogDesc = "變更人員卡片日期期限";
                int count2 = 0;
                foreach (var card in this.odo.GetQueryResult<CardEntity>("SELECT * FROM B01_Card WHERE PsnID IN @PsnIDs ",new {PsnIDs=PsnChange}))
                {
                    this.odo.Execute(UpdateCardAuth, new { CardNo = card.CardNo, IPInfo = IpInfo,UserID=Sa.Web.Fun.GetSessionStr(this,"UserID") });
                    log.LogInfo ="CardNo "+ card.CardNo + " Update " + ddlTypeName.Replace("Psn","Card")  + "  to  " + Request["PsnDate"];
                    this.odo.SetSysLogCreate(log);
                    count2++;
                }
                string ResultMsg = string.Format("變更成功，人員筆數 {0} 筆，卡片筆數 {1} 筆", PsnChange.Count, count2);
                sMsg += ResultMsg;
            }             
            //設定時間異動程序
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                message = sMsg, psn_list=PsnChange
            }));
            Response.End();
        }
        
        #endregion

        #endregion
    }
}