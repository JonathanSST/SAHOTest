using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.Web._98._9818
{
    public partial class DragLine : System.Web.UI.Page
    {

        public OrmDataObject odo;

        public List<EquVms> datalist = new List<EquVms>();

        public string PicName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("JsTouch", "/Scripts/jquery.ui.touch-punch.min.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("DragLine", "DragLine.js");//加入同一頁面所需的JavaScript檔案

            this.odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            var list = this.odo.GetQueryResult("SELECT * FROM B03_MapBackground WHERE PicID=@PicID", new { PicID = Request["PicID"] });
            foreach(var o in list)
            {
                PicName = Convert.ToString(o.PicDesc);
            }
            this.datalist = this.odo.GetQueryResult<EquVms>(@"SELECT A.*,PointX,PointY FROM 
                                                                                                         B01_EquData A
                                                                                                         INNER JOIN B03_MapPoint B ON A.EquID = B.EquID AND B.PicID=@PicID", new { PicID = Request["PicID"] }).ToList();

            if (Request["PageEvent"] != null && Request["PageEvent"] == "SaveLine")
            {
                this.odo.Execute("DELETE B03_MapRoute WHERE EquStart=@EquStart AND EquEnd=@EquEnd AND PicID=@PicID",
                    new { EquStart = Request["EquStart"], EquEnd = Request["EquEnd"], LineRoute = Request["LineRoute"], PicID = Request["PicID"] });
                this.odo.Execute("INSERT INTO B03_MapRoute (EquStart,EquEnd,LineRoute,PicID) VALUES (@EquStart,@EquEnd,@LineRoute,@PicID)",
                    new { EquStart = Request["EquStart"], EquEnd = Request["EquEnd"], LineRoute = Request["LineRoute"], PicID = Request["PicID"] });
            }
        }

    }//end class
}//end namespace