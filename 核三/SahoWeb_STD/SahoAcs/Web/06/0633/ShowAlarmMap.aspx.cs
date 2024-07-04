using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs.Web._06._0633
{
    public partial class ShowAlarmMap : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<CardLogMap> logs = new List<CardLogMap>();
        public int RecordID = 0;
        public int EquID = 0;
        public int EquNo = 0;
        public string CardNo = "";
        public string EquName = "";
        public CardLogMap logdata = new CardLogMap();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["record_id"] != null)
            {
                this.logs = this.odo.GetQueryResult<CardLogMap>(@"SELECT A.*,B.EquName,B.EquID
                                                FROM 
	                                                B01_AlarmLog A
	                                                INNER JOIN B01_EquData B ON A.EquNo=B.EquNo
	                                                WHERE RecordID=@ID ",
                                                   new {ID=Request["record_id"]}).ToList();
                foreach(var o in this.logs)
                {
                    this.logdata = o;
                }
            }
        }

    }
}