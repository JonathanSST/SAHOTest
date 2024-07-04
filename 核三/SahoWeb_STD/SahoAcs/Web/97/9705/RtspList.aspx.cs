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



namespace SahoAcs.Web._97._9705
{
    public partial class RtspList :SahoAcs.DBClass.BasePage
    {
        public List<RtspEntity> GlobalRtspList = new List<RtspEntity>();

        public string RtspHost = "WS://";
        public int EvoPre = 0;

        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] == null)
            {
                this.SetRtspList();
            }
            
           if(Request["PageEvent"]!=null && Request["PageEvent"] == "Save")
            {
                this.SetSaveRtspList();
            }

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("JsFun", "RtspList.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
        }




        private void SetSaveRtspList()
        {
            if (Request.Form["RtspServer"] != null)
            {
                this.RtspHost = Request.Form["RtspHost"];
            }
            List<RtspEntity> RtspData = new List<RtspEntity>();
            if (Request.Form["RtspVideo"] != null && Request.Form.GetValues("RtspVideo").Length > 0)
            {
                RtspData = Request.Form.GetValues("RtspVideo").Select(i => new RtspEntity() { RtspVideo = i }).ToList();               
            }
            if(Request.Form["RtspMemo"]!=null && Request.Form.GetValues("RtspMemo").Length > 0)
            {
                RtspData.ForEach(i =>
                {                    
                    if (Request.Form.GetValues("RtspMemo").Length > RtspData.IndexOf(i))
                    {
                        i.RtspMemo = Request.Form.GetValues("RtspMemo")[RtspData.IndexOf(i)];
                    }
                    if (Request.Form.GetValues("ResourceID").Length > RtspData.IndexOf(i))
                    {
                        i.ResourceID = int.Parse(Request.Form.GetValues("ResourceID")[RtspData.IndexOf(i)]);
                    }
                });
            }
            //資料序列化處理
            //RtspData = RtspData.Where(i => !string.IsNullOrEmpty(i.RtspVideo)).ToList();
            //這裡進行foreach，將0的進行新增，大於1的Resource進行修改
            this.od.Execute("INSERT INTO B03_RtspInfo (RtspVideo,RtspMemo) VALUES (@RtspVideo,@RtspMemo)", RtspData.Where(i => i.ResourceID == 0 && !string.IsNullOrEmpty(i.RtspVideo)));
            this.od.Execute("UPDATE B03_RtspInfo SET RtspVideo=@RtspVideo,RtspMemo=@RtspMemo WHERE ResourceID=@ResourceID", RtspData.Where(i => i.ResourceID > 0));
            this.od.Execute("DELETE B03_RtspInfo WHERE ResourceID IN @DataID ", new { DataID = RtspData.Where(i=>string.IsNullOrEmpty(i.RtspVideo)).Select(i => i.ResourceID) });
            /* 這裡 20210813 改用 B03_RtspInfo
            string RtspVideoListJoin = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(RtspData);
            if (this.od.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='RtspVideoList' ").Count() > 0)
            {
                this.od.Execute("UPDATE B00_SysParameter SET ParaValue=@ParaValue,UpdateUserID=@User,UpdateTime=GETDATE() WHERE ParaNo=@ParaNo",
                    new { ParaValue = RtspVideoListJoin, ParaNo = "RtspVideoList", User = Sa.Web.Fun.GetSessionStr(this, "UserID") });
            }
            else
            {
                this.od.Execute(@"INSERT INTO B00_SysParameter 
                        (ParaNo,ParaValue,ParaClass,ParaName,ParaType,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                        VALUES (@ParaNo,@ParaValue,'HideSystem',@ParaName,'String',@User,GETDATE(),@User,GETDATE())",
                        new { ParaValue = RtspVideoListJoin, ParaNo = "RtspVideoList", User = "Saho", ParaName = "Rtsp攝影機路徑位置清單" });
            }
            */
            this.od.Execute(@"UPDATE B00_SysUser SET UserRtspServerUrl=@RtspHost", new { RtspHost = Request.Form["RtspHost"] });
            
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK",success=true }));
            Response.End();
        }


        private void SetRtspList()
        {            
            this.GlobalRtspList = this.od.GetQueryResult<RtspEntity>("SELECT * FROM B03_RtspInfo").ToList();
            this.RtspHost = this.od.GetStrScalar("SELECT UserRtspServerUrl FROM B00_SysUser WHERE UserID=@UserID", new { UserID = Sa.Web.Fun.GetSessionStr(this, "UserID") });
        }

    }//end class
}//end namespace