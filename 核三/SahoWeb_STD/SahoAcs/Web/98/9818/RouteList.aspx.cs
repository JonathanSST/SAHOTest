using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using System.IO;
using SahoAcs.DBModel;
using System.Drawing;



namespace SahoAcs.Web
{
    public partial class RouteList : System.Web.UI.Page
    {

        OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());

        public MapBackground mapobj = new MapBackground();

        public List<B03MapRoute> RouteListData = new List<B03MapRoute>();
        public List<B03MapRoute> UnRouteListData = new List<B03MapRoute>();
        protected void Page_Load(object sender, EventArgs e)
        {           
            if(Request["PageEvent"]!=null && Request["PageEvent"] == "Edit")
            {
                this.mapobj = this.odo.GetQueryResult<MapBackground>("SELECT * FROM B03_MapBackground WHERE PicID=@PicID", new { PicID = Request["PicID"] }).First();                
                this.RouteListData = this.odo.GetQueryResult<B03MapRoute>(@"SELECT * FROM B03_MapRoute WHERE PicID=@PicID 
                                                                                                                                            ORDER BY EquStart", new { PicID = Request["PicID"] }).ToList();               
                var equdatas = this.odo.GetQueryResult<EquAdj>("SELECT * FROM B01_EquData");
                string StringLineRouteFormat = "{0}({1}) → {2}({3})";
                foreach(var o in RouteListData)
                {
                    o.LineRoute = string.Format(StringLineRouteFormat,
                        equdatas.Where(i => i.EquID == o.EquStart).FirstOrDefault().EquName,
                        equdatas.Where(i => i.EquID == o.EquStart).FirstOrDefault().EquNo,
                        equdatas.Where(i => i.EquID == o.EquEnd).FirstOrDefault().EquName,
                        equdatas.Where(i => i.EquID == o.EquEnd).FirstOrDefault().EquNo);
                }
                var equ_result = this.odo.GetQueryResult(@"SELECT B.*,A.EquNo FROM 
	                                                                                                     B01_EquData A
	                                                                                                     INNER JOIN B03_MapPoint B ON A.EquID=B.EquID AND PicID=@PicID ", new { PicID = Request["PicID"] });
                foreach(var o in equ_result)
                {
                    foreach(var p in equ_result)
                    {
                        if (Convert.ToInt32(p.EquID) == Convert.ToInt32(o.EquID))
                        {
                            continue;
                        }
                        if (this.RouteListData.Where(i => i.EquStart == Convert.ToInt32(o.EquID) && i.EquEnd == Convert.ToInt32(p.EquID)).Count() == 0)
                        {
                            this.UnRouteListData.Add(new DBModel.B03MapRoute()
                            {
                                LineRoute= string.Format(StringLineRouteFormat,equdatas.Where(i => i.EquID == Convert.ToInt32(o.EquID)).FirstOrDefault().EquName,
                                equdatas.Where(i => i.EquID == Convert.ToInt32(o.EquID)).FirstOrDefault().EquNo,
                                equdatas.Where(i => i.EquID == Convert.ToInt32(p.EquID)).FirstOrDefault().EquName,
                                equdatas.Where(i => i.EquID == Convert.ToInt32(p.EquID)).FirstOrDefault().EquNo),
                                EquStart=Convert.ToInt32(o.EquID),
                                EquEnd=Convert.ToInt32(p.EquID),
                                PicID=this.mapobj.PicID
                            });
                        }
                    }
                }
            }     
        }

        

    }//end class
}//end namespace