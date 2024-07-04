using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using DapperDataObjectLib;
using SahoAcs.DBClass;

namespace SahoAcs
{
    public partial class AreaInfo : Sa.BasePage
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<dynamic> AreaList = new List<dynamic>();
        public List<dynamic> AreaListForTrt = new List<dynamic>();
        public List<dynamic> PersonExt = new List<dynamic>();
        public int AllPsnCount = 0;
        public int AreaCount = 0;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.AreaList = this.odo.GetQueryResult(@"SELECT * FROM B01_CtrlArea WHERE CtrlAreaPsnCount>0").ToList();
            this.AreaListForTrt = this.odo.GetQueryResult(@"SELECT 
	                                                                                                        A.CtrlAreaNo,A.CtrlAreaName,COUNT(B.PsnNo) AS CtrlAreaPsnCount
                                                                                                        FROM
	                                                                                                        B01_CtrlArea A
	                                                                                                        LEFT JOIN B01_Person B ON A.CtrlAreaNo=B.PsnAreaNo WHERE  PsnIsLev=0 AND PsnNoneLog=0
                                                                                                        GROUP BY A.CtrlAreaNo,A.CtrlAreaName HAVING COUNT(B.PsnNo)>0 ").ToList();
            this.PersonExt = this.odo.GetQueryResult(@"SELECT A.*,PsnName,PsnAreaNo,PsnNo FROM B01_PersonExt A INNER JOIN B01_Person B ON A.PsnID=B.PsnID 
                                WHERE ISNULL(PsnAreaNo,'') <>'' AND ISNULL(CtrlAreaNo,'') <>'' AND A.LastTime>=@NowDate AND PsnIsLev=0 AND PsnNoneLog=0", new {NowDate = this.GetZoneTime().AddDays(-1)} ).ToList();
            this.AllPsnCount = this.AreaListForTrt.Sum(i => Convert.ToInt32(i.CtrlAreaPsnCount));
                //this.odo.GetIntScalar("SELECT SUM(ISNULL(CtrlAreaPsnCount,0)) FROM B01_CtrlArea ");      //各管制區的應到人數
            this.AreaCount = this.PersonExt.Count;
        }
    }//end page class
}//end namespace