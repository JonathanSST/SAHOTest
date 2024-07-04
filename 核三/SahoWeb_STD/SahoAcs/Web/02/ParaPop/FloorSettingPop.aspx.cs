using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs
{
    public partial class FloorSettingPop : Sa.BasePage
    {
        public List<FloorAccess> FloorList = new List<FloorAccess>();
        public string FloorBinValue="";
        string FloorName = "";
        string ParaValue = "";
        public string ParaName = "";
        public string ParaDesc = "";
        public OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //FloorName = Request["FloorName"].ToString();
            //ParaValue = Request["ParaValue"].ToString();
            //ParaName = Request["ParaName"].ToString();
            hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            this.hideEquID.Value = Request["EquID"];
            this.hideEquParaID.Value = Request["EquParaID"];
            this.hideParaValue.Value = Request["ParaValue"];
            this.FloorList = this.od.GetQueryResult<FloorAccess>("SELECT * FROM B01_ElevatorFloor WHERE EquID=@EquID",new {EquID=this.hideEquID.Value}).ToList();
            foreach(var o in this.od.GetQueryResult("SELECT * FROM B01_EquParaDef WHERE EquParaID=@EquParaID",new {EquParaID=this.hideEquParaID.Value}))
            {
                this.ParaDesc = Convert.ToString(o.ParaDesc);
                this.hideParaName.Value = Convert.ToString(o.ParaName);
            }            
            this.hideFloorData.Value = Sa.Change.HexToBin(this.hideParaValue.Value, 48);            
            string js = "<script src='/Web/02/ParaPop/FloorSettingPop.js' type='text/javascript'/>";
            js += "<script type='text/javascript'>SetLoadData();</script>";
            //ClientScript.RegisterClientScriptInclude("FloorSettingPop", "/Web/02/ParaPop/FloorSettingPop.js");//加入同一頁面所需的JavaScript檔案            
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
        }

    }//end class

}//end namespace