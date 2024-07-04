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



namespace SahoAcs.Web._98._9825
{
    public partial class EquMapList : System.Web.UI.Page
    {
        public List<VMcrinfo> McrInfos = new List<VMcrinfo>();
        public string MapSrc="";

        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MapSrc = WebAppService.GetSysParaData("EquMapSrc");
            if (Request["PageEvent"] != null && Request["PageEvent"]=="Load")
            {
                this.SetEquList();
            }
            if(Request["PageEvent"]!=null && Request["PageEvent"] == "LoadMap")
            {
                this.SetEquMap();
            }

            using (var reader = new System.IO.StreamReader(Request.InputStream))
            {                
                string json = reader.ReadToEnd();
                var serializer = new JavaScriptSerializer();
                //var result = serializer.DeserializeObject(json);             
                dynamic resp = serializer.DeserializeObject(json);
                if (resp != null)
                {
                    string PageEvent = Convert.ToString(resp["PageEvent"]);
                    
                    if (PageEvent == "Save")
                    {
                        this.SetSaveMapList(resp);                        
                    }
                    //string result_json = Convert.ToString(result);
                }                
            }

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("MapList", "MapList.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("D3js", "d3.v3.min.js");//加入同一頁面所需的JavaScript檔案
        }




        private void SetSaveMapList(dynamic paradic)
        {
            List<EquMapEntity> InputEquEvoPara = new List<EquMapEntity>();
            foreach (var s in paradic["EquMapList"])
            {
                InputEquEvoPara.Add(new EquMapEntity()
                {
                    EquNo = Convert.ToString(s["equ_no"]),
                    MapObjID = Convert.ToString(s["obj_id"]),
                    PointX=(int)Convert.ToDouble(s["x"]),
                    PointY=(int)Convert.ToDouble(s["y"]),
                    IsOpen=(int)Convert.ToDouble(s["is_open"])                    
                });
            }
            foreach(var o in InputEquEvoPara)
            {
                if(this.od.GetQueryResult("SELECT * FROM B03_EquMapSetting WHERE MapObjID=@MapObjID", o).Count() == 0)
                {
                    this.od.Execute(@"INSERT INTO B03_EquMapSetting 
                        (EquNo,MapObjID,PointX,PointY, IsOpen) 
                        VALUES (@EquNo,@MapObjID,@PointX,@PointY, @IsOpen)", o);
                }
                else
                {
                    this.od.Execute(@"UPDATE B03_EquMapSetting 
                        SET EquNo=@EquNo,PointX=@PointX,PointY=@PointY
                        WHERE MapObjID=@MapObjID AND IsOpen=@IsOpen", o);
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK",success=true }));
            Response.End();
        }


        private void SetEquList()
        {
            this.McrInfos = this.od.GetQueryResult<VMcrinfo>("SELECT * FROM V_EquAliveState").ToList();     //系統設備清單
            var paras = this.od.GetQueryResult<EquMapEntity>(@"SELECT B.*,AliveState,ISNULL(VarStateTime,'') AS VarStateTime,A.EquName FROM V_EquAliveState A
                                        INNER JOIN B03_EquMapSetting B ON A.EquNo=B.EquNo");          //系統座標物件清單
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true, equ_data=McrInfos, map_obj=paras }));
            Response.End();
        }

        private void SetEquMap()
        {
            var paras = this.od.GetQueryResult<EquMapEntity>(@"SELECT B.*,AliveState,ISNULL(VarStateTime,'') AS VarStateTime,A.EquName FROM V_EquAliveState A
                                        INNER JOIN B03_EquMapSetting B ON A.EquNo=B.EquNo");          //系統座標物件清單
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true,  map_obj = paras }));
            Response.End();
        }
    }//end class
}//end namespace