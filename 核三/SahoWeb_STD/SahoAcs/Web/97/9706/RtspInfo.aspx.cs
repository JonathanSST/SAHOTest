using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SahoAcs.DBModel;
using DapperDataObjectLib;



namespace SahoAcs.Web._97._9706
{
    public partial class RtspInfo : System.Web.UI.Page
    {
        public List<RtspEntity> GlobalRtspList = new List<RtspEntity>();
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        protected void Page_Load(object sender, EventArgs e)
        {
            this.GlobalRtspList = this.odo.GetQueryResult<RtspEntity>("SELECT * FROM B03_RtspInfo").ToList();
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true, video_list = this.GlobalRtspList }));
            Response.End();
        }



    }
}