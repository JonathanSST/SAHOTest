using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using OfficeOpenXml;


namespace SahoAcs
{
    public partial class InAreaList : System.Web.UI.Page
    {
        public int InAreaCounts = 0;

        public List<CardLogModel> AllStayList = new List<CardLogModel>();

        string LogStateCmdStr = "SELECT * FROM B00_CardLogState ";

        protected void Page_Load(object sender, EventArgs e)
        {
            OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());
            string cmdstr = @"SELECT A.*,C.OrgStrucID FROM B01_CardLog A
                                                        INNER JOIN  (
                                                        SELECT 
	                                                        PsnNo,MAX(CardTime) AS CardTime
                                                        FROM 
	                                                        B01_CardLog A
                                                        WHERE 
	                                                        CardTime BETWEEN @DateS AND @DateE
	                                                        AND PsnNo <> '' AND LogStatus='0'
                                                        GROUP BY PsnNo ) AS B ON A.PsnNo=B.PsnNo AND A.CardTime=B.CardTime
														LEFT JOIN V_PsnCard C ON A.PsnNo=C.PsnNo
                                                        WHERE EquDir='進' AND A.PsnNo<>'' AND LogStatus='0' ";
            //var AreaList = odo.GetQueryResult(cmdstr,new {DateS=DateTime.Now.AddDays(-1), DateE=DateTime.Now});
            this.AllStayList = odo.GetQueryResult<CardLogModel>(cmdstr, new { DateS = DateTime.Now.AddDays(-1), DateE = DateTime.Now }).ToList();
            var statelist = odo.GetQueryResult(this.LogStateCmdStr);            
            //var logPerson = this.odo.GetQueryResult<CardLogModel>(this.QueryPerson).ToList();
            var OrgCompany = odo.GetQueryResult<OrgStrucInfo>("SELECT * FROM OrgStrucAllData('Company')");
            AllStayList.ForEach(i =>
            {
                i.LogStatus = statelist.Where(state => Convert.ToInt32(state.Code) == int.Parse(i.LogStatus)).FirstOrDefault().StateDesc;
                i.DepName = OrgCompany.Where(o => int.Parse(o.OrgStrucID) == i.OrgStrucID).Count() > 0 ? OrgCompany.Where(o => int.Parse(o.OrgStrucID) == i.OrgStrucID).FirstOrDefault().OrgName : "";
            });
            InAreaCounts = this.AllStayList.Count;
        }

    }//end page class
}//end namespace