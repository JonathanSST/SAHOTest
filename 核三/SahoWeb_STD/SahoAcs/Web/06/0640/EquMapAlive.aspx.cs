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
using Sa.DB;

namespace SahoAcs.Web._06._0640
{ 
    public partial class EquMapAlive : System.Web.UI.Page
    {
        public List<VMcrinfo> McrInfos = new List<VMcrinfo>();
        public string MapSrc="";
        DBReader dr = null;
        public string PicID = string.Empty;
        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            //this.MapSrc = WebAppService.GetSysParaData("EquMapSrc");
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (!IsPostBack)
            {
                oAcsDB.GetDataReader(" Select * From B03_MapBackground WHERE  PicType = 'Equ' order by PicID ; ", out dr);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        this.ddlMapList.Items.Add(new ListItem()
                        {
                            Text = dr.DataReader["PicDesc"].ToString(),
                            Value = dr.DataReader["PicID"].ToString()
                        });
                    }
                }
                this.ddlMapList.Items.Insert(0, new ListItem("請選擇", String.Empty));
            }

            if (!string.IsNullOrEmpty(this.ddlMapList.SelectedValue))
            {
                this.PicID = this.ddlMapList.SelectedValue.ToString();
                string PicName = od.GetStrScalar(string.Format("SELECT PicNAme FROM B03_MapBackground Where PicID = '{0}' ", PicID));
                if (!string.IsNullOrEmpty(PicName))
                {
                    this.MapSrc = @"/" + PicName.Replace("\\", "//").ToString().Trim();

                    ClientScript.RegisterStartupScript(ClientScript.GetType(), "LoadEquMap", "<script>LoadEquMap();</script>");
                }
            }

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
                if (json.Length < 10000)
                {
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
                //InputEquEvoPara.Add(new EquMapEntity()
                //{
                //    EquNo = Convert.ToString(s["equ_no"]),
                //    MapObjID = Convert.ToString(s["obj_id"]),
                //    PointX=(int)Convert.ToDouble(s["x"]),
                //    PointY=(int)Convert.ToDouble(s["y"]),
                //    IsOpen=(int)Convert.ToDouble(s["is_open"])                    
                //});
                InputEquEvoPara.Add(new EquMapEntity()
                {
                    EquNo = Convert.ToString(s["equ_no"]),
                    MapObjID = Convert.ToString(s["obj_id"]),
                    PointX = (int)Convert.ToDouble(s["x"]),
                    PointY = (int)Convert.ToDouble(s["y"]),
                    IsOpen = (int)Convert.ToDouble(s["is_open"]),
                    PicID = Convert.ToString(s["pic_id"])
                });
            }
            foreach(var o in InputEquEvoPara)
            {
                //if(this.od.GetQueryResult("SELECT * FROM B03_EquMapSetting WHERE MapObjID=@MapObjID", o).Count() == 0)
                //{
                //    this.od.Execute(@"INSERT INTO B03_EquMapSetting 
                //        (EquNo,MapObjID,PointX,PointY, IsOpen) 
                //        VALUES (@EquNo,@MapObjID,@PointX,@PointY, @IsOpen)", o);
                //}
                //else
                //{
                //    this.od.Execute(@"UPDATE B03_EquMapSetting 
                //        SET EquNo=@EquNo,PointX=@PointX,PointY=@PointY
                //        WHERE MapObjID=@MapObjID, IsOpen=@IsOpen", o);
                //}
                if (this.od.GetQueryResult("SELECT * FROM B03_EquMapSetting WHERE EquNo=@EquNo And PicID=@PicID", o).Count() == 0)
                {
                    this.od.Execute(@"INSERT INTO B03_EquMapSetting 
                        (EquNo,MapObjID,PointX,PointY, IsOpen,PicID) 
                        VALUES (@EquNo,@MapObjID,@PointX,@PointY, @IsOpen,@PicID)", o);
                }
                else
                {
                    this.od.Execute(@"UPDATE B03_EquMapSetting 
                        SET EquNo=@EquNo,PointX=@PointX,PointY=@PointY
                        WHERE MapObjID=@MapObjID AND IsOpen=@IsOpen And PicID=@PicID ", o);
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK",success=true }));
            Response.End();
        }


        private void SetEquList()
        {
            this.McrInfos = this.od.GetQueryResult<VMcrinfo>("SELECT * FROM B01_EquData").ToList();     //系統設備清單
            var paras = this.od.GetQueryResult<EquMapEntity>(@"SELECT B.*,AliveState,ISNULL(VarStateTime,'') AS VarStateTime,A.EquName FROM V_EquAliveState A
                                        INNER JOIN B03_EquMapSetting B ON A.EquNo=B.EquNo");          //系統座標物件清單
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true, equ_data=McrInfos, map_obj=paras }));
            Response.End();
        }

        private void SetEquMap()
        {
            //var paras = this.od.GetQueryResult<EquMapEntity>(@"SELECT B.*,AliveState,ISNULL(VarStateTime,'') AS VarStateTime,A.EquName FROM V_EquAliveState A
            //                            INNER JOIN B03_EquMapSetting B ON A.EquNo=B.EquNo");          //系統座標物件清單
            if (Request["PicID"] != null)
            {
                this.PicID = Request["PicID"].ToString();
            }
            string sql = @"SELECT B.*,AliveState,ISNULL(VarStateTime,'') AS VarStateTime,A.EquName FROM V_EquAliveState A
                                        INNER JOIN B03_EquMapSetting B ON A.EquNo=B.EquNo Where 1=1 ";
            if (!string.IsNullOrEmpty(this.PicID))
            {
                sql += " And B.PicID = '" + this.PicID + "' ";
            }
            var paras = this.od.GetQueryResult<EquMapEntity>(sql);          //系統座標物件清單
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true,  map_obj = paras }));
            Response.End();
        }
    }//end class
}//end namespace