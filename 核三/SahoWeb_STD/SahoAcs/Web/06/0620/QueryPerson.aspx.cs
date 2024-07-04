using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using System.Data;

namespace SahoAcs.Web._06._0620
{
    public partial class QueryPerson : System.Web.UI.Page
    {
        string psn_no = "";
        public List<PersonCar> PersonCarList = new List<PersonCar>();
        OrmDataObject odo = new OrmDataObject("MsSql",
            string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PsnNo"] != null && Request["PsnNo"] != "")
            {
                string psn_name = Request["PsnNo"].ToString() + "%";
                string command_str = @"
                SELECT 
	                p.*
	                ,c.CardNo
	                ,m.CarA1
	                ,m.CarA2
	                ,m.CarB1
	                ,m.CarB2,m.CertNo
                FROM 
	                B01_Person p 
	                INNER JOIN B01_Card c ON p.PsnID=c.PsnID 
	                LEFT JOIN B05_MakeCard m ON m.PsnNo=p.PsnNo
                WHERE 
	                1=1
                    AND (p.PsnNo = @PsnNo OR c.CardNo = @PsnNo 
                    OR p.PsnName LIKE ('%'+@PsnNo+'%')
                    OR m.CarA1=@PsnNo 
                    OR m.CarA2=@PsnNo OR m.CarB1=@PsnNo OR m.CarB2=@PsnNo)";
                this.PersonCarList = this.odo.GetQueryResult<PersonCar>(command_str, new { PsnNo = Request["PsnNo"],
                    PsnName=psn_name }).ToList();
            }
        }

        public class PersonCar
        {
            public string PsnNo { get; set; }
            public string PsnName { get; set; }
            public string CardNo { get; set; }
            public string CarA1 { get; set; }
            public string CarA2 { get; set; }
            public string CarB1 { get; set; }
            public string CarB2 { get; set; }
            public string CertNo { get; set; }
        }
    }//end class
}//end namespace