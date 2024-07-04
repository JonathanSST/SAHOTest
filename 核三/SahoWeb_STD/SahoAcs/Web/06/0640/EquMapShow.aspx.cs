using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Script.Serialization;

using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;


namespace SahoAcs.Web._06._0640
{
    public partial class EquMapShow : System.Web.UI.Page
    {
        public OrmDataObject odo;
        public List<EquAdj> datalist = new List<EquAdj>();
        public string PicName = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("EquMapShow", "EquMapShow.js");//加入同一頁面所需的JavaScript檔案
            odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            var list = this.odo.GetQueryResult("SELECT * FROM B03_MapBackground WHERE PicID=@PicID", new { PicID = Request["PicID"] });
            foreach (var o in list)
            {
                PicName = Convert.ToString(o.PicDesc);
            }
        }
    }
}