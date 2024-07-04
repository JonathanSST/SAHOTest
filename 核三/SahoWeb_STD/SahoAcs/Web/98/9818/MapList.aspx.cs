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
    public partial class MapList : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<MapBackground> MapDataList = new List<MapBackground>();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.MapDataList = this.odo.GetQueryResult<MapBackground>("SELECT * FROM B03_MapBackground ").ToList();
            }
            catch (Exception ex)
            {
                Sa.Fun.EventLog(ex.GetBaseException().Message);
            }
            
            

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("MapList", "MapList.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案

        }//end Page_Load


    }//end class
}//end namesapce