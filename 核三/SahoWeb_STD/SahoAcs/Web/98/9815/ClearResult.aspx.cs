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


namespace SahoAcs
{
    public partial class ClearResult : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql",
           string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;

        public List<QueryCardLogClear> logs = new List<QueryCardLogClear>();
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
                @"SELECT A.*,B.EquName AS NewName,B.EquClass AS NewClass FROM B01_CardLog A
                    INNER JOIN B01_EquData B ON A.EquNo=B.EquNo 
                    WHERE LogStatus=160
                AND CardTime BETWEEN @CardTime1 AND @CardTime2
                ORDER BY CardTime DESC";

            this.logs = this.odo.GetQueryResult<QueryCardLogClear>(strSQL,
                new
                {                    
                    CardTime1 = Request["CardTime1"].ToString(),
                    CardTime2 = Request["CardTime2"].ToString()
                }).ToList();            
            //this.hDataRowCount.Value = this.logs.Count().ToString();
            this._datacount = this.logs.Count();
            //Response.Clear();
            //Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
            //    new { message = "OK",data=this.logs,data_count=this.logs.Count() }));
            //Response.End();     
        }

        #endregion       

        public class QueryCardLogClear
        {
            public int RecordId { get; set; }
            public string CardNo { get; set; }
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
            public string NewName { get; set; }
            public string NewClass { get; set; }
        }

    }//end class
}//end namespace