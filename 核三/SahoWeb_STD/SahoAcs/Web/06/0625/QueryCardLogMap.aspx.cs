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
    public partial class QueryCardLogMap : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<CardLogMap> logmaps = new List<CardLogMap>();

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("QueryCardLogMap", "QueryCardLogMap.js");        //加入同一頁面所需的JavaScript檔案

            if (Request["PageEvent"] != null)
            {
                this.SetQuery();
            }
            else
            {
                var logs=this.odo.GetQueryResult<CardLogMap>("SELECT TOP 1 * FROM B01_CardLog ORDER BY RecordID DESC").ToList();
                if (logs.Count > 0)
                {
                    this.Calendar_CardTimeSDate.DateValue = string.Format("{0:yyyy/MM/dd 00:00:00}", logs.First().CardTime);
                    this.Calendar_CardTimeEDate.DateValue = string.Format("{0:yyyy/MM/dd 23:59:59}", logs.First().CardTime);
                }
                else
                {
                    this.Calendar_CardTimeSDate.DateValue = string.Format("{0:yyyy/MM/dd 00:00:00}", DateTime.Now);
                    this.Calendar_CardTimeEDate.DateValue = string.Format("{0:yyyy/MM/dd 23:59:59}", DateTime.Now);
                }                
            }
            //this.SetQuery();
        }


        private void SetQuery()
        {
            string cmd_str = @"SELECT 
	                                            CardNo,P.PsnNo,EquNo,EquName ,OSA.OrgName,P.PsnName,StateDesc,CardTime,RecordID
                                            FROM 
	                                            B01_CardLog CL
	                                            INNER JOIN B01_Person P ON CL.PsnNo=P.PsnNo
	                                            INNER JOIN OrgStrucAllData('Company') OSA ON CL.OrgStruc=OrgIDList 
                                                INNER JOIN B00_CardLogState ON LogStatus=Code WHERE CardTime BETWEEN @DateS AND @DateE ORDER BY CardNo,CardTime";
            this.logmaps = this.odo.GetQueryResult<CardLogMap>(cmd_str,new {DateS=Request["DateS"],DateE=Request["DateE"] }).ToList();
            var count = this.logmaps.Count();
        }

    }//end class
}//end namespace