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
    public partial class DragPoint : System.Web.UI.Page
    {
        public OrmDataObject odo;

        public List<EquAdj> datalist = new List<EquAdj>();
        public string PicName = "";
        public int rotate = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("JsTouch", "/Scripts/jquery.ui.touch-punch.min.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("DragPoint", "DragPoint.js");//加入同一頁面所需的JavaScript檔案

            odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            var list = this.odo.GetQueryResult("SELECT * FROM B03_MapBackground WHERE PicID=@PicID", new { PicID = Request["PicID"] });
            foreach (var o in list)
            {
                PicName = Convert.ToString(o.PicDesc);
            }

            this.datalist = this.odo.GetQueryResult<EquAdj>("SELECT * FROM B01_EquData A").ToList();
            var maps = this.odo.GetQueryResult("SELECT * FROM B03_MapBackground WHERE PicID=@PicID", new { PicID = Request["PicID"] });
            if (maps.Count() > 0)
            {
                this.rotate = Convert.ToInt32(maps.First().PicAngle);
            }
        }

    }//end class
}//end namespace