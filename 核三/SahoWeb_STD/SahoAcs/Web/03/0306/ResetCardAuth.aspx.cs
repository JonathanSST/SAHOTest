using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using System.Collections;
using System.Data;
using System.IO;
using OfficeOpenXml;


namespace SahoAcs.Web._03._0306
{
    public partial class ResetCardAuth : System.Web.UI.Page
    {
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        OrmDataObject odo = new OrmDataObject("MsSql",
            string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;

        public List<ResetCardData> logs = new List<ResetCardData>();
        public int group_count = 0;
        public string OldPsnName = "";


        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            this.oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            this.oScriptManager.EnablePageMethods = true;            

            string js = "<script type='text/javascript'>OnLoad();</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ResetCardAuth", "ResetCardAuth.js");        //加入同一頁面所需的JavaScript檔案

            if (!IsPostBack)
            {
                if (Request["PageEvent"] == "Save"&&this.Request["CardNo"]!=null)
                {
                    string no = Request["CardNo"];
                    this.SetExec(no);
                }                
                this.Query();             
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];             
            }
        }

        private void SetExec(string card_no)
        {
            //取得重複的設備及卡片資料
            List<CardDblData> cardequs = this.odo.GetQueryResult<CardDblData>(@"SELECT 
	                CardNo
	                ,EquID
	                ,COUNT(*) AS EquCnt
                FROM 
	                B01_CardAuth WHERE OpMode<>'Del' AND CardNo=@CardNo
                GROUP BY 
	                EquID,CardNo
                HAVING COUNT(*)>1", new { CardNo = card_no }).ToList();
            foreach (var o in cardequs)
            {
                this.odo.Execute(@"DELETE B01_CardAuth
                WHERE 
	                CardNo=@CardNo AND EquID=@EquID
	                AND (ProcKey<(SELECT 
	                MAX(ProcKey) 
                FROM 
	                B01_CardAuth 
                WHERE 
	                CardNo=@CardNo AND EquID=@EquID))", new {CardNo=o.CardNo,EquID=o.EquID });
            }
            //string cmd = "UPDATE B01_Card SET CardAuthAllow='0' WHERE CardNo=@CardNo";
            //this.odo.Execute(cmd, new { CardNo = card_no });
            //cmd = " EXEC CardAuth_Update @sCardNo = @CardNo,@sUserID = '" + Session["UserId"].ToString() + "',@sFromProc = 'Person',@sFromIP = '::1',@sOpDesc = '重設權限' ; ";
            //this.odo.Execute(cmd, new { CardNo = card_no });
            //cmd = "UPDATE B01_Card SET CardAuthAllow='1' WHERE CardNo=@CardNo";
            //this.odo.Execute(cmd, new { CardNo = card_no });
            //cmd = " EXEC CardAuth_Update @sCardNo = @CardNo,@sUserID = '" + Session["UserId"].ToString() + "',@sFromProc = 'Person',@sFromIP = '::1',@sOpDesc = '重設權限' ; ";
            //this.odo.Execute(cmd, new { CardNo = card_no });
        }

        #region Query
        private void Query()
        {
            List<CardAuthError> error_list = this.odo.GetQueryResult<CardAuthError>(@"SELECT DISTINCT CardNo from (
                SELECT 
	                CardNo
	                ,EquID
	                ,COUNT(*) AS EquCnt
                FROM 
	                B01_CardAuth WHERE OpMode<>'Del'
                GROUP BY 
	                EquID,CardNo
                HAVING COUNT(*)>1
                ) AS T1").ToList();
            this.logs = this.odo.GetQueryResult<ResetCardData>(@"SELECT
	            C.CardNo,P.PsnNo,C.CardType,P.PsnName,PA.ItemName
            FROM 
            B01_Card C
            INNER JOIN B01_Person P ON P.PsnID=C.PsnID
            INNER JOIN B00_ItemList PA ON PA.ItemNo=C.CardType AND ItemClass='CardType' WHERE C.CardNo IN @CardError", 
                                                                                                                     new {CardError=error_list.Select(i=>i.CardNo) }).ToList();
            this.hDataRowCount.Value = this.logs.Count().ToString();           
            this._datacount = this.logs.Count();
            //this.MainRepeater.DataSource = this.logs;
            //this.MainRepeater.DataBind();            
        }
        #endregion                
       

    }

    public class CardAuthError
    {
        public string CardNo { get; set; }
    }

    public class CardDblData
    {
        public string CardNo { get; set; }
        public string EquID { get; set; }
    }

    public class ResetCardData
    {
        public string CardNo { get; set; }
        public string PsnNo { get; set; }
        public string CardType { get; set; }
        public string PsnName { get; set; }
        public string ItemName { get; set; }
    }

}