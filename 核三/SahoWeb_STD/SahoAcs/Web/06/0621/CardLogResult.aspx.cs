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


namespace SahoAcs.Web._06._0621
{
    public partial class CardLogResult : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql",
           string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;

        public List<QueryCardLog> logs = new List<QueryCardLog>();
        public string OldPsnName = "";
        public int group_count = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.QueryCondition();
            }            
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
                FROM " + strUnionTB + " AS CL " +
                @"INNER JOIN B01_Person P ON CL.PsnNo=P.PsnNo
	            INNER JOIN OrgStrucAllData('Company') OSA ON CL.OrgStruc=OrgIDList 
                LEFT JOIN B05_MakeCard BMC ON BMC.PsnNo=CL.PsnNo
                WHERE PsnEName LIKE @PsnEName 
                AND CardTime BETWEEN @CardTime1 AND @CardTime2
                ORDER BY CL.PsnName,CardTime";

            this.logs = this.odo.GetQueryResult<QueryCardLog>(strSQL,
                new
                {
                    PsnEName = Request["PsnEName"].ToString() + "%",
                    CardTime1 = Request["CardTime1"].ToString(),
                    CardTime2 = Request["CardTime2"].ToString()
                }).ToList();
            this.logs = this.logs.OrderBy(i => i.CardTime).OrderBy(i => i.PsnName).ToList();
            this.hDataRowCount.Value = this.logs.Count().ToString();
            this._datacount = this.logs.Count();
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK",data=this.logs,data_count=this.logs.Count() }));
            Response.End();     
        }

        #endregion


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
}