using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;



namespace SahoAcs.Web._98._9823
{
    public partial class DoBackup : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("DoBackup", "DoBackup.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            if (Request.Form["PageEvent"] != null && Request.Form["PageEvent"] == "DoBackup")
            {
                this.odo.Execute("EXECUTE sp_executesql BACKUP DATABASE ZenchenDB TO DISK=@PathName ", new
                {
                    PathName = @"E:\Database_Backup\ZenchenDB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak"
                });
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
                {
                    is_success=true, message="備份完成"
                }));
                Response.End();
            }                   
         }      
    }//end form class
}//end namespace