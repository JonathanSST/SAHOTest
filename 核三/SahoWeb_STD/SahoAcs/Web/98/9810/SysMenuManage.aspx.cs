using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using System.Collections;
using System.Data;
using System.IO;
using OfficeOpenXml;


namespace SahoAcs.Web._98._9810
{
    public partial class SysMenuManage : System.Web.UI.Page
    {
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        OrmDataObject odo = new OrmDataObject("MsSql",
            string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;
        public List<SysMenuModel> SysMenus = new List<SysMenuModel>();
        public int group_count = 0;
        public string OldPsnName = "";


        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {                        
            if (!IsPostBack)
            {
                if (Request["PageEvent"]!=null&&Request["PageEvent"] == "Save")
                {
                    this.SetExec();
                }
                else
                {
                    string js = "<script type='text/javascript'>OnLoad();</script>";
                    ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
                    ClientScript.RegisterClientScriptInclude("ResetCardAuth", "SysMenuManage.js");        //加入同一頁面所需的JavaScript檔案
                    ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
                    ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
                    this.Query();             
                }                                
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];             
            }
        }

        private void SetExec()
        {
            string[] MenuNos = Request.Form.GetValues("MenuNo");
            string[] MenuNames = Request.Form.GetValues("MenuName");
            string[] FunTracks = Request.Form.GetValues("FunTrack");
            string[] MenuIsUse = Request.Form.GetValues("MenuIsUse");
            string[] MenuOrder = Request.Form.GetValues("MenuOrder");
            int row_count = MenuOrder.Length;
            string exception_str = "";
            for (int i = 0; i < MenuNos.Length; i++)
            {
                this.odo.Execute(@"UPDATE B00_SysMenu 
                    SET MenuName=@MenuName,FunTrack=@FunTrack,MenuIsUse=@MenuIsUse,MenuOrder=@MenuOrder
                    WHERE MenuNo=@MenuNo", new {MenuNo=MenuNos[i],
                    MenuName=MenuNames[i],
                    FunTrack=FunTracks[i],
                    MenuIsUse=MenuIsUse[i],
                    MenuOrder=MenuOrder[i]
                });
                exception_str += this.odo.DbExceptionMessage+";";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                message = "Ok",
                success = true,
                error_msg = exception_str
            }));
            Response.End();
        }

        #region Query
        private void Query()
        {
            this.SysMenus = this.odo.GetQueryResult<SysMenuModel>(@"SELECT
	                MenuName
	                ,MenuNo	
	                ,FunTrack 
	                ,MenuOrder
	                ,MenuIsUse
	                ,UpMenuNo
	                ,(SELECT MenuName FROM B00_SysMenu P WHERE P.MenuNo=C.UpMenuNo) AS UpMenuName
                FROM 
	                B00_SysMenu C
                where MenuType='F'
                ORDER BY
	                UpMenuNo,MenuOrder").ToList();
            this.hDataRowCount.Value = this.SysMenus.Count.ToString();
        }
        #endregion                
       
        public class SysMenuModel
        {
            public string MenuName { get; set; }
            public string MenuNo { get; set; }
            public string FunTrack { get; set; }
            public string MenuIsUse { get; set; }
            public string UpMenuNo { get; set; }
            public string UpMenuName { get; set; }
            public int MenuOrder { get; set; }
        }


    }//end class
}//end namespace