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


namespace SahoAcs.Web._06._0621
{
    public partial class QueryCompCardLog : System.Web.UI.Page
    {
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        OrmDataObject odo = new OrmDataObject("MsSql",
            string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;

        public List<QueryCardLog> logs = new List<QueryCardLog>();
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
                else
                {
                    //this.Query();
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

        protected void QueryButtonButton_Click(object sender, EventArgs e)
        {
            Query();            
        }

        protected void ExportButton_Click(object sender, EventArgs e)
        {
            Query();
            ExportExcel();
        }


        #region Query By Condition

        private void QueryCondition()
        {

            // ex .2016/09/21
            string strUnionTB = 
                Pub.ReturnNewNestedSerachSQL("0621", 
                Request["CardTime1"].ToString(), 
                Request["CardTime2"].ToString());

            string strSQL =
                @"SELECT 
	                BMC.Undertaker,P.PsnName,P.PsnEName,
	                CL.PsnNo, 
	                CL.OrgStruc,
	                CL.CardTime,
	                CL.LogStatus,
	                CL.EquName,
	                P.PsnETime,
	                OSA.OrgNameList,
	                OSA.OrgName,
	                OSA.OrgID,
	                CL.EquDir,
                    CL.RecordId
                FROM " + strUnionTB + " AS CL "+
	            @"INNER JOIN B01_Person P ON CL.PsnNo=P.PsnNo
	            INNER JOIN OrgStrucAllData('Company') OSA ON CL.OrgStruc=OrgIDList 
                LEFT JOIN B05_MakeCard BMC ON BMC.PsnNo=CL.PsnNo
                WHERE PsnEName LIKE @PsnEName 
                AND CardTime BETWEEN @CardTime1 AND @CardTime2
                ORDER BY CL.PsnName,CardTime";

            this.logs = this.odo.GetQueryResult<QueryCardLog>(strSQL, 
                new { PsnEName = Request["PsnEName"].ToString()+"%",
                      CardTime1 = Request["CardTime1"].ToString(),
                      CardTime2 = Request["CardTime2"].ToString()
                }).ToList();
            this.logs = this.logs.OrderBy(i => i.CardTime).OrderBy(i => i.PsnName).ToList();
            this.hDataRowCount.Value = this.logs.Count().ToString();
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
                @"SELECT 
	                BMC.Undertaker,P.PsnName,P.PsnEName,
	                CL.PsnNo, 
	                CL.OrgStruc,
	                CL.CardTime,
	                CL.LogStatus,
	                CL.EquName,
	                P.PsnETime,
	                OSA.OrgNameList,
	                OSA.OrgName,
	                OSA.OrgID,
	                CL.EquDir,
                    CL.RecordId
                FROM " + strUnionTB + " AS CL " +
                @"INNER JOIN B01_Person P ON CL.PsnNo=P.PsnNo
	            INNER JOIN OrgStrucAllData('Company') OSA ON CL.OrgStruc=OrgIDList 
                LEFT JOIN B05_MakeCard BMC ON BMC.PsnNo=CL.PsnNo
                WHERE PsnEName LIKE @PsnEName 
                AND CardTime BETWEEN @CardTime1 AND @CardTime2
                ORDER BY CL.PsnName,CardTime";


            this.logs = this.odo.GetQueryResult<QueryCardLog>(strSQL,new
                                                                                                                       {
                                                                                                                           PsnEName = txtPsnEName.Text+"%",
                                                                                                                           CardTime1=this.Calendar_CardTimeSDate.DateValue,
                                                                                                                           CardTime2=this.Calendar_CardTimeEDate.DateValue}).ToList();
            this.hDataRowCount.Value = this.logs.Count().ToString();
            this._datacount = this.logs.Count();
            //this.MainRepeater.DataSource = this.logs;
            //this.MainRepeater.DataBind();            
        }
        #endregion                


        #region ExportExcel
        public void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CardLog");
            //DataTable dtCardLog = ProcDt;

            //Title
            ws.Cells[1, 1].Value = "廠商名稱";
            ws.Cells[1, 2].Value = "姓名";
            ws.Cells[1, 3].Value = "證號";
            ws.Cells[1, 4].Value = "帳號";
            ws.Cells[1, 5].Value = "刷卡時間";
            ws.Cells[1, 6].Value = "地點";
            ws.Cells[1, 7].Value = "狀態";
            ws.Cells[1, 8].Value = "有效時間";
            ws.Cells[1, 9].Value = "申請人";
            int int_state = 0;
            //Content
            for (int i = 0; i< this.logs.Count; i++)
            {
                ws.Cells[int_state + 2, 1].Value = logs[i].PsnEName;
                ws.Cells[int_state + 2, 2].Value = logs[i].PsnName;
                ws.Cells[int_state + 2, 3].Value = logs[i].PsnNo;
                ws.Cells[int_state + 2, 4].Value = logs[i].PsnNo;
                ws.Cells[int_state + 2, 5].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", logs[i].CardTime);
                ws.Cells[int_state + 2, 6].Value = logs[i].EquName;
                ws.Cells[int_state + 2, 7].Value = logs[i].LogStatus=="0"?"成功":"";
                ws.Cells[int_state + 2, 8].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", logs[i].PsnETime);
                ws.Cells[int_state + 2, 9].Value = logs[i].Undertaker;
                this.group_count++;
                if (i>0)
                {
                    if ((i+1)==this.logs.Count||this.logs[i].PsnName != this.logs[i + 1].PsnName)
                    {
                        int_state++;
                        ws.Cells[int_state + 2, 1, int_state + 2, 9].Merge = true;
                        ws.Cells[int_state + 2, 1].Value = string.Format("總數 = {0} 筆",this.group_count);
                        this.group_count = 0;                        
                    }
                }
                int_state++;                
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=CardLog.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }        

        #endregion

    }

    public class QueryCardLog
    {
        public int RecordId { get; set; }
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public string PsnEName { get; set; }
        public string EquName { get; set; }
        public string EquDir { get; set; }
        public DateTime? PsnETime { get; set; }
        public string LogStatus { get; set; }
        public DateTime CardTime { get; set; }
        public string Undertaker { get; set; }
    }

}