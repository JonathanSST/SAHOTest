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


namespace SahoAcs
{
    public partial class CardLogClear : System.Web.UI.Page
    {
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        OrmDataObject odo = new OrmDataObject("MsSql",
            string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;

        public List<QueryCardLogClear> logs = new List<QueryCardLogClear>();
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
            //this.oScriptManager.EnablePageMethods = true;
            //this.oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);
            //this.oScriptManager.RegisterAsyncPostBackControl(this.ExportButton);

            ClientScript.RegisterClientScriptInclude("QueryCompCardLog", "QueryCompCardLog.js");        //加入同一頁面所需的JavaScript檔案            
            //this.QueryButton.Attributes["onclick"] = "SetQuery(this); return false;";            
            if (!IsPostBack)
            {
                //var comp_list = this.odo.GetQueryResult("SELECT * FROM B01_OrgData WHERE OrgClass='Company' ORDER BY OrgName ");
                //foreach (var cc in comp_list)
                //{
                //    this.ddl_OrgCompany.Items.Add(new ListItem() { Text = Convert.ToString(cc.OrgName), Value = Convert.ToString(cc.OrgID) });
                //}
                this.Calendar_CardTimeSDate.DateValue = string.Format("{0:yyyy/MM/dd 00:00:00}", DateTime.Now);
                this.Calendar_CardTimeEDate.DateValue = string.Format("{0:yyyy/MM/dd 23:59:59}", DateTime.Now);
                if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
                {
                    this.QueryCondition();
                }
                else if (Request["PageEvent"] != null && Request["PageEvent"] == "Delete")
                {
                    this.SetDelete();
                }
                //this.QueryCondition();
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                //if (sFormTarget == this.QueryButton.ClientID)
                //{
                //    this.ExportExcel();      
                //}                                
            }
        }

        private void SetDelete()
        {
            List<string> RecordIDs = Request["DeleteList"].Split(',').Where(s=>s!="").ToList();
            string strSQL =@"DELETE B01_CardLog WHERE RecordID IN @RecordIDs";
            this.odo.Execute(strSQL, new{RecordIDs=RecordIDs});
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { result = "OK",message=this.odo.DbExceptionMessage }));
            Response.End();     
        }

        protected void QueryButtonButton_Click(object sender, EventArgs e)
        {
            Query();            
        }

        protected void ExportButton_Click(object sender, EventArgs e)
        {
            Query();            
        }


        #region Query By Condition

        private void QueryCondition()
        {
            string strSQL =
              @"SELECT * FROM B01_CardLog WHERE LogStatus=160
                AND CardTime BETWEEN @CardTime1 AND @CardTime2
                ORDER BY CardTime DESC";

            this.logs = this.odo.GetQueryResult<QueryCardLogClear>(strSQL, 
                new {
                      CardTime1 = Request["CardTime1"].ToString(),
                      CardTime2 = Request["CardTime2"].ToString()
                }).ToList();            
            //this.hDataRowCount.Value = this.logs.Count().ToString();
            this._datacount = this.logs.Count();
            //this.MainRepeater.DataSource = this.logs;
            //this.MainRepeater.DataBind();           
        }

        #endregion

        #region Query
        private void Query()
        {
            // ex .2016/09/21
            string strUnionTB =
                Pub.ReturnNewNestedSerachSQL("0621",
                this.Calendar_CardTimeSDate.DateValue,
                this.Calendar_CardTimeEDate.DateValue);

            string strSQL =
               @"SELECT * FROM B01_CardLog WHERE LogStatus=160
                AND CardTime BETWEEN @CardTime1 AND @CardTime2
                ORDER BY CardTime DESC";

            this.logs = this.odo.GetQueryResult<QueryCardLogClear>(strSQL,new
                                                                                                                       {                                                                                                                           
                                                                                                                           CardTime1=this.Calendar_CardTimeSDate.DateValue,
                                                                                                                           CardTime2=this.Calendar_CardTimeEDate.DateValue}).ToList();
            //this.hDataRowCount.Value = this.logs.Count().ToString();
            this._datacount = this.logs.Count();
            //this.MainRepeater.DataSource = this.logs;
            //this.MainRepeater.DataBind();            
        }
        #endregion                                

        public class QueryCardLogClear
        {
            public int RecordId { get; set; }
            public string PsnNo { get; set; }
            public string PsnName { get; set; }
            public int OrgID { get; set; }
            public string OrgName { get; set; }
            public string EquNo { get; set; }
            public string EquName { get; set; }
            public string EquDir { get; set; }
            public DateTime? PsnETime { get; set; }
            public string LogStatus { get; set; }
            public DateTime CardTime { get; set; }
            public string Undertaker { get; set; }
            public string CardNo { get; set; }
        }

    }    
}